namespace CrazySolitaire
{
    public partial class FrmEndRound : Form
    {
        public FrmEndRound()
        {
            InitializeComponent();
        }

        public void FrmEndRound_Load()
        {
            Game.RoundForm = this;
        }

        private void InitializeComponent()
        {
            btnNextRound = new Button();
            txtbxResult = new TextBox();
            pictureBox1 = new PictureBox();
            pictureBox2 = new PictureBox();
            txtbxCredit = new TextBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            SuspendLayout();
            // 
            // btnNextRound
            // 
            btnNextRound.Location = new Point(423, 521);
            btnNextRound.Name = "btnNextRound";
            btnNextRound.Size = new Size(119, 34);
            btnNextRound.TabIndex = 0;
            btnNextRound.Text = "Next Round";
            btnNextRound.UseVisualStyleBackColor = true;
            btnNextRound.Click += btnNextRound_Click;
            // 
            // txtbxResult
            // 
            txtbxResult.Enabled = false;
            txtbxResult.Location = new Point(329, 463);
            txtbxResult.Name = "txtbxResult";
            txtbxResult.RightToLeft = RightToLeft.No;
            txtbxResult.Size = new Size(312, 23);
            txtbxResult.TabIndex = 1;
            txtbxResult.Text = "You lose, if you reach -500 social credit you will EXPLODE! ";
            txtbxResult.TextAlign = HorizontalAlignment.Center;
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = Color.FromArgb(0, 0, 64);
            pictureBox1.BackgroundImage = Properties.Resources.feelsSadMan;
            pictureBox1.BackgroundImageLayout = ImageLayout.Stretch;
            pictureBox1.Location = new Point(1, 1);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(475, 390);
            pictureBox1.TabIndex = 2;
            pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            pictureBox2.BackColor = Color.FromArgb(0, 0, 64);
            pictureBox2.BackgroundImage = Properties.Resources.failure;
            pictureBox2.BackgroundImageLayout = ImageLayout.Stretch;
            pictureBox2.Location = new Point(482, 1);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(494, 390);
            pictureBox2.TabIndex = 3;
            pictureBox2.TabStop = false;
            // 
            // txtbxCredit
            // 
            txtbxCredit.Location = new Point(378, 492);
            txtbxCredit.Name = "txtbxCredit";
            txtbxCredit.Size = new Size(205, 23);
            txtbxCredit.TabIndex = 4;
            // 
            // FrmEndRound
            // 
            BackColor = Color.FromArgb(64, 0, 64);
            ClientSize = new Size(976, 637);
            Controls.Add(txtbxCredit);
            Controls.Add(pictureBox2);
            Controls.Add(pictureBox1);
            Controls.Add(txtbxResult);
            Controls.Add(btnNextRound);
            Name = "FrmEndRound";
            StartPosition = FormStartPosition.CenterScreen;
            FormClosing += FrmEndRound_FormClosing;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }
        public TextBox txtbxResult;
        public PictureBox pictureBox1;
        public PictureBox pictureBox2;
        public TextBox txtbxCredit;
        public Button btnNextRound;

        private void btnNextRound_Click(object sender, EventArgs e)
        {
            FrmGame.Instance = null;
            FrmGame frmGame = new();
            frmGame.Show();
            Hide();
        }

        //when the game is closed close the title form to prevent resource leak
        private void FrmEndRound_FormClosing(object sender, FormClosingEventArgs e)
        {
            Game.TitleForm.Close();
        }
    }
}


