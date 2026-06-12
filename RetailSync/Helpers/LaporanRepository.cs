using Npgsql;
using System;
using System.Collections.Generic;

namespace RetailSync.Helpers
{
    public class ProdukTerlaris
    {
        public int     No           { get; set; }
        public string  NamaProduk   { get; set; } = "";
        public int     TotalTerjual { get; set; }
        public decimal TotalPenjualan { get; set; }
    }

    public class DetailTransaksiRow
    {
        public int      IdTransaksi { get; set; }
        public DateTime Waktu       { get; set; }
        public string   Pelanggan   { get; set; } = "";  // nama_toko / id_penguna
        public decimal  Total       { get; set; }
        public string   Kasir       { get; set; } = "";
    }

    static class LaporanRepository
    {
        // ── Summary harian ────────────────────────────────────────────────────
        public static (decimal totalPenjualan, int totalTransaksi, int produkTerjual)
            GetSummaryHarian(DateTime tanggal)
        {
            string sql = @"
                SELECT
                    COALESCE(SUM(t.total), 0)           AS total_penjualan,
                    COUNT(DISTINCT t.id_transaksi)       AS total_transaksi,
                    COALESCE(SUM(it.jumlah), 0)         AS produk_terjual
                FROM transaksi t
                LEFT JOIN item_transaksi it ON it.id_transaksi = t.id_transaksi
                WHERE DATE(t.waktu) = @tgl";

            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("tgl", tanggal.Date);
            using var r = cmd.ExecuteReader();
            r.Read();
            return (r.GetDecimal(0), (int)r.GetInt64(1), (int)r.GetInt64(2));
        }

        // ── Produk terlaris hari ini ──────────────────────────────────────────
        public static List<ProdukTerlaris> GetProdukTerlaris(DateTime tanggal, int topN = 10)
        {
            var list = new List<ProdukTerlaris>();
            string sql = @"
                SELECT
                    p.nama_produk,
                    SUM(it.jumlah)    AS total_terjual,
                    SUM(it.subtotal)  AS total_penjualan
                FROM item_transaksi it
                JOIN transaksi t ON t.id_transaksi = it.id_transaksi
                JOIN produk p    ON p.id_produk    = it.id_produk
                WHERE DATE(t.waktu) = @tgl
                GROUP BY p.id_produk, p.nama_produk
                ORDER BY total_terjual DESC
                LIMIT @topn";

            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("tgl",  tanggal.Date);
            cmd.Parameters.AddWithValue("topn", topN);
            using var r = cmd.ExecuteReader();
            int no = 1;
            while (r.Read())
            {
                list.Add(new ProdukTerlaris
                {
                    No            = no++,
                    NamaProduk    = r.GetString(0),
                    TotalTerjual  = (int)r.GetInt64(1),
                    TotalPenjualan = r.GetDecimal(2)
                });
            }
            return list;
        }

        // ── Detail transaksi hari ini ─────────────────────────────────────────
        public static List<DetailTransaksiRow> GetDetailTransaksi(DateTime tanggal)
        {
            var list = new List<DetailTransaksiRow>();
            string sql = @"
                SELECT
                    t.id_transaksi,
                    t.waktu,
                    COALESCE(t.nama_toko, '-')           AS pelanggan,
                    t.total,
                    COALESCE(p.nama, t.id_penguna::text) AS kasir
                FROM transaksi t
                LEFT JOIN pengguna p ON p.id_pengguna = t.id_penguna
                WHERE DATE(t.waktu) = @tgl
                ORDER BY t.waktu DESC";

            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("tgl", tanggal.Date);
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                list.Add(new DetailTransaksiRow
                {
                    IdTransaksi = r.GetInt32(0),
                    Waktu       = r.GetDateTime(1),
                    Pelanggan   = r.GetString(2),
                    Total       = r.IsDBNull(3) ? 0 : r.GetDecimal(3),
                    Kasir       = r.IsDBNull(4) ? "-" : r.GetString(4)
                });
            }
            return list;
        }
    }
}
