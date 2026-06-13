using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailSync.Models
{
    // Pilar INHERITANSI: Admin mewarisi seluruh sifat dari Pengguna
    public class Admin : Pengguna
    {
        // ID Role untuk Admin di database adalah 1
        public Admin(int id, string nama, string username, bool isAktif)
            : base(id, nama, username, 1, isAktif) { }

        // Pilar POLIMORFISME: Menimpa fungsionalitas dasar untuk kebutuhan Admin
        public override string GetRoleLabel() => "[ ADMIN — HQ/Owner ]";

        public override string[] GetMenuItems() => new[]
        {
            "1. Kelola Data Produk",
            "2. Lihat Laporan Penjualan",
            "3. Lihat Riwayat Harga",
            "4. Cek Barang Tidak Laku",
            "5. Kelola Produk Paket (Bundling)",
            "6. Lihat Laporan Keuangan",
            "7. Lihat Produk Terlaris",
            "8. Logout"
        };
    }
}
