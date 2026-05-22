using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailSync.Models
{
    // ═══════════════════════════════════════════════════
    //  INHERITANCE + POLYMORPHISM
    //  BranchManager extends Pengguna
    //  Association: punya properti NamaCabang
    // ═══════════════════════════════════════════════════
    class BranchManager : Pengguna
    {
        // Association: BranchManager berelasi dengan Cabang
        public string NamaCabang { get; set; }

        public BranchManager(int id, string nama, string username, string namaCabang = "Cabang Utama")
            : base(id, nama, username, "Branch Manager")
        {
            NamaCabang = namaCabang;
        }

        // Polymorphism: implementasi berbeda dari SuperAdmin
        public override string GetRoleLabel() => $"[ BRANCH MANAGER — {NamaCabang} ]";

        public override string[] GetMenuItems() => new[]
        {
            "1. Kelola Inventori (Gudang/Toko)",
            "2. Transfer Stok Antar Cabang",
            "3. Transaksi Penjualan (Kasir)",
            "4. Update Stok Otomatis",
            "5. Lihat Laporan Harian",
            "6. Lihat Produk Terlaris",
            "7. Kelola Pesanan",
            "8. Monitoring Stok Real-time",
            "── ────────────────────────",
            "0. Logout"
        };
    }
}
