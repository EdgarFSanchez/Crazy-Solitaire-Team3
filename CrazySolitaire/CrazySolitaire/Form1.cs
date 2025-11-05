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

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            StyleGameButton(goodmanbtn, Color.Gold, Color.Black);
            StyleGameButton(button2, Color.Gold, Color.Black);
            StyleGameButton(button3, Color.MediumPurple, Color.White);
            btnResetBg.Text = "Default";
            StyleGameButton(btnResetBg, Color.ForestGreen, Color.White);

            UpdateSocialCreditText();
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

        private void HandleScoreChanged(int _) => UpdateSocialCreditText();
        private void HandleMultiplerChanged(int _) => UpdateSocialCreditText();

        private void UpdateSocialCreditText()
        {
            if (InvokeRequired) { BeginInvoke(new Action(UpdateSocialCreditText)); return; }
            Sclbl.Text = $"Social Credit: {ScoreManager.Score}";
        }

        private void goodmanbtn_Click(object sender, EventArgs e)
        {
            HandleBackgroundAction("saul", Properties.Resources.latest1, price: 500);
        }

        private void Lebronbtn_Click(object sender, EventArgs e)
        {
            HandleBackgroundAction("lebron", Properties.Resources.hq720, price: 500);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            HandleBackgroundAction("saul", Properties.Resources.latest1, price: 1500);
        }

        private void btnResetBg_Click(object sender, EventArgs e)
        {
            FrmGame.Instance?.ClearBackgroundToDefaultGreen();
            UpdateBgButtonsUi();
            MessageBox.Show("Background reset to default green.");
        }

        private void HandleBackgroundAction(string id, Image img, int price)
        {
            if (!StoreSession.IsBackgroundOwned(id))
            {
                if (!StoreSession.TryPurchaseBackground(id, price, out var error))
                {
                    MessageBox.Show(error, "Store", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }

            FrmGame.Instance?.ApplyBackgroundWithId(img, id);
            UpdateSocialCreditText();
            UpdateBgButtonsUi();
            MessageBox.Show(StoreSession.IsBackgroundOwned(id) ? "Equipped!" : "Purchased & Equipped!");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            const int price = 0;

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

        private void StyleGameButton(Button b, Color baseColor, Color? textColor = null)
        {
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
            UpdateSingleBgButton(goodmanbtn, "saul", equippedId);
            UpdateSingleBgButton(button2, "lebron", equippedId);

            btnResetBg.Text = "Default";
            StyleGameButton(btnResetBg, Color.ForestGreen, Color.White);
        }

        private void bglbl_Click(object sender, EventArgs e) { }
        private void storelbl_Click(object sender, EventArgs e) { }
        private void Sclbl_Click(object sender, EventArgs e) { }
    }
}
