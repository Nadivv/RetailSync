using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailSync.Models
{
    public class Kategori
    {
        public int IdKategori { get; set; }
        public string NamaKategori { get; set; }

        public Kategori(int id, string nama)
        {
            IdKategori = id;
            NamaKategori = nama;
        }
    }
}
