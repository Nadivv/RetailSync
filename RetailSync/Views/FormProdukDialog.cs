using RetailSync.Helpers;
using RetailSync.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace RetailSync
{
    /// <summary>
    /// Dialog untuk Tambah / Edit Produk
    /// </summary>
    public class FormProdukDialog : Form
    {
        // ── Mode ─────────────────────────────────────────────────────────────
        private readonly bool _isEdit;
        private readonly ProdukItem? _produk;

        // ── Controls ─────────────────────────────────────────────────────────
        private TextBox   _txtNama      = null!;
        private ComboBox  _cmbKategori  = null!;
        private TextBox   _txtHarga     = null!;
        private NumericUpDown _nudStok  = null!;
        private DateTimePicker _dtpExp  = null!;
        private CheckBox  _chkExpAda    = null!;
        private Button    _btnSimpan    = null!;
        private Button    _btnBatal     = null!;

        private List<(int Id, string Nama)> _kategoriList = new();

        // ── Constructor ───────────────────────────────────────────────────────
        public FormProdukDialog(ProdukItem? produk = null)
        {
            _isEdit = produk != null;
            _produk = produk;
            BangunUI();
            if (_isEdit) IsiFormDariProduk();
        }

        // ── Build UI ──────────────────────────────────────────────────────────
        private void BangunUI()
        {
            Text            = _isEdit ? "Edit Produk" : "Tambah Produk Baru";
            Size            = new Size(420, 380);
            StartPosition   = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox     = false;
            MinimizeBox     = false;
            BackColor       = Color.White;
            Font            = new Font("Segoe UI", 9.5f);

            int labelX = 20, controlX = 130, ctrlW = 250, gap = 45, startY = 20;

            // Nama Produk
            Tambah(new Label { Text = "Nama Produk", Location = new Point(labelX, startY + 5), AutoSize = true });
            _txtNama = new TextBox { Location = new Point(controlX, startY), Size = new Size(ctrlW, 28) };
            Tambah(_txtNama);

            // Kategori
            Tambah(new Label { Text = "Kategori", Location = new Point(labelX, startY + gap + 5), AutoSize = true });
            _cmbKategori = new ComboBox
            {
                Location      = new Point(controlX, startY + gap),
                Size          = new Size(ctrlW, 28),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            MuatKategori();
            Tambah(_cmbKategori);

            // Harga
            Tambah(new Label { Text = "Harga (Rp)", Location = new Point(labelX, startY + gap * 2 + 5), AutoSize = true });
            _txtHarga = new TextBox { Location = new Point(controlX, startY + gap * 2), Size = new Size(ctrlW, 28) };
            Tambah(_txtHarga);

            // Stok
            Tambah(new Label { Text = "Stok", Location = new Point(labelX, startY + gap * 3 + 5), AutoSize = true });
            _nudStok = new NumericUpDown
            {
                Location = new Point(controlX, startY + gap * 3),
                Size     = new Size(ctrlW, 28),
                Maximum  = 999999,
                Minimum  = 0
            };
            Tambah(_nudStok);

            // Tgl Expired toggle
            _chkExpAda = new CheckBox
            {
                Text     = "Ada Tanggal Expired",
                Location = new Point(labelX, startY + gap * 4 + 5),
                AutoSize = true,
                Checked  = false
            };
            _chkExpAda.CheckedChanged += (s, e) => _dtpExp.Enabled = _chkExpAda.Checked;
            Tambah(_chkExpAda);

            // DateTimePicker
            _dtpExp = new DateTimePicker
            {
                Location = new Point(controlX, startY + gap * 4),
                Size     = new Size(ctrlW, 28),
                Format   = DateTimePickerFormat.Short,
                Value    = DateTime.Today.AddYears(1),
                Enabled  = false
            };
            Tambah(_dtpExp);

            // Tombol
            _btnSimpan = new Button
            {
                Text      = _isEdit ? "Simpan Perubahan" : "Tambah Produk",
                Location  = new Point(controlX, startY + gap * 5 + 10),
                Size      = new Size(155, 36),
                BackColor = Color.FromArgb(31, 41, 55),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor    = Cursors.Hand
            };
            _btnSimpan.FlatAppearance.BorderSize = 0;
            _btnSimpan.Click += BtnSimpan_Click;
            Tambah(_btnSimpan);

            _btnBatal = new Button
            {
                Text      = "Batal",
                Location  = new Point(controlX + 165, startY + gap * 5 + 10),
                Size      = new Size(85, 36),
                BackColor = Color.FromArgb(229, 231, 235),
                ForeColor = Color.FromArgb(55, 65, 81),
                FlatStyle = FlatStyle.Flat,
                Cursor    = Cursors.Hand
            };
            _btnBatal.FlatAppearance.BorderSize = 0;
            _btnBatal.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };
            Tambah(_btnBatal);
        }

        private void Tambah(Control c) => Controls.Add(c);

        // ── Muat kategori dari DB ─────────────────────────────────────────────
        private void MuatKategori()
        {
            _kategoriList = ProdukRepository.GetKategoriWithId();
            _cmbKategori.Items.Clear();
            foreach (var k in _kategoriList)
                _cmbKategori.Items.Add(k.Nama);
            if (_cmbKategori.Items.Count > 0)
                _cmbKategori.SelectedIndex = 0;
        }

        // ── Isi form saat mode Edit ───────────────────────────────────────────
        private void IsiFormDariProduk()
        {
            if (_produk == null) return;
            _txtNama.Text  = _produk.NamaProduk;
            _txtHarga.Text = _produk.Harga.ToString("N0").Replace(",", "");

            _nudStok.Value = Math.Min(_produk.Stok, (int)_nudStok.Maximum);

            // Pilih kategori
            int idx = _kategoriList.FindIndex(k => k.Nama == _produk.Kategori);
            if (idx >= 0) _cmbKategori.SelectedIndex = idx;

            if (_produk.TglExpired.HasValue)
            {
                _chkExpAda.Checked = true;
                _dtpExp.Enabled    = true;
                _dtpExp.Value      = _produk.TglExpired.Value;
            }
        }

        // ── Simpan ────────────────────────────────────────────────────────────
        private void BtnSimpan_Click(object? sender, EventArgs e)
        {
            string nama = _txtNama.Text.Trim();
            if (string.IsNullOrEmpty(nama))
            {
                MessageBox.Show("Nama produk tidak boleh kosong.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(_txtHarga.Text.Replace(".", "").Replace(",", ""), out decimal harga) || harga < 0)
            {
                MessageBox.Show("Harga harus berupa angka positif.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_cmbKategori.SelectedIndex < 0)
            {
                MessageBox.Show("Pilih kategori produk.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int      idKategori = _kategoriList[_cmbKategori.SelectedIndex].Id;
            int      stok       = (int)_nudStok.Value;
            DateTime? expired   = _chkExpAda.Checked ? _dtpExp.Value.Date : (DateTime?)null;

            try
            {
                if (_isEdit && _produk != null)
                    ProdukRepository.Update(_produk.IdProduk, harga, stok, expired, nama, idKategori);
                else
                    ProdukRepository.Tambah(nama, idKategori, harga, stok, expired);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal menyimpan: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
