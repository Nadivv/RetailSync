using Npgsql;
using System;
using System.Collections.Generic;

namespace RetailSync.Helpers
{
    public class RestockRow
    {
        public int      IdRestock      { get; set; }
        public DateTime Tanggal        { get; set; }
        public string   NamaProduk     { get; set; } = "";
        public int      JumlahDitambah { get; set; }
        public int      StokSebelum    { get; set; }
        public int      StokSetelah    { get; set; }
    }

    static class RestockRepository
    {
        // ── Simpan restock + update stok produk ───────────────────────────────
        public static void SimpanRestock(int idProduk, int jumlahTambah, int stokSekarang)
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var trx = conn.BeginTransaction();
            try
            {
                // Insert ke riwayat_harga sebagai log (kita pakai tabel sendiri kalau ada,
                // atau simpan ke kolom notes di riwayat_harga dengan harga_lama = stok lama)
                // Karena DB tidak punya tabel restock, kita update stok produk saja
                // dan catat di tabel riwayat_harga dengan keterangan jumlah
                int stokBaru = stokSekarang + jumlahTambah;

                using var upd = new NpgsqlCommand(
                    "UPDATE produk SET stok = @stok WHERE id_produk = @id", conn, trx);
                upd.Parameters.AddWithValue("stok", stokBaru);
                upd.Parameters.AddWithValue("id",   idProduk);
                upd.ExecuteNonQuery();

                // Catat ke riwayat_harga sebagai log restock (harga_lama = stok sebelum, harga_baru = stok sesudah)
                using var log = new NpgsqlCommand(@"
                    INSERT INTO riwayat_harga (id_produk, nama_produk, harga_lama, harga_baru)
                    SELECT id_produk, CONCAT('[RESTOCK+', @tambah::text, '] ', nama_produk),
                           @sebelum, @sesudah
                    FROM produk WHERE id_produk = @id", conn, trx);
                log.Parameters.AddWithValue("tambah",  jumlahTambah);
                log.Parameters.AddWithValue("sebelum", (decimal)stokSekarang);
                log.Parameters.AddWithValue("sesudah", (decimal)stokBaru);
                log.Parameters.AddWithValue("id",      idProduk);
                log.ExecuteNonQuery();

                trx.Commit();
            }
            catch
            {
                trx.Rollback();
                throw;
            }
        }

        // ── Ambil riwayat restock dari riwayat_harga ──────────────────────────
        public static List<RestockRow> GetRiwayat(int top = 20)
        {
            var list = new List<RestockRow>();
            string sql = @"
                SELECT
                    id_riwayat_harga,
                    tgl_ubah,
                    nama_produk,
                    CAST(harga_baru - harga_lama AS INT)  AS jumlah_tambah,
                    CAST(harga_lama AS INT)               AS stok_sebelum,
                    CAST(harga_baru AS INT)               AS stok_setelah
                FROM riwayat_harga
                WHERE nama_produk LIKE '[RESTOCK%'
                ORDER BY tgl_ubah DESC
                LIMIT @top";

            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("top", top);
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                list.Add(new RestockRow
                {
                    IdRestock      = r.GetInt32(0),
                    Tanggal        = r.GetDateTime(1),
                    NamaProduk     = r.GetString(2),
                    JumlahDitambah = r.GetInt32(3),
                    StokSebelum    = r.GetInt32(4),
                    StokSetelah    = r.GetInt32(5)
                });
            }
            return list;
        }
    }
}
