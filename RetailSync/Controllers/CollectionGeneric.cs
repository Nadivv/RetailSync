using Npgsql;
using RetailSync.Helpers;
using RetailSync.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailSync.Controllers
{
    public class ProdukController
    {
        public List<Produk> AmbilSemuaProduk()
        {
            var list = new List<Produk>();
            string sql = "SELECT id_produk, nama_produk, id_kategori, harga_beli, harga_jual, stok FROM produk ORDER BY id_produk";

            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var cmd = new NpgsqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Produk(
                    reader.GetInt32(0),
                    reader.GetString(1),
                    reader.GetInt32(2),
                    reader.GetDecimal(3),
                    reader.GetDecimal(4),
                    reader.GetInt32(5)
                ));
            }
            return list;
        }

        public void TambahProduk(Produk p)
        {
            string sql = @"INSERT INTO produk (nama_produk, id_kategori, harga_beli, harga_jual, stok) 
                           VALUES (@nama, @kat, @hbeli, @hjual, @stok)";

            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("nama", p.NamaProduk);
            cmd.Parameters.AddWithValue("kat", p.IdKategori);
            cmd.Parameters.AddWithValue("hbeli", p.HargaBeli);
            cmd.Parameters.AddWithValue("hjual", p.HargaJual);
            cmd.Parameters.AddWithValue("stok", p.Stok);
            cmd.ExecuteNonQuery();
        }
    }
}