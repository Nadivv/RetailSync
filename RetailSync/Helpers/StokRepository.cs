using Npgsql;
using System;
using System.Collections.Generic;

namespace RetailSync.Helpers
{
    public class StokRow
    {
        public int    IdProduk          { get; set; }
        public string NamaProduk        { get; set; } = "";
        public string Kategori          { get; set; } = "";
        public int    Stok              { get; set; }
        public string PerubahanTerakhir { get; set; } = "-";
        public string Sumber            { get; set; } = "-";

        public string Status => Stok == 0 ? "Stok Habis"
                              : Stok <= 10 ? "Stok Menipis"
                              : "Tersedia";
    }

    static class StokRepository
    {
        // ── Semua produk + info perubahan stok terakhir dari item_transaksi ───
        public static List<StokRow> GetStokLengkap()
        {
            var list = new List<StokRow>();
            // Ambil produk beserta waktu & sumber perubahan stok terakhir
            // Sumber = 'Transaksi' jika ada di item_transaksi, 'Manual' jika tidak
            string sql = @"
                SELECT
                    p.id_produk,
                    p.nama_produk,
                    k.nama_kategori,
                    p.stok,
                    COALESCE(
                        TO_CHAR(MAX(t.waktu), 'DD/MM/YY HH24:MI'),
                        TO_CHAR(p.created_at, 'DD/MM/YY HH24:MI')
                    ) AS perubahan_terakhir,
                    CASE WHEN MAX(t.waktu) IS NOT NULL THEN 'Transaksi'
                         ELSE 'Manual'
                    END AS sumber
                FROM produk p
                JOIN kategori k ON p.id_kategori = k.id_kategori
                LEFT JOIN item_transaksi it ON it.id_produk = p.id_produk
                LEFT JOIN transaksi t       ON t.id_transaksi = it.id_transaksi
                GROUP BY p.id_produk, p.nama_produk, k.nama_kategori, p.stok, p.created_at
                ORDER BY p.id_produk";

            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var cmd = new NpgsqlCommand(sql, conn);
            using var r   = cmd.ExecuteReader();
            while (r.Read())
            {
                list.Add(new StokRow
                {
                    IdProduk          = r.GetInt32(0),
                    NamaProduk        = r.GetString(1),
                    Kategori          = r.GetString(2),
                    Stok              = r.GetInt32(3),
                    PerubahanTerakhir = r.IsDBNull(4) ? "-" : r.GetString(4),
                    Sumber            = r.IsDBNull(5) ? "-" : r.GetString(5)
                });
            }
            return list;
        }

        // ── Summary: total produk, masuk & keluar hari ini, menipis ──────────
        public static (int total, int masukHariIni, int keluarHariIni, int menipis) GetStokSummary()
        {
            string sql = @"
                SELECT
                    (SELECT COUNT(*) FROM produk) AS total,
                    -- stok masuk: produk yang created_at hari ini (baru ditambah)
                    (SELECT COUNT(*) FROM produk
                     WHERE DATE(created_at) = CURRENT_DATE) AS masuk,
                    -- stok keluar: jumlah item transaksi terjual hari ini
                    (SELECT COUNT(DISTINCT id_produk) FROM item_transaksi it
                     JOIN transaksi t ON t.id_transaksi = it.id_transaksi
                     WHERE DATE(t.waktu) = CURRENT_DATE) AS keluar,
                    (SELECT COUNT(*) FROM produk WHERE stok > 0 AND stok <= 10) AS menipis";

            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var cmd = new NpgsqlCommand(sql, conn);
            using var r   = cmd.ExecuteReader();
            r.Read();
            return (
                r.GetInt32(0),
                r.GetInt32(1),
                r.GetInt32(2),
                r.GetInt32(3)
            );
        }
    }
}
