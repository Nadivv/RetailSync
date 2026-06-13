using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailSync.Models
{
    public class Transaksi
    {
        public int IdTransaksi { get; set; }
        public int IdPengguna { get; set; } // Berelasi dengan id_pengguna di database
        public DateTime Waktu { get; set; }
        public List<ItemTransaksi> Items { get; set; }
        public decimal Total => Items?.Sum(i => i.Subtotal) ?? 0;

        public Transaksi(int idPengguna)
        {
            IdPengguna = idPengguna;
            Waktu = DateTime.Now;
            Items = new List<ItemTransaksi>();
        }
    }
}
