using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailSync.Models
{
    // Pilar INHERITANSI: Manager mewarisi seluruh sifat dari Pengguna
    public class Manager : Pengguna
    {
        // ID Role untuk Manager di database adalah 2
        public Manager(int id, string nama, string username, bool isAktif)
            : base(id, nama, username, 2, isAktif) { }

        // Pilar POLIMORFISME: Menimpa fungsionalitas dasar untuk kebutuhan Manager
        public override string GetRoleLabel() => "[ MANAGER/KASIR ]";

        // Menu dibatasi hanya yang terhubung garis biru pada Use Case Diagram
        public override string[] GetMenuItems() => new[]
        {
            "1. Kelola Inventori (Gudang / Toko)",
            "2. Transfer Stok Antar Cabang",
            "3. Transaksi Penjualan (Kasir)",
            "4. Update Stok Otomatis",
            "5. Lihat Laporan Harian",
            "6. Kelola Pesanan",
            "7. Logout"
        };
    }
}