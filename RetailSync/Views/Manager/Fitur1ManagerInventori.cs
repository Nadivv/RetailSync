using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace RetailSync
{
    public partial class Fitur1ManagerInventori : Form
    {
        public Fitur1ManagerInventori()
        {
            InitializeComponent();
            textBox1.TextChanged += textBox1_TextChanged;
            label1.Click += label1_Click;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            label1.Visible = string.IsNullOrEmpty(textBox1.Text);
        }

        private void FiturInventori_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {
            textBox1.Focus();
        }
    }
}
