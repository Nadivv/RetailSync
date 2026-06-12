using Npgsql;
using System;
using System.Collections.Generic;

namespace RetailSync.Helpers
{
    public class TransaksiRow
    {
        public int      IdTransaksi { get; set; }
        public DateTime Waktu       { get; set; }
        public string   NamaKasir   { get; set; } = "";
        public string   NamaToko    { get; set; } = "";
        public string   NamaProduk  { get; set; } = "";
        public decimal  Total       { get; set; }
    }

    static class TransaksiRepository
    {
        // ── Riwayat transaksi dengan filter tanggal ──────────────────────────
        public static List<TransaksiRow> GetByRange(DateTime dari, DateTime sampai)
        {
            var list = new List<TransaksiRow>();
            string sql = @"
                SELECT id_transaksi, waktu, id_penguna, nama_toko, nama_produk, total
                FROM transaksi
                WHERE waktu >= @dari AND waktu < @sampai
                ORDER BY waktu DESC";

            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("dari",   dari.Date);
            cmd.Parameters.AddWithValue("sampai", sampai.Date.AddDays(1));
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                list.Add(new TransaksiRow
                {
                    IdTransaksi = r.GetInt32(0),
                    Waktu       = r.GetDateTime(1),
                    NamaKasir   = r.IsDBNull(2) ? "-" : r.GetInt32(2).ToString(),
                    NamaToko    = r.IsDBNull(3) ? "-" : r.GetString(3),
                    NamaProduk  = r.IsDBNull(4) ? "-" : r.GetString(4),
                    Total       = r.IsDBNull(5) ? 0   : r.GetDecimal(5)
                });
            }
            return list;
        }

        // ── Summary: total transaksi & total pemasukan dalam range ───────────
        public static (int jumlah, decimal pemasukan) GetRangeSummary(DateTime dari, DateTime sampai)
        {
            string sql = @"
                SELECT COUNT(*), COALESCE(SUM(total), 0)
                FROM transaksi
                WHERE waktu >= @dari AND waktu < @sampai";
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("dari",   dari.Date);
            cmd.Parameters.AddWithValue("sampai", sampai.Date.AddDays(1));
            using var r = cmd.ExecuteReader();
            r.Read();
            return ((int)r.GetInt64(0), r.GetDecimal(1));
        }

        // ── Simpan transaksi baru (ACID) ──────────────────────────────────────
        public static bool SimpanTransaksi(int idPengguna, string namaToko,
                                           string namaProduk, int idProduk,
                                           int jumlah, decimal hargaSatuan)
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var trx = conn.BeginTransaction();
            try
            {
                // Cek stok
                using var cekCmd = new NpgsqlCommand(
                    "SELECT stok FROM produk WHERE id_produk = @id FOR UPDATE", conn, trx);
                cekCmd.Parameters.AddWithValue("id", idProduk);
                var stokObj = cekCmd.ExecuteScalar();
                if (stokObj == null || (int)stokObj < jumlah)
                    throw new Exception("Stok tidak mencukupi.");

                decimal total = hargaSatuan * jumlah;

                // Insert transaksi
                using var ins = new NpgsqlCommand(@"
                    INSERT INTO transaksi (id_penguna, nama_toko, nama_produk, total)
                    VALUES (@idp, @toko, @produk, @total)
                    RETURNING id_transaksi", conn, trx);
                ins.Parameters.AddWithValue("idp",    idPengguna);
                ins.Parameters.AddWithValue("toko",   namaToko);
                ins.Parameters.AddWithValue("produk", namaProduk);
                ins.Parameters.AddWithValue("total",  total);
                int idTrx = Convert.ToInt32(ins.ExecuteScalar());

                // Insert item_transaksi
                using var insItem = new NpgsqlCommand(@"
                    INSERT INTO item_transaksi (id_transaksi, id_produk, jumlah, harga_satuan, subtotal)
                    VALUES (@trx, @prod, @jml, @harga, @sub)", conn, trx);
                insItem.Parameters.AddWithValue("trx",   idTrx);
                insItem.Parameters.AddWithValue("prod",  idProduk);
                insItem.Parameters.AddWithValue("jml",   jumlah);
                insItem.Parameters.AddWithValue("harga", hargaSatuan);
                insItem.Parameters.AddWithValue("sub",   total);
                insItem.ExecuteNonQuery();

                // Kurangi stok
                using var updStok = new NpgsqlCommand(
                    "UPDATE produk SET stok = stok - @jml WHERE id_produk = @id", conn, trx);
                updStok.Parameters.AddWithValue("jml", jumlah);
                updStok.Parameters.AddWithValue("id",  idProduk);
                updStok.ExecuteNonQuery();

                trx.Commit();
                return true;
            }
            catch
            {
                trx.Rollback();
                throw;
            }
        }
    }
}
