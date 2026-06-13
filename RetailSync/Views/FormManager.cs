using RetailSync.Helpers;
using RetailSync.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace RetailSync
{
    public class FormManager : Form
    {
        // ── Layout utama ─────────────────────────────────────────────────────
        private Panel _sidebar     = null!;
        private Panel _topbar      = null!;
        private Panel _pageArea    = null!;

        // ── Sidebar state ────────────────────────────────────────────────────
        private int   _activeMenu  = 0;
        private readonly List<Panel> _menuItems = new();

        // ── Halaman aktif ────────────────────────────────────────────────────
        private Panel? _currentPage;

        // ── Data tabel inventori ─────────────────────────────────────────────
        private List<ProdukItem> _allProduk  = new();
        private List<ProdukItem> _filtered   = new();
        private int _currentPage_ = 1;
        private const int PageSize = 9;

        private Label  _lblTotal = null!, _lblTersedia = null!,
                       _lblMenipis = null!, _lblHabis = null!;
        private TextBox  _search    = null!;
        private ComboBox _cmbStatus = null!, _cmbKat = null!;
        private Button   _btnTambah = null!, _btnEdit = null!,
                         _btnHapus  = null!, _btnPrev = null!, _btnNext = null!;
        private Label    _lblPag    = null!;
        private Panel    _tableBody = null!;
        private readonly List<Panel>   _rows   = new();
        private readonly List<Label[]> _cells  = new();
        private ProdukItem? _selectedProduk;

        // ── Menu definitions ─────────────────────────────────────────────────
        private static readonly (string Icon, string Label)[] Menus =
        {
            ("⊞",  "Dashboard"),
            ("☰",  "1. Kelola Inventori Toko"),
            ("📊", "2. Transaksi Penjualan Kasir"),
            ("🔄", "3. Update Stok Otomatis"),
            ("📋", "4. Lihat Laporan Harian"),
            ("📦", "5. Kelola Pesanan"),
            ("📡", "6. Monitoring Stok Real-Time"),
        };

        public FormManager()
        {
            InitForm();
            // PENTING: urutan Controls.Add untuk DockStyle harus terbalik dari
            // urutan visual. Fill harus ditambah pertama, lalu Top, lalu Left.
            BangunPageArea();   // DockStyle.Fill  — ditambah pertama
            BangunTopbar();     // DockStyle.Top   — ditambah kedua
            BangunSidebar();    // DockStyle.Left  — ditambah terakhir
            NavigasiKe(1);
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Form setup
        // ─────────────────────────────────────────────────────────────────────
        private void InitForm()
        {
            Text            = "RetailSync — Manager";
            WindowState     = FormWindowState.Maximized;
            MinimumSize     = new Size(1000, 600);
            BackColor       = Color.FromArgb(243, 244, 246);
            Font            = new Font("Segoe UI", 9f);
            StartPosition   = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.Sizable;
        }

        // ─────────────────────────────────────────────────────────────────────
        //  SIDEBAR
        // ─────────────────────────────────────────────────────────────────────
        private void BangunSidebar()
        {
            _sidebar = new Panel
            {
                Dock      = DockStyle.Left,
                Width     = 220,
                BackColor = Color.FromArgb(15, 23, 42)   // slate-900
            };
            Controls.Add(_sidebar);

            // Logo area
            var logoPanel = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 64,
                BackColor = Color.FromArgb(15, 23, 42)
            };
            logoPanel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using var br = new SolidBrush(Color.FromArgb(37, 99, 235));
                e.Graphics.FillEllipse(br, 18, 16, 32, 32);
                using var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                using var fnt = new Font("Segoe UI", 12f, FontStyle.Bold);
                e.Graphics.DrawString("RS", fnt, Brushes.White, new RectangleF(18, 16, 32, 32), sf);
                using var f2 = new Font("Segoe UI", 11f, FontStyle.Bold);
                e.Graphics.DrawString("RetailSync", f2, Brushes.White, new PointF(58, 20));
                using var f3 = new Font("Segoe UI", 7.5f);
                e.Graphics.DrawString("Manager", f3, new SolidBrush(Color.FromArgb(148, 163, 184)), new PointF(60, 38));
            };
            _sidebar.Controls.Add(logoPanel);

            // Divider
            _sidebar.Controls.Add(new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 1,
                BackColor = Color.FromArgb(30, 41, 59)
            });

            // User info
            var userPanel = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 56,
                BackColor = Color.FromArgb(15, 23, 42),
                Padding   = new Padding(16, 8, 16, 8)
            };
            userPanel.Paint += (s, e) =>
            {
                using var f1 = new Font("Segoe UI", 9f, FontStyle.Bold);
                e.Graphics.DrawString(UserContext.Nama, f1, Brushes.White, new PointF(16, 12));
                using var f2 = new Font("Segoe UI", 7.5f);
                e.Graphics.DrawString("Manager", f2, new SolidBrush(Color.FromArgb(100, 116, 139)), new PointF(16, 30));
            };
            _sidebar.Controls.Add(userPanel);

            _sidebar.Controls.Add(new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 1,
                BackColor = Color.FromArgb(30, 41, 59)
            });

            // Menu items — wrapper agar bisa scroll
            var menuWrapper = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = Menus.Length * 52 + 8,
                BackColor = Color.Transparent,
                Padding   = new Padding(0, 8, 0, 0)
            };
            _sidebar.Controls.Add(menuWrapper);

            for (int i = 0; i < Menus.Length; i++)
            {
                int idx = i;
                var (icon, label) = Menus[i];

                var item = new Panel
                {
                    Location  = new Point(0, 8 + i * 52),
                    Size      = new Size(220, 48),
                    BackColor = Color.Transparent,
                    Cursor    = Cursors.Hand,
                    Tag       = i
                };
                item.Paint += (s, e) => GambarMenuItem(e.Graphics, idx, item);
                item.Click += (s, e) => NavigasiKe(idx);
                item.MouseEnter += (s, e) => { if (idx != _activeMenu) { item.BackColor = Color.FromArgb(25, 35, 55); item.Invalidate(); } };
                item.MouseLeave += (s, e) => { if (idx != _activeMenu) { item.BackColor = Color.Transparent; item.Invalidate(); } };

                // Buat label transparan di atas agar klik tetap ter-capture
                var lbl = new Label
                {
                    Text      = $"  {icon}   {label}",
                    Location  = new Point(0, 0),
                    Size      = new Size(220, 48),
                    Font      = new Font("Segoe UI", 9f),
                    ForeColor = Color.FromArgb(148, 163, 184),
                    BackColor = Color.Transparent,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Cursor    = Cursors.Hand
                };
                lbl.Click += (s, e) => NavigasiKe(idx);
                lbl.MouseEnter += (s, e) => { if (idx != _activeMenu) { item.BackColor = Color.FromArgb(25, 35, 55); item.Invalidate(); } };
                lbl.MouseLeave += (s, e) => { if (idx != _activeMenu) { item.BackColor = Color.Transparent; item.Invalidate(); } };

                item.Controls.Add(lbl);
                menuWrapper.Controls.Add(item);
                _menuItems.Add(item);
            }

            // Logout di bawah
            var logoutPanel = new Panel
            {
                Dock      = DockStyle.Bottom,
                Height    = 52,
                BackColor = Color.Transparent,
                Cursor    = Cursors.Hand
            };
            logoutPanel.Paint += (s, e) =>
            {
                using var f = new Font("Segoe UI", 9f);
                e.Graphics.DrawString("  ⏻   Logout", f,
                    new SolidBrush(Color.FromArgb(239, 68, 68)), new PointF(12, 16));
            };
            logoutPanel.Click += (s, e) => Logout();
            _sidebar.Controls.Add(logoutPanel);

            _sidebar.Controls.Add(new Panel
            {
                Dock      = DockStyle.Bottom,
                Height    = 1,
                BackColor = Color.FromArgb(30, 41, 59)
            });
        }

        private void GambarMenuItem(Graphics g, int idx, Panel item)
        {
            bool aktif = idx == _activeMenu;
            if (aktif)
            {
                // background highlight
                using var br = new SolidBrush(Color.FromArgb(37, 99, 235, 40));
                g.FillRectangle(new SolidBrush(Color.FromArgb(30, 58, 138)), 0, 0, item.Width, item.Height);
                // strip kiri
                using var acc = new SolidBrush(Color.FromArgb(37, 99, 235));
                g.FillRectangle(acc, 0, 0, 4, item.Height);
            }

            // Update label warna
            if (item.Controls.Count > 0 && item.Controls[0] is Label lbl)
            {
                lbl.ForeColor = aktif
                    ? Color.White
                    : Color.FromArgb(148, 163, 184);
                lbl.Font = aktif
                    ? new Font("Segoe UI", 9f, FontStyle.Bold)
                    : new Font("Segoe UI", 9f);
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        //  TOPBAR
        // ─────────────────────────────────────────────────────────────────────
        private void BangunTopbar()
        {
            _topbar = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 56,
                BackColor = Color.White
            };
            _topbar.Paint += (s, e) =>
            {
                using var pen = new Pen(Color.FromArgb(229, 231, 235));
                e.Graphics.DrawLine(pen, 0, _topbar.Height - 1, _topbar.Width, _topbar.Height - 1);
            };
            Controls.Add(_topbar);

            // Page title label — diupdate saat navigasi
            var lblPage = new Label
            {
                Name      = "lblPageTitle",
                Text      = "Kelola Inventori Toko",
                Font      = new Font("Segoe UI", 12f, FontStyle.Bold),
                ForeColor = Color.FromArgb(17, 24, 39),
                AutoSize  = true,
                Location  = new Point(24, 16)
            };
            _topbar.Controls.Add(lblPage);
        }

        // ─────────────────────────────────────────────────────────────────────
        //  PAGE AREA
        // ─────────────────────────────────────────────────────────────────────
        private void BangunPageArea()
        {
            _pageArea = new Panel
            {
                Dock      = DockStyle.Fill,
                BackColor = Color.FromArgb(243, 244, 246),
                Padding   = new Padding(24, 20, 24, 20)
            };
            Controls.Add(_pageArea);
        }

        // ─────────────────────────────────────────────────────────────────────
        //  NAVIGASI
        // ─────────────────────────────────────────────────────────────────────
        private void NavigasiKe(int idx)
        {
            _activeMenu = idx;

            // Update sidebar visual
            foreach (var item in _menuItems)
                item.Invalidate();

            // Update topbar title
            var lbl = _topbar.Controls["lblPageTitle"] as Label;
            if (lbl != null) lbl.Text = Menus[idx].Label;

            // Ganti halaman
            _currentPage?.Dispose();
            _currentPage = null;
            _pageArea.Controls.Clear();

            _currentPage = idx switch
            {
                1 => BuatHalamanInventori(),
                2 => BuatHalamanTransaksi(),
                3 => BuatHalamanUpdateStok(),
                4 => BuatHalamanLaporanHarian(),
                5 => BuatHalamanKelolaPesanan(),
                6 => BuatHalamanMonitoringStok(),
                0 => BuatHalamanDashboard(),
                _ => BuatHalamanKosong(Menus[idx].Label)
            };

            _pageArea.Controls.Add(_currentPage);
        }

        // ─────────────────────────────────────────────────────────────────────
        //  HALAMAN: DASHBOARD
        // ─────────────────────────────────────────────────────────────────────
        private Panel BuatHalamanDashboard()
        {
            var page = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };

            // ── Greeting ──────────────────────────────────────────────────────
            page.Controls.Add(new Label
            {
                Text      = $"Selamat datang, {UserContext.Nama} 👋",
                Font      = new Font("Segoe UI", 14f, FontStyle.Bold),
                ForeColor = Color.FromArgb(17, 24, 39),
                AutoSize  = true,
                Location  = new Point(0, 0)
            });
            page.Controls.Add(new Label
            {
                Text      = $"Hari ini, {DateTime.Now:dddd dd MMMM yyyy}  •  RetailSync Manager",
                Font      = new Font("Segoe UI", 9f),
                ForeColor = Color.FromArgb(107, 114, 128),
                AutoSize  = true,
                Location  = new Point(0, 30)
            });

            // ── Baris 1: 4 KPI cards ──────────────────────────────────────────
            var row1 = new Panel
            {
                Location  = new Point(0, 62),
                Height    = 90,
                BackColor = Color.Transparent,
                Anchor    = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            page.Controls.Add(row1);
            page.Resize += (s, e) => row1.Width = page.ClientSize.Width;
            row1.Resize += (s, e) => RelayoutRowN(row1, 4, 12);

            // Load data
            int    totalProduk = 0, stokMenipis = 0, stokHabis = 0, trxHariIni = 0;
            decimal penjualanHariIni = 0;
            try
            {
                var (tot, _, men, hab) = ProdukRepository.GetSummary();
                totalProduk  = tot;
                stokMenipis  = men;
                stokHabis    = hab;
                var (tp, tt, _) = LaporanRepository.GetSummaryHarian(DateTime.Today);
                penjualanHariIni = tp;
                trxHariIni       = tt;
            }
            catch { /* DB mungkin belum ada data */ }

            var kpiDefs = new[]
            {
                ("💰", "Penjualan Hari Ini",  "Rp " + penjualanHariIni.ToString("N0"), Color.FromArgb(22, 163,  74)),
                ("🧾", "Transaksi Hari Ini",  trxHariIni.ToString() + " transaksi",    Color.FromArgb(37,  99, 235)),
                ("⚠",  "Stok Menipis",        stokMenipis.ToString() + " produk",      Color.FromArgb(234, 88,  12)),
                ("🚫", "Stok Habis",           stokHabis.ToString() + " produk",        Color.FromArgb(220,  38,  38)),
            };
            foreach (var (icon, title, value, color) in kpiDefs)
                row1.Controls.Add(BuatKpiCard(icon, title, value, color));

            // ── Baris 2: 3 info cards ─────────────────────────────────────────
            var row2 = new Panel
            {
                Location  = new Point(0, 62 + 90 + 14),
                Height    = 90,
                BackColor = Color.Transparent,
                Anchor    = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            page.Controls.Add(row2);
            page.Resize += (s, e) => row2.Width = page.ClientSize.Width;
            row2.Resize += (s, e) => RelayoutRowN(row2, 3, 12);

            decimal penjualanBulanIni = 0;
            int trxBulanIni = 0;
            try
            {
                var dari   = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                var (pm, jm, _, _, _, _) = KeuanganRepository.GetSummary(dari, DateTime.Today);
                penjualanBulanIni = pm;
                trxBulanIni       = jm;
            }
            catch { }

            var infoDefs = new[]
            {
                ("📦", "Total Produk Terdaftar", totalProduk.ToString() + " produk",               Color.FromArgb(147, 51, 234)),
                ("📊", "Penjualan Bulan Ini",    "Rp " + penjualanBulanIni.ToString("N0"),          Color.FromArgb(37,  99, 235)),
                ("🔄", "Transaksi Bulan Ini",    trxBulanIni.ToString() + " transaksi",             Color.FromArgb(22, 163,  74)),
            };
            foreach (var (icon, title, value, color) in infoDefs)
                row2.Controls.Add(BuatKpiCard(icon, title, value, color));

            // ── Baris 3: quick-nav cards ──────────────────────────────────────
            int row3Y = 62 + 90 + 14 + 90 + 20;
            page.Controls.Add(new Label
            {
                Text      = "Menu Cepat",
                Font      = new Font("Segoe UI", 10f, FontStyle.Bold),
                ForeColor = Color.FromArgb(17, 24, 39),
                AutoSize  = true,
                Location  = new Point(0, row3Y)
            });

            var row3 = new Panel
            {
                Location  = new Point(0, row3Y + 26),
                Height    = 80,
                BackColor = Color.Transparent,
                Anchor    = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            page.Controls.Add(row3);
            page.Resize += (s, e) => row3.Width = page.ClientSize.Width;
            row3.Resize += (s, e) => RelayoutRowN(row3, 4, 12);

            var navDefs = new[]
            {
                (1, "☰",  "Kelola Inventori",   Color.FromArgb(37,  99, 235)),
                (2, "📊", "Transaksi Baru",      Color.FromArgb(22, 163,  74)),
                (4, "📋", "Laporan Harian",      Color.FromArgb(147, 51, 234)),
                (5, "📦", "Restock Produk",      Color.FromArgb(234,  88,  12)),
            };
            foreach (var (navIdx, icon, label, color) in navDefs)
            {
                int captIdx = navIdx;
                var card = BuatNavCard(icon, label, color);
                card.Click += (s, e) => NavigasiKe(captIdx);
                // Label di dalam juga perlu klik
                foreach (Control c in card.Controls)
                    c.Click += (s, e) => NavigasiKe(captIdx);
                row3.Controls.Add(card);
            }

            return page;
        }

        // ── KPI Card ──────────────────────────────────────────────────────────
        private static Panel BuatKpiCard(string icon, string title, string value, Color accent)
        {
            var card = new Panel { BackColor = Color.White, Size = new Size(200, 90) };
            card.Paint += (s, e) =>
            {
                using var pen = new Pen(Color.FromArgb(229, 231, 235));
                e.Graphics.DrawRectangle(pen, 0, 0, card.Width - 1, card.Height - 1);
                using var br = new SolidBrush(accent);
                e.Graphics.FillRectangle(br, 0, 0, 4, card.Height);
            };

            card.Controls.Add(new Label
            {
                Text = icon, Font = new Font("Segoe UI", 20f),
                Location = new Point(12, 12), Size = new Size(36, 36),
                BackColor = Color.Transparent, TextAlign = ContentAlignment.MiddleCenter
            });
            card.Controls.Add(new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 13f, FontStyle.Bold),
                ForeColor = Color.FromArgb(17, 24, 39),
                Location = new Point(56, 8), Size = new Size(200, 30),
                BackColor = Color.Transparent, AutoSize = false
            });
            card.Controls.Add(new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 8f),
                ForeColor = Color.FromArgb(107, 114, 128),
                Location = new Point(56, 44), AutoSize = true,
                BackColor = Color.Transparent
            });
            return card;
        }

        // ── Nav Card (quick-nav) ──────────────────────────────────────────────
        private static Panel BuatNavCard(string icon, string label, Color accent)
        {
            var card = new Panel
            {
                BackColor = accent, Size = new Size(160, 80), Cursor = Cursors.Hand
            };
            card.Paint += (s, e) =>
            {
                using var br = new SolidBrush(Color.FromArgb(30, Color.Black));
                e.Graphics.FillRectangle(br, card.Width - 60, 0, 60, card.Height);
            };
            card.Controls.Add(new Label
            {
                Text = icon, Font = new Font("Segoe UI", 22f),
                Location = new Point(card.Width - 46, 10), Size = new Size(40, 40),
                BackColor = Color.Transparent, ForeColor = Color.FromArgb(180, 255, 255, 255),
                TextAlign = ContentAlignment.MiddleCenter, Cursor = Cursors.Hand
            });
            card.Controls.Add(new Label
            {
                Text = label,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(12, 22), AutoSize = true,
                BackColor = Color.Transparent, Cursor = Cursors.Hand
            });
            return card;
        }

        // ── Helper: layout N kontrol dalam satu baris dengan gap ─────────────
        private static void RelayoutRowN(Panel row, int count, int gap)
        {
            int avail = row.ClientSize.Width;
            if (avail <= 0 || row.Controls.Count == 0) return;
            int n  = Math.Min(count, row.Controls.Count);
            int cw = (avail - gap * (n - 1)) / n;
            for (int i = 0; i < row.Controls.Count; i++)
            {
                row.Controls[i].Location = new Point(i * (cw + gap), 0);
                row.Controls[i].Size     = new Size(cw, row.Height);
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        //  HALAMAN: KOSONG / PLACEHOLDER
        // ─────────────────────────────────────────────────────────────────────
        private Panel BuatHalamanKosong(string title)
        {
            var p = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };
            p.Controls.Add(new Label
            {
                Text      = $"{title}\n\nFitur ini akan segera tersedia.",
                Font      = new Font("Segoe UI", 11f),
                ForeColor = Color.FromArgb(107, 114, 128),
                AutoSize  = true,
                Location  = new Point(0, 0)
            });
            return p;
        }

        // ─────────────────────────────────────────────────────────────────────
        //  HALAMAN: KELOLA INVENTORI TOKO
        // ─────────────────────────────────────────────────────────────────────
        private Panel BuatHalamanInventori()
        {
            var page = new Panel
            {
                Dock      = DockStyle.Fill,
                BackColor = Color.Transparent
            };

            // ── Sub-header ────────────────────────────────────────────────────
            var sub = new Label
            {
                Text      = "Kelola data produk, stok, kategori, dan informasi inventori toko",
                Font      = new Font("Segoe UI", 9f),
                ForeColor = Color.FromArgb(107, 114, 128),
                AutoSize  = true,
                Location  = new Point(0, 0)
            };
            page.Controls.Add(sub);

            // ── Summary Cards ─────────────────────────────────────────────────
            var cardDefs = new[]
            {
                ("Total Produk",  "🛒", Color.FromArgb(37,  99, 235)),
                ("Stok Tersedia", "📦", Color.FromArgb(22, 163,  74)),
                ("Stok Menipis",  "⚠",  Color.FromArgb(234, 88,  12)),
                ("Stok Habis",    "🚫", Color.FromArgb(220,  38,  38)),
            };
            var angkaRef = new Label[4];
            var cardRow = new Panel
            {
                Location  = new Point(0, 28),
                Height    = 82,
                BackColor = Color.Transparent,
                Anchor    = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            page.Controls.Add(cardRow);

            page.Resize += (s, e) => cardRow.Width = page.ClientSize.Width;
            cardRow.Resize += (s, e) =>
            {
                int gap   = 12;
                int total = (cardRow.Width - gap * 3) / 4;
                for (int i = 0; i < 4; i++)
                {
                    if (i < cardRow.Controls.Count)
                    {
                        cardRow.Controls[i].Location = new Point(i * (total + gap), 0);
                        cardRow.Controls[i].Width    = total;
                    }
                }
            };

            for (int i = 0; i < 4; i++)
            {
                var (ttl, icon, clr) = cardDefs[i];
                var (card, lbl) = BuatSummaryCard(ttl, icon, clr, 82);
                cardRow.Controls.Add(card);
                angkaRef[i] = lbl;
            }
            _lblTotal    = angkaRef[0];
            _lblTersedia = angkaRef[1];
            _lblMenipis  = angkaRef[2];
            _lblHabis    = angkaRef[3];

            // ── Main table panel ──────────────────────────────────────────────
            var tableCard = new Panel
            {
                Location  = new Point(0, 122),
                BackColor = Color.White,
                Anchor    = AnchorStyles.Top | AnchorStyles.Left
                          | AnchorStyles.Right | AnchorStyles.Bottom
            };
            tableCard.Paint += (s, e) =>
            {
                using var pen = new Pen(Color.FromArgb(229, 231, 235));
                e.Graphics.DrawRectangle(pen, 0, 0, tableCard.Width - 1, tableCard.Height - 1);
            };
            page.Controls.Add(tableCard);

            page.Resize += (s, e) =>
            {
                tableCard.Width  = page.ClientSize.Width;
                tableCard.Height = page.ClientSize.Height - 122;
            };

            // Toolbar
            var toolbar = new Panel
            {
                Location  = new Point(1, 1),
                Height    = 52,
                BackColor = Color.White
            };
            tableCard.Controls.Add(toolbar);
            tableCard.Resize += (s, e) => toolbar.Width = tableCard.ClientSize.Width - 2;
            BangunToolbarInventori(toolbar);

            // Divider
            var d1 = new Panel { Location = new Point(0, 53), Height = 1, BackColor = Color.FromArgb(229, 231, 235) };
            tableCard.Controls.Add(d1);
            tableCard.Resize += (s, e) => d1.Width = tableCard.ClientSize.Width;

            // Header kolom
            var colHeader = new Panel
            {
                Location  = new Point(0, 54),
                Height    = 36,
                BackColor = Color.FromArgb(248, 250, 252)
            };
            tableCard.Controls.Add(colHeader);
            tableCard.Resize += (s, e) =>
            {
                colHeader.Width = tableCard.ClientSize.Width;
                GambarHeaderKolom(colHeader);
            };
            GambarHeaderKolom(colHeader);

            var d2 = new Panel { Location = new Point(0, 90), Height = 1, BackColor = Color.FromArgb(229, 231, 235) };
            tableCard.Controls.Add(d2);
            tableCard.Resize += (s, e) => d2.Width = tableCard.ClientSize.Width;

            // Table body
            _tableBody = new Panel
            {
                Location  = new Point(0, 91),
                BackColor = Color.White
            };
            tableCard.Controls.Add(_tableBody);
            tableCard.Resize += (s, e) =>
            {
                _tableBody.Size = new Size(tableCard.ClientSize.Width, tableCard.ClientSize.Height - 91 - 44);
                RefreshBaris();
            };

            // Pagination
            var pagPanel = new Panel
            {
                BackColor = Color.White,
                Height    = 44
            };
            tableCard.Controls.Add(pagPanel);
            tableCard.Resize += (s, e) =>
            {
                pagPanel.Width    = tableCard.ClientSize.Width;
                pagPanel.Location = new Point(0, tableCard.ClientSize.Height - 44);
            };
            BangunPaginasiPanel(pagPanel);

            // Build baris
            BangunBarisTabel();

            // Load data
            MuatDataInventori();

            return page;
        }

        // ─────────────────────────────────────────────────────────────────────
        //  HALAMAN: TRANSAKSI PENJUALAN KASIR
        // ─────────────────────────────────────────────────────────────────────
        private DateTimePicker? _dtpDari, _dtpSampai;
        private Panel? _trxTableBody;
        private Label? _lblTrxPag, _lblTotalTrx, _lblTotalPemasukan;
        private Button? _btnTrxPrev, _btnTrxNext;
        private List<TransaksiRow> _allTrx   = new();
        private List<TransaksiRow> _filtTrx  = new();
        private int _trxPage = 1;
        private const int TrxPageSize = 10;

        private Panel BuatHalamanTransaksi()
        {
            var page = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };

            // Sub-title
            page.Controls.Add(new Label
            {
                Text      = "Pantau performa penjualan melalui laporan yang lengkap dan akurat.",
                Font      = new Font("Segoe UI", 9f),
                ForeColor = Color.FromArgb(107, 114, 128),
                AutoSize  = true,
                Location  = new Point(0, 0)
            });

            // ── Summary strip ─────────────────────────────────────────────────
            var summaryStrip = new Panel
            {
                Location  = new Point(0, 26),
                Height    = 64,
                BackColor = Color.Transparent,
                Anchor    = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            page.Controls.Add(summaryStrip);
            page.Resize += (s, e) => summaryStrip.Width = page.ClientSize.Width;

            _lblTotalTrx = new Label
            {
                Text      = "–",
                Font      = new Font("Segoe UI", 18f, FontStyle.Bold),
                ForeColor = Color.FromArgb(37, 99, 235),
                AutoSize  = true,
                Location  = new Point(0, 4)
            };
            var lblTotalTrxCaption = new Label
            {
                Text      = "Total Transaksi",
                Font      = new Font("Segoe UI", 8f),
                ForeColor = Color.FromArgb(107, 114, 128),
                AutoSize  = true,
                Location  = new Point(0, 38)
            };
            _lblTotalPemasukan = new Label
            {
                Text      = "–",
                Font      = new Font("Segoe UI", 18f, FontStyle.Bold),
                ForeColor = Color.FromArgb(22, 163, 74),
                AutoSize  = true,
                Location  = new Point(160, 4)
            };
            var lblPemasukanCaption = new Label
            {
                Text      = "Total Pemasukan",
                Font      = new Font("Segoe UI", 8f),
                ForeColor = Color.FromArgb(107, 114, 128),
                AutoSize  = true,
                Location  = new Point(160, 38)
            };
            summaryStrip.Controls.AddRange(new Control[]
            { _lblTotalTrx, lblTotalTrxCaption, _lblTotalPemasukan, lblPemasukanCaption });

            // ── Filter tanggal ────────────────────────────────────────────────
            var filterPanel = new Panel
            {
                Location  = new Point(0, 96),
                Height    = 36,
                BackColor = Color.Transparent,
                Anchor    = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            page.Controls.Add(filterPanel);

            filterPanel.Controls.Add(new Label
            {
                Text      = "Dari:",
                Font      = new Font("Segoe UI", 9f),
                ForeColor = Color.FromArgb(55, 65, 81),
                AutoSize  = true,
                Location  = new Point(0, 8)
            });
            _dtpDari = new DateTimePicker
            {
                Format   = DateTimePickerFormat.Short,
                Value    = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1),
                Location = new Point(40, 4),
                Size     = new Size(130, 28),
                Font     = new Font("Segoe UI", 9f)
            };
            _dtpDari.ValueChanged += (s, e) => MuatDataTransaksi();
            filterPanel.Controls.Add(_dtpDari);

            filterPanel.Controls.Add(new Label
            {
                Text      = "s/d",
                Font      = new Font("Segoe UI", 9f),
                ForeColor = Color.FromArgb(55, 65, 81),
                AutoSize  = true,
                Location  = new Point(178, 8)
            });
            _dtpSampai = new DateTimePicker
            {
                Format   = DateTimePickerFormat.Short,
                Value    = DateTime.Today,
                Location = new Point(200, 4),
                Size     = new Size(130, 28),
                Font     = new Font("Segoe UI", 9f)
            };
            _dtpSampai.ValueChanged += (s, e) => MuatDataTransaksi();
            filterPanel.Controls.Add(_dtpSampai);

            // Tombol Transaksi Baru
            var btnBaru = new Button
            {
                Text      = "＋ Transaksi Baru",
                Size      = new Size(130, 30),
                BackColor = Color.FromArgb(22, 163, 74),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font      = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                Cursor    = Cursors.Hand
            };
            btnBaru.Click += (s, e) => BukaDlgTransaksiBaru();
            filterPanel.Controls.Add(btnBaru);
            filterPanel.Resize += (s, e) =>
                btnBaru.Location = new Point(filterPanel.ClientSize.Width - btnBaru.Width, 3);
            page.Resize += (s, e) => filterPanel.Width = page.ClientSize.Width;

            // ── Tabel Data Penjualan ──────────────────────────────────────────
            var tableCard = new Panel
            {
                Location  = new Point(0, 140),
                BackColor = Color.White,
                Anchor    = AnchorStyles.Top | AnchorStyles.Left
                          | AnchorStyles.Right | AnchorStyles.Bottom
            };
            tableCard.Paint += (s, e) =>
            {
                using var pen = new Pen(Color.FromArgb(229, 231, 235));
                e.Graphics.DrawRectangle(pen, 0, 0, tableCard.Width - 1, tableCard.Height - 1);
            };
            page.Controls.Add(tableCard);
            page.Resize += (s, e) =>
            {
                tableCard.Width  = page.ClientSize.Width;
                tableCard.Height = Math.Max(page.ClientSize.Height - 140, 200);
            };

            // Label "Data Penjualan"
            var lblDataJudul = new Label
            {
                Text      = "Data Penjualan",
                Font      = new Font("Segoe UI", 10f, FontStyle.Bold),
                ForeColor = Color.FromArgb(17, 24, 39),
                Location  = new Point(14, 12),
                AutoSize  = true,
                BackColor = Color.White
            };
            tableCard.Controls.Add(lblDataJudul);

            // Header kolom
            var colHeader = new Panel
            {
                Location  = new Point(0, 40),
                Height    = 34,
                BackColor = Color.FromArgb(248, 250, 252)
            };
            tableCard.Controls.Add(colHeader);
            tableCard.Resize += (s, e) =>
            {
                colHeader.Width = tableCard.ClientSize.Width;
                GambarHeaderTrx(colHeader);
            };
            GambarHeaderTrx(colHeader);

            var divH = new Panel { Location = new Point(0, 74), Height = 1, BackColor = Color.FromArgb(229, 231, 235) };
            tableCard.Controls.Add(divH);
            tableCard.Resize += (s, e) => divH.Width = tableCard.ClientSize.Width;

            // Body
            _trxTableBody = new Panel { Location = new Point(0, 75), BackColor = Color.White };
            tableCard.Controls.Add(_trxTableBody);
            tableCard.Resize += (s, e) =>
            {
                _trxTableBody.Size = new Size(tableCard.ClientSize.Width,
                    tableCard.ClientSize.Height - 75 - 44);
                RefreshTrxBaris();
            };

            // Pagination
            var pagPanel = new Panel { BackColor = Color.White, Height = 44 };
            tableCard.Controls.Add(pagPanel);
            tableCard.Resize += (s, e) =>
            {
                pagPanel.Width    = tableCard.ClientSize.Width;
                pagPanel.Location = new Point(0, tableCard.ClientSize.Height - 44);
            };
            BuatTrxPaginasi(pagPanel);

            // Baris kosong
            BangunBarisTrx();

            // Load data
            MuatDataTransaksi();

            return page;
        }

        // ── Header kolom transaksi ────────────────────────────────────────────
        private static readonly string[] TrxColH  = { "No", "Waktu", "Kasir (ID)", "Nama Toko", "Produk", "Total" };
        private static readonly int[]    TrxColPct = { 4, 16, 12, 18, 30, 14 };

        private void GambarHeaderTrx(Panel hp)
        {
            hp.Controls.Clear();
            if (hp.Width < 50) return;
            int tot = TrxColPct.Sum(), x = 0;
            for (int i = 0; i < TrxColH.Length; i++)
            {
                int w = hp.Width * TrxColPct[i] / tot;
                hp.Controls.Add(new Label
                {
                    Text      = TrxColH[i],
                    Font      = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                    ForeColor = Color.FromArgb(75, 85, 99),
                    Location  = new Point(x + 10, 0),
                    Size      = new Size(w, 34),
                    TextAlign = ContentAlignment.MiddleLeft,
                    BackColor = Color.Transparent
                });
                x += w;
            }
        }

        // ── Baris tabel transaksi ─────────────────────────────────────────────
        private readonly List<Panel>   _trxRows  = new();
        private readonly List<Label[]> _trxCells = new();

        private void BangunBarisTrx()
        {
            if (_trxTableBody == null) return;
            _trxTableBody.Controls.Clear();
            _trxRows.Clear();
            _trxCells.Clear();
            int rowH = 40;

            for (int i = 0; i < TrxPageSize; i++)
            {
                int ri = i;
                var rp = new Panel
                {
                    Location  = new Point(0, i * rowH),
                    Height    = rowH,
                    BackColor = i % 2 == 0 ? Color.White : Color.FromArgb(249, 250, 251)
                };
                rp.Paint += (s, e) =>
                {
                    using var pen = new Pen(Color.FromArgb(243, 244, 246));
                    e.Graphics.DrawLine(pen, 0, rp.Height - 1, rp.Width, rp.Height - 1);
                };

                var cols = new Label[6];
                for (int c = 0; c < 6; c++)
                {
                    cols[c] = new Label
                    {
                        AutoSize  = false,
                        Font      = new Font("Segoe UI", 8.5f),
                        ForeColor = Color.FromArgb(55, 65, 81),
                        BackColor = Color.Transparent,
                        TextAlign = c == 5 ? ContentAlignment.MiddleRight : ContentAlignment.MiddleLeft,
                        Height    = rowH,
                        Top       = 0
                    };
                    rp.Controls.Add(cols[c]);
                }
                rp.Resize += (s, e) => LayoutTrxKolom(rp, cols, rowH);

                _trxRows.Add(rp);
                _trxCells.Add(cols);
                _trxTableBody.Controls.Add(rp);
            }
            RefreshTrxBaris();
        }

        private void RefreshTrxBaris()
        {
            if (_trxTableBody == null) return;
            int w = _trxTableBody.ClientSize.Width;
            if (w <= 0) return;
            for (int i = 0; i < _trxRows.Count; i++)
            {
                _trxRows[i].Size     = new Size(w, 40);
                _trxRows[i].Location = new Point(0, i * 40);
                LayoutTrxKolom(_trxRows[i], _trxCells[i], 40);
            }
        }

        private void LayoutTrxKolom(Panel rp, Label[] cols, int h)
        {
            int tot = TrxColPct.Sum(), x = 0, w = rp.ClientSize.Width;
            if (w <= 0) return;
            for (int c = 0; c < cols.Length; c++)
            {
                int cw = w * TrxColPct[c] / tot;
                cols[c].Location = new Point(x + (c == 5 ? 0 : 10), 0);
                cols[c].Size     = new Size(Math.Max(cw - 10, 10), h);
                x += cw;
            }
        }

        private void BuatTrxPaginasi(Panel bar)
        {
            _lblTrxPag = new Label
            {
                AutoSize  = true,
                Font      = new Font("Segoe UI", 8.5f),
                ForeColor = Color.FromArgb(107, 114, 128),
                Location  = new Point(12, 14),
                BackColor = Color.Transparent
            };
            bar.Controls.Add(_lblTrxPag);

            _btnTrxNext = BtnPag("›");
            _btnTrxPrev = BtnPag("‹");
            _btnTrxNext.Click += (s, e) =>
            { if (_trxPage < TrxTotalHal) { _trxPage++; IsiBarisTrx(); UpdateTrxPag(); } };
            _btnTrxPrev.Click += (s, e) =>
            { if (_trxPage > 1) { _trxPage--; IsiBarisTrx(); UpdateTrxPag(); } };

            bar.Controls.Add(_btnTrxPrev);
            bar.Controls.Add(_btnTrxNext);
            bar.Resize += (s, e) =>
            {
                _btnTrxNext.Location = new Point(bar.ClientSize.Width - 38, 8);
                _btnTrxPrev.Location = new Point(bar.ClientSize.Width - 70, 8);
            };
        }

        private int TrxTotalHal => Math.Max(1, (int)Math.Ceiling((double)_filtTrx.Count / TrxPageSize));

        private void MuatDataTransaksi()
        {
            if (_dtpDari == null || _dtpSampai == null) return;
            try
            {
                _allTrx  = TransaksiRepository.GetByRange(_dtpDari.Value, _dtpSampai.Value);
                _filtTrx = _allTrx;
                _trxPage = 1;

                var (jml, pemasukan) = TransaksiRepository.GetRangeSummary(_dtpDari.Value, _dtpSampai.Value);
                if (_lblTotalTrx      != null) _lblTotalTrx.Text      = jml.ToString();
                if (_lblTotalPemasukan!= null) _lblTotalPemasukan.Text = "Rp " + pemasukan.ToString("N0");

                IsiBarisTrx();
                UpdateTrxPag();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat transaksi: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void IsiBarisTrx()
        {
            if (_trxRows.Count == 0) return;
            var page = _filtTrx.Skip((_trxPage - 1) * TrxPageSize).Take(TrxPageSize).ToList();
            for (int i = 0; i < TrxPageSize; i++)
            {
                if (i < page.Count)
                {
                    var t  = page[i];
                    int no = (_trxPage - 1) * TrxPageSize + i + 1;
                    _trxCells[i][0].Text = no.ToString();
                    _trxCells[i][1].Text = t.Waktu.ToString("dd/MM/yy HH:mm");
                    _trxCells[i][2].Text = t.NamaKasir;
                    _trxCells[i][3].Text = t.NamaToko;
                    _trxCells[i][4].Text = t.NamaProduk;
                    _trxCells[i][5].Text = "Rp " + t.Total.ToString("N0");
                    _trxCells[i][5].ForeColor = Color.FromArgb(22, 163, 74);
                    _trxRows[i].Visible = true;
                }
                else
                {
                    _trxRows[i].Visible = false;
                }
            }
        }

        private void UpdateTrxPag()
        {
            if (_lblTrxPag == null) return;
            int start = _filtTrx.Count == 0 ? 0 : (_trxPage - 1) * TrxPageSize + 1;
            int end   = Math.Min(_trxPage * TrxPageSize, _filtTrx.Count);
            _lblTrxPag.Text      = $"Menampilkan {start}–{end} dari {_filtTrx.Count} transaksi";
            if (_btnTrxPrev != null) _btnTrxPrev.Enabled = _trxPage > 1;
            if (_btnTrxNext != null) _btnTrxNext.Enabled = _trxPage < TrxTotalHal;
        }

        // ── Dialog transaksi baru ─────────────────────────────────────────────
        private void BukaDlgTransaksiBaru()
        {
            using var dlg = new FormTransaksiBaru(UserContext.IdPengguna);
            if (dlg.ShowDialog(this) == DialogResult.OK)
                MuatDataTransaksi();
        }
        // ─────────────────────────────────────────────────────────────────────
        //  HALAMAN 3: UPDATE STOK OTOMATIS
        // ─────────────────────────────────────────────────────────────────────
        private TextBox?  _stokSearch   = null;
        private ComboBox? _stokCmbSt    = null;
        private ComboBox? _stokCmbKat   = null;
        private Panel?    _stokBody     = null;
        private Label?    _stokPagLbl   = null;
        private Button?   _stokPrev     = null, _stokNext = null;
        private List<StokRow> _allStok  = new();
        private List<StokRow> _filtStok = new();
        private int _stokPage = 1;
        private const int StokPageSize = 8;

        private Label _lblStokTotal = null!, _lblStokMasuk = null!,
                      _lblStokKeluar = null!, _lblStokMenipis = null!;

        private Panel BuatHalamanUpdateStok()
        {
            var page = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };

            page.Controls.Add(new Label
            {
                Text      = "Stok akan terupdate secara otomatis ketika ada transaksi penjualan atau penerimaan barang.",
                Font      = new Font("Segoe UI", 9f),
                ForeColor = Color.FromArgb(107, 114, 128),
                AutoSize  = true,
                Location  = new Point(0, 0)
            });

            // ── Summary cards ─────────────────────────────────────────────────
            var cardDefs = new[]
            {
                ("Total Produk",        "🛒", Color.FromArgb(37,  99, 235)),
                ("Stok Masuk Hari Ini", "⬆",  Color.FromArgb(22, 163,  74)),
                ("Stok Keluar Hari Ini","⬇",  Color.FromArgb(234, 88,  12)),
                ("Stok Menipis",        "⚠",  Color.FromArgb(220,  38,  38)),
            };
            var angkaRef = new Label[4];
            var cardRow  = BuatCardRow(page, 26, cardDefs, angkaRef);
            _lblStokTotal   = angkaRef[0];
            _lblStokMasuk   = angkaRef[1];
            _lblStokKeluar  = angkaRef[2];
            _lblStokMenipis = angkaRef[3];

            // ── Table card ────────────────────────────────────────────────────
            var tableCard = BuatTableCard(page, 26 + 82 + 14);

            // Toolbar
            var toolbar = new Panel
            {
                Location  = new Point(1, 1),
                Height    = 52,
                BackColor = Color.White
            };
            tableCard.Controls.Add(toolbar);
            tableCard.Resize += (s, e) => toolbar.Width = tableCard.ClientSize.Width - 2;

            _stokSearch = new TextBox
            {
                PlaceholderText = "Cari nama produk...",
                Font            = new Font("Segoe UI", 9f),
                BorderStyle     = BorderStyle.FixedSingle,
                Location        = new Point(12, 12),
                Size            = new Size(180, 28)
            };
            _stokSearch.TextChanged += (s, e) => FilterStok();

            _stokCmbSt = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font          = new Font("Segoe UI", 9f),
                Location      = new Point(200, 12),
                Size          = new Size(140, 28)
            };
            _stokCmbSt.Items.AddRange(new[] { "Semua Status", "Tersedia", "Stok Menipis", "Stok Habis" });
            _stokCmbSt.SelectedIndex = 0;
            _stokCmbSt.SelectedIndexChanged += (s, e) => FilterStok();

            _stokCmbKat = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font          = new Font("Segoe UI", 9f),
                Location      = new Point(348, 12),
                Size          = new Size(140, 28)
            };
            _stokCmbKat.Items.AddRange(ProdukRepository.GetKategori().ToArray());
            _stokCmbKat.SelectedIndex = 0;
            _stokCmbKat.SelectedIndexChanged += (s, e) => FilterStok();

            var btnRefresh = new Button
            {
                Text      = "↻ Refresh",
                Size      = new Size(88, 28),
                BackColor = Color.FromArgb(107, 114, 128),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font      = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                Cursor    = Cursors.Hand
            };
            btnRefresh.Click += (s, e) => MuatDataStok();
            toolbar.Controls.AddRange(new Control[] { _stokSearch, _stokCmbSt, _stokCmbKat, btnRefresh });
            toolbar.Resize += (s, e) =>
                btnRefresh.Location = new Point(toolbar.ClientSize.Width - btnRefresh.Width - 10, 12);

            // Divider + header kolom
            PasangDividerHeader(tableCard, 53,
                new[] { "No", "Nama Produk", "Kategori", "Stok Saat Ini", "Perubahan Terakhir", "Sumber", "Status" },
                new[] { 4, 24, 13, 12, 16, 14, 11 });

            // Body
            _stokBody = new Panel { Location = new Point(0, 91), BackColor = Color.White };
            tableCard.Controls.Add(_stokBody);
            tableCard.Resize += (s, e) =>
            {
                _stokBody.Size = new Size(tableCard.ClientSize.Width,
                    tableCard.ClientSize.Height - 91 - 44);
                RefreshStokBaris();
            };

            // Pagination
            var pagPanel = new Panel { BackColor = Color.White, Height = 44 };
            tableCard.Controls.Add(pagPanel);
            tableCard.Resize += (s, e) =>
            {
                pagPanel.Width    = tableCard.ClientSize.Width;
                pagPanel.Location = new Point(0, tableCard.ClientSize.Height - 44);
            };
            BuatStokPaginasi(pagPanel);

            BangunBarisStok();
            MuatDataStok();

            return page;
        }

        // ── Helper: baris card row ────────────────────────────────────────────
        private Panel BuatCardRow(Panel page, int topOffset,
            (string, string, Color)[] defs, Label[] refs)
        {
            var cardRow = new Panel
            {
                Location  = new Point(0, topOffset + 22),
                Height    = 82,
                BackColor = Color.Transparent,
                Anchor    = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            page.Controls.Add(cardRow);
            page.Resize  += (s, e) => cardRow.Width = page.ClientSize.Width;
            cardRow.Resize += (s, e) =>
            {
                int gap = 12, w = (cardRow.Width - gap * 3) / 4;
                for (int i = 0; i < Math.Min(4, cardRow.Controls.Count); i++)
                {
                    cardRow.Controls[i].Location = new Point(i * (w + gap), 0);
                    cardRow.Controls[i].Width    = w;
                }
            };
            for (int i = 0; i < defs.Length; i++)
            {
                var (ttl, icon, clr) = defs[i];
                var (card, lbl) = BuatSummaryCard(ttl, icon, clr, 82);
                cardRow.Controls.Add(card);
                refs[i] = lbl;
            }
            return cardRow;
        }

        // ── Helper: buat table card dengan resize ─────────────────────────────
        private Panel BuatTableCard(Panel page, int topY)
        {
            var tc = new Panel
            {
                Location  = new Point(0, topY + 14),
                BackColor = Color.White,
                Anchor    = AnchorStyles.Top | AnchorStyles.Left
                          | AnchorStyles.Right | AnchorStyles.Bottom
            };
            tc.Paint += (s, e) =>
            {
                using var pen = new Pen(Color.FromArgb(229, 231, 235));
                e.Graphics.DrawRectangle(pen, 0, 0, tc.Width - 1, tc.Height - 1);
            };
            page.Controls.Add(tc);
            page.Resize += (s, e) =>
            {
                tc.Width  = page.ClientSize.Width;
                tc.Height = Math.Max(page.ClientSize.Height - topY - 14, 200);
            };
            return tc;
        }

        // ── Helper: pasang divider + header kolom ─────────────────────────────
        private void PasangDividerHeader(Panel tc, int divY, string[] headers, int[] pcts)
        {
            var d1 = new Panel { Location = new Point(0, divY), Height = 1, BackColor = Color.FromArgb(229, 231, 235) };
            tc.Controls.Add(d1);
            tc.Resize += (s, e) => d1.Width = tc.ClientSize.Width;

            var hRow = new Panel { Location = new Point(0, divY + 1), Height = 34, BackColor = Color.FromArgb(248, 250, 252) };
            tc.Controls.Add(hRow);
            tc.Resize += (s, e) =>
            {
                hRow.Width = tc.ClientSize.Width;
                GambarHeader(hRow, headers, pcts);
            };
            GambarHeader(hRow, headers, pcts);

            var d2 = new Panel { Location = new Point(0, divY + 35), Height = 1, BackColor = Color.FromArgb(229, 231, 235) };
            tc.Controls.Add(d2);
            tc.Resize += (s, e) => d2.Width = tc.ClientSize.Width;
        }

        private void GambarHeader(Panel hp, string[] headers, int[] pcts)
        {
            hp.Controls.Clear();
            if (hp.Width < 50) return;
            int tot = 0; foreach (var p in pcts) tot += p;
            int x = 0;
            for (int i = 0; i < headers.Length; i++)
            {
                int w = hp.Width * pcts[i] / tot;
                hp.Controls.Add(new Label
                {
                    Text      = headers[i],
                    Font      = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                    ForeColor = Color.FromArgb(75, 85, 99),
                    Location  = new Point(x + 10, 0),
                    Size      = new Size(w, 34),
                    TextAlign = ContentAlignment.MiddleLeft,
                    BackColor = Color.Transparent
                });
                x += w;
            }
        }

        // ── Baris tabel stok ──────────────────────────────────────────────────
        private static readonly int[] StokPct = { 4, 24, 13, 12, 16, 14, 11 };
        private readonly List<Panel>   _stokRows  = new();
        private readonly List<Label[]> _stokCells = new();

        private void BangunBarisStok()
        {
            if (_stokBody == null) return;
            _stokBody.Controls.Clear();
            _stokRows.Clear();
            _stokCells.Clear();
            int rowH = 40;
            for (int i = 0; i < StokPageSize; i++)
            {
                int ri = i;
                var rp = new Panel
                {
                    Location  = new Point(0, i * rowH),
                    Height    = rowH,
                    BackColor = i % 2 == 0 ? Color.White : Color.FromArgb(249, 250, 251)
                };
                rp.Paint += (s, e) =>
                {
                    using var pen = new Pen(Color.FromArgb(243, 244, 246));
                    e.Graphics.DrawLine(pen, 0, rp.Height - 1, rp.Width, rp.Height - 1);
                };
                var cols = new Label[7];
                for (int c = 0; c < 7; c++)
                {
                    cols[c] = new Label
                    {
                        AutoSize  = false,
                        Font      = new Font("Segoe UI", 8.5f),
                        ForeColor = Color.FromArgb(55, 65, 81),
                        BackColor = Color.Transparent,
                        TextAlign = ContentAlignment.MiddleLeft,
                        Height    = rowH, Top = 0
                    };
                    rp.Controls.Add(cols[c]);
                }
                rp.Resize += (s, e) =>
                {
                    int tot = 0; foreach (var p in StokPct) tot += p;
                    int x = 0, w = rp.ClientSize.Width;
                    if (w <= 0) return;
                    for (int c = 0; c < cols.Length; c++)
                    {
                        int cw = w * StokPct[c] / tot;
                        cols[c].Location = new Point(x + 10, 0);
                        cols[c].Size     = new Size(Math.Max(cw - 10, 10), rowH);
                        x += cw;
                    }
                };
                _stokRows.Add(rp);
                _stokCells.Add(cols);
                _stokBody.Controls.Add(rp);
            }
            RefreshStokBaris();
        }

        private void RefreshStokBaris()
        {
            if (_stokBody == null) return;
            int w = _stokBody.ClientSize.Width;
            if (w <= 0) return;
            for (int i = 0; i < _stokRows.Count; i++)
            {
                _stokRows[i].Size     = new Size(w, 40);
                _stokRows[i].Location = new Point(0, i * 40);
                _stokRows[i].PerformLayout();
            }
        }

        private void BuatStokPaginasi(Panel bar)
        {
            _stokPagLbl = new Label
            {
                AutoSize  = true,
                Font      = new Font("Segoe UI", 8.5f),
                ForeColor = Color.FromArgb(107, 114, 128),
                Location  = new Point(12, 14),
                BackColor = Color.Transparent
            };
            bar.Controls.Add(_stokPagLbl);
            _stokNext = BtnPag("›");
            _stokPrev = BtnPag("‹");
            _stokNext.Click += (s, e) =>
            { if (_stokPage < StokTotalHal) { _stokPage++; IsiBarisStok(); UpdateStokPag(); } };
            _stokPrev.Click += (s, e) =>
            { if (_stokPage > 1) { _stokPage--; IsiBarisStok(); UpdateStokPag(); } };
            bar.Controls.Add(_stokPrev);
            bar.Controls.Add(_stokNext);
            bar.Resize += (s, e) =>
            {
                _stokNext.Location = new Point(bar.ClientSize.Width - 38, 8);
                _stokPrev.Location = new Point(bar.ClientSize.Width - 70, 8);
            };
        }

        private int StokTotalHal => Math.Max(1, (int)Math.Ceiling((double)_filtStok.Count / StokPageSize));

        private void MuatDataStok()
        {
            try
            {
                _allStok = StokRepository.GetStokLengkap();
                var (tot, masuk, keluar, menipis) = StokRepository.GetStokSummary();
                _lblStokTotal.Text   = tot.ToString();
                _lblStokMasuk.Text   = masuk.ToString();
                _lblStokKeluar.Text  = keluar.ToString();
                _lblStokMenipis.Text = menipis.ToString();
                FilterStok();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat stok: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FilterStok()
        {
            if (_stokSearch == null) return;
            string kw  = _stokSearch.Text.Trim().ToLower();
            string st  = _stokCmbSt?.Text  ?? "Semua Status";
            string kat = _stokCmbKat?.Text ?? "Semua Kategori";

            _filtStok = _allStok.Where(p =>
                (string.IsNullOrEmpty(kw) || p.NamaProduk.ToLower().Contains(kw)) &&
                (st  == "Semua Status"   || p.Status   == st) &&
                (kat == "Semua Kategori" || p.Kategori  == kat)
            ).ToList();

            _stokPage = 1;
            IsiBarisStok();
            UpdateStokPag();
        }

        private void IsiBarisStok()
        {
            if (_stokRows.Count == 0) return;
            var page = _filtStok.Skip((_stokPage - 1) * StokPageSize).Take(StokPageSize).ToList();
            for (int i = 0; i < StokPageSize; i++)
            {
                if (i < page.Count)
                {
                    var r  = page[i];
                    int no = (_stokPage - 1) * StokPageSize + i + 1;
                    _stokCells[i][0].Text = no.ToString();
                    _stokCells[i][1].Text = r.NamaProduk;
                    _stokCells[i][2].Text = r.Kategori;
                    _stokCells[i][3].Text = r.Stok.ToString();
                    _stokCells[i][4].Text = r.PerubahanTerakhir;
                    _stokCells[i][5].Text = r.Sumber;
                    _stokCells[i][6].Text = r.Status;
                    _stokCells[i][6].ForeColor = r.Status == "Tersedia"
                        ? Color.FromArgb(22, 163, 74)
                        : r.Status == "Stok Menipis"
                            ? Color.FromArgb(234, 88, 12)
                            : Color.FromArgb(220, 38, 38);
                    _stokRows[i].Visible = true;
                }
                else
                {
                    _stokRows[i].Visible = false;
                }
            }
        }

        private void UpdateStokPag()
        {
            if (_stokPagLbl == null) return;
            int start = _filtStok.Count == 0 ? 0 : (_stokPage - 1) * StokPageSize + 1;
            int end   = Math.Min(_stokPage * StokPageSize, _filtStok.Count);
            _stokPagLbl.Text     = $"Menampilkan {start}–{end} dari {_filtStok.Count} data";
            if (_stokPrev != null) _stokPrev.Enabled = _stokPage > 1;
            if (_stokNext != null) _stokNext.Enabled = _stokPage < StokTotalHal;
        }

        // ─────────────────────────────────────────────────────────────────────
        //  HALAMAN 4: LIHAT LAPORAN HARIAN
        // ─────────────────────────────────────────────────────────────────────
        private DateTimePicker? _dtpLaporan;
        private Label? _lblLapTotalPenjualan, _lblLapTotalTrx, _lblLapProdukTerjual;

        private Panel?         _terlarisBdy;
        private readonly List<Panel>   _terlarisPanels = new();
        private readonly List<Label[]> _terlarisCells  = new();
        private static readonly string[] TerlarisCols  = { "No", "Produk", "Terjual", "Total Penjualan" };
        private static readonly int[]    TerlarisPct   = { 5, 45, 15, 29 };

        private Panel?         _detailBdy;
        private readonly List<Panel>   _detailPanels = new();
        private readonly List<Label[]> _detailCells  = new();
        private static readonly string[] DetailCols   = { "No. Transaksi", "Waktu", "Pelanggan", "Total", "Kasir" };
        private static readonly int[]    DetailPct    = { 15, 18, 30, 17, 16 };

        private Panel BuatHalamanLaporanHarian()
        {
            var page = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };

            page.Controls.Add(new Label
            {
                Text      = "Identifikasi produk dengan penjualan rendah atau tidak aktif.",
                Font      = new Font("Segoe UI", 9f),
                ForeColor = Color.FromArgb(107, 114, 128),
                AutoSize  = true,
                Location  = new Point(0, 0)
            });

            // Filter tanggal
            var filterRow = new Panel
            {
                Location  = new Point(0, 24),
                Height    = 32,
                BackColor = Color.Transparent,
                Anchor    = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            page.Controls.Add(filterRow);
            page.Resize += (s, e) => filterRow.Width = page.ClientSize.Width;

            filterRow.Controls.Add(new Label
            {
                Text      = "Tanggal:",
                Font      = new Font("Segoe UI", 9f),
                ForeColor = Color.FromArgb(55, 65, 81),
                AutoSize  = true,
                Location  = new Point(0, 7)
            });
            _dtpLaporan = new DateTimePicker
            {
                Format   = DateTimePickerFormat.Short,
                Value    = DateTime.Today,
                Location = new Point(64, 2),
                Size     = new Size(130, 28),
                Font     = new Font("Segoe UI", 9f)
            };
            _dtpLaporan.ValueChanged += (s, e) => MuatLaporanHarian();
            filterRow.Controls.Add(_dtpLaporan);

            var btnExp = new Button
            {
                Text      = "⬇ Export",
                Size      = new Size(90, 28),
                BackColor = Color.FromArgb(37, 99, 235),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font      = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                Cursor    = Cursors.Hand
            };
            btnExp.Click += (s, e) => ExportLaporan();
            filterRow.Controls.Add(btnExp);
            filterRow.Resize += (s, e) =>
                btnExp.Location = new Point(filterRow.ClientSize.Width - btnExp.Width, 2);

            // 3 Summary cards
            var cardDefs3 = new (string, string, Color)[]
            {
                ("Total Penjualan", "💰", Color.FromArgb(22, 163,  74)),
                ("Total Transaksi", "🛒", Color.FromArgb(37,  99, 235)),
                ("Produk Terjual",  "📦", Color.FromArgb(147, 51, 234)),
            };
            var refs3   = new Label[3];
            var cardRow3 = new Panel
            {
                Location  = new Point(0, 64),
                Height    = 82,
                BackColor = Color.Transparent,
                Anchor    = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            page.Controls.Add(cardRow3);
            page.Resize    += (s, e) => cardRow3.Width = page.ClientSize.Width;
            cardRow3.Resize += (s, e) =>
            {
                int gap = 12, w = (cardRow3.Width - gap * 2) / 3;
                for (int i = 0; i < Math.Min(3, cardRow3.Controls.Count); i++)
                {
                    cardRow3.Controls[i].Location = new Point(i * (w + gap), 0);
                    cardRow3.Controls[i].Width    = w;
                }
            };
            for (int i = 0; i < 3; i++)
            {
                var (ttl, icon, clr) = cardDefs3[i];
                var (card, lbl) = BuatSummaryCard(ttl, icon, clr, 82);
                cardRow3.Controls.Add(card);
                refs3[i] = lbl;
            }
            _lblLapTotalPenjualan = refs3[0];
            _lblLapTotalTrx       = refs3[1];
            _lblLapProdukTerjual  = refs3[2];

            int tableTop = 64 + 82 + 14;

            // Tabel Produk Terlaris
            var tcTerlaris = new Panel { Location = new Point(0, tableTop), BackColor = Color.White };
            tcTerlaris.Paint += TableBorder;
            page.Controls.Add(tcTerlaris);

            // Tabel Detail Transaksi
            var tcDetail = new Panel { BackColor = Color.White };
            tcDetail.Paint += TableBorder;
            page.Controls.Add(tcDetail);

            page.Resize += (s, e) =>
            {
                int avail = page.ClientSize.Width;
                int h     = Math.Max((page.ClientSize.Height - tableTop - 14) / 2, 180);

                tcTerlaris.SetBounds(0, tableTop, avail, h);
                tcDetail.SetBounds(0, tableTop + h + 10, avail, h);
            };

            BuatInnerTable(tcTerlaris, "Produk Terlaris",  TerlarisCols, TerlarisPct,
                out _terlarisBdy, _terlarisPanels, _terlarisCells);
            BuatInnerTable(tcDetail,   "Detail Transaksi", DetailCols,   DetailPct,
                out _detailBdy,   _detailPanels,   _detailCells);

            MuatLaporanHarian();
            return page;
        }

        private static void TableBorder(object? s, PaintEventArgs e)
        {
            if (s is Panel p)
            {
                using var pen = new Pen(Color.FromArgb(229, 231, 235));
                e.Graphics.DrawRectangle(pen, 0, 0, p.Width - 1, p.Height - 1);
            }
        }

        private void BuatInnerTable(Panel tc, string judul,
            string[] headers, int[] pcts,
            out Panel? bodyRef,
            List<Panel> rowList, List<Label[]> cellList)
        {
            tc.Controls.Add(new Label
            {
                Text      = judul,
                Font      = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(17, 24, 39),
                Location  = new Point(14, 10),
                AutoSize  = true,
                BackColor = Color.White
            });

            var hRow = new Panel { Location = new Point(0, 36), Height = 34, BackColor = Color.FromArgb(248, 250, 252) };
            tc.Controls.Add(hRow);
            tc.Resize += (s, e) =>
            {
                hRow.Width = tc.ClientSize.Width;
                GambarHeader(hRow, headers, pcts);
            };
            GambarHeader(hRow, headers, pcts);

            var div = new Panel { Location = new Point(0, 70), Height = 1, BackColor = Color.FromArgb(229, 231, 235) };
            tc.Controls.Add(div);
            tc.Resize += (s, e) => div.Width = tc.ClientSize.Width;

            var body = new Panel { Location = new Point(0, 71), BackColor = Color.White };
            tc.Controls.Add(body);
            tc.Resize += (s, e) =>
                body.Size = new Size(tc.ClientSize.Width, Math.Max(tc.ClientSize.Height - 71, 20));

            bodyRef = body;
            BuatBarisGeneric(body, rowList, cellList, 8, pcts);
        }

        private void BuatBarisGeneric(Panel body, List<Panel> rowList,
            List<Label[]> cellList, int maxRows, int[] pcts)
        {
            body.Controls.Clear();
            rowList.Clear();
            cellList.Clear();
            int rowH = 38;
            for (int i = 0; i < maxRows; i++)
            {
                var rp = new Panel
                {
                    Location  = new Point(0, i * rowH),
                    Height    = rowH,
                    BackColor = i % 2 == 0 ? Color.White : Color.FromArgb(249, 250, 251)
                };
                rp.Paint += (s, e) =>
                {
                    using var pen = new Pen(Color.FromArgb(243, 244, 246));
                    e.Graphics.DrawLine(pen, 0, rp.Height - 1, rp.Width, rp.Height - 1);
                };
                var cols = new Label[pcts.Length];
                for (int c = 0; c < pcts.Length; c++)
                {
                    cols[c] = new Label
                    {
                        AutoSize  = false,
                        Font      = new Font("Segoe UI", 8.5f),
                        ForeColor = Color.FromArgb(55, 65, 81),
                        BackColor = Color.Transparent,
                        TextAlign = ContentAlignment.MiddleLeft,
                        Height    = rowH, Top = 0
                    };
                    rp.Controls.Add(cols[c]);
                }
                int[] cp = pcts;
                rp.Resize += (s, e) => RelayoutCols(rp, cols, rowH, cp);
                rowList.Add(rp);
                cellList.Add(cols);
                body.Controls.Add(rp);
            }
            // Initial layout
            body.Resize += (s, e) =>
            {
                int w = body.ClientSize.Width; if (w <= 0) return;
                for (int i = 0; i < rowList.Count; i++)
                {
                    rowList[i].Size     = new Size(w, rowH);
                    rowList[i].Location = new Point(0, i * rowH);
                    RelayoutCols(rowList[i], cellList[i], rowH, pcts);
                }
            };
        }

        private void RelayoutCols(Panel rp, Label[] cols, int h, int[] pcts)
        {
            int tot = 0; foreach (var v in pcts) tot += v;
            int x = 0, w = rp.ClientSize.Width;
            if (w <= 0) return;
            for (int c = 0; c < cols.Length; c++)
            {
                int cw = w * pcts[c] / tot;
                cols[c].Location = new Point(x + 8, 0);
                cols[c].Size     = new Size(Math.Max(cw - 8, 10), h);
                x += cw;
            }
        }

        private void MuatLaporanHarian()
        {
            if (_dtpLaporan == null) return;
            var tgl = _dtpLaporan.Value.Date;
            try
            {
                var (tp, tt, ptj) = LaporanRepository.GetSummaryHarian(tgl);
                _lblLapTotalPenjualan!.Text = "Rp " + tp.ToString("N0");
                _lblLapTotalTrx!.Text       = tt.ToString();
                _lblLapProdukTerjual!.Text  = ptj.ToString();

                var terlaris = LaporanRepository.GetProdukTerlaris(tgl, 8);
                for (int i = 0; i < _terlarisCells.Count; i++)
                {
                    if (i < terlaris.Count)
                    {
                        var r = terlaris[i];
                        _terlarisCells[i][0].Text = r.No.ToString();
                        _terlarisCells[i][1].Text = r.NamaProduk;
                        _terlarisCells[i][2].Text = r.TotalTerjual.ToString();
                        _terlarisCells[i][3].Text = "Rp " + r.TotalPenjualan.ToString("N0");
                        _terlarisCells[i][3].ForeColor = Color.FromArgb(22, 163, 74);
                        _terlarisPanels[i].Visible = true;
                    }
                    else _terlarisPanels[i].Visible = false;
                }

                var detail = LaporanRepository.GetDetailTransaksi(tgl);
                for (int i = 0; i < _detailCells.Count; i++)
                {
                    if (i < detail.Count)
                    {
                        var r = detail[i];
                        _detailCells[i][0].Text = r.IdTransaksi.ToString();
                        _detailCells[i][1].Text = r.Waktu.ToString("HH:mm:ss");
                        _detailCells[i][2].Text = r.Pelanggan;
                        _detailCells[i][3].Text = "Rp " + r.Total.ToString("N0");
                        _detailCells[i][3].ForeColor = Color.FromArgb(22, 163, 74);
                        _detailCells[i][4].Text = r.Kasir;
                        _detailPanels[i].Visible = true;
                    }
                    else _detailPanels[i].Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat laporan: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExportLaporan()
        {
            if (_dtpLaporan == null) return;
            var tgl = _dtpLaporan.Value.Date;
            try
            {
                var (tp, tt, ptj) = LaporanRepository.GetSummaryHarian(tgl);
                var terlaris      = LaporanRepository.GetProdukTerlaris(tgl, 20);
                var detail        = LaporanRepository.GetDetailTransaksi(tgl);

                var sb = new System.Text.StringBuilder();
                sb.AppendLine($"LAPORAN HARIAN RetailSync — {tgl:dd MMMM yyyy}");
                sb.AppendLine(new string('=', 55));
                sb.AppendLine($"Total Penjualan : Rp {tp:N0}");
                sb.AppendLine($"Total Transaksi : {tt}");
                sb.AppendLine($"Produk Terjual  : {ptj}");
                sb.AppendLine();
                sb.AppendLine("PRODUK TERLARIS");
                sb.AppendLine(new string('-', 55));
                foreach (var r in terlaris)
                    sb.AppendLine($"{r.No,3}. {r.NamaProduk,-30} Terjual:{r.TotalTerjual,5}  Rp {r.TotalPenjualan:N0}");
                sb.AppendLine();
                sb.AppendLine("DETAIL TRANSAKSI");
                sb.AppendLine(new string('-', 55));
                foreach (var r in detail)
                    sb.AppendLine($"#{r.IdTransaksi,-6} {r.Waktu:HH:mm:ss}  {r.Pelanggan,-20}  Rp {r.Total:N0}  ({r.Kasir})");

                Clipboard.SetText(sb.ToString());
                MessageBox.Show("Laporan disalin ke clipboard!", "Export Sukses",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal export: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        //  HALAMAN 5: KELOLA PESANAN (RESTOCK)
        // ─────────────────────────────────────────────────────────────────────
        private TextBox?  _pesSearch    = null;
        private ComboBox? _pesCmbKat    = null;
        private Panel?    _pesTblBody   = null;
        private Label?    _pesPagLbl    = null;
        private Button?   _pesPrev      = null, _pesNext = null;
        private List<ProdukItem> _allPesProduk  = new();
        private List<ProdukItem> _filtPesProduk = new();
        private int _pesPage = 1;
        private const int PesPageSize = 8;

        // Form restock
        private ComboBox? _restockCmbProduk = null;
        private Label?    _restockLblStok   = null;
        private NumericUpDown? _restockNud  = null;

        // Riwayat
        private Panel?         _riwayatBody   = null;
        private List<Panel>    _riwayatPanels = new();
        private List<Label[]>  _riwayatCells  = new();

        private Panel BuatHalamanKelolaPesanan()
        {
            var page = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };

            // ── Layout: kiri tabel produk, kanan form restock ─────────────────
            var panelKiri  = new Panel { Location = new Point(0, 0), BackColor = Color.Transparent };
            var panelKanan = new Panel { BackColor = Color.Transparent };
            page.Controls.Add(panelKiri);
            page.Controls.Add(panelKanan);

            page.Resize += (s, e) =>
            {
                int h     = page.ClientSize.Height;
                int avail = page.ClientSize.Width;
                int kiriW = Math.Max((avail - 12) * 62 / 100, 300);
                int kananW = avail - kiriW - 12;
                panelKiri.SetBounds(0,        0, kiriW,  h);
                panelKanan.SetBounds(kiriW+12, 0, kananW, h);
            };

            // ── Panel Kiri: Daftar Produk ──────────────────────────────────────
            var tcProduk = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };
            tcProduk.Paint += TableBorder;
            panelKiri.Controls.Add(tcProduk);

            tcProduk.Controls.Add(new Label
            {
                Text = "Daftar Produk", Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(17,24,39), Location = new Point(14, 10), AutoSize = true
            });

            // Search + filter
            var toolbar = new Panel { Location = new Point(0, 38), Height = 40, BackColor = Color.White };
            tcProduk.Controls.Add(toolbar);
            tcProduk.Resize += (s, e) => toolbar.Width = tcProduk.ClientSize.Width;

            _pesSearch = new TextBox
            {
                PlaceholderText = "Cari nama produk...",
                Font            = new Font("Segoe UI", 9f),
                BorderStyle     = BorderStyle.FixedSingle,
                Location        = new Point(10, 6),
                Size            = new Size(180, 28)
            };
            _pesSearch.TextChanged += (s, e) => FilterPesProduk();
            toolbar.Controls.Add(_pesSearch);

            _pesCmbKat = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font          = new Font("Segoe UI", 9f),
                Size          = new Size(140, 28)
            };
            _pesCmbKat.Items.AddRange(ProdukRepository.GetKategori().ToArray());
            _pesCmbKat.SelectedIndex = 0;
            _pesCmbKat.SelectedIndexChanged += (s, e) => FilterPesProduk();
            toolbar.Controls.Add(_pesCmbKat);
            toolbar.Resize += (s, e) =>
                _pesCmbKat.Location = new Point(toolbar.ClientSize.Width - _pesCmbKat.Width - 10, 6);

            // Header kolom produk
            PasangDividerHeader(tcProduk, 78,
                new[] { "No", "Produk", "Kategori", "Stok" },
                new[] { 6, 46, 30, 12 });

            // Body produk
            _pesTblBody = new Panel { Location = new Point(0, 115), BackColor = Color.White };
            tcProduk.Controls.Add(_pesTblBody);
            tcProduk.Resize += (s, e) =>
            {
                _pesTblBody.Size = new Size(tcProduk.ClientSize.Width,
                    tcProduk.ClientSize.Height - 115 - 44);
                RefreshPesBaris();
            };

            // Pagination
            var pagPanel = new Panel { BackColor = Color.White, Height = 44 };
            tcProduk.Controls.Add(pagPanel);
            tcProduk.Resize += (s, e) =>
            {
                pagPanel.Width    = tcProduk.ClientSize.Width;
                pagPanel.Location = new Point(0, tcProduk.ClientSize.Height - 44);
            };
            BuatPesPaginasi(pagPanel);
            BangunBarisPes();

            // ── Panel Kanan: Form Restock ──────────────────────────────────────
            var tcForm = new Panel { Dock = DockStyle.Top, Height = 260, BackColor = Color.White };
            tcForm.Paint += TableBorder;
            panelKanan.Controls.Add(tcForm);
            panelKanan.Resize += (s, e) => tcForm.Width = panelKanan.ClientSize.Width;

            tcForm.Controls.Add(new Label
            {
                Text = "Form Restock Barang", Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(17,24,39), Location = new Point(14, 10), AutoSize = true
            });

            int fy = 38, fx = 14, fw = 0;
            tcForm.Resize += (s, e) => fw = tcForm.ClientSize.Width - fx * 2;

            void FLbl(string t, int y) => tcForm.Controls.Add(new Label
            {
                Text = t, Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(55,65,81), Location = new Point(fx, y), AutoSize = true
            });

            FLbl("Produk", fy);
            _restockCmbProduk = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font          = new Font("Segoe UI", 9f),
                Location      = new Point(fx, fy + 18),
                Anchor        = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            MuatRestockCombo();
            _restockCmbProduk.SelectedIndexChanged += (s, e) => UpdateRestockInfo();
            tcForm.Controls.Add(_restockCmbProduk);
            tcForm.Resize += (s, e) =>
                _restockCmbProduk.Width = tcForm.ClientSize.Width - fx * 2;

            fy += 58;
            FLbl("Stok saat ini", fy);
            _restockLblStok = new Label
            {
                Text      = "-",
                Font      = new Font("Segoe UI", 9f),
                ForeColor = Color.FromArgb(55,65,81),
                Location  = new Point(fx, fy + 18),
                BackColor = Color.FromArgb(243,244,246),
                AutoSize  = false, Height = 28,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding   = new Padding(6,0,0,0),
                Anchor    = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            tcForm.Controls.Add(_restockLblStok);
            tcForm.Resize += (s, e) =>
                _restockLblStok.Width = tcForm.ClientSize.Width - fx * 2;

            fy += 58;
            FLbl("Tambah Stok", fy);
            _restockNud = new NumericUpDown
            {
                Location = new Point(fx, fy + 18),
                Minimum  = 1, Maximum = 99999, Value = 1,
                Font     = new Font("Segoe UI", 9f),
                Anchor   = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            tcForm.Controls.Add(_restockNud);
            tcForm.Resize += (s, e) =>
                _restockNud.Width = tcForm.ClientSize.Width - fx * 2;

            fy += 58;
            var btnSimpanRestock = new Button
            {
                Text      = "💾  Simpan Restock",
                Font      = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                BackColor = Color.FromArgb(22, 163, 74),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Height    = 40,
                Location  = new Point(fx, fy),
                Anchor    = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Cursor    = Cursors.Hand
            };
            btnSimpanRestock.FlatAppearance.BorderSize = 0;
            btnSimpanRestock.Click += BtnSimpanRestock_Click;
            tcForm.Controls.Add(btnSimpanRestock);
            tcForm.Resize += (s, e) =>
                btnSimpanRestock.Width = tcForm.ClientSize.Width - fx * 2;

            // ── Panel Riwayat Restock ──────────────────────────────────────────
            var tcRiwayat = new Panel { BackColor = Color.White };
            tcRiwayat.Paint += TableBorder;
            panelKanan.Controls.Add(tcRiwayat);
            panelKanan.Resize += (s, e) =>
            {
                tcRiwayat.Width    = panelKanan.ClientSize.Width;
                tcRiwayat.Location = new Point(0, tcForm.Height + 12);
                tcRiwayat.Height   = panelKanan.ClientSize.Height - tcForm.Height - 12;
            };

            tcRiwayat.Controls.Add(new Label
            {
                Text = "Riwayat Restock Terakhir", Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(17,24,39), Location = new Point(14, 10), AutoSize = true
            });

            PasangDividerHeader(tcRiwayat, 36,
                new[] { "Tanggal", "Produk", "Jumlah Ditambah", "Stok Sebelum", "Stok Setelah" },
                new[] { 18, 34, 16, 16, 14 });

            _riwayatBody = new Panel { Location = new Point(0, 73), BackColor = Color.White };
            tcRiwayat.Controls.Add(_riwayatBody);
            tcRiwayat.Resize += (s, e) =>
            {
                _riwayatBody.Size = new Size(tcRiwayat.ClientSize.Width,
                    Math.Max(tcRiwayat.ClientSize.Height - 73, 20));
                RefreshRiwayatBaris();
            };

            BuatBarisGeneric(_riwayatBody, _riwayatPanels, _riwayatCells, 6,
                new[] { 18, 34, 16, 16, 14 });

            // Load data
            MuatPesProduk();
            MuatRiwayatRestock();

            return page;
        }

        private void MuatRestockCombo()
        {
            _restockCmbProduk!.Items.Clear();
            var list = ProdukRepository.GetAll();
            foreach (var p in list)
                _restockCmbProduk.Items.Add(p);
            _restockCmbProduk.DisplayMember = "NamaProduk";
            if (_restockCmbProduk.Items.Count > 0)
                _restockCmbProduk.SelectedIndex = 0;
        }

        private void UpdateRestockInfo()
        {
            if (_restockCmbProduk?.SelectedItem is ProdukItem p && _restockLblStok != null)
                _restockLblStok.Text = "  " + p.Stok.ToString() + " unit";
        }

        private void BtnSimpanRestock_Click(object? s, EventArgs e)
        {
            if (_restockCmbProduk?.SelectedItem is not ProdukItem p) return;
            int tambah = (int)(_restockNud?.Value ?? 1);
            try
            {
                RestockRepository.SimpanRestock(p.IdProduk, tambah, p.Stok);
                MessageBox.Show($"Restock {p.NamaProduk} +{tambah} berhasil!", "Sukses",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                MuatPesProduk();
                MuatRiwayatRestock();
                MuatRestockCombo();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Tabel produk di halaman pesanan
        private static readonly int[] PesPct = { 6, 46, 30, 12 };
        private readonly List<Panel>   _pesPanels = new();
        private readonly List<Label[]> _pesCells  = new();

        private void BangunBarisPes()
        {
            if (_pesTblBody == null) return;
            _pesTblBody.Controls.Clear();
            _pesPanels.Clear();
            _pesCells.Clear();
            int rowH = 40;
            for (int i = 0; i < PesPageSize; i++)
            {
                var rp = new Panel
                {
                    Location  = new Point(0, i * rowH), Height = rowH,
                    BackColor = i % 2 == 0 ? Color.White : Color.FromArgb(249, 250, 251)
                };
                rp.Paint += (s, e) =>
                {
                    using var pen = new Pen(Color.FromArgb(243,244,246));
                    e.Graphics.DrawLine(pen, 0, rp.Height-1, rp.Width, rp.Height-1);
                };
                var cols = new Label[4];
                for (int c = 0; c < 4; c++)
                {
                    cols[c] = new Label
                    {
                        AutoSize=false, Font=new Font("Segoe UI",8.5f),
                        ForeColor=Color.FromArgb(55,65,81), BackColor=Color.Transparent,
                        TextAlign=c==3?ContentAlignment.MiddleRight:ContentAlignment.MiddleLeft,
                        Height=rowH, Top=0
                    };
                    rp.Controls.Add(cols[c]);
                }
                int[] cp = PesPct;
                rp.Resize += (s, e) => RelayoutCols(rp, cols, rowH, cp);
                _pesPanels.Add(rp); _pesCells.Add(cols);
                _pesTblBody.Controls.Add(rp);
            }
            RefreshPesBaris();
        }

        private void RefreshPesBaris()
        {
            int w = _pesTblBody?.ClientSize.Width ?? 0;
            if (w <= 0) return;
            for (int i = 0; i < _pesPanels.Count; i++)
            {
                _pesPanels[i].Size     = new Size(w, 40);
                _pesPanels[i].Location = new Point(0, i * 40);
                RelayoutCols(_pesPanels[i], _pesCells[i], 40, PesPct);
            }
        }

        private void MuatPesProduk()
        {
            _allPesProduk  = ProdukRepository.GetAll();
            _filtPesProduk = _allPesProduk;
            _pesPage = 1;
            IsiBarisPes();
            UpdatePesPag();
        }

        private void FilterPesProduk()
        {
            string kw  = _pesSearch?.Text.Trim().ToLower() ?? "";
            string kat = _pesCmbKat?.Text ?? "Semua Kategori";
            _filtPesProduk = _allPesProduk.Where(p =>
                (string.IsNullOrEmpty(kw) || p.NamaProduk.ToLower().Contains(kw)) &&
                (kat == "Semua Kategori" || p.Kategori == kat)
            ).ToList();
            _pesPage = 1;
            IsiBarisPes();
            UpdatePesPag();
        }

        private int PesTotalHal => Math.Max(1, (int)Math.Ceiling((double)_filtPesProduk.Count / PesPageSize));

        private void IsiBarisPes()
        {
            if (_pesPanels.Count == 0) return;
            var pg = _filtPesProduk.Skip((_pesPage-1)*PesPageSize).Take(PesPageSize).ToList();
            for (int i = 0; i < PesPageSize; i++)
            {
                if (i < pg.Count)
                {
                    var p = pg[i];
                    _pesCells[i][0].Text = ((_pesPage-1)*PesPageSize+i+1).ToString();
                    _pesCells[i][1].Text = p.NamaProduk;
                    _pesCells[i][2].Text = p.Kategori;
                    _pesCells[i][3].Text = p.Stok.ToString();
                    _pesCells[i][3].ForeColor = p.Stok == 0 ? Color.FromArgb(220,38,38)
                        : p.Stok <= 10 ? Color.FromArgb(234,88,12)
                        : Color.FromArgb(22,163,74);
                    _pesPanels[i].Visible = true;
                }
                else _pesPanels[i].Visible = false;
            }
        }

        private void BuatPesPaginasi(Panel bar)
        {
            _pesPagLbl = new Label
            {
                AutoSize=true, Font=new Font("Segoe UI",8.5f),
                ForeColor=Color.FromArgb(107,114,128),
                Location=new Point(12,14), BackColor=Color.Transparent
            };
            bar.Controls.Add(_pesPagLbl);
            _pesNext = BtnPag("›"); _pesPrev = BtnPag("‹");
            _pesNext.Click += (s,e) => { if(_pesPage<PesTotalHal){_pesPage++;IsiBarisPes();UpdatePesPag();} };
            _pesPrev.Click += (s,e) => { if(_pesPage>1){_pesPage--;IsiBarisPes();UpdatePesPag();} };
            bar.Controls.Add(_pesPrev); bar.Controls.Add(_pesNext);
            bar.Resize += (s,e) =>
            {
                _pesNext.Location = new Point(bar.ClientSize.Width-38, 8);
                _pesPrev.Location = new Point(bar.ClientSize.Width-70, 8);
            };
        }

        private void UpdatePesPag()
        {
            if (_pesPagLbl == null) return;
            int st = _filtPesProduk.Count==0?0:(_pesPage-1)*PesPageSize+1;
            int en = Math.Min(_pesPage*PesPageSize, _filtPesProduk.Count);
            _pesPagLbl.Text    = $"Menampilkan {st}–{en} dari {_filtPesProduk.Count} data";
            if(_pesPrev!=null) _pesPrev.Enabled = _pesPage>1;
            if(_pesNext!=null) _pesNext.Enabled = _pesPage<PesTotalHal;
        }

        private void RefreshRiwayatBaris()
        {
            int w = _riwayatBody?.ClientSize.Width ?? 0;
            if (w <= 0) return;
            for (int i = 0; i < _riwayatPanels.Count; i++)
            {
                _riwayatPanels[i].Size     = new Size(w, 38);
                _riwayatPanels[i].Location = new Point(0, i * 38);
                RelayoutCols(_riwayatPanels[i], _riwayatCells[i], 38,
                    new[] { 18, 34, 16, 16, 14 });
            }
        }

        private void MuatRiwayatRestock()
        {
            try
            {
                var list = RestockRepository.GetRiwayat(6);
                for (int i = 0; i < _riwayatCells.Count; i++)
                {
                    if (i < list.Count)
                    {
                        var r = list[i];
                        _riwayatCells[i][0].Text = r.Tanggal.ToString("dd/MM/yy HH:mm");
                        _riwayatCells[i][1].Text = r.NamaProduk.Replace("[RESTOCK] ","");
                        _riwayatCells[i][2].Text = "+" + r.JumlahDitambah;
                        _riwayatCells[i][2].ForeColor = Color.FromArgb(22,163,74);
                        _riwayatCells[i][3].Text = r.StokSebelum.ToString();
                        _riwayatCells[i][4].Text = r.StokSetelah.ToString();
                        _riwayatPanels[i].Visible = true;
                    }
                    else _riwayatPanels[i].Visible = false;
                }
            }
            catch { /* riwayat kosong = ok */ }
        }

        // ─────────────────────────────────────────────────────────────────────
        //  HALAMAN 6: MONITORING STOK REAL-TIME (KEUANGAN)
        // ─────────────────────────────────────────────────────────────────────
        private DateTimePicker? _monDari = null, _monSampai = null;
        private Label? _monPemasukan = null, _monPengeluaran = null,
                       _monLaba = null, _monSaldo = null;
        private Panel?         _monBody   = null;
        private readonly List<Panel>   _monPanels = new();
        private readonly List<Label[]> _monCells  = new();
        private List<KeuanganRow> _allKeu   = new();
        private string _monFilter = "Semua";

        // Tab buttons
        private Button? _tabSemua = null, _tabMasuk = null, _tabKeluar = null;

        private static readonly string[] MonCols = { "Tanggal", "Kategori", "Deskripsi", "Pemasukan", "Pengeluaran", "Saldo", "Metode", "Keterangan" };
        private static readonly int[]    MonPct  = { 10, 10, 20, 11, 11, 11, 8, 13 };

        private Panel BuatHalamanMonitoringStok()
        {
            var page = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };

            page.Controls.Add(new Label
            {
                Text      = "Pantau pemasukan, pengeluaran, dan laba rugi bisnis anda.",
                Font      = new Font("Segoe UI", 9f),
                ForeColor = Color.FromArgb(107, 114, 128),
                AutoSize  = true,
                Location  = new Point(0, 0)
            });

            // Filter tanggal
            var fRow = new Panel { Location=new Point(0,24), Height=32, BackColor=Color.Transparent,
                Anchor=AnchorStyles.Top|AnchorStyles.Left|AnchorStyles.Right };
            page.Controls.Add(fRow);
            page.Resize += (s,e) => fRow.Width = page.ClientSize.Width;

            fRow.Controls.Add(new Label { Text="Dari:", Font=new Font("Segoe UI",9f),
                ForeColor=Color.FromArgb(55,65,81), AutoSize=true, Location=new Point(0,7) });
            _monDari = new DateTimePicker
            {
                Format=DateTimePickerFormat.Short,
                Value=new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1),
                Location=new Point(44,2), Size=new Size(125,28), Font=new Font("Segoe UI",9f)
            };
            _monDari.ValueChanged += (s,e) => MuatKeuangan();
            fRow.Controls.Add(_monDari);

            fRow.Controls.Add(new Label { Text="s/d", Font=new Font("Segoe UI",9f),
                ForeColor=Color.FromArgb(55,65,81), AutoSize=true, Location=new Point(178,7) });
            _monSampai = new DateTimePicker
            {
                Format=DateTimePickerFormat.Short,
                Value=DateTime.Today,
                Location=new Point(202,2), Size=new Size(125,28), Font=new Font("Segoe UI",9f)
            };
            _monSampai.ValueChanged += (s,e) => MuatKeuangan();
            fRow.Controls.Add(_monSampai);

            // Summary cards 4
            var cardDefs4 = new (string, string, Color)[]
            {
                ("Total Pemasukan",  "📈", Color.FromArgb(22, 163,  74)),
                ("Total Pengeluaran","📉", Color.FromArgb(234,  88,  12)),
                ("Laba Bersih",      "💹", Color.FromArgb(37,  99, 235)),
                ("Saldo Akhir",      "🏦", Color.FromArgb(147,  51, 234)),
            };
            var refs4   = new Label[4];
            var cr4     = new Panel { Location=new Point(0,64), Height=82, BackColor=Color.Transparent,
                Anchor=AnchorStyles.Top|AnchorStyles.Left|AnchorStyles.Right };
            page.Controls.Add(cr4);
            page.Resize   += (s,e) => cr4.Width = page.ClientSize.Width;
            cr4.Resize    += (s,e) =>
            {
                int gap=12, w=(cr4.Width-gap*3)/4;
                for(int i=0;i<Math.Min(4,cr4.Controls.Count);i++)
                { cr4.Controls[i].Location=new Point(i*(w+gap),0); cr4.Controls[i].Width=w; }
            };
            for (int i = 0; i < 4; i++)
            {
                var (ttl,icon,clr) = cardDefs4[i];
                var (card,lbl) = BuatSummaryCard(ttl, icon, clr, 82);
                cr4.Controls.Add(card); refs4[i] = lbl;
            }
            _monPemasukan   = refs4[0];
            _monPengeluaran = refs4[1];
            _monLaba        = refs4[2];
            _monSaldo       = refs4[3];

            // Table card
            int tableTop4 = 64 + 82 + 14;
            var tc = new Panel { Location=new Point(0,tableTop4), BackColor=Color.White,
                Anchor=AnchorStyles.Top|AnchorStyles.Left|AnchorStyles.Right|AnchorStyles.Bottom };
            tc.Paint += TableBorder;
            page.Controls.Add(tc);
            page.Resize += (s,e) =>
            {
                tc.Width  = page.ClientSize.Width;
                tc.Height = Math.Max(page.ClientSize.Height - tableTop4, 200);
            };

            // Title
            tc.Controls.Add(new Label { Text="Rincian Transaksi Keuangan",
                Font=new Font("Segoe UI",9.5f,FontStyle.Bold),
                ForeColor=Color.FromArgb(17,24,39), Location=new Point(14,10), AutoSize=true });

            // Tab buttons
            _tabSemua  = BuatTab("Semua",       true);
            _tabMasuk  = BuatTab("Pemasukan",   false);
            _tabKeluar = BuatTab("Pengeluaran", false);

            _tabSemua.Click  += (s,e) => GantiTabMon("Semua");
            _tabMasuk.Click  += (s,e) => GantiTabMon("Pemasukan");
            _tabKeluar.Click += (s,e) => GantiTabMon("Pengeluaran");

            _tabSemua.Location  = new Point(14, 36);
            _tabMasuk.Location  = new Point(14 + _tabSemua.Width + 8, 36);
            _tabKeluar.Location = new Point(14 + _tabSemua.Width + 8 + _tabMasuk.Width + 8, 36);
            tc.Controls.Add(_tabSemua);
            tc.Controls.Add(_tabMasuk);
            tc.Controls.Add(_tabKeluar);

            // Header
            PasangDividerHeader(tc, 68, MonCols, MonPct);

            // Body
            _monBody = new Panel { Location=new Point(0,105), BackColor=Color.White };
            tc.Controls.Add(_monBody);
            tc.Resize += (s,e) =>
            {
                _monBody.Size = new Size(tc.ClientSize.Width, Math.Max(tc.ClientSize.Height-105, 20));
                RefreshMonBaris();
            };

            BuatBarisGeneric(_monBody, _monPanels, _monCells, 12, MonPct);
            MuatKeuangan();
            return page;
        }

        private static Button BuatTab(string txt, bool aktif) => new Button
        {
            Text      = txt,
            Size      = new Size(90, 26),
            BackColor = aktif ? Color.FromArgb(37, 99, 235) : Color.FromArgb(243, 244, 246),
            ForeColor = aktif ? Color.White : Color.FromArgb(55, 65, 81),
            FlatStyle = FlatStyle.Flat,
            Font      = new Font("Segoe UI", 8.5f, aktif ? FontStyle.Bold : FontStyle.Regular),
            Cursor    = Cursors.Hand,
            Tag       = txt
        };

        private void GantiTabMon(string filter)
        {
            _monFilter = filter;
            foreach (var btn in new[] { _tabSemua, _tabMasuk, _tabKeluar })
            {
                if (btn == null) continue;
                bool aktif = (string?)btn.Tag == filter;
                btn.BackColor = aktif ? Color.FromArgb(37,99,235) : Color.FromArgb(243,244,246);
                btn.ForeColor = aktif ? Color.White : Color.FromArgb(55,65,81);
                btn.Font      = new Font("Segoe UI", 8.5f, aktif?FontStyle.Bold:FontStyle.Regular);
            }
            IsiBarisKeu();
        }

        private void RefreshMonBaris()
        {
            int w = _monBody?.ClientSize.Width ?? 0;
            if (w <= 0) return;
            for (int i = 0; i < _monPanels.Count; i++)
            {
                _monPanels[i].Size     = new Size(w, 38);
                _monPanels[i].Location = new Point(0, i * 38);
                RelayoutCols(_monPanels[i], _monCells[i], 38, MonPct);
            }
        }

        private void MuatKeuangan()
        {
            if (_monDari == null || _monSampai == null) return;
            try
            {
                var (pm, jm, pk, jk, lb, sa) = KeuanganRepository.GetSummary(_monDari.Value, _monSampai.Value);
                _monPemasukan!.Text   = "Rp " + pm.ToString("N0");
                _monPengeluaran!.Text = "Rp " + pk.ToString("N0");
                _monLaba!.Text        = "Rp " + lb.ToString("N0");
                _monSaldo!.Text       = "Rp " + sa.ToString("N0");

                _allKeu = KeuanganRepository.GetRincian(_monDari.Value, _monSampai.Value, "Semua");
                IsiBarisKeu();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat keuangan: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void IsiBarisKeu()
        {
            var filtered = _monFilter == "Semua" ? _allKeu
                : _allKeu.Where(r => r.Tipe == _monFilter).ToList();

            for (int i = 0; i < _monCells.Count; i++)
            {
                if (i < filtered.Count)
                {
                    var r = filtered[i];
                    _monCells[i][0].Text = r.Tanggal.ToString("dd/MM/yy HH:mm");
                    _monCells[i][1].Text = r.Kategori;
                    _monCells[i][2].Text = r.Deskripsi;
                    _monCells[i][3].Text = r.Pemasukan > 0 ? "Rp "+r.Pemasukan.ToString("N0") : "-";
                    _monCells[i][3].ForeColor = Color.FromArgb(22,163,74);
                    _monCells[i][4].Text = r.Pengeluaran > 0 ? "Rp "+r.Pengeluaran.ToString("N0") : "-";
                    _monCells[i][4].ForeColor = Color.FromArgb(220,38,38);
                    _monCells[i][5].Text = "Rp "+r.Saldo.ToString("N0");
                    _monCells[i][6].Text = r.Metode;
                    _monCells[i][7].Text = r.Keterangan;
                    _monPanels[i].Visible = true;
                }
                else _monPanels[i].Visible = false;
            }
        }

        private (Panel, Label) BuatSummaryCard(string title, string icon, Color accent, int h)        {
            var card = new Panel
            {
                Size      = new Size(180, h),
                BackColor = Color.White
            };
            card.Paint += (s, e) =>
            {
                using var pen = new Pen(Color.FromArgb(229, 231, 235));
                e.Graphics.DrawRectangle(pen, 0, 0, card.Width - 1, card.Height - 1);
                // strip kiri
                using var br = new SolidBrush(accent);
                e.Graphics.FillRectangle(br, 0, 0, 4, card.Height);
            };

            var lblIcon = new Label
            {
                Text      = icon,
                Font      = new Font("Segoe UI", 20f),
                Location  = new Point(14, 10),
                Size      = new Size(36, 36),
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter
            };
            card.Controls.Add(lblIcon);

            var lblNum = new Label
            {
                Text      = "–",
                Font      = new Font("Segoe UI", 18f, FontStyle.Bold),
                ForeColor = Color.FromArgb(17, 24, 39),
                Location  = new Point(58, 6),
                Size      = new Size(110, 36),
                BackColor = Color.Transparent
            };
            card.Controls.Add(lblNum);

            card.Controls.Add(new Label
            {
                Text      = title,
                Font      = new Font("Segoe UI", 8f),
                ForeColor = Color.FromArgb(107, 114, 128),
                Location  = new Point(58, 44),
                AutoSize  = true,
                BackColor = Color.Transparent
            });

            return (card, lblNum);
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Toolbar Inventori
        // ─────────────────────────────────────────────────────────────────────
        private void BangunToolbarInventori(Panel tb)
        {
            int y = 12;

            _search = new TextBox
            {
                PlaceholderText = "Cari nama produk...",
                Font            = new Font("Segoe UI", 9f),
                BorderStyle     = BorderStyle.FixedSingle,
                Location        = new Point(12, y),
                Size            = new Size(180, 28)
            };
            _search.TextChanged += (s, e) => FilterInventori();

            _cmbStatus = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font          = new Font("Segoe UI", 9f),
                Location      = new Point(200, y),
                Size          = new Size(130, 28)
            };
            _cmbStatus.Items.AddRange(new[] { "Semua Status", "Tersedia", "Stok Menipis", "Stok Habis" });
            _cmbStatus.SelectedIndex = 0;
            _cmbStatus.SelectedIndexChanged += (s, e) => FilterInventori();

            _cmbKat = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font          = new Font("Segoe UI", 9f),
                Location      = new Point(338, y),
                Size          = new Size(130, 28)
            };
            _cmbKat.Items.AddRange(ProdukRepository.GetKategori().ToArray());
            _cmbKat.SelectedIndex = 0;
            _cmbKat.SelectedIndexChanged += (s, e) => FilterInventori();

            _btnTambah = BtnAksi("＋ Tambah", Color.FromArgb(22, 163, 74), Color.White);
            _btnEdit   = BtnAksi("✎ Edit",    Color.FromArgb(37, 99, 235), Color.White);
            _btnHapus  = BtnAksi("✕ Hapus",   Color.FromArgb(220, 38, 38), Color.White);
            var btnRef = BtnAksi("↻",          Color.FromArgb(107, 114, 128), Color.White);
            btnRef.Size       = new Size(32, 28);
            _btnEdit.Enabled  = false;
            _btnHapus.Enabled = false;

            _btnTambah.Click += (s, e) =>
            {
                using var dlg = new FormProdukDialog();
                if (dlg.ShowDialog(this) == DialogResult.OK) { _selectedProduk = null; AktifkanAksi(false); MuatDataInventori(); }
            };
            _btnEdit.Click += (s, e) =>
            {
                if (_selectedProduk == null) return;
                using var dlg = new FormProdukDialog(_selectedProduk);
                if (dlg.ShowDialog(this) == DialogResult.OK) { _selectedProduk = null; AktifkanAksi(false); MuatDataInventori(); }
            };
            _btnHapus.Click += (s, e) =>
            {
                if (_selectedProduk == null) return;
                if (MessageBox.Show($"Hapus \"{_selectedProduk.NamaProduk}\"?", "Konfirmasi",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    try { ProdukRepository.Hapus(_selectedProduk.IdProduk); _selectedProduk = null; AktifkanAksi(false); MuatDataInventori(); }
                    catch (Exception ex) { MessageBox.Show("Gagal: " + ex.Message); }
                }
            };
            btnRef.Click += (s, e) => { _selectedProduk = null; AktifkanAksi(false); MuatDataInventori(); };

            tb.Controls.AddRange(new Control[]
            { _search, _cmbStatus, _cmbKat, _btnTambah, _btnEdit, _btnHapus, btnRef });

            tb.Resize += (s, e) => PosisikanTombolToolbar(tb, y, btnRef);
            PosisikanTombolToolbar(tb, y, btnRef);
        }

        private void PosisikanTombolToolbar(Panel tb, int y, Button btnRef)
        {
            int rx = tb.ClientSize.Width - 10;
            btnRef.Location    = new Point(rx - btnRef.Width,       y); rx -= btnRef.Width + 6;
            _btnHapus.Location = new Point(rx - _btnHapus.Width,    y); rx -= _btnHapus.Width + 6;
            _btnEdit.Location  = new Point(rx - _btnEdit.Width,     y); rx -= _btnEdit.Width + 6;
            _btnTambah.Location= new Point(rx - _btnTambah.Width,   y);
        }

        private static Button BtnAksi(string txt, Color bg, Color fg) => new Button
        {
            Text      = txt,
            Size      = new Size(90, 28),
            BackColor = bg,
            ForeColor = fg,
            FlatStyle = FlatStyle.Flat,
            Font      = new Font("Segoe UI", 8.5f, FontStyle.Bold),
            Cursor    = Cursors.Hand
        };

        // ─────────────────────────────────────────────────────────────────────
        //  Header kolom tabel
        // ─────────────────────────────────────────────────────────────────────
        private static readonly string[] ColH  = { "No", "Nama Produk", "Kategori", "Harga", "Stok", "Status" };
        private static readonly int[]    ColPct = { 4, 30, 14, 17, 8, 13 };

        private void GambarHeaderKolom(Panel hp)
        {
            hp.Controls.Clear();
            if (hp.Width < 50) return;
            int tot = ColPct.Sum(), x = 0;
            for (int i = 0; i < ColH.Length; i++)
            {
                int w = hp.Width * ColPct[i] / tot;
                hp.Controls.Add(new Label
                {
                    Text      = ColH[i],
                    Font      = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                    ForeColor = Color.FromArgb(75, 85, 99),
                    Location  = new Point(x + 10, 0),
                    Size      = new Size(w, 36),
                    TextAlign = ContentAlignment.MiddleLeft,
                    BackColor = Color.Transparent
                });
                x += w;
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Baris tabel
        // ─────────────────────────────────────────────────────────────────────
        private void BangunBarisTabel()
        {
            _tableBody.Controls.Clear();
            _rows.Clear();
            _cells.Clear();
            int rowH = 42;

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
                rp.Paint += (s, e) =>
                {
                    using var pen = new Pen(Color.FromArgb(243, 244, 246));
                    e.Graphics.DrawLine(pen, 0, rp.Height - 1, rp.Width, rp.Height - 1);
                };
                rp.Click += (s, e) => PilihBaris(ri);

                var cols = new Label[6];
                for (int c = 0; c < 6; c++)
                {
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
                    int ci = c;
                    cols[c].Click += (s, e) => PilihBaris(ri);
                    rp.Controls.Add(cols[c]);
                }
                rp.Resize += (s, e) => LayoutKolom(rp, cols, rowH);

                _rows.Add(rp);
                _cells.Add(cols);
                _tableBody.Controls.Add(rp);
            }
            RefreshBaris();
        }

        private void RefreshBaris()
        {
            int w = _tableBody.ClientSize.Width;
            if (w <= 0) return;
            for (int i = 0; i < _rows.Count; i++)
            {
                _rows[i].Size     = new Size(w, 42);
                _rows[i].Location = new Point(0, i * 42);
                LayoutKolom(_rows[i], _cells[i], 42);
            }
        }

        private void LayoutKolom(Panel rp, Label[] cols, int h)
        {
            int tot = ColPct.Sum(), x = 0, w = rp.ClientSize.Width;
            if (w <= 0) return;
            for (int c = 0; c < cols.Length; c++)
            {
                int cw = w * ColPct[c] / tot;
                cols[c].Location = new Point(x + 10, 0);
                cols[c].Size     = new Size(Math.Max(cw - 10, 10), h);
                x += cw;
            }
        }

        private void PilihBaris(int idx)
        {
            int di = (_currentPage_ - 1) * PageSize + idx;
            if (di >= _filtered.Count) return;
            _selectedProduk = _filtered[di];
            AktifkanAksi(true);
            for (int i = 0; i < _rows.Count; i++)
            {
                bool sel = i == idx;
                var bg = sel ? Color.FromArgb(219, 234, 254)
                             : i % 2 == 0 ? Color.White : Color.FromArgb(249, 250, 251);
                _rows[i].BackColor = bg;
                foreach (var l in _cells[i]) l.BackColor = sel ? bg : Color.Transparent;
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Pagination
        // ─────────────────────────────────────────────────────────────────────
        private void BangunPaginasiPanel(Panel bar)
        {
            _lblPag = new Label
            {
                AutoSize  = true,
                Font      = new Font("Segoe UI", 8.5f),
                ForeColor = Color.FromArgb(107, 114, 128),
                Location  = new Point(12, 14),
                BackColor = Color.Transparent
            };
            bar.Controls.Add(_lblPag);

            _btnNext = BtnPag("›");
            _btnPrev = BtnPag("‹");
            _btnNext.Click += (s, e) =>
            { if (_currentPage_ < TotalHal) { _currentPage_++; IsiBaris(); UpdatePag(); } };
            _btnPrev.Click += (s, e) =>
            { if (_currentPage_ > 1) { _currentPage_--; IsiBaris(); UpdatePag(); } };

            bar.Controls.Add(_btnPrev);
            bar.Controls.Add(_btnNext);
            bar.Resize += (s, e) =>
            {
                _btnNext.Location = new Point(bar.ClientSize.Width - 38, 8);
                _btnPrev.Location = new Point(bar.ClientSize.Width - 70, 8);
            };
        }

        private static Button BtnPag(string t) => new Button
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
        //  Data
        // ─────────────────────────────────────────────────────────────────────
        private void MuatDataInventori()
        {
            try
            {
                _allProduk = ProdukRepository.GetAll();
                var (tot, ter, men, hab) = ProdukRepository.GetSummary();
                _lblTotal.Text    = tot.ToString();
                _lblTersedia.Text = ter.ToString();
                _lblMenipis.Text  = men.ToString();
                _lblHabis.Text    = hab.ToString();
                FilterInventori();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FilterInventori()
        {
            if (_search == null) return;
            string kw  = _search.Text.Trim().ToLower();
            string st  = _cmbStatus?.Text ?? "Semua Status";
            string kat = _cmbKat?.Text    ?? "Semua Kategori";

            _filtered = _allProduk.Where(p =>
                (string.IsNullOrEmpty(kw) || p.NamaProduk.ToLower().Contains(kw)) &&
                (st  == "Semua Status"   || p.StatusStok == st) &&
                (kat == "Semua Kategori" || p.Kategori   == kat)
            ).ToList();

            _currentPage_ = 1;
            IsiBaris();
            UpdatePag();
        }

        private int TotalHal => Math.Max(1, (int)Math.Ceiling((double)_filtered.Count / PageSize));

        private void IsiBaris()
        {
            if (_rows.Count == 0) return;
            var page = _filtered.Skip((_currentPage_ - 1) * PageSize).Take(PageSize).ToList();
            for (int i = 0; i < PageSize; i++)
            {
                if (i < page.Count)
                {
                    var p  = page[i];
                    int no = (_currentPage_ - 1) * PageSize + i + 1;
                    bool sel = _selectedProduk?.IdProduk == p.IdProduk;
                    _cells[i][0].Text = no.ToString();
                    _cells[i][1].Text = p.NamaProduk;
                    _cells[i][2].Text = p.Kategori;
                    _cells[i][3].Text = "Rp " + p.Harga.ToString("N0");
                    _cells[i][4].Text = p.Stok.ToString();
                    _cells[i][5].Text = p.StatusStok;
                    _cells[i][5].ForeColor = p.StatusStok == "Tersedia"
                        ? Color.FromArgb(22, 163, 74)
                        : p.StatusStok == "Stok Menipis"
                            ? Color.FromArgb(234, 88, 12)
                            : Color.FromArgb(220, 38, 38);
                    var bg = sel ? Color.FromArgb(219, 234, 254)
                                 : i % 2 == 0 ? Color.White : Color.FromArgb(249, 250, 251);
                    _rows[i].BackColor = bg;
                    foreach (var l in _cells[i]) l.BackColor = sel ? bg : Color.Transparent;
                    _rows[i].Visible = true;
                }
                else
                {
                    _rows[i].Visible = false;
                }
            }
        }

        private void UpdatePag()
        {
            if (_lblPag == null) return;
            int start = _filtered.Count == 0 ? 0 : (_currentPage_ - 1) * PageSize + 1;
            int end   = Math.Min(_currentPage_ * PageSize, _filtered.Count);
            _lblPag.Text     = $"Menampilkan {start}–{end} dari {_filtered.Count} data";
            _btnPrev.Enabled = _currentPage_ > 1;
            _btnNext.Enabled = _currentPage_ < TotalHal;
        }

        private void AktifkanAksi(bool v)
        {
            if (_btnEdit  != null) _btnEdit.Enabled  = v;
            if (_btnHapus != null) _btnHapus.Enabled = v;
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Logout
        // ─────────────────────────────────────────────────────────────────────
        private void Logout()
        {
            if (MessageBox.Show("Yakin ingin logout?", "Logout",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                foreach (Form f in Application.OpenForms)
                    if (f is Login) { f.Show(); break; }
                Close();
            }
        }
    }
}
