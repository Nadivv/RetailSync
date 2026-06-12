using Npgsql;
using RetailSync.Models;
using System;
using System.Collections.Generic;

namespace RetailSync.Helpers
{
    static class ProdukRepository
    {
        // ── Ambil semua produk dengan join kategori ──────────────────────────
        public static List<ProdukItem> GetAll()
        {
            var list = new List<ProdukItem>();
            string sql = @"
                SELECT p.id_produk, p.nama_produk, k.nama_kategori,
                       p.harga, p.stok, p.tgl_expired, p.is_expired
                FROM produk p
                JOIN kategori k ON p.id_kategori = k.id_kategori
                ORDER BY p.id_produk";

            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var cmd = new NpgsqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new ProdukItem
                {
                    IdProduk    = reader.GetInt32(0),
                    NamaProduk  = reader.GetString(1),
                    Kategori    = reader.GetString(2),
                    Harga       = reader.GetDecimal(3),
                    Stok        = reader.GetInt32(4),
                    TglExpired  = reader.IsDBNull(5) ? null : reader.GetDateTime(5),
                    IsExpired   = reader.GetBoolean(6)
                });
            }
            return list;
        }

        // ── Summary counts ───────────────────────────────────────────────────
        public static (int total, int tersedia, int menipis, int habis) GetSummary()
        {
            string sql = @"
                SELECT
                    COUNT(*)                                   AS total,
                    COUNT(*) FILTER (WHERE stok > 10)          AS tersedia,
                    COUNT(*) FILTER (WHERE stok > 0 AND stok <= 10) AS menipis,
                    COUNT(*) FILTER (WHERE stok = 0)           AS habis
                FROM produk";

            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var cmd = new NpgsqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();
            reader.Read();
            return (
                reader.GetInt32(0),
                reader.GetInt32(1),
                reader.GetInt32(2),
                reader.GetInt32(3)
            );
        }

        // ── Semua nama kategori ───────────────────────────────────────────────
        public static List<string> GetKategori()
        {
            var list = new List<string> { "Semua Kategori" };
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var cmd = new NpgsqlCommand("SELECT nama_kategori FROM kategori ORDER BY nama_kategori", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) list.Add(reader.GetString(0));
            return list;
        }

        // ── Tambah produk ────────────────────────────────────────────────────
        public static void Tambah(string nama, int idKategori, decimal harga, int stok, DateTime? expired)
        {
            string sql = @"INSERT INTO produk (nama_produk, id_kategori, harga, stok, tgl_expired)
                           VALUES (@nama, @kat, @harga, @stok, @exp)";
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("nama",  nama);
            cmd.Parameters.AddWithValue("kat",   idKategori);
            cmd.Parameters.AddWithValue("harga", harga);
            cmd.Parameters.AddWithValue("stok",  stok);
            cmd.Parameters.AddWithValue("exp",   expired.HasValue ? expired.Value : DBNull.Value);
            cmd.ExecuteNonQuery();
        }

        // ── Update lengkap (nama, kategori, harga, stok, expired) ────────────
        public static void Update(int idProduk, decimal hargaBaru, int stokBaru, DateTime? expired,
                                  string? namaBaru = null, int? idKategoriBaru = null)
        {
            string sql = @"UPDATE produk
                           SET harga = @harga, stok = @stok, tgl_expired = @exp
                             , nama_produk  = COALESCE(@nama, nama_produk)
                             , id_kategori  = COALESCE(@kat,  id_kategori)
                           WHERE id_produk = @id";
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("harga", hargaBaru);
            cmd.Parameters.AddWithValue("stok",  stokBaru);
            cmd.Parameters.AddWithValue("exp",   expired.HasValue ? (object)expired.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("nama",  namaBaru != null ? (object)namaBaru : DBNull.Value);
            cmd.Parameters.AddWithValue("kat",   idKategoriBaru.HasValue ? (object)idKategoriBaru.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("id",    idProduk);
            cmd.ExecuteNonQuery();
        }

        // ── Hapus produk ─────────────────────────────────────────────────────
        public static void Hapus(int idProduk)
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var cmd = new NpgsqlCommand("DELETE FROM produk WHERE id_produk = @id", conn);
            cmd.Parameters.AddWithValue("id", idProduk);
            cmd.ExecuteNonQuery();
        }

        // ── Ambil id_kategori berdasarkan nama ────────────────────────────────
        public static int GetIdKategori(string namaKategori)
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var cmd = new NpgsqlCommand(
                "SELECT id_kategori FROM kategori WHERE nama_kategori = @nama", conn);
            cmd.Parameters.AddWithValue("nama", namaKategori);
            var result = cmd.ExecuteScalar();
            return result != null ? Convert.ToInt32(result) : 0;
        }

        // ── Ambil semua kategori beserta id ───────────────────────────────────
        public static List<(int Id, string Nama)> GetKategoriWithId()
        {
            var list = new List<(int, string)>();
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var cmd = new NpgsqlCommand(
                "SELECT id_kategori, nama_kategori FROM kategori ORDER BY nama_kategori", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                list.Add((reader.GetInt32(0), reader.GetString(1)));
            return list;
        }

        // ── Cari produk by id ─────────────────────────────────────────────────
        public static ProdukItem? GetById(int idProduk)
        {
            string sql = @"
                SELECT p.id_produk, p.nama_produk, k.nama_kategori,
                       p.harga, p.stok, p.tgl_expired, p.is_expired
                FROM produk p
                JOIN kategori k ON p.id_kategori = k.id_kategori
                WHERE p.id_produk = @id";
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("id", idProduk);
            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;
            return new ProdukItem
            {
                IdProduk   = reader.GetInt32(0),
                NamaProduk = reader.GetString(1),
                Kategori   = reader.GetString(2),
                Harga      = reader.GetDecimal(3),
                Stok       = reader.GetInt32(4),
                TglExpired = reader.IsDBNull(5) ? null : reader.GetDateTime(5),
                IsExpired  = reader.GetBoolean(6)
            };
        }
    }
}
