using System;
using System.Collections.Generic;

namespace RetailSync.Models
{
    // Pilar ABSTRAKSI: Kelas dibuat 'abstract' karena tidak boleh diinstansiasi secara langsung
    public abstract class Pengguna
    {
        // Pilar ENKAPSULASI: Menggunakan private dan properti get; dan set;
        private int IdPengguna { get; set; }
        private string Nama { get; set; }
        private string Username { get; set; }
        private int IdRole { get; set; }
        private bool IsAktif { get; set; }

        protected Pengguna(int id, string nama, string username, int idRole, bool isAktif)
        {
            IdPengguna = id;
            Nama = nama;
            Username = username;
            IdRole = idRole;
            IsAktif = isAktif;
        }

        // Pilar POLIMORFISME: Metode virtual/abstract yang wajib diimplementasikan ulang oleh turunannya
        public abstract string GetRoleLabel();
        public abstract string[] GetMenuItems();
    }
}