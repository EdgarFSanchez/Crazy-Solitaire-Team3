using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace CrazySolitaire
{
    /// <summary>
    /// Display a small CAPTCHA overlay that ask the player to type a short code
    /// within a time limit. Rewards the player if they complete it and punishes
    /// on failure. The hotkeys we added are temporarily disabled to make sure 
    /// focus remains on the input box
    /// </summary>
    public class CaptchaEvent : RandomEventBase
    {
        // Unique identifier for the event.
        public override string Id => "captcha";

        // Player-facing name shown in UI.
        public override string DisplayName => "Type It Fast!";

        // Whether the event is generally beneficial to the player
        public override bool IsHelpful => true;

        /// <summary>
        /// Whether the event should block normal input. This event leaves the form enabled so the 
        /// cref = "Textbox" can grab focus and the user can type.
        /// </summary>
        public override bool IsBlocking => false;

        // Minimum time before this event is able to happen again.
        public override TimeSpan? Cooldown => TimeSpan.FromSeconds(45);

        private const int TimeLimitSeconds = 8;
        private const int BoxWidth = 380;
        private const int BoxHeight = 200;
        private const string Alphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";

        private Panel? _overlay;
        private System.Windows.Forms.Timer? _timer;
        private int _timeLeft;
        private string _challenge = string.Empty;
        private Label? _label;
        private TextBox? _tb;
        private readonly Random _rng = new();

        /// <summary>
        /// Displays the overlay, focuses the input box, and starts the countdown
        /// </summary>
        /// <param name="mainForm"></param>
        /// <param name="game"></param>
        /// <param name="onCompleted"></param>
        public override void Start(System.Windows.Forms.Form mainForm, IGameApi game, System.Action onCompleted)
        {
            _overlay = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(140, 0, 0, 0) };

            var inner = new Panel { Size = new Size(380, 200), BackColor = Color.White };
            inner.Location = new Point(
                (mainForm.ClientSize.Width - inner.Width) / 2,
                (mainForm.ClientSize.Height - inner.Height) / 2);

            _label = new Label { AutoSize = false, Dock = DockStyle.Top, Height = 60, TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Segoe UI", 18, FontStyle.Bold) };
            _tb = new TextBox { Dock = DockStyle.Top, Font = new Font("Consolas", 16) };
            var hint = new Label { AutoSize = false, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Segoe UI", 10), Text = "Type the code exactly. Reward if correct; penalty if wrong or time runs out." };

            inner.Controls.Add(hint);
            inner.Controls.Add(_tb);
            inner.Controls.Add(_label);
            _overlay.Controls.Add(inner);
            mainForm.Controls.Add(_overlay);
            _overlay.BringToFront();

            if (mainForm is FrmGame fg) fg.SetHotkeysSuppressed(true);

            var rng = new Random();
            const string Alphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            _challenge = new string(Enumerable.Range(0, 5).Select(_ => Alphabet[rng.Next(Alphabet.Length)]).ToArray());

            _timeLeft = 8;
            UpdateLabel();

            // Focus after layout completes
            mainForm.BeginInvoke(new Action(() =>
            {
                _tb!.Focus();
                _tb.Select();
                _tb.SelectionStart = 0;
                _tb.SelectionLength = _tb.TextLength;
            }));

            _tb.TextChanged += (_, __) =>
            {
                if (_tb!.Text.Trim().ToUpperInvariant() == _challenge)
                    Complete(game, mainForm, onCompleted, success: true);
            };

            _timer = new System.Windows.Forms.Timer { Interval = 1000 };
            _timer.Tick += (_, __) =>
            {
                _timeLeft--;
                UpdateLabel();
                if (_timeLeft <= 0) Complete(game, mainForm, onCompleted, success: false);
            };
            _timer.Start();
        }

        /// <summary>
        /// Stops timers, applies score change, removes the overlay, re-enables hotkeys, and invokes the completion callback.
        /// </summary>
        private void UpdateLabel()
        {
            if (_label != null) _label.Text = $"CAPTCHA: {_challenge}\r\nTime: {_timeLeft}s";
        }

        private void Complete(IGameApi game, System.Windows.Forms.Form mainForm, System.Action onCompleted, bool success)
        {
            _timer?.Stop();
            _timer?.Dispose();
            _timer = null;

            if (success) game.AddScore(+75);
            else game.AddScore(-50);

            if (mainForm is FrmGame fg) fg.SetHotkeysSuppressed(false);

            if (_overlay != null)
            {
                mainForm.Controls.Remove(_overlay);
                _overlay.Dispose();
                _overlay = null;
            }

            onCompleted();
        }

        /// <summary>
        /// Emergency cleanup if the event is cancelled externally.
        /// </summary>
        public override void Cancel()
        {
            _timer?.Stop();
            _timer?.Dispose();
            _timer = null;
            _overlay?.Dispose();
            _overlay = null;
        }
    }
}
