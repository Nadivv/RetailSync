namespace RetailSync.Models
{
    /// <summary>
    /// Kelas static untuk menyimpan informasi sesi (session) pengguna yang sedang aktif.
    /// Karena bersifat static, data ini dapat diakses langsung dari Form atau Controller mana pun tanpa perlu instansiasi.
    /// </summary>
    public static class UserContext
    {
        // Menyimpan ID unik pengguna dari database (Primary Key)
        public static int IdPengguna { get; set; }

        // Menyimpan Nama lengkap pengguna (misal: "Budi Santoso")
        public static string Nama { get; set; } = string.Empty;

        // Menyimpan Username yang digunakan untuk login (misal: "budi_mgr")
        public static string Username { get; set; } = string.Empty;

        // Menyimpan ID Role dari tabel roles (misal: 1 untuk Admin, 2 untuk Manager)
        public static int IdRole { get; set; }

        // Menyimpan Nama Hak Akses / Jabatan (misal: "Admin" atau "Manager")
        public static string NamaRole { get; set; } = string.Empty;

        /// Fungsi untuk membersihkan semua data sesi saat pengguna Logout
        public static void Clear()
        {
            IdPengguna = 0;
            Nama = string.Empty;
            Username = string.Empty;
            IdRole = 0;
            NamaRole = string.Empty;
        }
    }
}
