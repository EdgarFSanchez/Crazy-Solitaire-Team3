using System;
using System.Drawing;
using System.Windows.Forms;

namespace CrazySolitaire
{
    /// <summary>
    /// In game Store form: lets players buy and equip backgrounds along with purchasing modifiers
    /// Keeps button labels in sync (Buy/equip/equipped) and shows live social credit.
    /// </summary>
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Style buttons, sync the initial UI state, and hook score events.
        /// </summary>
        /// <param name="e">EventArgs: load event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Button styling
            StyleGameButton(goodmanbtn, Color.Gold, Color.Black);
            StyleGameButton(button2, Color.Gold, Color.Black);
            StyleGameButton(button3, Color.MediumPurple, Color.White);
            btnResetBg.Text = "Default";
            StyleGameButton(btnResetBg, Color.ForestGreen, Color.White);

            // Wild Card buy button (designer button under the white card)
            StyleGameButton(btnWildBuy, Color.Gold, Color.Black);

            // First paint the labels
            UpdateSocialCreditText();
            UpdateBgButtonsUi();

            // Keep the store display in sync with score/multipler changes
            ScoreManager.OnScoreChanged += HandleScoreChanged;
            ScoreManager.OnMultiplierChanged += HandleMultiplerChanged;
        }

        /// <summary>
        /// Unsubscribe from events when closing to avoid dangling handlers.
        /// </summary>
        /// <param name="e">FormClosedEventArgs: close event data.</param>
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            ScoreManager.OnScoreChanged -= HandleScoreChanged;
            ScoreManager.OnMultiplierChanged -= HandleMultiplerChanged;
            base.OnFormClosed(e);
        }

        // Event the label refresh
        private void HandleScoreChanged(int _) => UpdateSocialCreditText();
        private void HandleMultiplerChanged(int _) => UpdateSocialCreditText();

        /// <summary>
        /// Shows current social credit on the store header.
        /// </summary>
        private void UpdateSocialCreditText()
        {
            if (InvokeRequired) { BeginInvoke(new Action(UpdateSocialCreditText)); return; }
            Sclbl.Text = $"Social Credit: {ScoreManager.Score}";
        }

        /// <summary>
        /// Buy or equip Saul Goodman background.
        /// </summary>
        /// <param name="sender">object: event source.</param>
        /// <param name="e">EventArgs: click event data.</param>
        private void goodmanbtn_Click(object sender, EventArgs e)
        {
            HandleBackgroundAction("saul", Properties.Resources.latest1, price: 500);
        }

        /// <summary>
        /// Buy or equip Lebron James background
        /// </summary>
        /// <param name="sender">object: event source.</param>
        /// <param name="e">EventArgs: click event data.</param>
        private void Lebronbtn_Click(object sender, EventArgs e)
        {
            HandleBackgroundAction("lebron", Properties.Resources.hq720, price: 500);
        }

        private void pictureBox1_Click(object sender, EventArgs e) { }

        /// <summary>
        /// Reset background to default green
        /// </summary>
        /// <param name="sender">object: event source.</param>
        /// <param name="e">EventArgs: click event data.</param>
        private void btnResetBg_Click(object sender, EventArgs e)
        {
            FrmGame.Instance?.ClearBackgroundToDefaultGreen();
            UpdateBgButtonsUi();
            MessageBox.Show("Background reset to default green.");
        }

        /// <summary>
        /// Shared buy/equip flow:
        /// -> If not owned, try to purchase (validates balance and no double charging this session)
        /// -> Equip immediately and refresh labels and buttons.
        /// </summary>
        /// <param name="id">string: background ID.</param>
        /// <param name="img">Image: background image to apply.</param>
        /// <param name="price">int: cost in credits.</param>
        private void HandleBackgroundAction(string id, Image img, int price)
        {
            // Purchase once per game if not owned.
            if (!StoreSession.IsBackgroundOwned(id))
            {
                if (!StoreSession.TryPurchaseBackground(id, price, out var error))
                {
                    MessageBox.Show(error, "Store", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }

            // Equip and refresh UI
            FrmGame.Instance?.ApplyBackgroundWithId(img, id);
            UpdateSocialCreditText();
            UpdateBgButtonsUi();
            MessageBox.Show(StoreSession.IsBackgroundOwned(id) ? "Equipped!" : "Purchased & Equipped!");
        }

        /// <summary>
        /// Purchase the 2x credits modifier for this session.
        /// </summary>
        /// <param name="sender">object: event source.</param>
        /// <param name="e">EventArgs: click event data.</param>
        private void button3_Click(object sender, EventArgs e)
        {
            // 1500 is roughly how much you get for completeling a full game
            const int price = 1500;

            if (ScoreManager.Score < price)
            {
                MessageBox.Show($"You need {price} Social Credit to buy this modifier.");
                return;
            }

            ScoreManager.SubtractPoints(price);
            ScoreManager.PurchaseDoubleCredits();
            UpdateSocialCreditText();

            MessageBox.Show("2× Social Credit purchased! It will stay active.");
        }

        private const int BtnMinW = 110;
        private const int BtnMinH = 36;

        private const int WildPrice = 2000;

        // ======= Button styling =======

        /// <summary>
        /// Fix button style to look more like a game should.
        /// Hovering gives an effect.
        /// Strong border/brightness so its visible when hovering.
        /// </summary>
        /// <param name="b">Button: target button.</param>
        /// <param name="baseColor">Color: base fill color.</param>
        /// <param name="textColor">Color?: optional text color.</param>
        private void StyleGameButton(Button b, Color baseColor, Color? textColor = null)
        {
            // Layout
            b.AutoSize = true;
            b.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            b.MinimumSize = new Size(BtnMinW, BtnMinH);
            b.Padding = new Padding(12, 6, 12, 6);
            b.UseVisualStyleBackColor = false;
            b.FlatStyle = FlatStyle.Flat;
            b.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            b.Cursor = Cursors.Hand;
            b.ForeColor = textColor ?? Color.Black;

            // Visual for states
            var baseCol = baseColor;
            var hoverCol = Adjust(baseCol, +0.25);
            var downCol = Adjust(baseCol, -0.22);
            var border = Adjust(baseCol, -0.50);
            var borderHov = Adjust(baseCol, -0.65);
            var borderDn = Adjust(baseCol, -0.78);

            // Baseline
            b.BackColor = baseCol;
            b.FlatAppearance.BorderSize = 2;
            b.FlatAppearance.BorderColor = border;

            b.FlatAppearance.MouseOverBackColor = hoverCol;
            b.FlatAppearance.MouseDownBackColor = downCol;

            // Attach handlers once to avoid stacking duplicates
            if (!Equals(b.Tag, "hover-wired"))
            {
                b.MouseEnter += (_, __) =>
                {
                    b.BackColor = hoverCol;
                    b.FlatAppearance.BorderColor = borderHov;
                    b.FlatAppearance.BorderSize = 3;
                };
                b.MouseLeave += (_, __) =>
                {
                    b.BackColor = baseCol;
                    b.FlatAppearance.BorderColor = border;
                    b.FlatAppearance.BorderSize = 2;
                };
                b.MouseDown += (_, __) =>
                {
                    b.BackColor = downCol;
                    b.FlatAppearance.BorderColor = borderDn;
                };
                b.MouseUp += (_, __) =>
                {
                    b.BackColor = hoverCol;
                    b.FlatAppearance.BorderColor = borderHov;
                };
                b.Tag = "hover-wired";
            }
        }

        /// <summary>
        /// Already equipped state (disable)
        /// </summary>
        /// <param name="b">Button: target button.</param>
        private void SetEquipped(Button b)
        {
            b.Enabled = false;
            b.Text = "EQUIPPED";
            StyleGameButton(b, Color.Silver, Color.Black);
        }


        /// <summary>
        /// Owned but not active
        /// </summary>
        /// <param name="b">Button: target button.</param>
        private void SetEquip(Button b)
        {
            b.Enabled = true;
            b.Text = "EQUIP";
            StyleGameButton(b, Color.Gold, Color.Black);
        }

        /// <summary>
        /// Not owned yet
        /// </summary>
        /// <param name="b">Button: target button.</param>
        private void SetBuy(Button b)
        {
            b.Enabled = true;
            b.Text = "BUY";
            StyleGameButton(b, Color.Gold, Color.Black);
        }

        /// <summary>
        /// Decide which state the background of the button should show.
        /// Can be buy, equip, or equipped
        /// </summary>
        /// <param name="btn">Button: target button.</param>
        /// <param name="id">string: background ID for this button.</param>
        /// <param name="equippedId">string: currently equipped background ID.</param>
        private void UpdateSingleBgButton(Button btn, string id, string equippedId)
        {
            if (equippedId == id)
            {
                SetEquipped(btn);
            }
            else if (StoreSession.IsBackgroundOwned(id))
            {
                SetEquip(btn);
            }
            else
            {
                SetBuy(btn);
            }
        }

        // ======= Small color utils =======
        private static int Clamp(int v, int min, int max) => v < min ? min : (v > max ? max : v);

        /// <summary>
        /// Blend between two colors
        /// </summary>
        /// <param name="a">Color: start color.</param>
        /// <param name="b">Color: end color.</param>
        /// <param name="t">double: blend factor 0..1.</param>
        /// <returns>Color: blended color.</returns>
        private static Color Blend(Color a, Color b, double t)
        {
            int r = (int)Math.Round(a.R + (b.R - a.R) * t);
            int g = (int)Math.Round(a.G + (b.G - a.G) * t);
            int bl = (int)Math.Round(a.B + (b.B - a.B) * t);
            return Color.FromArgb(255, Clamp(r, 0, 255), Clamp(g, 0, 255), Clamp(bl, 0, 255));
        }

        /// <summary>
        /// Lighten or darken a color by blending towards white or black
        /// </summary>
        /// <param name="c">Color: base color.</param>
        /// <param name="amount">double: positive to lighten, negative to darken.</param>
        /// <returns>Color: adjusted color.</returns>
        private static Color Adjust(Color c, double amount)
        {
            return amount >= 0 ? Blend(c, Color.White, amount) : Blend(c, Color.Black, -amount);
        }

        // ======= Batch refresh for button states =======

        /// <summary>
        /// Refresh all background buttons based on what is equipped and owned.
        /// Also styles the default button since its not purchasable
        /// </summary>
        private void UpdateBgButtonsUi()
        {
            var equippedId = FrmGame.Instance?.CurrentBackgroundId ?? "";
            UpdateSingleBgButton(goodmanbtn, "saul", equippedId);
            UpdateSingleBgButton(button2, "lebron", equippedId);

            btnResetBg.Text = "Default";
            StyleGameButton(btnResetBg, Color.ForestGreen, Color.White);
        }

        // Designer placeholders (label clicked
        private void bglbl_Click(object sender, EventArgs e) { }
        private void storelbl_Click(object sender, EventArgs e) { }
        private void Sclbl_Click(object sender, EventArgs e) { }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Purchase and spawn a Wild Card (2000 Social Credit).
        /// Prevents charging if one is already active.
        /// </summary>
        /// <param name="sender">object: Source of the event (the Wild Card buy button).</param>
        /// <param name="e">EventArgs: Click event data.</param>
        private void btnWildBuy_Click(object sender, EventArgs e)
        {
            // Already have one active? Do not charge.
            if (Game.WildCard != null)
            {
                MessageBox.Show("You already have a Wild Card active. Use it first!");
                return;
            }

            // Not enough credit guard
            if (ScoreManager.Score < WildPrice)
            {
                MessageBox.Show($"You need {WildPrice} Social Credit to buy this modifier.");
                return;
            }

            // Spawn first, then charge (avoids charging if the game rejects).
            var before = Game.WildCard;
            Game.SpawnWildCard();

            // If spawn did nothing (still the same reference), skip charging.
            if (ReferenceEquals(Game.WildCard, before))
                return;

            // Deduct price and refresh UI
            ScoreManager.SubtractPoints(WildPrice);
            UpdateSocialCreditText();
        }
    }
}
