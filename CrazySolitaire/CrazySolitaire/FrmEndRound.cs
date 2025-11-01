namespace CrazySolitaire
{
    public partial class FrmEndRound : Form
    {
        public FrmEndRound()
        {
            InitializeComponent();
        }

        public FrmEndRound_Load()
        {
            Game.RoundForm = this;
        }

        private void InitializeComponent()
        {
            btnNextRound = new Button();
            SuspendLayout();
            // 
            // btnNextRound
            // 
            btnNextRound.Location = new Point(517, 110);
            btnNextRound.Name = "btnNextRound";
            btnNextRound.Size = new Size(112, 34);
            btnNextRound.TabIndex = 0;
            btnNextRound.Text = "button1";
            btnNextRound.UseVisualStyleBackColor = true;
            btnNextRound.Click += button1_Click;
            // 
            // FrmEndRound
            // 
            BackColor = Color.FromArgb(64, 0, 64);
            ClientSize = new Size(1394, 1062);
            Controls.Add(btnNextRound);
            Name = "FrmEndRound";
            StartPosition = FormStartPosition.CenterScreen;
            ResumeLayout(false);

        }
        private Button btnNextRound;

        private void button1_Click(object sender, EventArgs e)
        {
            Game.Reset();
        }
    }
}


