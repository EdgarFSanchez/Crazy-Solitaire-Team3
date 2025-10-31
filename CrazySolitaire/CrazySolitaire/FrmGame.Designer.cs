namespace CrazySolitaire
{
    partial class FrmGame
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
            pbStock = new PictureBox();
            panTalon = new Panel();
            panFoundationStack_Clubs = new Panel();
            panFoundationStack_Hearts = new Panel();
            panFoundationStack_Spades = new Panel();
            panFoundationStack_Diamonds = new Panel();
            panTableauStack_0 = new Panel();
            panTableauStack_1 = new Panel();
            panTableauStack_2 = new Panel();
            panTableauStack_3 = new Panel();
            panTableauStack_4 = new Panel();
            panTableauStack_5 = new Panel();
            panTableauStack_6 = new Panel();
            lblScore = new Label();
            ((System.ComponentModel.ISupportInitialize)pbStock).BeginInit();
            SuspendLayout();
            // 
            // pbStock
            // 
            pbStock.BackgroundImage = Properties.Resources.back_green;
            pbStock.BackgroundImageLayout = ImageLayout.Stretch;
            pbStock.BorderStyle = BorderStyle.Fixed3D;
            pbStock.Location = new Point(57, 45);
            pbStock.Margin = new Padding(4, 5, 4, 5);
            pbStock.Name = "pbStock";
            pbStock.Size = new Size(127, 207);
            pbStock.TabIndex = 0;
            pbStock.TabStop = false;
            pbStock.Click += pbStock_Click;
            // 
            // panTalon
            // 
            panTalon.Location = new Point(229, 45);
            panTalon.Margin = new Padding(4, 5, 4, 5);
            panTalon.Name = "panTalon";
            panTalon.Size = new Size(237, 210);
            panTalon.TabIndex = 1;
            // 
            // panFoundationStack_Clubs
            // 
            panFoundationStack_Clubs.AllowDrop = true;
            panFoundationStack_Clubs.BackgroundImage = Properties.Resources.FoundationStackBg_Clubs;
            panFoundationStack_Clubs.BackgroundImageLayout = ImageLayout.Stretch;
            panFoundationStack_Clubs.BorderStyle = BorderStyle.FixedSingle;
            panFoundationStack_Clubs.Location = new Point(610, 43);
            panFoundationStack_Clubs.Margin = new Padding(4, 5, 4, 5);
            panFoundationStack_Clubs.Name = "panFoundationStack_Clubs";
            panFoundationStack_Clubs.Size = new Size(142, 209);
            panFoundationStack_Clubs.TabIndex = 2;
            // 
            // panFoundationStack_Hearts
            // 
            panFoundationStack_Hearts.BackgroundImage = Properties.Resources.FoundationStackBg_Hearts;
            panFoundationStack_Hearts.BackgroundImageLayout = ImageLayout.Stretch;
            panFoundationStack_Hearts.BorderStyle = BorderStyle.FixedSingle;
            panFoundationStack_Hearts.Location = new Point(794, 45);
            panFoundationStack_Hearts.Margin = new Padding(4, 5, 4, 5);
            panFoundationStack_Hearts.Name = "panFoundationStack_Hearts";
            panFoundationStack_Hearts.Size = new Size(142, 209);
            panFoundationStack_Hearts.TabIndex = 3;
            // 
            // panFoundationStack_Spades
            // 
            panFoundationStack_Spades.BackgroundImage = Properties.Resources.FoundationStackBg_Spades;
            panFoundationStack_Spades.BackgroundImageLayout = ImageLayout.Stretch;
            panFoundationStack_Spades.BorderStyle = BorderStyle.FixedSingle;
            panFoundationStack_Spades.Location = new Point(979, 45);
            panFoundationStack_Spades.Margin = new Padding(4, 5, 4, 5);
            panFoundationStack_Spades.Name = "panFoundationStack_Spades";
            panFoundationStack_Spades.Size = new Size(142, 209);
            panFoundationStack_Spades.TabIndex = 3;
            // 
            // panFoundationStack_Diamonds
            // 
            panFoundationStack_Diamonds.BackgroundImage = Properties.Resources.FoundationStackBg_Diamonds;
            panFoundationStack_Diamonds.BackgroundImageLayout = ImageLayout.Stretch;
            panFoundationStack_Diamonds.BorderStyle = BorderStyle.FixedSingle;
            panFoundationStack_Diamonds.Location = new Point(1163, 45);
            panFoundationStack_Diamonds.Margin = new Padding(4, 5, 4, 5);
            panFoundationStack_Diamonds.Name = "panFoundationStack_Diamonds";
            panFoundationStack_Diamonds.Size = new Size(142, 209);
            panFoundationStack_Diamonds.TabIndex = 3;
            // 
            // panTableauStack_0
            // 
            panTableauStack_0.AllowDrop = true;
            panTableauStack_0.Location = new Point(57, 423);
            panTableauStack_0.Margin = new Padding(4, 5, 4, 5);
            panTableauStack_0.Name = "panTableauStack_0";
            panTableauStack_0.Size = new Size(144, 510);
            panTableauStack_0.TabIndex = 4;
            // 
            // panTableauStack_1
            // 
            panTableauStack_1.Location = new Point(241, 423);
            panTableauStack_1.Margin = new Padding(4, 5, 4, 5);
            panTableauStack_1.Name = "panTableauStack_1";
            panTableauStack_1.Size = new Size(144, 510);
            panTableauStack_1.TabIndex = 5;
            // 
            // panTableauStack_2
            // 
            panTableauStack_2.Location = new Point(426, 423);
            panTableauStack_2.Margin = new Padding(4, 5, 4, 5);
            panTableauStack_2.Name = "panTableauStack_2";
            panTableauStack_2.Size = new Size(144, 510);
            panTableauStack_2.TabIndex = 5;
            // 
            // panTableauStack_3
            // 
            panTableauStack_3.Location = new Point(610, 423);
            panTableauStack_3.Margin = new Padding(4, 5, 4, 5);
            panTableauStack_3.Name = "panTableauStack_3";
            panTableauStack_3.Size = new Size(144, 510);
            panTableauStack_3.TabIndex = 5;
            // 
            // panTableauStack_4
            // 
            panTableauStack_4.Location = new Point(794, 423);
            panTableauStack_4.Margin = new Padding(4, 5, 4, 5);
            panTableauStack_4.Name = "panTableauStack_4";
            panTableauStack_4.Size = new Size(144, 510);
            panTableauStack_4.TabIndex = 5;
            // 
            // panTableauStack_5
            // 
            panTableauStack_5.Location = new Point(979, 423);
            panTableauStack_5.Margin = new Padding(4, 5, 4, 5);
            panTableauStack_5.Name = "panTableauStack_5";
            panTableauStack_5.Size = new Size(144, 510);
            panTableauStack_5.TabIndex = 5;
            // 
            // panTableauStack_6
            // 
            panTableauStack_6.Location = new Point(1163, 423);
            panTableauStack_6.Margin = new Padding(4, 5, 4, 5);
            panTableauStack_6.Name = "panTableauStack_6";
            panTableauStack_6.Size = new Size(144, 510);
            panTableauStack_6.TabIndex = 5;
            // 
            // lblScore
            // 
            lblScore.AutoSize = true;
            lblScore.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblScore.ForeColor = Color.White;
            lblScore.Location = new Point(57, 314);
            lblScore.Name = "lblScore";
            lblScore.Size = new Size(225, 48);
            lblScore.TabIndex = 6;
            lblScore.Text = " Social Cred:";
            // 
            // FrmGame
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(0, 64, 0);
            ClientSize = new Size(1394, 1123);
            Controls.Add(lblScore);
            Controls.Add(panTableauStack_6);
            Controls.Add(panTableauStack_5);
            Controls.Add(panTableauStack_4);
            Controls.Add(panTableauStack_3);
            Controls.Add(panTableauStack_2);
            Controls.Add(panTableauStack_1);
            Controls.Add(panTableauStack_0);
            Controls.Add(panFoundationStack_Diamonds);
            Controls.Add(panFoundationStack_Spades);
            Controls.Add(panFoundationStack_Hearts);
            Controls.Add(panFoundationStack_Clubs);
            Controls.Add(panTalon);
            Controls.Add(pbStock);
            Margin = new Padding(4, 5, 4, 5);
            Name = "FrmGame";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Crazy Solitaire";
            FormClosing += FrmGame_FormClosing;
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)pbStock).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pbStock;
        private Panel panTalon;
        private Panel panFoundationStack_Clubs;
        private Panel panFoundationStack_Hearts;
        private Panel panFoundationStack_Spades;
        private Panel panFoundationStack_Diamonds;
        private Panel panTableauStack_0;
        private Panel panTableauStack_1;
        private Panel panTableauStack_2;
        private Panel panTableauStack_3;
        private Panel panTableauStack_4;
        private Panel panTableauStack_5;
        private Panel panTableauStack_6;
        private Label lblScore;
    }
}
