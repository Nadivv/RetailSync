using RetailSync.Helpers;
using RetailSync.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace RetailSync
{
    public partial class Form7 : Form
    {
        // ── State ────────────────────────────────────────────────────────────
        private List<ProdukItem> _allProduk = new();
        private List<ProdukItem> _filtered  = new();
        private int _currentPage = 1;
        private const int PageSize = 8;

        // ── Summary labels ───────────────────────────────────────────────────
        private Label _lblTotal = null!, _lblTersedia = null!,
                      _lblMenipis = null!, _lblHabis = null!;

        // ── Filter ───────────────────────────────────────────────────────────
        private TextBox  _searchBox   = null!;
        private ComboBox _cmbStatus   = null!;
        private ComboBox _cmbKategori = null!;

        // ── Action buttons ───────────────────────────────────────────────────
        private Button _btnTambah = null!, _btnEdit = null!,
                       _btnHapus  = null!, _btnRefresh = null!;

        // ── Table rows ───────────────────────────────────────────────────────
        private readonly List<Panel>   _rowPanels = new();
        private readonly List<Label[]> _rowLabels = new();

        // ── Pagination ───────────────────────────────────────────────────────
        private Label  _lblPagInfo = null!;
        private Button _btnPrev = null!, _btnNext = null!;

        // ── Selection ────────────────────────────────────────────────────────
        private ProdukItem? _selectedProduk;

        // ── Layout refs ──────────────────────────────────────────────────────
        private Panel _contentPanel = null!;
        private Panel _tableBodyPanel = null!;

        public Form7()
        {
            InitializeComponent();
        }

        private void Form7_Load(object sender, EventArgs e)
        {
            // Sembunyikan semua elemen Designer yang mengganggu
            SembunyikanDesigner();

            // Bangun layout baru
            BangunUI();

            // Sidebar active marker
            TandaiSidebarAktif();

            // Load data
            MuatData();
        }

        // ─────────────────────────────────────────────────────────────────────
        private void SembunyikanDesigner()
        {
            panel2.Visible   = false;
            button8.Visible  = false;
            button9.Visible  = false;
            button10.Visible = false;
            button11.Visible = false;
            button13.Visible = false;
            label22.Visible  = false;
            foreach (Control c in new Control[]
            {
                label3, label4, label5, label6, label7, label8,
                label9, label10, label11, label12, label13, label14,
                label15, label16, label17, label18, label19, label20, label21
            })
                c.Visible = false;
        }

        // ─────────────────────────────────────────────────────────────────────
        //  UI Builder
        // ─────────────────────────────────────────────────────────────────────
        private void BangunUI()
        {
            // panel1 (sidebar) sudah DockStyle.Left width=200
            // _contentPanel mengisi area kanan dengan Anchor
            _contentPanel = new Panel
            {
                BackColor = Color.FromArgb(243, 244, 246),
                Location  = new Point(208, 0),
                Anchor    = AnchorStyles.Top | AnchorStyles.Left
                          | AnchorStyles.Right | AnchorStyles.Bottom
            };
            // Ukuran initial + update saat resize form
            AturUkuranContent();
            this.Resize += (s, e) => AturUkuranContent();
            Controls.Add(_contentPanel);
            _contentPanel.BringToFront();

            int pad = 20;

            // ── Title ─────────────────────────────────────────────────────────
            var lblTitle = new Label
            {
                Text      = "1. Kelola Inventori Toko",
                Font      = new Font("Segoe UI", 14f, FontStyle.Bold),
                ForeColor = Color.FromArgb(17, 24, 39),
                AutoSize  = true,
                Location  = new Point(pad, 16)
            };
            _contentPanel.Controls.Add(lblTitle);

            var lblSub = new Label
            {
                Text      = "Kelola data produk, stok, kategori, dan informasi inventori toko",
                Font      = new Font("Segoe UI", 9f),
                ForeColor = Color.FromArgb(107, 114, 128),
                AutoSize  = true,
                Location  = new Point(pad, 42)
            };
            _contentPanel.Controls.Add(lblSub);

            // ── Summary cards ─────────────────────────────────────────────────
            // 4 kartu di baris Y=68, lebar dinamis saat resize
            int cardY = 68;
            int cardH = 80;
            var cardDefs = new[]
            {
                ("Total Produk",  "🛒", Color.FromArgb(37,99,235)),
                ("Stok Tersedia", "📦", Color.FromArgb(22,163,74)),
                ("Stok Menipis",  "⚠",  Color.FromArgb(234,88,12)),
                ("Stok Habis",    "🚫", Color.FromArgb(220,38,38)),
            };
            var angkaLabels = new Label[4];
            for (int i = 0; i < 4; i++)
            {
                var (judul, ikon, warna) = cardDefs[i];
                var (card, lbl) = BuatCard(judul, ikon, warna, 0, cardY, cardH);
                _contentPanel.Controls.Add(card);
                angkaLabels[i] = lbl;
                int ci = i;
                _contentPanel.Resize += (s, e) =>
                {
                    int avail  = _contentPanel.ClientSize.Width - pad * 2;
                    int cardW  = (avail - 30) / 4;
                    card.Location = new Point(pad + ci * (cardW + 10), cardY);
                    card.Width    = cardW;
                };
            }
            _lblTotal    = angkaLabels[0];
            _lblTersedia = angkaLabels[1];
            _lblMenipis  = angkaLabels[2];
            _lblHabis    = angkaLabels[3];

            // ── Table card ────────────────────────────────────────────────────
            int tableY = cardY + cardH + 16;
            var tableCard = new Panel
            {
                BackColor = Color.White,
                Location  = new Point(pad, tableY),
                Anchor    = AnchorStyles.Top | AnchorStyles.Left
                          | AnchorStyles.Right | AnchorStyles.Bottom
            };
            // border dengan Paint
            tableCard.Paint += (s, pe) =>
            {
                using var pen = new Pen(Color.FromArgb(229, 231, 235));
                pe.Graphics.DrawRectangle(pen, 0, 0, tableCard.Width-1, tableCard.Height-1);
            };
            _contentPanel.Controls.Add(tableCard);
            _contentPanel.Resize += (s, e) =>
            {
                int tw = _contentPanel.ClientSize.Width - pad * 2;
                int th = _contentPanel.ClientSize.Height - tableY - 16;
                if (tw > 100 && th > 100)
                    tableCard.Size = new Size(tw, th);
            };

            // ── Toolbar di dalam tableCard ────────────────────────────────────
            var toolbar = new Panel
            {
                Location  = new Point(0, 0),
                Height    = 52,
                BackColor = Color.White,
                Anchor    = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            tableCard.Controls.Add(toolbar);
            tableCard.Resize += (s, e) => toolbar.Width = tableCard.ClientSize.Width;
            BangunToolbar(toolbar);

            // garis di bawah toolbar
            var div1 = new Panel
            {
                Location  = new Point(0, 52),
                Height    = 1,
                BackColor = Color.FromArgb(229, 231, 235),
                Anchor    = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            tableCard.Controls.Add(div1);
            tableCard.Resize += (s, e) => div1.Width = tableCard.ClientSize.Width;

            // header kolom
            var headerRow = new Panel
            {
                Location  = new Point(0, 53),
                Height    = 34,
                BackColor = Color.FromArgb(249, 250, 251),
                Anchor    = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            tableCard.Controls.Add(headerRow);
            tableCard.Resize += (s, e) =>
            {
                headerRow.Width = tableCard.ClientSize.Width;
                BangunHeaderKolom(headerRow);
            };
            BangunHeaderKolom(headerRow);

            // garis di bawah header
            var div2 = new Panel
            {
                Location  = new Point(0, 87),
                Height    = 1,
                BackColor = Color.FromArgb(229, 231, 235),
                Anchor    = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            tableCard.Controls.Add(div2);
            tableCard.Resize += (s, e) => div2.Width = tableCard.ClientSize.Width;

            // body tabel
            _tableBodyPanel = new Panel
            {
                Location  = new Point(0, 88),
                BackColor = Color.White,
                Anchor    = AnchorStyles.Top | AnchorStyles.Left
                          | AnchorStyles.Right | AnchorStyles.Bottom
            };
            tableCard.Controls.Add(_tableBodyPanel);
            tableCard.Resize += (s, e) =>
            {
                int bw = tableCard.ClientSize.Width;
                int bh = tableCard.ClientSize.Height - 88 - 40;
                if (bw > 0 && bh > 0)
                {
                    _tableBodyPanel.Size = new Size(bw, bh);
                    ReposisiBaris(_tableBodyPanel);
                }
            };

            // pagination bar
            var pagPanel = new Panel
            {
                BackColor = Color.White,
                Anchor    = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
            tableCard.Controls.Add(pagPanel);
            tableCard.Resize += (s, e) =>
            {
                pagPanel.Size     = new Size(tableCard.ClientSize.Width, 40);
                pagPanel.Location = new Point(0, tableCard.ClientSize.Height - 40);
            };
            BangunPaginasi(pagPanel);

            // Bangun baris kosong sekali
            BangunBarisTabel();
        }

        private void AturUkuranContent()
        {
            _contentPanel.Size = new Size(
                ClientSize.Width  - 208,
                ClientSize.Height
            );
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Summary Card
        // ─────────────────────────────────────────────────────────────────────
        private (Panel card, Label angka) BuatCard(string judul, string ikon,
            Color aksen, int x, int y, int h)
        {
            var card = new Panel
            {
                BackColor = Color.White,
                Location  = new Point(x, y),
                Size      = new Size(185, h)
            };
            card.Paint += (s, pe) =>
            {
                using var pen = new Pen(Color.FromArgb(229, 231, 235));
                pe.Graphics.DrawRectangle(pen, 0, 0, card.Width-1, card.Height-1);
            };

            // strip warna kiri
            card.Controls.Add(new Panel
            {
                BackColor = aksen,
                Size      = new Size(4, h),
                Location  = new Point(0, 0)
            });

            var lblIkon = new Label
            {
                Text      = ikon,
                Font      = new Font("Segoe UI", 18f),
                Location  = new Point(12, 12),
                AutoSize  = true,
                BackColor = Color.Transparent
            };
            card.Controls.Add(lblIkon);

            var lblNum = new Label
            {
                Text      = "–",
                Font      = new Font("Segoe UI", 17f, FontStyle.Bold),
                ForeColor = Color.FromArgb(17, 24, 39),
                Location  = new Point(58, 6),
                Size      = new Size(100, 36),
                BackColor = Color.Transparent
            };
            card.Controls.Add(lblNum);

            card.Controls.Add(new Label
            {
                Text      = judul,
                Font      = new Font("Segoe UI", 8f),
                ForeColor = Color.FromArgb(107, 114, 128),
                Location  = new Point(58, 44),
                AutoSize  = true,
                BackColor = Color.Transparent
            });

            return (card, lblNum);
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Toolbar
        // ─────────────────────────────────────────────────────────────────────
        private void BangunToolbar(Panel tb)
        {
            int y = 12;

            _searchBox = new TextBox
            {
                PlaceholderText = "Cari nama produk...",
                Font            = new Font("Segoe UI", 9f),
                BorderStyle     = BorderStyle.FixedSingle,
                Location        = new Point(12, y),
                Size            = new Size(190, 28)
            };
            _searchBox.TextChanged += (s, e) => TerapkanFilter();
            tb.Controls.Add(_searchBox);

            _cmbStatus = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font          = new Font("Segoe UI", 9f),
                Location      = new Point(210, y),
                Size          = new Size(135, 28)
            };
            _cmbStatus.Items.AddRange(new[] { "Semua Status", "Tersedia", "Stok Menipis", "Stok Habis" });
            _cmbStatus.SelectedIndex = 0;
            _cmbStatus.SelectedIndexChanged += (s, e) => TerapkanFilter();
            tb.Controls.Add(_cmbStatus);

            _cmbKategori = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font          = new Font("Segoe UI", 9f),
                Location      = new Point(354, y),
                Size          = new Size(135, 28)
            };
            var cats = ProdukRepository.GetKategori();
            _cmbKategori.Items.AddRange(cats.ToArray());
            _cmbKategori.SelectedIndex = 0;
            _cmbKategori.SelectedIndexChanged += (s, e) => TerapkanFilter();
            tb.Controls.Add(_cmbKategori);

            // Tombol — geser ke kanan saat resize
            _btnTambah  = BuatBtn("+ Tambah",  Color.FromArgb(22, 163, 74),  Color.White);
            _btnEdit    = BuatBtn("✎ Edit",     Color.FromArgb(37, 99, 235),  Color.White);
            _btnHapus   = BuatBtn("✕ Hapus",    Color.FromArgb(220, 38, 38),  Color.White);
            _btnRefresh = BuatBtn("↻",          Color.FromArgb(107, 114, 128),Color.White);
            _btnRefresh.Size  = new Size(32, 28);
            _btnEdit.Enabled  = false;
            _btnHapus.Enabled = false;

            _btnTambah.Click  += BtnTambah_Click;
            _btnEdit.Click    += BtnEdit_Click;
            _btnHapus.Click   += BtnHapus_Click;
            _btnRefresh.Click += (s, e) => { _selectedProduk = null; AktifkanAksi(false); MuatData(); };

            tb.Controls.Add(_btnTambah);
            tb.Controls.Add(_btnEdit);
            tb.Controls.Add(_btnHapus);
            tb.Controls.Add(_btnRefresh);

            tb.Resize += (s, e) => PosisikanTombol(tb, y);
            PosisikanTombol(tb, y);
        }

        private void PosisikanTombol(Panel tb, int y)
        {
            int rx = tb.ClientSize.Width - 12;
            _btnRefresh.Location = new Point(rx - _btnRefresh.Width, y); rx -= _btnRefresh.Width + 6;
            _btnHapus.Location   = new Point(rx - _btnHapus.Width,   y); rx -= _btnHapus.Width + 6;
            _btnEdit.Location    = new Point(rx - _btnEdit.Width,     y); rx -= _btnEdit.Width + 6;
            _btnTambah.Location  = new Point(rx - _btnTambah.Width,   y);
        }

        private static Button BuatBtn(string text, Color back, Color fore) => new Button
        {
            Text      = text,
            Size      = new Size(88, 28),
            BackColor = back,
            ForeColor = fore,
            FlatStyle = FlatStyle.Flat,
            Font      = new Font("Segoe UI", 8.5f, FontStyle.Bold),
            Cursor    = Cursors.Hand
        };

        // ─────────────────────────────────────────────────────────────────────
        //  Header kolom tabel
        // ─────────────────────────────────────────────────────────────────────
        private static readonly string[] _colNames = { "No", "Nama Produk", "Kategori", "Harga", "Stok", "Status" };
        private static readonly int[]    _colPct   = { 4, 28, 14, 18, 8, 14 };

        private void BangunHeaderKolom(Panel hp)
        {
            hp.Controls.Clear();
            if (hp.Width < 50) return;
            int total = _colPct.Sum();
            int x = 0;
            for (int i = 0; i < _colNames.Length; i++)
            {
                int w = hp.Width * _colPct[i] / total;
                hp.Controls.Add(new Label
                {
                    Text      = _colNames[i],
                    Font      = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                    ForeColor = Color.FromArgb(75, 85, 99),
                    BackColor = Color.Transparent,
                    Location  = new Point(x + 8, 0),
                    Size      = new Size(w - 8, 34),
                    TextAlign = ContentAlignment.MiddleLeft
                });
                x += w;
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Baris tabel
        // ─────────────────────────────────────────────────────────────────────
        private void BangunBarisTabel()
        {
            _tableBodyPanel.Controls.Clear();
            _rowPanels.Clear();
            _rowLabels.Clear();

            int rowH = 40;
            for (int i = 0; i < PageSize; i++)
            {
                int ri = i;
                var rp = new Panel
                {
                    Location  = new Point(0, i * rowH),
                    Height    = rowH,
                    BackColor = i % 2 == 0 ? Color.White : Color.FromArgb(249, 250, 251),
                    Cursor    = Cursors.Hand
                };
                rp.Paint += (s, pe) =>
                {
                    using var pen = new Pen(Color.FromArgb(243, 244, 246));
                    pe.Graphics.DrawLine(pen, 0, rp.Height - 1, rp.Width, rp.Height - 1);
                };
                rp.Click += (s, e) => PilihBaris(ri);

                var cols = new Label[6];
                for (int c = 0; c < 6; c++)
                {
                    int ci = c;
                    cols[c] = new Label
                    {
                        AutoSize  = false,
                        Font      = new Font("Segoe UI", 8.5f),
                        ForeColor = Color.FromArgb(55, 65, 81),
                        BackColor = Color.Transparent,
                        TextAlign = ContentAlignment.MiddleLeft,
                        Height    = rowH,
                        Top       = 0,
                        Cursor    = Cursors.Hand
                    };
                    cols[c].Click += (s, e) => PilihBaris(ri);
                    rp.Controls.Add(cols[c]);
                }
                rp.Resize += (s, e) => LayoutKolom(rp, cols, rowH);

                _rowPanels.Add(rp);
                _rowLabels.Add(cols);
                _tableBodyPanel.Controls.Add(rp);
            }

            ReposisiBaris(_tableBodyPanel);
        }

        private void ReposisiBaris(Panel body)
        {
            int w = body.ClientSize.Width;
            if (w <= 0) return;
            int rowH = 40;
            for (int i = 0; i < _rowPanels.Count; i++)
            {
                _rowPanels[i].Size     = new Size(w, rowH);
                _rowPanels[i].Location = new Point(0, i * rowH);
                LayoutKolom(_rowPanels[i], _rowLabels[i], rowH);
            }
        }

        private void LayoutKolom(Panel rp, Label[] cols, int rowH)
        {
            int total = _colPct.Sum();
            int x = 0, w = rp.ClientSize.Width;
            if (w <= 0) return;
            for (int c = 0; c < cols.Length; c++)
            {
                int cw = w * _colPct[c] / total;
                cols[c].Location = new Point(x + 8, 0);
                cols[c].Size     = new Size(cw - 8, rowH);
                x += cw;
            }
        }

        private void PilihBaris(int idx)
        {
            int di = (_currentPage - 1) * PageSize + idx;
            if (di >= _filtered.Count) return;

            _selectedProduk = _filtered[di];
            AktifkanAksi(true);

            for (int i = 0; i < _rowPanels.Count; i++)
            {
                bool sel  = i == idx;
                var  bg   = sel ? Color.FromArgb(219, 234, 254)
                                : i % 2 == 0 ? Color.White : Color.FromArgb(249, 250, 251);
                _rowPanels[i].BackColor = bg;
                foreach (var l in _rowLabels[i]) l.BackColor = sel ? bg : Color.Transparent;
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Pagination bar
        // ─────────────────────────────────────────────────────────────────────
        private void BangunPaginasi(Panel bar)
        {
            _lblPagInfo = new Label
            {
                AutoSize  = true,
                Font      = new Font("Segoe UI", 8.5f),
                ForeColor = Color.FromArgb(107, 114, 128),
                Location  = new Point(12, 12),
                BackColor = Color.Transparent
            };
            bar.Controls.Add(_lblPagInfo);

            _btnNext = BuatBtnPag("›");
            _btnPrev = BuatBtnPag("‹");
            _btnNext.Click += (s, e) =>
            {
                if (_currentPage < TotalHalaman) { _currentPage++; TampilkanHalaman(); UpdatePagInfo(); }
            };
            _btnPrev.Click += (s, e) =>
            {
                if (_currentPage > 1) { _currentPage--; TampilkanHalaman(); UpdatePagInfo(); }
            };

            bar.Controls.Add(_btnPrev);
            bar.Controls.Add(_btnNext);
            bar.Resize += (s, e) =>
            {
                _btnNext.Location = new Point(bar.ClientSize.Width - 36, 6);
                _btnPrev.Location = new Point(bar.ClientSize.Width - 68, 6);
            };
        }

        private static Button BuatBtnPag(string t) => new Button
        {
            Text      = t,
            Size      = new Size(28, 28),
            FlatStyle = FlatStyle.Flat,
            Font      = new Font("Segoe UI", 11f, FontStyle.Bold),
            BackColor = Color.FromArgb(243, 244, 246),
            ForeColor = Color.FromArgb(55, 65, 81),
            Cursor    = Cursors.Hand
        };

        // ─────────────────────────────────────────────────────────────────────
        //  Data loading & filtering
        // ─────────────────────────────────────────────────────────────────────
        private void MuatData()
        {
            try
            {
                _allProduk = ProdukRepository.GetAll();
                var (total, tersedia, menipis, habis) = ProdukRepository.GetSummary();
                _lblTotal.Text    = total.ToString();
                _lblTersedia.Text = tersedia.ToString();
                _lblMenipis.Text  = menipis.ToString();
                _lblHabis.Text    = habis.ToString();
                TerapkanFilter();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat data: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TerapkanFilter()
        {
            string kw  = _searchBox.Text.Trim().ToLower();
            string st  = _cmbStatus.Text;
            string kat = _cmbKategori.Text;

            _filtered = _allProduk.Where(p =>
                (string.IsNullOrEmpty(kw) || p.NamaProduk.ToLower().Contains(kw)) &&
                (st  == "Semua Status"   || p.StatusStok == st) &&
                (kat == "Semua Kategori" || p.Kategori   == kat)
            ).ToList();

            _currentPage = 1;
            TampilkanHalaman();
            UpdatePagInfo();
        }

        private int TotalHalaman => Math.Max(1, (int)Math.Ceiling((double)_filtered.Count / PageSize));

        private void TampilkanHalaman()
        {
            var page = _filtered.Skip((_currentPage - 1) * PageSize).Take(PageSize).ToList();
            IsiBaris(page);
        }

        private void IsiBaris(List<ProdukItem> items)
        {
            for (int i = 0; i < PageSize; i++)
            {
                if (i < items.Count)
                {
                    var p  = items[i];
                    int no = (_currentPage - 1) * PageSize + i + 1;
                    bool sel = _selectedProduk?.IdProduk == p.IdProduk;

                    _rowLabels[i][0].Text = no.ToString();
                    _rowLabels[i][1].Text = p.NamaProduk;
                    _rowLabels[i][2].Text = p.Kategori;
                    _rowLabels[i][3].Text = "Rp " + p.Harga.ToString("N0");
                    _rowLabels[i][4].Text = p.Stok.ToString();
                    _rowLabels[i][5].Text = p.StatusStok;
                    _rowLabels[i][5].ForeColor = p.StatusStok == "Tersedia"
                        ? Color.FromArgb(22, 163, 74)
                        : p.StatusStok == "Stok Menipis"
                            ? Color.FromArgb(234, 88, 12)
                            : Color.FromArgb(220, 38, 38);

                    var bg = sel ? Color.FromArgb(219, 234, 254)
                                 : i % 2 == 0 ? Color.White : Color.FromArgb(249, 250, 251);
                    _rowPanels[i].BackColor = bg;
                    foreach (var l in _rowLabels[i]) l.BackColor = sel ? bg : Color.Transparent;
                    _rowPanels[i].Visible = true;
                }
                else
                {
                    _rowPanels[i].Visible = false;
                }
            }
        }

        private void UpdatePagInfo()
        {
            int start = _filtered.Count == 0 ? 0 : (_currentPage - 1) * PageSize + 1;
            int end   = Math.Min(_currentPage * PageSize, _filtered.Count);
            _lblPagInfo.Text = $"Menampilkan {start}–{end} dari {_filtered.Count} data";
            _btnPrev.Enabled = _currentPage > 1;
            _btnNext.Enabled = _currentPage < TotalHalaman;
        }

        // ─────────────────────────────────────────────────────────────────────
        //  CRUD
        // ─────────────────────────────────────────────────────────────────────
        private void AktifkanAksi(bool v) { _btnEdit.Enabled = v; _btnHapus.Enabled = v; }

        private void BtnTambah_Click(object? s, EventArgs e)
        {
            using var dlg = new FormProdukDialog();
            if (dlg.ShowDialog(this) == DialogResult.OK)
            { _selectedProduk = null; AktifkanAksi(false); MuatData(); }
        }

        private void BtnEdit_Click(object? s, EventArgs e)
        {
            if (_selectedProduk == null) return;
            using var dlg = new FormProdukDialog(_selectedProduk);
            if (dlg.ShowDialog(this) == DialogResult.OK)
            { _selectedProduk = null; AktifkanAksi(false); MuatData(); }
        }

        private void BtnHapus_Click(object? s, EventArgs e)
        {
            if (_selectedProduk == null) return;
            if (MessageBox.Show($"Hapus \"{_selectedProduk.NamaProduk}\"?", "Konfirmasi",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try
                {
                    ProdukRepository.Hapus(_selectedProduk.IdProduk);
                    _selectedProduk = null; AktifkanAksi(false); MuatData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal: " + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Sidebar active indicator
        // ─────────────────────────────────────────────────────────────────────
        private void TandaiSidebarAktif()
        {
            var strip = new Panel
            {
                BackColor = Color.FromArgb(250, 204, 21),
                Size      = new Size(4, button2.Height - 10),
                Location  = new Point(0, button2.Top + 5)
            };
            panel1.Controls.Add(strip);
            strip.BringToFront();
        }

        // ── Stub event handlers dari Designer ────────────────────────────────
        private void label9_Click(object sender, EventArgs e)  { }
        private void label17_Click(object sender, EventArgs e) { }
        private void label22_Click(object sender, EventArgs e) { }
        private void panel2_Paint(object sender, PaintEventArgs e)   { }
        private void panel2_Paint_1(object sender, PaintEventArgs e) { }

        private void button13_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Yakin ingin logout?", "Logout",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                foreach (Form f in Application.OpenForms)
                    if (f is Form1) { f.Show(); break; }
                this.Close();
            }
        }
    }
}
