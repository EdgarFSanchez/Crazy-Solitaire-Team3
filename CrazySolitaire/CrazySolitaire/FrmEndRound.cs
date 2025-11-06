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
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            SuspendLayout();
            // 
            // btnNextRound
            // 
            btnNextRound.Location = new Point(405, 301);
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
            txtbxResult.Location = new Point(391, 241);
            txtbxResult.Name = "txtbxResult";
            txtbxResult.RightToLeft = RightToLeft.Yes;
            txtbxResult.Size = new Size(150, 23);
            txtbxResult.TabIndex = 1;
            txtbxResult.Text = "You Lose";
            txtbxResult.TextAlign = HorizontalAlignment.Center;
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = Color.FromArgb(0, 0, 64);
            pictureBox1.BackgroundImage = Properties.Resources.feelsSadMan;
            pictureBox1.BackgroundImageLayout = ImageLayout.Stretch;
            pictureBox1.Location = new Point(12, 102);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(373, 390);
            pictureBox1.TabIndex = 2;
            pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            pictureBox2.BackColor = Color.FromArgb(0, 0, 64);
            pictureBox2.BackgroundImage = Properties.Resources.failure;
            pictureBox2.BackgroundImageLayout = ImageLayout.Stretch;
            pictureBox2.Location = new Point(547, 102);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(417, 390);
            pictureBox2.TabIndex = 3;
            pictureBox2.TabStop = false;
            // 
            // FrmEndRound
            // 
            BackColor = Color.FromArgb(64, 0, 64);
            ClientSize = new Size(976, 637);
            Controls.Add(pictureBox2);
            Controls.Add(pictureBox1);
            Controls.Add(txtbxResult);
            Controls.Add(btnNextRound);
            Name = "FrmEndRound";
            StartPosition = FormStartPosition.CenterScreen;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }
        public TextBox txtbxResult;
        public PictureBox pictureBox1;
        public PictureBox pictureBox2;
        public Button btnNextRound;

        private void btnNextRound_Click(object sender, EventArgs e)
        {
            FrmGame.Instance = null;
            FrmGame frmGame = new();
            frmGame.Show();
            Hide();
        }
    }
}


