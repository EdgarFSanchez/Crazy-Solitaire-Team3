namespace CrazySolitaire
{
    partial class Form1
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
            pictureBox1 = new PictureBox();
            goodmanbtn = new Button();
            lebron = new PictureBox();
            button2 = new Button();
            button3 = new Button();
            btnResetBg = new Button();
            storelbl = new Label();
            Sclbl = new Label();
            bglbl = new Label();
            Modlbl = new Label();
            label1 = new Label();
            pictureBox2 = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)lebron).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.BackgroundImage = Properties.Resources.latest1;
            pictureBox1.BackgroundImageLayout = ImageLayout.Stretch;
            pictureBox1.BorderStyle = BorderStyle.Fixed3D;
            pictureBox1.Location = new Point(448, 110);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(193, 112);
            pictureBox1.TabIndex = 2;
            pictureBox1.TabStop = false;
            pictureBox1.Click += pictureBox1_Click;
            // 
            // goodmanbtn
            // 
            goodmanbtn.BackColor = Color.Gold;
            goodmanbtn.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            goodmanbtn.Location = new Point(503, 228);
            goodmanbtn.Name = "goodmanbtn";
            goodmanbtn.Size = new Size(80, 37);
            goodmanbtn.TabIndex = 1;
            goodmanbtn.Text = "BUY";
            goodmanbtn.UseVisualStyleBackColor = false;
            goodmanbtn.Click += goodmanbtn_Click;
            // 
            // lebron
            // 
            lebron.BackColor = Color.Transparent;
            lebron.BackgroundImage = Properties.Resources.hq720;
            lebron.BackgroundImageLayout = ImageLayout.Stretch;
            lebron.BorderStyle = BorderStyle.FixedSingle;
            lebron.Location = new Point(238, 110);
            lebron.Name = "lebron";
            lebron.Size = new Size(193, 112);
            lebron.TabIndex = 5;
            lebron.TabStop = false;
            // 
            // button2
            // 
            button2.BackColor = Color.Gold;
            button2.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button2.Location = new Point(299, 228);
            button2.Name = "button2";
            button2.Size = new Size(80, 37);
            button2.TabIndex = 6;
            button2.Text = "BUY";
            button2.UseVisualStyleBackColor = false;
            button2.Click += Lebronbtn_Click;
            // 
            // button3
            // 
            button3.BackColor = Color.Gold;
            button3.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button3.Location = new Point(67, 427);
            button3.Name = "button3";
            button3.Size = new Size(80, 37);
            button3.TabIndex = 10;
            button3.Text = "BUY";
            button3.UseVisualStyleBackColor = false;
            button3.Click += button3_Click;
            // 
            // btnResetBg
            // 
            btnResetBg.BackColor = Color.Gold;
            btnResetBg.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnResetBg.Location = new Point(67, 228);
            btnResetBg.Name = "btnResetBg";
            btnResetBg.Size = new Size(80, 37);
            btnResetBg.TabIndex = 11;
            btnResetBg.Text = "Default";
            btnResetBg.UseVisualStyleBackColor = false;
            btnResetBg.Click += btnResetBg_Click;
            // 
            // storelbl
            // 
            storelbl.AutoSize = true;
            storelbl.BackColor = Color.Transparent;
            storelbl.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            storelbl.ForeColor = SystemColors.Control;
            storelbl.Location = new Point(12, 9);
            storelbl.Name = "storelbl";
            storelbl.Size = new Size(73, 32);
            storelbl.TabIndex = 12;
            storelbl.Text = "Store";
            storelbl.Click += storelbl_Click;
            // 
            // Sclbl
            // 
            Sclbl.AutoSize = true;
            Sclbl.BackColor = Color.Transparent;
            Sclbl.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            Sclbl.ForeColor = SystemColors.Control;
            Sclbl.Location = new Point(593, 9);
            Sclbl.Name = "Sclbl";
            Sclbl.Size = new Size(171, 32);
            Sclbl.TabIndex = 13;
            Sclbl.Text = "Social Credit: ";
            Sclbl.Click += Sclbl_Click;
            // 
            // bglbl
            // 
            bglbl.AutoSize = true;
            bglbl.BackColor = Color.Transparent;
            bglbl.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            bglbl.ForeColor = SystemColors.Control;
            bglbl.Location = new Point(12, 75);
            bglbl.Name = "bglbl";
            bglbl.Size = new Size(163, 32);
            bglbl.TabIndex = 14;
            bglbl.Text = "Backgrounds";
            bglbl.Click += bglbl_Click;
            // 
            // Modlbl
            // 
            Modlbl.AutoSize = true;
            Modlbl.BackColor = Color.Transparent;
            Modlbl.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            Modlbl.ForeColor = SystemColors.Control;
            Modlbl.Location = new Point(25, 302);
            Modlbl.Name = "Modlbl";
            Modlbl.Size = new Size(124, 32);
            Modlbl.TabIndex = 15;
            Modlbl.Text = "Modifiers";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.Transparent;
            label1.Font = new Font("Segoe UI Black", 36F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.FromArgb(128, 128, 255);
            label1.Location = new Point(62, 348);
            label1.Name = "label1";
            label1.Size = new Size(85, 65);
            label1.TabIndex = 16;
            label1.Text = "2x";
            // 
            // pictureBox2
            // 
            pictureBox2.BackColor = Color.FromArgb(0, 64, 0);
            pictureBox2.BackgroundImageLayout = ImageLayout.Stretch;
            pictureBox2.BorderStyle = BorderStyle.Fixed3D;
            pictureBox2.Location = new Point(25, 110);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(193, 112);
            pictureBox2.TabIndex = 17;
            pictureBox2.TabStop = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(0, 64, 0);
            BackgroundImage = Properties.Resources.shopbg;
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(976, 637);
            Controls.Add(pictureBox2);
            Controls.Add(label1);
            Controls.Add(Modlbl);
            Controls.Add(bglbl);
            Controls.Add(Sclbl);
            Controls.Add(storelbl);
            Controls.Add(btnResetBg);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(lebron);
            Controls.Add(pictureBox1);
            Controls.Add(goodmanbtn);
            Name = "Form1";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)lebron).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private PictureBox pictureBox1;
        private Button goodmanbtn;
        private PictureBox lebron;
        private Button button2;
        private Button button3;
        private Button btnResetBg;
        private Label storelbl;
        private Label Sclbl;
        private Label bglbl;
        private Label Modlbl;
        private Label label1;
        private PictureBox pictureBox2;
    }
}