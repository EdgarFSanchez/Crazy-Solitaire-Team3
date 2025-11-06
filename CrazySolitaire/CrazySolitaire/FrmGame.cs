using CrazySolitaire.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using static System.Formats.Asn1.AsnWriter;

namespace CrazySolitaire
{
    public partial class FrmGame : Form
    {
        public static LinkedList<Card> CurDragCards { get; private set; } = new();
        public static IDragFrom CardsDraggedFrom { get; private set; }
        internal static FrmGame Instance { get; set; }

        private System.Windows.Forms.Timer doublePointsTimer;           // timer to count down duration
        private bool isDoublePointsActive = false;                      // flag to prevent multiple activations

        // Random events: thin game surface + scheduler
        private IGameApi _gameApi;
        private RandomEventManager _eventManager;

        public string CurrentBackgroundId { get; private set; } = "";

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }

        public FrmGame()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Instance = this;
            KeyPreview = true;

            Panel[] panTableauStacks = new Panel[7];
            for (int i = 0; i < 7; i++)
            {
                panTableauStacks[i] = (Panel)Controls.Find($"panTableauStack_{i}", false)[0];
            }

            Dictionary<Suit, Panel> panFoundationStacks = new()
            {
                [Suit.DIAMONDS] = panFoundationStack_Diamonds,
                [Suit.SPADES] = panFoundationStack_Spades,
                [Suit.HEARTS] = panFoundationStack_Hearts,
                [Suit.CLUBS] = panFoundationStack_Clubs,
            };

            Game.Init(panTalon, panTableauStacks, panFoundationStacks);

            // Random events: wire up manager and tune pacing
            _gameApi = new GameApi(this);
            _eventManager = new RandomEventManager(this, _gameApi)
            {
                MinDrawsBetweenEvents = 3,
                MaxDrawsBetweenEvents = 6,
                PercentChanceWhenThresholdHit = 70,
                MaxEventsPerGame = 6,
                MaxPerEventPerGame = 3,
                MinSecondsBetweenEvents = 7
            };
            // Eligible events and their relative weights
            _eventManager.Register(new CaptchaEvent { Weight = 7 });
            _eventManager.Register(new CursorShakeEvent { Weight = 8 });

            doublePointsTimer = new System.Windows.Forms.Timer { Interval = 10_000 };
            doublePointsTimer.Tick += (s, e) =>
            {
                if (!ScoreManager.PermanentDoubleCredits)
                {
                    ScoreManager.SetMultiplier(1);
                }

                isDoublePointsActive = false;
                doublePointsTimer.Stop();

                SetScoreLabelForBackground(CurrentBackgroundId);
            };

            // Only reset multiplier, not score, when starting a new round
            ScoreManager.OnScoreChanged += (score) => { lblScore.Text = $"Social Cred: {score}"; };
            lblScore.Text = $"Social Cred: {ScoreManager.Score}"; // set initial label

            ClearBackgroundToDefaultGreen();
            SetScoreLabelForBackground("");
            CrazySolitaire.Properties.Settings.Default.SelectedBackgroundId = "";
            CrazySolitaire.Properties.Settings.Default.Save();
        }

        // lets events (like captcha) temporarily disable hotkeys
        private bool _suppressHotkeys = false;
        public void SetHotkeysSuppressed(bool value) => _suppressHotkeys = value;

        public static void EndOfRound(Boolean win)
        {
            FrmEndRound endRound = new();
            if (win)
            {
                endRound.txtbxResult.Text = "You win!";
                endRound.pictureBox1.BackgroundImage = Resources.psych;
                endRound.pictureBox2.BackgroundImage = Resources.greatSuccess;
                ScoreManager.AddPoints(500);
            }
            else
            {
                ScoreManager.SubtractPoints(250);
            }

            endRound.Show();
            Instance.Hide();

        }

        private void pbStock_Click(object sender, EventArgs e)
        {
            if (pbStock.BackgroundImage is null)
            {
                Game.StockReloadCount++;

                if (Game.StockReloadCount > 3)
                {
                    FrmGame.EndOfRound(false);
                    Hide();
                }
                else
                {
                    // On recycle: quick screen shake to show damage taken
                    var shake = new ScreenShakeEvent();
                    shake.Start(this, _gameApi, () => { });

                    Game.Talon.ReleaseIntoDeck(Game.Deck);

                    pbStock.BackgroundImage = Game.StockReloadCount switch
                    {
                        1 => Resources.back_green,
                        2 => Resources.back_orange,
                        3 => Resources.back_red,
                        _ => pbStock.BackgroundImage
                    };
                }
            }
            else
            {
                for (int i = 0; i < 1; i++)
                {
                    Card c = Game.Deck.Acquire();

                    if (c != null)
                    {
                        Game.Talon.AddCard(c);
                        c.AdjustLocation(i * 20, 0);
                        c.PicBox.BringToFront();
                    }
                }

                if (Game.Deck.IsEmpty())
                {
                    pbStock.BackgroundImage = null;
                }
                // notify the scheduler that a draw just happened
                _eventManager.OnCardDrawn();
            }
        }

        public void ApplyBackground(Image img, ImageLayout layout = ImageLayout.Stretch)
        {
            this.BackgroundImage = img;
            this.BackgroundImageLayout = layout;
        }

        public void SetScoreLabelForBackground(string id)
        {
            lblScore.ForeColor = (id == "lebron") ? Color.Black : Color.White;
        }


        public void ApplyBackgroundWithId(Image img, string id, ImageLayout layout = ImageLayout.Stretch)
        {
            this.BackgroundImage = img;
            this.BackgroundImageLayout = layout;
            CurrentBackgroundId = id;
            SetScoreLabelForBackground(id);
        }

        public void ClearBackgroundToDefaultGreen()
        {
            this.BackgroundImage = null;
            this.BackgroundImageLayout = ImageLayout.Stretch;
            CurrentBackgroundId = "";
            SetScoreLabelForBackground("");
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
            if (doublePointsTimer != null)
            {
                doublePointsTimer.Stop();
                doublePointsTimer.Dispose();
            }
            // cleanly stop anything still running
            _eventManager?.CancelActiveEvents();
            Game.TitleForm.Close();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // allow overlays to suppress hotkeys while active
            if (_suppressHotkeys) return base.ProcessCmdKey(ref msg, keyData);

            if (keyData == Keys.D && !isDoublePointsActive)
            {
                ActivateDoublePoints();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void ActivateDoublePoints()
        {
            isDoublePointsActive = true;
            ScoreManager.SetMultiplier(2);
            lblScore.ForeColor = Color.Red;
            doublePointsTimer.Start();
        }

        private void FrmGame_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.D && !isDoublePointsActive)
            {
                ActivateDoublePoints();
            }

            if (e.KeyCode == Keys.P)
            {
                EndOfRound(false);
            }
            
            if (e.KeyCode == Keys.W)
            {
                Game.SpawnWildCard();
            }
        }

        private void Store_Click(object sender, EventArgs e)
        {
            using (var store = new Form1())
            {
                store.StartPosition = FormStartPosition.CenterScreen;
                store.ShowDialog(this);
            }
        }

        // minimal surface the events can call into
        private sealed class GameApi : IGameApi
        {
            private readonly FrmGame _form;
            public GameApi(FrmGame form) { _form = form; }

            public bool IsBusyAnimating => false;
            public void PausePlayerInput() => _form.Enabled = false;
            public void ResumePlayerInput() => _form.Enabled = true;
            public void AddScore(int points) => ScoreManager.AddPoints(points);
            public void MultiplyScoreFor(TimeSpan duration, double factor) { }
            public void ShuffleStock() { }
            public void ShuffleOneTableauColumn() { }
            public void AutoMoveOneLegalCard() { }
            public void SetInvertedControls(bool inverted) { }
        }
    }
}
