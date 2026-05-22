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
    //  MODEL: Produk
    // ═══════════════════════════════════════════════════
    class Produk
    {
        public int IdProduk { get; set; }
        public string NamaProduk { get; set; }
        public string Kategori { get; set; }
        public double Harga { get; set; }
        public int Stok { get; set; }

        public Produk(int id, string nama, string kategori, double harga, int stok)
        {
            IdProduk = id;
            NamaProduk = nama;
            Kategori = kategori;
            Harga = harga;
            Stok = stok;
        }

        public override string ToString()
            => $"[{IdProduk:D3}] {NamaProduk,-25} | {Kategori,-15} | Rp {Harga,12:N0} | Stok: {Stok,5}";
    }

    // ═══════════════════════════════════════════════════
    //  COMPOSITION: Inventori memiliki List<Produk>
    //  Produk tidak bisa hidup tanpa Inventori
    //  Implements: ICRUDable, IMonitorable, ILaporanable
    // ═══════════════════════════════════════════════════
    class Inventori : ICRUDable, IMonitorable, ILaporanable
    {
        private List<Produk> _daftarProduk;  // Composition

        public Inventori()
        {
            _daftarProduk = new List<Produk>();
        }

        // ── ICRUDable ────────────────────────────────
        public void Tambah(string nama, string kategori, double harga, int stok)
        {
            string sql = @"INSERT INTO produk (nama_produk, kategori, harga, stok)
                           VALUES (@nama, @kat, @harga, @stok)";
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("nama", nama);
            cmd.Parameters.AddWithValue("kat", kategori);
            cmd.Parameters.AddWithValue("harga", harga);
            cmd.Parameters.AddWithValue("stok", stok);
            cmd.ExecuteNonQuery();
        }

        public List<Produk> Tampilkan()
        {
            _daftarProduk.Clear();
            string sql = "SELECT id_produk, nama_produk, kategori, harga, stok FROM produk ORDER BY id_produk";
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var cmd = new NpgsqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                _daftarProduk.Add(new Produk(
                    reader.GetInt32(0),
                    reader.GetString(1),
                    reader.GetString(2),
                    (double)reader.GetDecimal(3),
                    reader.GetInt32(4)
                ));
            return new List<Produk>(_daftarProduk);
        }

        public void Update(int id, double hargaBaru, int stokBaru)
        {
            string sql = "UPDATE produk SET harga = @harga, stok = @stok WHERE id_produk = @id";
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("harga", hargaBaru);
            cmd.Parameters.AddWithValue("stok", stokBaru);
            cmd.Parameters.AddWithValue("id", id);
            cmd.ExecuteNonQuery();
        }

        public void Hapus(int id)
        {
            string sql = "DELETE FROM produk WHERE id_produk = @id";
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("id", id);
            cmd.ExecuteNonQuery();
        }

        // ── IMonitorable ─────────────────────────────
        public List<Produk> MonitorStok() => Tampilkan();

        // ── ILaporanable ─────────────────────────────
        public string CetakLaporan()
        {
            var list = Tampilkan();
            var sb = new StringBuilder();
            sb.AppendLine($"LAPORAN INVENTORI — {DateTime.Now:dd/MM/yyyy HH:mm}");
            sb.AppendLine(new string('─', 70));
            sb.AppendLine($"{"ID",-6} {"Nama Produk",-28} {"Kategori",-15} {"Harga",14} {"Stok",6}");
            sb.AppendLine(new string('─', 70));
            double nilaiTotal = 0;
            foreach (var p in list)
            {
                sb.AppendLine($"{p.IdProduk,-6} {p.NamaProduk,-28} {p.Kategori,-15} Rp {p.Harga,11:N0} {p.Stok,6}");
                nilaiTotal += p.Harga * p.Stok;
            }
            sb.AppendLine(new string('─', 70));
            sb.AppendLine($"Total Nilai Stok: Rp {nilaiTotal:N0}");
            return sb.ToString();
        }

        // ── Transfer Stok ────────────────────────────
        public void TransferStok(int idProduk, string tujuanCabang, int jumlah)
        {
            string sql = @"INSERT INTO transfer_stok (id_produk, tujuan_cabang, jumlah, tgl_transfer)
                           VALUES (@id, @cab, @jml, NOW())";
            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("id", idProduk);
            cmd.Parameters.AddWithValue("cab", tujuanCabang);
            cmd.Parameters.AddWithValue("jml", jumlah);
            cmd.ExecuteNonQuery();
        }

        public List<Produk> GetSemuaProduk() => Tampilkan();
    }
}
