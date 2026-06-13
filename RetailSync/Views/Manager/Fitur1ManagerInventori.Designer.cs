namespace RetailSync
{
    partial class Fitur1ManagerInventori
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Fitur1ManagerInventori));
            panel1 = new Panel();
            textBox1 = new TextBox();
            label1 = new Label();
            label2 = new Label();
            comboBox1 = new ComboBox();
            groupBox1 = new GroupBox();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            label6 = new Label();
            label7 = new Label();
            label8 = new Label();
            textBox2 = new TextBox();
            comboBox2 = new ComboBox();
            textBox3 = new TextBox();
            textBox4 = new TextBox();
            numericUpDown1 = new NumericUpDown();
            checkBox1 = new CheckBox();
            dateTimePicker1 = new DateTimePicker();
            groupBox2 = new GroupBox();
            button1 = new Button();
            button2 = new Button();
            button3 = new Button();
            button4 = new Button();
            dataGridView1 = new DataGridView();
            colID = new DataGridViewTextBoxColumn();
            colNama = new DataGridViewTextBoxColumn();
            colKategori = new DataGridViewTextBoxColumn();
            colHargaBeli = new DataGridViewTextBoxColumn();
            colHargaJual = new DataGridViewTextBoxColumn();
            colStok = new DataGridViewTextBoxColumn();
            colTglExpired = new DataGridViewTextBoxColumn();
            colStatus = new DataGridViewTextBoxColumn();
            colKondisi = new DataGridViewTextBoxColumn();
            button5 = new Button();
            button6 = new Button();
            button7 = new Button();
            button8 = new Button();
            button9 = new Button();
            button10 = new Button();
            button11 = new Button();
            label9 = new Label();
            label10 = new Label();
            label11 = new Label();
            label12 = new Label();
            panel1.SuspendLayout();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).BeginInit();
            groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BackgroundImage = (Image)resources.GetObject("panel1.BackgroundImage");
            panel1.BackgroundImageLayout = ImageLayout.Stretch;
            panel1.Controls.Add(button11);
            panel1.Controls.Add(button10);
            panel1.Controls.Add(button9);
            panel1.Controls.Add(button8);
            panel1.Controls.Add(button7);
            panel1.Controls.Add(button6);
            panel1.Controls.Add(button5);
            panel1.Dock = DockStyle.Left;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(200, 749);
            panel1.TabIndex = 0;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(206, 121);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(135, 23);
            textBox1.TabIndex = 1;
            textBox1.TextChanged += textBox1_TextChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.White;
            label1.Font = new Font("Poppins", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.ForeColor = SystemColors.GrayText;
            label1.Location = new Point(210, 124);
            label1.Name = "label1";
            label1.Size = new Size(75, 19);
            label1.TabIndex = 2;
            label1.Text = "Cari Produk..";
            label1.Click += label1_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Poppins", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label2.ForeColor = SystemColors.ActiveCaptionText;
            label2.Location = new Point(450, 120);
            label2.Name = "label2";
            label2.Size = new Size(77, 22);
            label2.TabIndex = 3;
            label2.Text = "Filter Status";
            // 
            // comboBox1
            // 
            comboBox1.FormattingEnabled = true;
            comboBox1.Items.AddRange(new object[] { "Semua", "Menipis", "Habis", "Kadaluarsa" });
            comboBox1.Location = new Point(528, 119);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(121, 23);
            comboBox1.TabIndex = 4;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(dateTimePicker1);
            groupBox1.Controls.Add(checkBox1);
            groupBox1.Controls.Add(numericUpDown1);
            groupBox1.Controls.Add(textBox4);
            groupBox1.Controls.Add(textBox3);
            groupBox1.Controls.Add(comboBox2);
            groupBox1.Controls.Add(textBox2);
            groupBox1.Controls.Add(label8);
            groupBox1.Controls.Add(label7);
            groupBox1.Controls.Add(label6);
            groupBox1.Controls.Add(label5);
            groupBox1.Controls.Add(label4);
            groupBox1.Controls.Add(label3);
            groupBox1.Font = new Font("Poppins", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            groupBox1.Location = new Point(206, 150);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(237, 232);
            groupBox1.TabIndex = 5;
            groupBox1.TabStop = false;
            groupBox1.Text = "Input / Edit Data Produk";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Poppins", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label3.ForeColor = SystemColors.ActiveCaptionText;
            label3.Location = new Point(8, 31);
            label3.Name = "label3";
            label3.Size = new Size(93, 22);
            label3.TabIndex = 6;
            label3.Text = "Nama Produk:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Poppins", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label4.ForeColor = SystemColors.ActiveCaptionText;
            label4.Location = new Point(8, 59);
            label4.Name = "label4";
            label4.Size = new Size(62, 22);
            label4.TabIndex = 7;
            label4.Text = "Kategori:";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Poppins", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label5.ForeColor = SystemColors.ActiveCaptionText;
            label5.Location = new Point(8, 87);
            label5.Name = "label5";
            label5.Size = new Size(72, 22);
            label5.TabIndex = 8;
            label5.Text = "Harga Beli:";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Poppins", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label6.ForeColor = SystemColors.ActiveCaptionText;
            label6.Location = new Point(8, 114);
            label6.Name = "label6";
            label6.Size = new Size(77, 22);
            label6.TabIndex = 9;
            label6.Text = "Harga Jual:";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Poppins", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label7.ForeColor = SystemColors.ActiveCaptionText;
            label7.Location = new Point(8, 140);
            label7.Name = "label7";
            label7.RightToLeft = RightToLeft.No;
            label7.Size = new Size(65, 22);
            label7.TabIndex = 10;
            label7.Text = "Stok Sisa:";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new Font("Poppins", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label8.ForeColor = SystemColors.ActiveCaptionText;
            label8.Location = new Point(8, 201);
            label8.Name = "label8";
            label8.Size = new Size(72, 22);
            label8.TabIndex = 7;
            label8.Text = "dtpExpired";
            // 
            // textBox2
            // 
            textBox2.Location = new Point(117, 26);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(112, 25);
            textBox2.TabIndex = 6;
            // 
            // comboBox2
            // 
            comboBox2.FormattingEnabled = true;
            comboBox2.Items.AddRange(new object[] { "Sembako", "Minuman", "Kebersihan", "Cemilan" });
            comboBox2.Location = new Point(117, 53);
            comboBox2.Name = "comboBox2";
            comboBox2.Size = new Size(112, 30);
            comboBox2.TabIndex = 11;
            // 
            // textBox3
            // 
            textBox3.Location = new Point(117, 85);
            textBox3.Name = "textBox3";
            textBox3.Size = new Size(112, 25);
            textBox3.TabIndex = 12;
            // 
            // textBox4
            // 
            textBox4.Location = new Point(117, 112);
            textBox4.Name = "textBox4";
            textBox4.Size = new Size(112, 25);
            textBox4.TabIndex = 7;
            // 
            // numericUpDown1
            // 
            numericUpDown1.Location = new Point(117, 138);
            numericUpDown1.Name = "numericUpDown1";
            numericUpDown1.Size = new Size(112, 25);
            numericUpDown1.TabIndex = 13;
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Font = new Font("Poppins", 6.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            checkBox1.Location = new Point(117, 169);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(112, 20);
            checkBox1.TabIndex = 14;
            checkBox1.Text = "Punya Kadaluarsa?";
            checkBox1.UseVisualStyleBackColor = true;
            // 
            // dateTimePicker1
            // 
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.Location = new Point(117, 201);
            dateTimePicker1.Name = "dateTimePicker1";
            dateTimePicker1.Size = new Size(112, 25);
            dateTimePicker1.TabIndex = 15;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(button4);
            groupBox2.Controls.Add(button3);
            groupBox2.Controls.Add(button2);
            groupBox2.Controls.Add(button1);
            groupBox2.Font = new Font("Poppins", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            groupBox2.Location = new Point(206, 388);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(241, 88);
            groupBox2.TabIndex = 6;
            groupBox2.TabStop = false;
            groupBox2.Text = "Kontrol";
            // 
            // button1
            // 
            button1.Image = (Image)resources.GetObject("button1.Image");
            button1.Location = new Point(6, 37);
            button1.Name = "button1";
            button1.Size = new Size(43, 42);
            button1.TabIndex = 0;
            button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            button2.Image = (Image)resources.GetObject("button2.Image");
            button2.Location = new Point(55, 37);
            button2.Name = "button2";
            button2.Size = new Size(43, 42);
            button2.TabIndex = 1;
            button2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            button3.Image = (Image)resources.GetObject("button3.Image");
            button3.Location = new Point(137, 37);
            button3.Name = "button3";
            button3.Size = new Size(43, 42);
            button3.TabIndex = 2;
            button3.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            button4.Image = (Image)resources.GetObject("button4.Image");
            button4.Location = new Point(186, 37);
            button4.Name = "button4";
            button4.Size = new Size(43, 42);
            button4.TabIndex = 3;
            button4.UseVisualStyleBackColor = true;
            // 
            // dataGridView1
            // 
            dataGridView1.BackgroundColor = SystemColors.Control;
            dataGridView1.BorderStyle = BorderStyle.None;
            dataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridView1.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Columns.AddRange(new DataGridViewColumn[] { colID, colNama, colKategori, colHargaBeli, colHargaJual, colStok, colTglExpired, colStatus, colKondisi });
            dataGridView1.EnableHeadersVisualStyles = false;
            dataGridView1.Location = new Point(453, 150);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.Size = new Size(905, 470);
            dataGridView1.TabIndex = 7;
            // 
            // colID
            // 
            colID.HeaderText = "ID Produk";
            colID.Name = "colID";
            // 
            // colNama
            // 
            colNama.HeaderText = "Nama Produk";
            colNama.Name = "colNama";
            // 
            // colKategori
            // 
            colKategori.HeaderText = "Kategori";
            colKategori.Name = "colKategori";
            // 
            // colHargaBeli
            // 
            colHargaBeli.HeaderText = "Harga Beli";
            colHargaBeli.Name = "colHargaBeli";
            // 
            // colHargaJual
            // 
            colHargaJual.HeaderText = "Harga Jual";
            colHargaJual.Name = "colHargaJual";
            // 
            // colStok
            // 
            colStok.HeaderText = "Stok Sisa";
            colStok.Name = "colStok";
            // 
            // colTglExpired
            // 
            colTglExpired.HeaderText = "Tgl Kadaluarsa";
            colTglExpired.Name = "colTglExpired";
            // 
            // colStatus
            // 
            colStatus.HeaderText = "Status Inventori";
            colStatus.Name = "colStatus";
            // 
            // colKondisi
            // 
            colKondisi.HeaderText = "Kondisi Expired";
            colKondisi.Name = "colKondisi";
            // 
            // button5
            // 
            button5.BackgroundImage = (Image)resources.GetObject("button5.BackgroundImage");
            button5.FlatStyle = FlatStyle.Popup;
            button5.Image = (Image)resources.GetObject("button5.Image");
            button5.Location = new Point(0, 99);
            button5.Name = "button5";
            button5.Size = new Size(200, 65);
            button5.TabIndex = 0;
            button5.UseVisualStyleBackColor = true;
            // 
            // button6
            // 
            button6.BackgroundImage = (Image)resources.GetObject("button6.BackgroundImage");
            button6.FlatStyle = FlatStyle.Popup;
            button6.Image = (Image)resources.GetObject("button6.Image");
            button6.Location = new Point(0, 234);
            button6.Name = "button6";
            button6.Size = new Size(231, 65);
            button6.TabIndex = 1;
            button6.UseVisualStyleBackColor = true;
            // 
            // button7
            // 
            button7.BackgroundImage = (Image)resources.GetObject("button7.BackgroundImage");
            button7.FlatStyle = FlatStyle.Popup;
            button7.Image = (Image)resources.GetObject("button7.Image");
            button7.Location = new Point(-27, 299);
            button7.Name = "button7";
            button7.Size = new Size(227, 65);
            button7.TabIndex = 2;
            button7.UseVisualStyleBackColor = true;
            // 
            // button8
            // 
            button8.BackgroundImage = (Image)resources.GetObject("button8.BackgroundImage");
            button8.FlatStyle = FlatStyle.Popup;
            button8.Image = (Image)resources.GetObject("button8.Image");
            button8.Location = new Point(-27, 361);
            button8.Name = "button8";
            button8.Size = new Size(227, 65);
            button8.TabIndex = 3;
            button8.UseVisualStyleBackColor = true;
            // 
            // button9
            // 
            button9.BackgroundImage = (Image)resources.GetObject("button9.BackgroundImage");
            button9.FlatStyle = FlatStyle.Popup;
            button9.Image = (Image)resources.GetObject("button9.Image");
            button9.Location = new Point(0, 424);
            button9.Name = "button9";
            button9.Size = new Size(200, 65);
            button9.TabIndex = 8;
            button9.UseVisualStyleBackColor = true;
            // 
            // button10
            // 
            button10.BackgroundImage = (Image)resources.GetObject("button10.BackgroundImage");
            button10.FlatStyle = FlatStyle.Popup;
            button10.Image = (Image)resources.GetObject("button10.Image");
            button10.Location = new Point(0, 485);
            button10.Name = "button10";
            button10.Size = new Size(210, 65);
            button10.TabIndex = 9;
            button10.UseVisualStyleBackColor = true;
            // 
            // button11
            // 
            button11.BackgroundImage = (Image)resources.GetObject("button11.BackgroundImage");
            button11.FlatStyle = FlatStyle.Popup;
            button11.Image = (Image)resources.GetObject("button11.Image");
            button11.Location = new Point(0, 546);
            button11.Name = "button11";
            button11.Size = new Size(200, 203);
            button11.TabIndex = 10;
            button11.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            label9.Image = (Image)resources.GetObject("label9.Image");
            label9.Location = new Point(229, 29);
            label9.Name = "label9";
            label9.Size = new Size(26, 23);
            label9.TabIndex = 8;
            // 
            // label10
            // 
            label10.Image = (Image)resources.GetObject("label10.Image");
            label10.Location = new Point(261, 29);
            label10.Name = "label10";
            label10.Size = new Size(202, 23);
            label10.TabIndex = 9;
            // 
            // label11
            // 
            label11.Image = (Image)resources.GetObject("label11.Image");
            label11.Location = new Point(200, 93);
            label11.Name = "label11";
            label11.Size = new Size(1087, 11);
            label11.TabIndex = 10;
            // 
            // label12
            // 
            label12.Image = (Image)resources.GetObject("label12.Image");
            label12.Location = new Point(281, 93);
            label12.Name = "label12";
            label12.Size = new Size(1087, 11);
            label12.TabIndex = 11;
            // 
            // FiturInventori
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1370, 749);
            Controls.Add(label12);
            Controls.Add(label11);
            Controls.Add(label10);
            Controls.Add(label9);
            Controls.Add(dataGridView1);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Controls.Add(comboBox1);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(textBox1);
            Controls.Add(panel1);
            Name = "FiturInventori";
            Text = "FiturInventori";
            Load += FiturInventori_Load;
            panel1.ResumeLayout(false);
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).EndInit();
            groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel panel1;
        private TextBox textBox1;
        private Label label1;
        private Label label2;
        private ComboBox comboBox1;
        private GroupBox groupBox1;
        private ComboBox comboBox2;
        private TextBox textBox2;
        private Label label8;
        private Label label7;
        private Label label6;
        private Label label5;
        private Label label4;
        private Label label3;
        private DateTimePicker dateTimePicker1;
        private CheckBox checkBox1;
        private NumericUpDown numericUpDown1;
        private TextBox textBox4;
        private TextBox textBox3;
        private GroupBox groupBox2;
        private Button button1;
        private Button button4;
        private Button button3;
        private Button button2;
        private DataGridView dataGridView1;
        private DataGridViewTextBoxColumn colID;
        private DataGridViewTextBoxColumn colNama;
        private DataGridViewTextBoxColumn colKategori;
        private DataGridViewTextBoxColumn colHargaBeli;
        private DataGridViewTextBoxColumn colHargaJual;
        private DataGridViewTextBoxColumn colStok;
        private DataGridViewTextBoxColumn colTglExpired;
        private DataGridViewTextBoxColumn colStatus;
        private DataGridViewTextBoxColumn colKondisi;
        private Button button5;
        private Button button6;
        private Button button8;
        private Button button7;
        private Button button11;
        private Button button10;
        private Button button9;
        private Label label9;
        private Label label10;
        private Label label11;
        private Label label12;
    }
}