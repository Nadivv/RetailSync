using Npgsql;
using RetailSync.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailSync.Models
{
    // ═══════════════════════════════════════════════════
    //  MODEL: ItemTransaksi  (Aggregation)
    //  Bisa berdiri sendiri, di-aggregate oleh Transaksi
    // ═══════════════════════════════════════════════════
    class ItemTransaksi
    {
        public Produk Produk { get; set; }
        public int Jumlah { get; set; }
        public double Subtotal => Produk.Harga * Jumlah;

        public ItemTransaksi(Produk produk, int jumlah)
        {
            Produk = produk;
            Jumlah = jumlah;
        }
    }

    // ═══════════════════════════════════════════════════
    //  MODEL: Transaksi
    //  Association  → menyimpan referensi Pengguna (Kasir)
    //  Aggregation  → memiliki List<ItemTransaksi>
    // ═══════════════════════════════════════════════════
    class Transaksi
    {
        public int IdTransaksi { get; set; }
        public DateTime Waktu { get; set; }
        public Pengguna Kasir { get; set; }   // Association
        public List<ItemTransaksi> Items { get; set; }   // Aggregation

        public double Total => Items?.Sum(i => i.Subtotal) ?? 0;

        public Transaksi(int id, Pengguna kasir)
        {
            IdTransaksi = id;
            Waktu = DateTime.Now;
            Kasir = kasir;
            Items = new List<ItemTransaksi>();
        }

        public void TambahItem(ItemTransaksi item) => Items.Add(item);

        // Simpan transaksi ke database
        public void SimpanKeDB()
        {
            string sql = @"INSERT INTO transaksi (nama_kasir, nama_cabang, total, waktu)
                           VALUES (@kasir, @cabang, @total, @waktu)";
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("kasir", Kasir.Nama);
            cmd.Parameters.AddWithValue("cabang", Kasir is BranchManager bm ? bm.NamaCabang : "HQ");
            cmd.Parameters.AddWithValue("total", Total);
            cmd.Parameters.AddWithValue("waktu", Waktu);
            cmd.ExecuteNonQuery();
        }
    }
}
