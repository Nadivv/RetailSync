using System;
using System.Collections.Generic;

namespace RetailSync.Models
{
    // ═══════════════════════════════════════════════════
    // INTERFACES
    // ═══════════════════════════════════════════════════

    interface ICRUDable
    {
        void Tambah(string nama, string kategori, double harga, int stok);
        List<Produk> Tampilkan();
        void Update(int id, double hargaBaru, int stokBaru);
        void Hapus(int id);
    }

    interface ILaporanable
    {
        string CetakLaporan();
    }

    interface IMonitorable
    {
        List<Produk> MonitorStok();
    }

    // ═══════════════════════════════════════════════════
    // ABSTRACT CLASS PENGGUNA
    // ═══════════════════════════════════════════════════

    abstract class Pengguna
    {
        private int _idPengguna;
        private string _nama;
        private string _username;
        private string _role;

        public int IdPengguna
        {
            get { return _idPengguna; }
            protected set { _idPengguna = value; }
        }

        public string Nama
        {
            get { return _nama; }
            protected set { _nama = value; }
        }

        public string Username
        {
            get { return _username; }
            protected set { _username = value; }
        }

        public string Role
        {
            get { return _role; }
            protected set { _role = value; }
        }

        protected Pengguna(int id, string nama, string username, string role)
        {
            _idPengguna = id;
            _nama = nama;
            _username = username;
            _role = role;
        }

        public abstract string GetRoleLabel();
        public abstract string[] GetMenuItems();
    }

    // ═══════════════════════════════════════════════════
    // CLASS USER
    // ═══════════════════════════════════════════════════

    class User : Pengguna
    {
        public string Password { get; set; }

        public User(
            int id,
            string nama,
            string username,
            string password,
            string role
        ) : base(id, nama, username, role)
        {
            Password = password;
        }

        public override string GetRoleLabel()
        {
            return $"Role: {Role}";
        }

        public override string[] GetMenuItems()
        {
            if (Role == "Admin")
            {
                return new string[]
                {
                    "Dashboard",
                    "Kelola Produk",
                    "Kelola User",
                    "Laporan"
                };
            }

            return new string[]
            {
                "Dashboard",
                "Produk"
            };
        }
    }
}