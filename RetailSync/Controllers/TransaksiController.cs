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
    public class TransaksiController
    {
        // Menangani penyimpanan data yang aman dan berurutan sesuai skema ACID di file SQL Anda
        public bool SimpanTransaksi(Transaksi transaksi)
        {
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var tx = conn.BeginTransaction(); // Memulai blok aman TRANSAKSI DATABASE

            try
            {
                // Langkah A: Insert data utama transaksi
                string sqlTransaksi = "INSERT INTO transaksi (id_pengguna, total, waktu) VALUES (@idUser, @total, @waktu) RETURNING id_transaksi;";
                int newTransaksiId;
                using (var cmd = new NpgsqlCommand(sqlTransaksi, conn))
                {
                    cmd.Parameters.AddWithValue("idUser", transaksi.IdPengguna);
                    cmd.Parameters.AddWithValue("total", transaksi.Total);
                    cmd.Parameters.AddWithValue("waktu", transaksi.Waktu);
                    newTransaksiId = Convert.ToInt32(cmd.ExecuteScalar());
                }

                foreach (var item in transaksi.Items)
                {
                    // Langkah B: Kurangi stok produk secara berkala di database
                    string sqlUpdateStok = "UPDATE produk SET stok = stok - @qty WHERE id_produk = @idProd AND stok >= @qty";
                    using (var cmdUpdate = new NpgsqlCommand(sqlUpdateStok, conn))
                    {
                        cmdUpdate.Parameters.AddWithValue("qty", item.Jumlah);
                        cmdUpdate.Parameters.AddWithValue("idProd", item.ProdukTerkait.IdProduk);
                        int rowsAffected = cmdUpdate.ExecuteNonQuery();
                        if (rowsAffected == 0)
                        {
                            throw new Exception($"Stok produk '{item.ProdukTerkait.NamaProduk}' gagal diperbarui atau tidak mencukupi!");
                        }
                    }

                    // Langkah C: Catat baris data item transaksi ke tabel detail
                    string sqlItem = @"INSERT INTO item_transaksi (id_transaksi, id_produk, jumlah, harga_satuan, subtotal) 
                                       VALUES (@idTx, @idProd, @qty, @harga, @sub)";
                    using (var cmdItem = new NpgsqlCommand(sqlItem, conn))
                    {
                        cmdItem.Parameters.AddWithValue("idTx", newTransaksiId);
                        cmdItem.Parameters.AddWithValue("idProd", item.ProdukTerkait.IdProduk);
                        cmdItem.Parameters.AddWithValue("qty", item.Jumlah);
                        cmdItem.Parameters.AddWithValue("harga", item.HargaSatuan);
                        cmdItem.Parameters.AddWithValue("sub", item.Subtotal);
                        cmdItem.ExecuteNonQuery();
                    }
                }

                tx.Commit(); // Berhasil menyimpan seluruh rangkaian data ke database
                return true;
            }
            catch (Exception ex)
            {
                tx.Rollback(); // Batalkan semua perubahan jika terjadi kegagalan sistem di tengah jalan
                Console.WriteLine("Transaksi Gagal: " + ex.Message);
                return false;
            }
        }
    }
}
