using System;
using System.Drawing;
using System.Windows.Forms;

namespace CrazySolitaire
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        // ---------- Lifecycle ----------
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // style buttons
            StyleGameButton(goodmanbtn, Color.Gold, Color.Black);
            StyleGameButton(button2, Color.Gold, Color.Black);     // LeBron
            StyleGameButton(button3, Color.MediumPurple, Color.White); // 2x
            btnResetBg.Text = "Default";
            StyleGameButton(btnResetBg, Color.ForestGreen, Color.White);

            UpdateSocialCreditText();

            // now that styling is done, set states; also FrmGame.Instance is definitely set
            UpdateBgButtonsUi();

            ScoreManager.OnScoreChanged += HandleScoreChanged;
            ScoreManager.OnMultiplierChanged += HandleMultiplerChanged;
        }


        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            ScoreManager.OnScoreChanged -= HandleScoreChanged;
            ScoreManager.OnMultiplierChanged -= HandleMultiplerChanged;
            base.OnFormClosed(e);
        }

        // ---------- Score label updates ----------
        private void HandleScoreChanged(int _) => UpdateSocialCreditText();
        private void HandleMultiplerChanged(int _) => UpdateSocialCreditText();

        private void UpdateSocialCreditText()
        {
            if (InvokeRequired) { BeginInvoke(new Action(UpdateSocialCreditText)); return; }
            Sclbl.Text = $"Social Credit: {ScoreManager.Score}";
        }

        // ---------- Background actions ----------
        private void goodmanbtn_Click(object sender, EventArgs e)
        {
            // Saul Goodman (latest1)
            HandleBackgroundAction("saul", Properties.Resources.latest1, price: 0);
        }

        private void Lebronbtn_Click(object sender, EventArgs e)
        {
            // LeBron (change resource name if yours differs)
            HandleBackgroundAction("lebron", Properties.Resources.hq720, price: 0);
        }

        // Optional: clicking the Saul picture also buys/equips Saul
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            HandleBackgroundAction("saul", Properties.Resources.latest1, price: 1500);
        }

        private void btnResetBg_Click(object sender, EventArgs e)
        {
            // Equip default green (not a purchase)
            FrmGame.Instance?.ClearBackgroundToDefaultGreen();
            UpdateBgButtonsUi();
            MessageBox.Show("Background reset to default green.");
        }

        /// <summary>
        /// Handles both BUY and EQUIP:
        /// - If not owned, attempts to purchase once per session (no double charge)
        /// - Then equips and updates button states.
        /// </summary>
        private void HandleBackgroundAction(string id, Image img, int price)
        {
            // Not owned yet? Try to purchase (StoreSession prevents double-charge in session)
            if (!StoreSession.IsBackgroundOwned(id))
            {
                if (!StoreSession.TryPurchaseBackground(id, price, out var error))
                {
                    MessageBox.Show(error, "Store", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                // Purchased this session; fall through to equip
            }

            // Equip in the game window (also sets label color there)
            FrmGame.Instance?.ApplyBackgroundWithId(img, id);

            // Refresh UI
            UpdateSocialCreditText();
            UpdateBgButtonsUi();

            MessageBox.Show(StoreSession.IsBackgroundOwned(id) ? "Equipped!" : "Purchased & Equipped!");
        }

        // ---------- 2x modifier ----------
        private void button3_Click(object sender, EventArgs e)
        {
            const int price = 0; // debug price

            if (ScoreManager.Score < price)
            {
                MessageBox.Show($"You need {price} Social Credit to buy this modifier.");
                return;
            }

            ScoreManager.SubtractPoints(price);
            ScoreManager.PurchaseDoubleCredits(); // permanent 2x for this session
            UpdateSocialCreditText();

            MessageBox.Show("2× Social Credit purchased! It will stay active.");
        }

        private const int BtnMinW = 110;   // fits "EQUIPPED" at common DPIs
        private const int BtnMinH = 36;

        private void StyleGameButton(Button b, Color baseColor, Color? textColor = null)
        {
            // autosize so long text never clips
            b.AutoSize = true;
            b.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            b.MinimumSize = new Size(BtnMinW, BtnMinH);
            b.Padding = new Padding(12, 6, 12, 6);

            b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderSize = 0;
            b.BackColor = baseColor;
            b.FlatAppearance.MouseOverBackColor = ControlPaint.Light(baseColor);
            b.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(baseColor);
            b.ForeColor = textColor ?? Color.Black;
            b.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            b.Cursor = Cursors.Hand;
        }

        private void SetEquipped(Button b)
        {
            b.Enabled = false;
            b.Text = "EQUIPPED";
            StyleGameButton(b, Color.Silver, Color.Black);
        }

        private void SetEquip(Button b)
        {
            b.Enabled = true;
            b.Text = "EQUIP";
            StyleGameButton(b, Color.Gold, Color.Black);
        }

        private void SetBuy(Button b)
        {
            b.Enabled = true;
            b.Text = "BUY";
            StyleGameButton(b, Color.Gold, Color.Black);
        }


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

        private void UpdateBgButtonsUi()
        {
            var equippedId = FrmGame.Instance?.CurrentBackgroundId ?? "";

            // Saul and LeBron buttons
            UpdateSingleBgButton(goodmanbtn, "saul", equippedId);
            UpdateSingleBgButton(button2, "lebron", equippedId);

            // Default green isn't a purchase — always show "Default"
            btnResetBg.Text = "Default";
            StyleGameButton(btnResetBg, Color.ForestGreen, Color.White);
        }

        // (Designer-click stubs, safe to keep empty if wired)
        private void bglbl_Click(object sender, EventArgs e) { }
        private void storelbl_Click(object sender, EventArgs e) { }
        private void Sclbl_Click(object sender, EventArgs e) { }
    }
}
