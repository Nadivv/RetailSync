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
    public class AutentikasiController
    {
        public Pengguna Login(string username, string password)
        {
            // Query mencocokkan SHA256 sesuai enkripsi SQL bawaan kamu
            string sql = @"SELECT p.id_pengguna, p.nama, p.username, p.id_role, p.is_aktif, r.nama_role 
                           FROM pengguna p
                           JOIN roles r ON p.id_role = r.id_role
                           WHERE p.username = @uname AND p.password = encode(sha256(@pass::bytea), 'hex')";

            using var conn = DatabaseHelper.GetConnection();
            conn.Open();
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("uname", username);
            cmd.Parameters.AddWithValue("pass", password);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                int id = reader.GetInt32(0);
                string nama = reader.GetString(1);
                string uname = reader.GetString(2);
                int idRole = reader.GetInt32(3);
                bool isAktif = reader.GetBoolean(4);
                string namaRole = reader.GetString(5);

                if (!isAktif) return null;

                // Mengembalikan objek polimorfis berdasarkan peran pengguna
                if (namaRole == "Admin")
                    return new Admin(id, nama, uname, isAktif);
                else if (namaRole == "Manager")
                    return new Manager(id, nama, uname, isAktif);
            }
            return null; // Login gagal
        }
    }
}