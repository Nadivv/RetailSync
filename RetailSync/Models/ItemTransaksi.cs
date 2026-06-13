using Npgsql;
using RetailSync.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailSync.Models
{
    public class ItemTransaksi
    {
        public int IdItemTransaksi { get; set; }
        public int IdTransaksi { get; set; }

        public Produk ProdukTerkait { get; set; }
        public int Jumlah { get; set; }
        public decimal HargaSatuan { get; set; }
        public decimal Subtotal => HargaSatuan * Jumlah;

        public ItemTransaksi(Produk produk, int jumlah)
        {
            ProdukTerkait = produk;
            Jumlah = jumlah;
            HargaSatuan = produk.HargaJual; // Mengunci harga jual saat transaksi terjadi
        }
    }
}
