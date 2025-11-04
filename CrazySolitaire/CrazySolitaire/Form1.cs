using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
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

            UpdateSocialCreditText();

            ScoreManager.OnScoreChanged += HandleScoreChanged;
            ScoreManager.OnMultiplierChanged += HandleMultiplerChanged;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            ScoreManager.OnScoreChanged -= HandleScoreChanged;
            ScoreManager.OnMultiplierChanged -= HandleMultiplerChanged;
            base.OnFormClosed(e);
        }

        private void HandleScoreChanged(int _)
        {
            UpdateSocialCreditText();
        }

        private void HandleMultiplerChanged(int _)
        {
            UpdateSocialCreditText();
        }

        private void UpdateSocialCreditText()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(UpdateSocialCreditText));
                return;
            }
            lblScore.Text = $"Social Credit: {ScoreManager.Score}";
        }

        private void goodmanbtn_Click(object sender, EventArgs e)
        {
            // Saul Goodman (latest1)
            BuyAndEquipBackground(Properties.Resources.latest1, price: 0, idForSettings: "saul");
        }

        private void Lebronbtn_Click(object sender, EventArgs e)
        {
            // LeBron (use hq720 if that's the actual resource name; otherwise replace with your resource key)
            BuyAndEquipBackground(Properties.Resources.hq720, price: 0, idForSettings: "lebron");
        }


        private void pictureBox1_Click(object sender, EventArgs e)
        {
            const int price = 200;
            if (ScoreManager.Score < price)
            {
                MessageBox.Show($"You need {price} Social Credit to buy this background");
                return;

                ScoreManager.SubtractPoints(price);
                FrmGame.Instance?.ApplyBackground(Properties.Resources.latest1);
                MessageBox.Show("Background purchased and equipped!");
            }

        }
        private void BuyAndEquipBackground(Image img, int price, string idForSettings)
        {
            if (ScoreManager.Score < price)
            {
                MessageBox.Show($"You need {price} Social Credit to buy this background.");
                return;
            }

            ScoreManager.SubtractPoints(price);
            FrmGame.Instance?.ApplyBackground(img);          // applies immediately
            UpdateSocialCreditText();                        // refresh label

            Properties.Settings.Default.SelectedBackgroundId = idForSettings;
            Properties.Settings.Default.Save();

            MessageBox.Show("Background purchased and equipped!");
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }

}
