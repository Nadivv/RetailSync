using RetailSync.Helpers;
using RetailSync.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace RetailSync
{
    public partial class Form3 : Form
    {
        private List<ProdukItem> _allProduk = new();
        private List<ProdukItem> _filtered  = new();
        private int _currentPage = 1;
        private const int PageSize = 6;

        public Form3()
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
        }
    }
}
