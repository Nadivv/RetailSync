using RetailSync.Helpers;
using RetailSync.Models;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace RetailSync
{
    public class FormTransaksiBaru : Form
    {
        private readonly int _idPengguna;

        private ComboBox _cmbProduk  = null!;
        private TextBox  _txtToko    = null!;
        private NumericUpDown _nudJumlah = null!;
        private Label    _lblHarga   = null!;
        private Label    _lblSubtotal = null!;
        private Button   _btnSimpan  = null!;

        public FormTransaksiBaru(int idPengguna)
        {
            _idPengguna = idPengguna;
            BangunUI();
        }

        private void BangunUI()
        {
            Text            = "Transaksi Penjualan Baru";
            Size            = new Size(420, 340);
            StartPosition   = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox     = false;
            BackColor       = Color.White;
            Font            = new Font("Segoe UI", 9.5f);

            int lx = 20, cx = 140, cw = 240, gap = 44, y = 20;

            void Lbl(string t) => Controls.Add(new Label
            {
                Text = t, Location = new Point(lx, y + 5),
                AutoSize = true, ForeColor = Color.FromArgb(55, 65, 81)
            });

            // Produk
            Lbl("Produk");
            _cmbProduk = new ComboBox
            {
                Location      = new Point(cx, y),
                Size          = new Size(cw, 28),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font          = new Font("Segoe UI", 9f)
            };
            MuatProduk();
            _cmbProduk.SelectedIndexChanged += (s, e) => UpdateHarga();
            Controls.Add(_cmbProduk);
            y += gap;

            // Nama Toko
            Lbl("Nama Toko");
            _txtToko = new TextBox
            {
                Location    = new Point(cx, y),
                Size        = new Size(cw, 28),
                Text        = "RetailSync Outlet Utama"
            };
            Controls.Add(_txtToko);
            y += gap;

            // Jumlah
            Lbl("Jumlah");
            _nudJumlah = new NumericUpDown
            {
                Location = new Point(cx, y),
                Size     = new Size(cw, 28),
                Minimum  = 1,
                Maximum  = 9999,
                Value    = 1
            };
            _nudJumlah.ValueChanged += (s, e) => UpdateHarga();
            Controls.Add(_nudJumlah);
            y += gap;

            // Harga satuan
            Lbl("Harga Satuan");
            _lblHarga = new Label
            {
                Text      = "Rp 0",
                Location  = new Point(cx, y + 4),
                AutoSize  = true,
                Font      = new Font("Segoe UI", 9f),
                ForeColor = Color.FromArgb(55, 65, 81)
            };
            Controls.Add(_lblHarga);
            y += gap;

            // Subtotal
            Lbl("Subtotal");
            _lblSubtotal = new Label
            {
                Text      = "Rp 0",
                Location  = new Point(cx, y + 4),
                AutoSize  = true,
                Font      = new Font("Segoe UI", 10f, FontStyle.Bold),
                ForeColor = Color.FromArgb(22, 163, 74)
            };
            Controls.Add(_lblSubtotal);
            y += gap + 8;

            // Tombol
            _btnSimpan = new Button
            {
                Text      = "✔ Simpan Transaksi",
                Location  = new Point(cx, y),
                Size      = new Size(160, 34),
                BackColor = Color.FromArgb(22, 163, 74),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font      = new Font("Segoe UI", 9f, FontStyle.Bold),
                Cursor    = Cursors.Hand
            };
            _btnSimpan.FlatAppearance.BorderSize = 0;
            _btnSimpan.Click += BtnSimpan_Click;
            Controls.Add(_btnSimpan);

            var btnBatal = new Button
            {
                Text      = "Batal",
                Location  = new Point(cx + 168, y),
                Size      = new Size(72, 34),
                BackColor = Color.FromArgb(229, 231, 235),
                ForeColor = Color.FromArgb(55, 65, 81),
                FlatStyle = FlatStyle.Flat,
                Cursor    = Cursors.Hand
            };
            btnBatal.FlatAppearance.BorderSize = 0;
            btnBatal.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };
            Controls.Add(btnBatal);

            UpdateHarga();
        }

        private void MuatProduk()
        {
            var list = ProdukRepository.GetAll();
            _cmbProduk.Items.Clear();
            foreach (var p in list)
                _cmbProduk.Items.Add(p);
            _cmbProduk.DisplayMember = "NamaProduk";
            if (_cmbProduk.Items.Count > 0)
                _cmbProduk.SelectedIndex = 0;
        }

        private void UpdateHarga()
        {
            if (_cmbProduk.SelectedItem is ProdukItem p)
            {
                decimal subtotal = p.Harga * (int)_nudJumlah.Value;
                _lblHarga.Text    = "Rp " + p.Harga.ToString("N0");
                _lblSubtotal.Text = "Rp " + subtotal.ToString("N0");
            }
        }

        private void BtnSimpan_Click(object? s, EventArgs e)
        {
            if (_cmbProduk.SelectedItem is not ProdukItem produk)
            {
                MessageBox.Show("Pilih produk terlebih dahulu.", "Validasi",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(_txtToko.Text))
            {
                MessageBox.Show("Nama toko tidak boleh kosong.", "Validasi",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int jumlah = (int)_nudJumlah.Value;
            try
            {
                TransaksiRepository.SimpanTransaksi(
                    _idPengguna,
                    _txtToko.Text.Trim(),
                    produk.NamaProduk,
                    produk.IdProduk,
                    jumlah,
                    produk.Harga
                );
                MessageBox.Show("Transaksi berhasil disimpan!", "Sukses",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
