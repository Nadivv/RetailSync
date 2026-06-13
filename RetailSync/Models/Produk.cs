using Npgsql;
using RetailSync.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailSync.Models
{
    public class Produk
    {
        public int IdProduk { get; set; }
        public string NamaProduk { get; set; }
        public int IdKategori { get; set; } // Relasi ke Kategori
        public decimal HargaBeli { get; set; }
        public decimal HargaJual { get; set; }
        public int Stok { get; set; }

        public Produk(int id, string nama, int idKategori, decimal hargaBeli, decimal hargaJual, int stok)
        {
            IdProduk = id;
            NamaProduk = nama;
            IdKategori = idKategori;
            HargaBeli = hargaBeli;
            HargaJual = hargaJual;
            Stok = stok;
        }
    }
}
