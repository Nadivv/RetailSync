using Npgsql;
using RetailSync.Helpers;
using RetailSync.Models;
using System;
using System.Security.Cryptography;
using System.Text;

namespace RetailSync
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void TbUsername_TextChanged_2(object sender, EventArgs e)
        {

        }

        private void TbPassword_TextChanged_2(object sender, EventArgs e)
        {

        }

        private void button1_Click_2(object sender, EventArgs e)
        {
            try
            {
                using (NpgsqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    string query = @"
                    SELECT p.id_pengguna, p.nama, p.username, p.id_role, r.nama_role
                    FROM pengguna p
                    JOIN roles r ON p.id_role = r.id_role
                    WHERE p.username = @username
                    AND p.password = @password
                    AND p.is_aktif = TRUE";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", TbUsername.Text.Trim());
                        cmd.Parameters.AddWithValue("@password", HashPassword(TbPassword.Text));

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Simpan sesi
                                UserContext.IdPengguna = reader.GetInt32(0);
                                UserContext.Nama       = reader.GetString(1);
                                UserContext.Username   = reader.GetString(2);
                                UserContext.IdRole     = reader.GetInt32(3);
                                UserContext.NamaRole   = reader.GetString(4);

                                if (UserContext.IdRole == 1) // Admin
                                {
                                    Form2 admin = new Form2();
                                    admin.Show();
                                    this.Hide();
                                }
                                else if (UserContext.IdRole == 2) // Manager
                                {
                                    FormManager manager = new FormManager();
                                    manager.Show();
                                    this.Hide();
                                }
                            }
                            else
                            {
                                MessageBox.Show("Username atau Password salah!", "Login Gagal",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Koneksi Gagal",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes =
                    sha256.ComputeHash(
                        Encoding.UTF8.GetBytes(password)
                    );

                StringBuilder builder =
                    new StringBuilder();

                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }

                return builder.ToString();
            }
        }
    };
}