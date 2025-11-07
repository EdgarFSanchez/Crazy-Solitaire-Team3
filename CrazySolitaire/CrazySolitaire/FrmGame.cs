using CrazySolitaire.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using static System.Formats.Asn1.AsnWriter;

namespace CrazySolitaire
{
    /// <summary>
    /// Main game form for Crazy Solitaire
    /// </summary>
    public partial class FrmGame : Form
    {
        /// <summary>
        /// The currently dragged cards during a drag operation
        /// </summary>
        public static LinkedList<Card> CurDragCards { get; private set; } = new();

        /// <summary>
        /// The source from which the current set of cards is being dragged.
        /// </summary>
        public static IDragFrom CardsDraggedFrom { get; private set; }

        /// <summary>
        /// Singleton-like reference to the active game from instance
        /// </summary>
        internal static FrmGame Instance { get; set; }

        private System.Windows.Forms.Timer doublePointsTimer;           // Timer for tracking Double Points
        private bool isDoublePointsActive = false;                      // Indicates whether Double Points mode is currently active

        // Game logic components - handle random events and gameplay scheduling
        private IGameApi _gameApi;
        private RandomEventManager _eventManager;

        /// <summary>
        /// Stores the current background theme ID.
        /// </summary>
        public string CurrentBackgroundId { get; private set; } = "";

        /// <summary>
        /// Enables double buffering to prevent flicker
        /// </summary>
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

        /// <summary>
        /// Sets up the game form and initalizes all starting components
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            Instance = this;
            KeyPreview = true;

            // Grab all 7 tableau stack panels
            Panel[] panTableauStacks = new Panel[7];
            for (int i = 0; i < 7; i++)
            {
                panTableauStacks[i] = (Panel)Controls.Find($"panTableauStack_{i}", false)[0];
            }

            // Map each suit to its foundation panel
            Dictionary<Suit, Panel> panFoundationStacks = new()
            {
                [Suit.DIAMONDS] = panFoundationStack_Diamonds,
                [Suit.SPADES] = panFoundationStack_Spades,
                [Suit.HEARTS] = panFoundationStack_Hearts,
                [Suit.CLUBS] = panFoundationStack_Clubs,
            };

            // Initialize main game logic and layout
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

            // Timer for double points duration
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

            // Update score display
            ScoreManager.OnScoreChanged += (score) => { lblScore.Text = $"Social Cred: {score}"; };
            lblScore.Text = $"Social Cred: {ScoreManager.Score}"; 


            // Reset background and settings to default
            ClearBackgroundToDefaultGreen();
            SetScoreLabelForBackground("");
            CrazySolitaire.Properties.Settings.Default.SelectedBackgroundId = "";
            CrazySolitaire.Properties.Settings.Default.Save();
        }

        // Allows random events (like captcha) to temporarily disable hotkeys
        private bool _suppressHotkeys = false;
        public void SetHotkeysSuppressed(bool value) => _suppressHotkeys = value;

        /// <summary>
        /// Handles the end of a round, showing results and rewarding score.
        /// </summary>
        /// <param name="win"></param>
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
            endRound.txtbxCredit.Text = "You currently have " + ScoreManager.Score + " Social Credit";
            endRound.Show();
            Instance.Hide();

        }

        /// <summary>
        /// Handles clicks on the stock pile to draw or reload cards.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

                    // Change card back color as damage indicator
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
                // Draw one card from the deck
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
                _eventManager.OnCardDrawn();        // Notify the scheduler that a draw just happened
            }
        }

        /// <summary>
        /// Sets the form background image
        /// </summary>
        /// <param name="img">The image we want to select</param>
        /// <param name="layout">Nice fit for the image to be displayed</param>
        public void ApplyBackground(Image img, ImageLayout layout = ImageLayout.Stretch)
        {
            this.BackgroundImage = img;
            this.BackgroundImageLayout = layout;
        }

        /// <summary>
        /// Fix score text box when Lebron Background is selected
        /// </summary>
        /// <param name="id">The ID of the Lebron image</param>
        public void SetScoreLabelForBackground(string id)
        {
            lblScore.ForeColor = (id == "lebron") ? Color.Black : Color.White;
        }


        /// <summary>
        /// Applies a backgroun image and updates the background ID.
        /// </summary>
        /// <param name="img">The image to display as the background</param>
        /// <param name="id">A unique ID for the selected background</param>
        /// <param name="layout">Nice fit for the image to be displayed</param>
        public void ApplyBackgroundWithId(Image img, string id, ImageLayout layout = ImageLayout.Stretch)
        {
            this.BackgroundImage = img;
            this.BackgroundImageLayout = layout;
            CurrentBackgroundId = id;
            SetScoreLabelForBackground(id);
        }

        /// <summary>
        /// Resets background to the defualt green table color.
        /// </summary>
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

        /// <summary>
        /// Cleans up timers and active events when the form closes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmGame_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (doublePointsTimer != null)
            {
                doublePointsTimer.Stop();
                doublePointsTimer.Dispose();
            }
            _eventManager?.CancelActiveEvents();        // Cleanly stop anything still running
            Game.TitleForm.Close();
        }

        /// <summary>
        /// Activates the Double Points mode temporarily.
        /// </summary>
        private void ActivateDoublePoints()
        {
            isDoublePointsActive = true;
            ScoreManager.SetMultiplier(2);
            lblScore.ForeColor = Color.Red;
            doublePointsTimer.Start();
        }

        /// <summary>
        /// Handles keyboard shortcuts for round events and power-ups. (For testing)
        /// </summary>
        private void FrmGame_KeyDown(object sender, KeyEventArgs e)
        {
            // Allow overlays to suppress hotkeys while active
            if (_suppressHotkeys) return;

            switch (e.KeyCode)
            {
                case Keys.D when !isDoublePointsActive:
                    ActivateDoublePoints();
                    break;
                case Keys.P:
                    EndOfRound(false);
                    break;
                case Keys.W:
                    Game.SpawnWildCard();
                    break;
            }
        }

        /// <summary>
        /// Opens the in-game store window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Store_Click(object sender, EventArgs e)
        {
            using (var store = new Form1())
            {
                store.StartPosition = FormStartPosition.CenterScreen;
                store.ShowDialog(this);
            }
        }

        /// <summary>
        /// Provides a minimal interface that random events can use to interact with the game.
        /// Prevents events from directly manipulating the full game state.
        /// </summary>
        private sealed class GameApi : IGameApi
        {
            private readonly FrmGame _form;                  
            
            /// <summary>
            /// Constructor assigns the form reference above
            /// </summary>
            /// <param name="form"></param>
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
