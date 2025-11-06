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
            txtbxResult.Size = new Size(150, 31);
            txtbxResult.TabIndex = 1;
            txtbxResult.Text = "You Lose";
            txtbxResult.TextAlign = HorizontalAlignment.Center;
            txtbxResult.TextChanged += Result_TextChanged;
            // 
            // FrmEndRound
            // 
            BackColor = Color.FromArgb(64, 0, 64);
            ClientSize = new Size(976, 637);
            Controls.Add(txtbxResult);
            Controls.Add(btnNextRound);
            Name = "FrmEndRound";
            StartPosition = FormStartPosition.CenterScreen;
            ResumeLayout(false);
            PerformLayout();

        }
        public TextBox txtbxResult;
        public Button btnNextRound;

        private void btnNextRound_Click(object sender, EventArgs e)
        {
            FrmGame.Instance = null;
            FrmGame frmGame = new();
            frmGame.Show();
            Hide();
        }

        private void Result_TextChanged(object sender, EventArgs e)
        {

        }
    }
}


