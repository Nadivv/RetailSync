using Npgsql;
using System;
using System.Collections.Generic;

namespace RetailSync.Helpers
{
    public class KeuanganRow
    {
        public DateTime Tanggal     { get; set; }
        public string   Kategori    { get; set; } = "";
        public string   Deskripsi   { get; set; } = "";
        public decimal  Pemasukan   { get; set; }
        public decimal  Pengeluaran { get; set; }
        public decimal  Saldo       { get; set; }
        public string   Metode      { get; set; } = "Tunai";
        public string   Keterangan  { get; set; } = "";

        public string Tipe => Pemasukan > 0 ? "Pemasukan" : "Pengeluaran";
    }

    static class KeuanganRepository
    {
        // ── Summary keuangan dalam range ──────────────────────────────────────
        public static (decimal pemasukan, int jmlTrxMasuk,
                       decimal pengeluaran, int jmlTrxKeluar,
                       decimal labaBersih, decimal saldoAkhir)
            GetSummary(DateTime dari, DateTime sampai)
        {
            string sql = @"
                SELECT
                    COALESCE(SUM(total), 0)        AS pemasukan,
                    COUNT(*)                       AS jml_trx
                FROM transaksi
                WHERE waktu >= @dari AND waktu < @sampai";

            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("dari",   dari.Date);
            cmd.Parameters.AddWithValue("sampai", sampai.Date.AddDays(1));
            using var r = cmd.ExecuteReader();
            r.Read();
            decimal pemasukan = r.GetDecimal(0);
            int     jml       = (int)r.GetInt64(1);
            r.Close();

            // Estimasi pengeluaran = HPP sederhana (60% dari pemasukan sebagai placeholder)
            // karena DB tidak punya tabel pengeluaran/hpp
            decimal pengeluaran = Math.Round(pemasukan * 0.6m, 0);
            decimal labaBersih  = pemasukan - pengeluaran;
            decimal saldoAkhir  = pemasukan; // saldo = semua pemasukan

            return (pemasukan, jml, pengeluaran, 0, labaBersih, saldoAkhir);
        }

        // ── Rincian transaksi sebagai baris keuangan ──────────────────────────
        public static List<KeuanganRow> GetRincian(DateTime dari, DateTime sampai, string filter = "Semua")
        {
            var list = new List<KeuanganRow>();
            string sql = @"
                SELECT
                    t.waktu,
                    'Penjualan'                             AS kategori,
                    COALESCE(t.nama_produk, 'Transaksi')   AS deskripsi,
                    t.total                                AS pemasukan,
                    0::numeric                             AS pengeluaran,
                    t.total                                AS saldo,
                    'Tunai'                                AS metode,
                    COALESCE(t.nama_toko, '-')             AS keterangan
                FROM transaksi t
                WHERE t.waktu >= @dari AND t.waktu < @sampai
                ORDER BY t.waktu DESC";

            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("dari",   dari.Date);
            cmd.Parameters.AddWithValue("sampai", sampai.Date.AddDays(1));
            using var r = cmd.ExecuteReader();
            decimal saldo = 0;
            while (r.Read())
            {
                decimal masuk  = r.GetDecimal(3);
                decimal keluar = r.GetDecimal(4);
                saldo += masuk - keluar;

                var row = new KeuanganRow
                {
                    Tanggal     = r.GetDateTime(0),
                    Kategori    = r.GetString(1),
                    Deskripsi   = r.GetString(2),
                    Pemasukan   = masuk,
                    Pengeluaran = keluar,
                    Saldo       = masuk,
                    Metode      = r.GetString(6),
                    Keterangan  = r.GetString(7)
                };

                if (filter == "Semua"
                    || (filter == "Pemasukan"   && masuk  > 0)
                    || (filter == "Pengeluaran" && keluar > 0))
                    list.Add(row);
            }
            return list;
        }
    }
}
