namespace RetailSync
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            panel2 = new Panel();
            panel1 = new Panel();
            panel3 = new Panel();
            button1 = new Button();
            label3 = new Label();
            label2 = new Label();
            TbPassword = new TextBox();
            TbUsername = new TextBox();
            label1 = new Label();
            pictureBox1 = new PictureBox();
            panel1.SuspendLayout();
            panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // panel2
            // 
            panel2.BackgroundImage = Properties.Resources.sembako_retail_bg_1779284639719;
            panel2.Location = new Point(1, 0);
            panel2.Name = "panel2";
            panel2.Size = new Size(663, 997);
            panel2.TabIndex = 0;
            panel2.Paint += panel2_Paint;
            // 
            // panel1
            // 
            panel1.BackColor = SystemColors.ActiveCaptionText;
            panel1.BackgroundImage = Properties.Resources.pexels_adrien_olichon_1257089_2387532;
            panel1.Controls.Add(panel3);
            panel1.Location = new Point(661, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(957, 997);
            panel1.TabIndex = 1;
            // 
            // panel3
            // 
            panel3.BackColor = SystemColors.ControlDarkDark;
            panel3.BackgroundImageLayout = ImageLayout.Stretch;
            panel3.Controls.Add(button1);
            panel3.Controls.Add(label3);
            panel3.Controls.Add(label2);
            panel3.Controls.Add(TbPassword);
            panel3.Controls.Add(TbUsername);
            panel3.Controls.Add(label1);
            panel3.Controls.Add(pictureBox1);
            panel3.Location = new Point(174, 174);
            panel3.Name = "panel3";
            panel3.Size = new Size(643, 650);
            panel3.TabIndex = 2;
            // 
            // button1
            // 
            button1.BackColor = Color.DarkOrange;
            button1.Location = new Point(212, 534);
            button1.Margin = new Padding(4, 5, 4, 5);
            button1.Name = "button1";
            button1.Size = new Size(213, 62);
            button1.TabIndex = 22;
            button1.Text = "LOGIN";
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click_2;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 0);
            label3.Location = new Point(128, 405);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new Size(116, 32);
            label3.TabIndex = 20;
            label3.Text = "Password";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 0);
            label2.Location = new Point(128, 293);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(123, 32);
            label2.TabIndex = 19;
            label2.Text = "Username";
            // 
            // TbPassword
            // 
            TbPassword.BackColor = Color.MidnightBlue;
            TbPassword.Font = new Font("Segoe UI", 14F, FontStyle.Regular, GraphicsUnit.Point, 0);
            TbPassword.ForeColor = SystemColors.ControlLightLight;
            TbPassword.Location = new Point(128, 442);
            TbPassword.Margin = new Padding(4, 5, 4, 5);
            TbPassword.Name = "TbPassword";
            TbPassword.Size = new Size(386, 45);
            TbPassword.TabIndex = 21;
            TbPassword.Text = "Password";
            TbPassword.TextChanged += TbPassword_TextChanged_2;
            // 
            // TbUsername
            // 
            TbUsername.BackColor = Color.MidnightBlue;
            TbUsername.BorderStyle = BorderStyle.None;
            TbUsername.Font = new Font("Segoe UI", 14F, FontStyle.Regular, GraphicsUnit.Point, 0);
            TbUsername.ForeColor = SystemColors.ControlLightLight;
            TbUsername.Location = new Point(128, 330);
            TbUsername.Margin = new Padding(4, 5, 4, 5);
            TbUsername.Name = "TbUsername";
            TbUsername.PlaceholderText = "Username";
            TbUsername.Size = new Size(386, 38);
            TbUsername.TabIndex = 18;
            TbUsername.Text = "Username";
            TbUsername.TextChanged += TbUsername_TextChanged_2;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(197, 191);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(243, 48);
            label1.TabIndex = 17;
            label1.Text = "FORM LOGIN";
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = Color.Transparent;
            pictureBox1.BackgroundImage = (Image)resources.GetObject("pictureBox1.BackgroundImage");
            pictureBox1.BackgroundImageLayout = ImageLayout.Zoom;
            pictureBox1.Location = new Point(252, 13);
            pictureBox1.Margin = new Padding(4, 5, 4, 5);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(127, 148);
            pictureBox1.TabIndex = 16;
            pictureBox1.TabStop = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.AppWorkspace;
            ClientSize = new Size(1618, 992);
            Controls.Add(panel1);
            Controls.Add(panel2);
            Name = "Form1";
            Text = "RetailSync";
            Load += Form1_Load;
            panel1.ResumeLayout(false);
            panel3.ResumeLayout(false);
            panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private Panel panel2;
        private Panel panel1;
        private Panel panel3;
        private Button button1;
        private Label label3;
        private Label label2;
        private TextBox TbPassword;
        private TextBox TbUsername;
        private Label label1;
        private PictureBox pictureBox1;
    }
}
