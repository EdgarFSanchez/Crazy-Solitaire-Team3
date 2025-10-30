using CrazySolitaire.Properties;

namespace CrazySolitaire
{
    public partial class FrmGame : Form
    {
        public static LinkedList<Card> CurDragCards { get; private set; } = new();
        public static IDragFrom CardsDraggedFrom { get; private set; }
        public static FrmGame Instance { get; private set; }
        private System.Windows.Forms.Timer doublePointsTimer;           // timer to count down duration
        private bool isDoublePointsActive = false;                      // flag to prevent multiple activations


        protected override CreateParams CreateParams
        {
            get
            {
        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;    // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }

        public FrmGame()
        {
        public FrmGame()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        private void Form1_Load(object sender, EventArgs e)
        {
            Instance = this;
            Panel[] panTableauStacks = new Panel[7];
            for (int i = 0; i < 7; i++)
            {
            for (int i = 0; i < 7; i++)
            {
                panTableauStacks[i] = (Panel)Controls.Find($"panTableauStack_{i}", false)[0];
            }
            Dictionary<Suit, Panel> panFoundationStacks = new()
            {
            Dictionary<Suit, Panel> panFoundationStacks = new()
            {
                [Suit.DIAMONDS] = panFoundationStack_Diamonds,
                [Suit.SPADES] = panFoundationStack_Spades,
                [Suit.HEARTS] = panFoundationStack_Hearts,
                [Suit.CLUBS] = panFoundationStack_Clubs,
            };
            Game.Init(panTalon, panTableauStacks, panFoundationStacks);

            // Setup the Double Points Modifier timer
            doublePointsTimer = new System.Windows.Forms.Timer();
            doublePointsTimer.Interval = 10000;                         // 10 seconds of double points
            doublePointsTimer.Tick += (s, e) =>
            {
                ScoreManager.SetMultiplier(1);                          // Reset multiplier
                isDoublePointsActive = false;
                doublePointsTimer.Stop();
                lblScore.ForeColor = Color.White;                       // reset text color
            };

            ScoreManager.OnScoreChanged += (score) => { lblScore.Text = $"Social Cred: {score}"; };
            ScoreManager.Reset();
        }

        private void pbStock_Click(object sender, EventArgs e)
        {
            if (pbStock.BackgroundImage is null)
            {
        private void pbStock_Click(object sender, EventArgs e)
        {
            if (pbStock.BackgroundImage is null)
            {
                Game.StockReloadCount++;
                if (Game.StockReloadCount > 3)
                {
                if (Game.StockReloadCount > 3)
                {
                    Game.Explode();
                    MessageBox.Show("You computer has been infected with ransomware", "You have been infected", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    FrmYouLose frmYouLose = new();
                    frmYouLose.Show();
                    Hide();
                }
                else
                {
                else
                {
                    Game.Talon.ReleaseIntoDeck(Game.Deck);
                    pbStock.BackgroundImage = Game.StockReloadCount switch
                    {
                    pbStock.BackgroundImage = Game.StockReloadCount switch
                    {
                        1 => Resources.back_green,
                        2 => Resources.back_orange,
                        3 => Resources.back_red,
                    };
                }
            }
            else
            {
                for (int i = 0; i < 3; i++)
                {
            else
            {
                for (int i = 0; i < 3; i++)
                {
                    Card c = Game.Deck.Acquire();
                    if (c != null)
                    {
                    if (c != null)
                    {
                        Game.Talon.AddCard(c);
                        c.AdjustLocation(i * 20, 0);
                        c.PicBox.BringToFront();
                    }
                }
                if (Game.Deck.IsEmpty())
                {
                if (Game.Deck.IsEmpty())
                {
                    pbStock.BackgroundImage = null;
                }
            }
        }

        public static void DragCard(Card c)
        {
            CurDragCards.AddLast(c);
            CardsDraggedFrom = Game.FindDragFrom(c);
        }
        public static void StopDragCard(Card c)
        {
            if (CurDragCards.Contains(c))
                CurDragCards.Clear();
        }
        public static bool IsDraggingCard(Card c) => CurDragCards.Contains(c);

        private void FrmGame_FormClosing(object sender, FormClosingEventArgs e)
        {
        private void FrmGame_FormClosing(object sender, FormClosingEventArgs e)
        {
            Game.TitleForm.Close();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.D && !isDoublePointsActive)
            {
                ActivateDoublePoints();
                return true; // indicate we handled it
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void ActivateDoublePoints()
        {
            isDoublePointsActive = true;
            ScoreManager.SetMultiplier(2);
            lblScore.ForeColor = Color.Red;                 // visual cue
            doublePointsTimer.Start();
        }

        private void FrmGame_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.D && !isDoublePointsActive)
            {
                ActivateDoublePoints();
            }
        }
    }
}
