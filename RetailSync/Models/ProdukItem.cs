using System;

namespace RetailSync.Models
{
    // Model lengkap untuk tampilan inventori
    public class ProdukItem
    {
        public int IdProduk { get; set; }
        public string NamaProduk { get; set; }
        public string Kategori { get; set; }
        public decimal Harga { get; set; }
        public int Stok { get; set; }
        public DateTime? TglExpired { get; set; }
        public bool IsExpired { get; set; }

        public string StatusStok
        {
            get
            {
                if (Stok == 0) return "Stok Habis";
                if (Stok <= 10) return "Stok Menipis";
                return "Tersedia";
            }
        }

        public ProdukItem() { }
    }
}
