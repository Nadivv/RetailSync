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
            panel1 = new Panel();
            panel2 = new Panel();
            BtnLogin = new Button();
            label3 = new Label();
            label2 = new Label();
            TbPassword = new TextBox();
            TbUsername = new TextBox();
            label1 = new Label();
            pictureBox1 = new PictureBox();
            panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Location = new Point(661, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(727, 874);
            panel1.TabIndex = 0;
            panel1.Paint += panel1_Paint;
            // 
            // panel2
            // 
            panel2.BackgroundImage = Properties.Resources.sembako_retail_bg_1779284639719;
            panel2.Controls.Add(panel1);
            panel2.Location = new Point(1, 0);
            panel2.Name = "panel2";
            panel2.Size = new Size(663, 997);
            panel2.TabIndex = 0;
            panel2.Paint += panel2_Paint;
            // 
            // BtnLogin
            // 
            BtnLogin.BackColor = Color.DarkOrange;
            BtnLogin.Location = new Point(1044, 666);
            BtnLogin.Margin = new Padding(4, 5, 4, 5);
            BtnLogin.Name = "BtnLogin";
            BtnLogin.Size = new Size(213, 65);
            BtnLogin.TabIndex = 13;
            BtnLogin.Text = "LOGIN";
            BtnLogin.UseVisualStyleBackColor = false;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 12F);
            label3.Location = new Point(960, 531);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new Size(111, 32);
            label3.TabIndex = 12;
            label3.Text = "Password";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F);
            label2.Location = new Point(960, 416);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(121, 32);
            label2.TabIndex = 11;
            label2.Text = "Username";
            // 
            // TbPassword
            // 
            TbPassword.BackColor = SystemColors.InactiveCaption;
            TbPassword.BorderStyle = BorderStyle.None;
            TbPassword.Font = new Font("Segoe UI", 14F);
            TbPassword.Location = new Point(960, 571);
            TbPassword.Margin = new Padding(4, 5, 4, 5);
            TbPassword.Name = "TbPassword";
            TbPassword.PlaceholderText = "Password";
            TbPassword.Size = new Size(386, 38);
            TbPassword.TabIndex = 10;
            // 
            // TbUsername
            // 
            TbUsername.BackColor = SystemColors.InactiveCaption;
            TbUsername.BorderStyle = BorderStyle.None;
            TbUsername.Font = new Font("Segoe UI", 14F);
            TbUsername.Location = new Point(960, 456);
            TbUsername.Margin = new Padding(4, 5, 4, 5);
            TbUsername.Name = "TbUsername";
            TbUsername.PlaceholderText = "Username";
            TbUsername.Size = new Size(386, 38);
            TbUsername.TabIndex = 9;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(1029, 317);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(243, 48);
            label1.TabIndex = 8;
            label1.Text = "FORM LOGIN";
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = Color.Transparent;
            pictureBox1.BackgroundImage = (Image)resources.GetObject("pictureBox1.BackgroundImage");
            pictureBox1.BackgroundImageLayout = ImageLayout.Zoom;
            pictureBox1.Location = new Point(1084, 139);
            pictureBox1.Margin = new Padding(4, 5, 4, 5);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(127, 148);
            pictureBox1.TabIndex = 7;
            pictureBox1.TabStop = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1618, 992);
            Controls.Add(BtnLogin);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(TbPassword);
            Controls.Add(TbUsername);
            Controls.Add(label1);
            Controls.Add(pictureBox1);
            Controls.Add(panel2);
            Name = "Form1";
            Text = "RetailSync";
            Load += Form1_Load;
            panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel panel1;
        private Panel panel2;
        private Button BtnLogin;
        private Label label3;
        private Label label2;
        private TextBox TbPassword;
        private TextBox TbUsername;
        private Label label1;
        private PictureBox pictureBox1;
    }
}
