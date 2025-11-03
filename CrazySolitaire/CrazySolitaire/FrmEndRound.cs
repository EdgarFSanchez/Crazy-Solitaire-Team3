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
            Result = new TextBox();
            SuspendLayout();
            // 
            // btnNextRound
            // 
            btnNextRound.Location = new Point(608, 552);
            btnNextRound.Name = "btnNextRound";
            btnNextRound.Size = new Size(119, 34);
            btnNextRound.TabIndex = 0;
            btnNextRound.Text = "Next Round";
            btnNextRound.UseVisualStyleBackColor = true;
            btnNextRound.Click += btnNextRound_Click;
            // 
            // Result
            // 
            Result.Enabled = false;
            Result.Location = new Point(627, 388);
            Result.Name = "Result";
            Result.RightToLeft = RightToLeft.Yes;
            Result.Size = new Size(150, 31);
            Result.TabIndex = 1;
            Result.Text = "You Lose";
            Result.TextAlign = HorizontalAlignment.Center;
            Result.TextChanged += Result_TextChanged;
            // 
            // FrmEndRound
            // 
            BackColor = Color.FromArgb(64, 0, 64);
            ClientSize = new Size(1394, 1062);
            Controls.Add(Result);
            Controls.Add(btnNextRound);
            Name = "FrmEndRound";
            StartPosition = FormStartPosition.CenterScreen;
            ResumeLayout(false);
            PerformLayout();

        }
        private TextBox Result;
        private Button btnNextRound;

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


