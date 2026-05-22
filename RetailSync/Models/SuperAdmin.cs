using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailSync.Models
{
    // ═══════════════════════════════════════════════════
    //  INHERITANCE + POLYMORPHISM
    //  SuperAdmin extends Pengguna
    // ═══════════════════════════════════════════════════
    class SuperAdmin : Pengguna
    {
        public SuperAdmin(int id, string nama, string username)
            : base(id, nama, username, "Super Admin") { }

        // Polymorphism: implementasi berbeda dari BranchManager
        public override string GetRoleLabel() => "[ SUPER ADMIN — HQ/Owner ]";

        public override string[] GetMenuItems() => new[]
        {
            "1.  Kelola Data Vendor & Produk",
            "2.  Lihat Laporan Penjualan",
            "3.  Lihat Riwayat Harga",
            "4.  Cek Barang Tidak Laku",
            "5.  Kelola Produk Paket / Bundling",
            "6.  Lihat Laporan Keuangan",
            "7.  Kelola Inventori (Gudang/Toko)",
            "8. Transfer Stok Antar Cabang",
            "9. Transaksi Penjualan (Kasir)",
            "10. Update Stok Otomatis",
            "11. Lihat Laporan Harian",
            "12. Lihat Produk Terlaris",
            "13. Kelola Pesanan",
            "14. Monitoring Stok Real-time",
            "── ─────────────────────────",
            "0.  Logout"
        };
    }
}
