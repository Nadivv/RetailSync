namespace RetailSync.Models
{
    // Simpan sesi user yang sedang login (static agar bisa diakses dari mana saja)
    public static class UserContext
    {
        public static int IdPengguna { get; set; }
        public static string Nama { get; set; } = "";
        public static string Username { get; set; } = "";
        public static int IdRole { get; set; }
        public static string NamaRole { get; set; } = "";
    }
}
