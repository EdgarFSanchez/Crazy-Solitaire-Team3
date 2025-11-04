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
            textBox1 = new TextBox();
            pictureBox1 = new PictureBox();
            lblScore = new TextBox();
            goodmanbtn = new Button();
            textBox2 = new TextBox();
            lebron = new PictureBox();
            button2 = new Button();
            textBox3 = new TextBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)lebron).BeginInit();
            SuspendLayout();
            // 
            // textBox1
            // 
            textBox1.BackColor = Color.FromArgb(0, 64, 0);
            textBox1.BorderStyle = BorderStyle.None;
            textBox1.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            textBox1.ForeColor = SystemColors.Window;
            textBox1.Location = new Point(12, 12);
            textBox1.Name = "textBox1";
            textBox1.ReadOnly = true;
            textBox1.Size = new Size(100, 32);
            textBox1.TabIndex = 0;
            textBox1.Text = "Store";
            textBox1.TextChanged += textBox1_TextChanged;
            // 
            // pictureBox1
            // 
            pictureBox1.BackgroundImage = Properties.Resources.latest1;
            pictureBox1.BackgroundImageLayout = ImageLayout.Stretch;
            pictureBox1.Location = new Point(12, 110);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(193, 112);
            pictureBox1.TabIndex = 2;
            pictureBox1.TabStop = false;
            pictureBox1.Click += pictureBox1_Click;
            // 
            // lblScore
            // 
            lblScore.BackColor = Color.FromArgb(0, 64, 0);
            lblScore.BorderStyle = BorderStyle.None;
            lblScore.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblScore.ForeColor = SystemColors.Window;
            lblScore.Location = new Point(522, 21);
            lblScore.Name = "lblScore";
            lblScore.ReadOnly = true;
            lblScore.Size = new Size(295, 32);
            lblScore.TabIndex = 3;
            lblScore.Text = "Social Credit: ";
            // 
            // goodmanbtn
            // 
            goodmanbtn.BackColor = Color.Gold;
            goodmanbtn.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            goodmanbtn.Location = new Point(67, 228);
            goodmanbtn.Name = "goodmanbtn";
            goodmanbtn.Size = new Size(80, 37);
            goodmanbtn.TabIndex = 1;
            goodmanbtn.Text = "BUY";
            goodmanbtn.UseVisualStyleBackColor = false;
            goodmanbtn.Click += goodmanbtn_Click;
            // 
            // textBox2
            // 
            textBox2.BackColor = Color.FromArgb(0, 64, 0);
            textBox2.BorderStyle = BorderStyle.None;
            textBox2.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            textBox2.ForeColor = SystemColors.Window;
            textBox2.Location = new Point(16, 67);
            textBox2.Name = "textBox2";
            textBox2.ReadOnly = true;
            textBox2.Size = new Size(189, 32);
            textBox2.TabIndex = 4;
            textBox2.Text = "Background";
            // 
            // lebron
            // 
            lebron.BackColor = Color.Transparent;
            lebron.BackgroundImage = Properties.Resources.hq720;
            lebron.BackgroundImageLayout = ImageLayout.Stretch;
            lebron.Location = new Point(231, 110);
            lebron.Name = "lebron";
            lebron.Size = new Size(193, 112);
            lebron.TabIndex = 5;
            lebron.TabStop = false;
            // 
            // button2
            // 
            button2.BackColor = Color.Gold;
            button2.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button2.Location = new Point(291, 228);
            button2.Name = "button2";
            button2.Size = new Size(80, 37);
            button2.TabIndex = 6;
            button2.Text = "BUY";
            button2.UseVisualStyleBackColor = false;
            button2.Click += Lebronbtn_Click;
            // 
            // textBox3
            // 
            textBox3.Location = new Point(12, 309);
            textBox3.Name = "textBox3";
            textBox3.Size = new Size(100, 23);
            textBox3.TabIndex = 7;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(0, 64, 0);
            ClientSize = new Size(976, 637);
            Controls.Add(textBox3);
            Controls.Add(button2);
            Controls.Add(lebron);
            Controls.Add(textBox2);
            Controls.Add(lblScore);
            Controls.Add(pictureBox1);
            Controls.Add(goodmanbtn);
            Controls.Add(textBox1);
            Name = "Form1";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)lebron).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox textBox1;
        private PictureBox pictureBox1;
        private TextBox lblScore;
        private Button goodmanbtn;
        private TextBox textBox2;
        private PictureBox lebron;
        private Button button2;
        private TextBox textBox3;
    }
}