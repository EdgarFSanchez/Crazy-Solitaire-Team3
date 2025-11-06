using System;
using System.Drawing;
using System.Windows.Forms;

namespace CrazySolitaire
{
    /// <summary>
    /// Briefly jitters the system cursor and shows a small toast message with a countdown.
    /// Uses a ramp-up -> sustain -> ramp-down envelope and restores the curor when done.
    /// </summary>
    public class CursorShakeEvent : RandomEventBase
    {
        // Unique identifier for this event.
        public override string Id => "cursor_shake";

        // Player-facing name, shown in the toast
        public override string DisplayName => "Jitter!";

        // Not helpful, more annoying than anything, but thats what we want.
        public override bool IsHelpful => false;

        // Doesnt block the player from making moves
        public override bool IsBlocking => false;

        // Minimum time before this event is ready to be used again.
        public override TimeSpan? Cooldown => TimeSpan.FromSeconds(25);

        // Tuning
        private const int DurationMs = 5000;    // total effect duration
        private const int TickIntervalMs = 12;  // update frequency
        private const int MaxAmplitudePx = 18;  // max shake radius
        private const double RampUpEnd = 0.20;  // 0-20% for ramp up
        private const double SustainEnd = 0.85; // 20-85% to sustain
        private const int BurstPeriodMs = 120;  // burst every 120ms
        private const double BurstBoost = 1.2;  // burst amplitude multipler

        private System.Windows.Forms.Timer? _timer;
        private int _elapsedMs;
        private readonly Random _rng = new();
        private Point _lastAppliedOffset = Point.Empty;

        // Toast UI
        private Panel? _toast;
        private Label? _toastLabel;
        private int _lastShownSeconds = int.MaxValue;

        /// <summary>
        /// Starts shaking and shows a countdown toast.
        /// </summary>
        /// <param name="mainForm"></param>
        /// <param name="game"></param>
        /// <param name="onCompleted"></param>
        public override void Start(System.Windows.Forms.Form mainForm, IGameApi game, System.Action onCompleted)
        {
            _elapsedMs = 0;
            _lastAppliedOffset = Point.Empty;

            ShowToast(mainForm);        // says "Jitter! 5s"  
            UpdateToast(DurationMs);    // initialize label

            _timer = new System.Windows.Forms.Timer { Interval = TickIntervalMs };
            _timer.Tick += (s, e) =>
            {
                _elapsedMs += TickIntervalMs;

                // Recover the player's normal position by subtracting our last offset.
                var screenPos = Control.MousePosition;
                var naturalPos = new Point(screenPos.X - _lastAppliedOffset.X, screenPos.Y - _lastAppliedOffset.Y);

                // Envelope amount
                double t = Math.Min(1.0, _elapsedMs / (double)DurationMs);
                double ampFactor =
                    (t < RampUpEnd) ? (t / RampUpEnd) :
                    (t < SustainEnd) ? 1.0 :
                    Math.Max(0, 1 - (t - SustainEnd) / (1 - SustainEnd));

                int amp = Math.Max(2, (int)(ampFactor * MaxAmplitudePx));
                if ((_elapsedMs / BurstPeriodMs) % 2 == 0) amp = (int)(amp * BurstBoost);

                int dx = _rng.Next(-amp, amp + 1);
                int dy = _rng.Next(-amp, amp + 1);
                var newPos = new Point(naturalPos.X + dx, naturalPos.Y + dy);

                // Clamp within current screen
                var bounds = Screen.FromPoint(naturalPos).Bounds;
                newPos.X = Math.Max(bounds.Left, Math.Min(bounds.Right - 1, newPos.X));
                newPos.Y = Math.Max(bounds.Top, Math.Min(bounds.Bottom - 1, newPos.Y));

                _lastAppliedOffset = new Point(newPos.X - naturalPos.X, newPos.Y - naturalPos.Y);
                Cursor.Position = newPos;

                UpdateToast(DurationMs - _elapsedMs);

                if (_elapsedMs >= DurationMs)
                {
                    _timer!.Stop();
                    _timer.Dispose();
                    _timer = null;

                    try { Cursor.Position = naturalPos; } catch { /* ignore */ }
                    RemoveToast(mainForm);
                    onCompleted();
                }
            };
            _timer.Start();
        }

        /// <summary>
        /// Stops shaking and removes the toast message, restoring the cursor to normal
        /// </summary>
        public override void Cancel()
        {
            if (_timer is null) return;

            _timer.Stop();
            _timer.Dispose();
            _timer = null;

            try
            {
                var screenPos = Control.MousePosition;
                var naturalPos = new Point(screenPos.X - _lastAppliedOffset.X, screenPos.Y - _lastAppliedOffset.Y);
                Cursor.Position = naturalPos;
            }
            catch { /* ignore */ }

            _lastAppliedOffset = Point.Empty;

            if (_toast is not null && _toast.Parent is Form f)
                RemoveToast(f);
        }

        private void ShowToast(Form mainForm)
        {
            _toast = new Panel
            {
                Size = new Size(220, 36),
                BackColor = Color.Black
            };
            _toastLabel = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            _toast.Controls.Add(_toastLabel);

            int x = (mainForm.ClientSize.Width - _toast.Width) / 2;
            int y = 8;
            _toast.Location = new Point(Math.Max(0, x), y);
            _toast.Anchor = AnchorStyles.Top;

            mainForm.Controls.Add(_toast);
            _toast.BringToFront();
        }

        private void UpdateToast(int msRemaining)
        {
            if (_toastLabel == null) return;
            int secs = Math.Max(0, (int)Math.Ceiling(msRemaining / 1000.0));
            if (secs == _lastShownSeconds) return; // avoid needless updates
            _lastShownSeconds = secs;
            _toastLabel.Text = $"{DisplayName}: {secs}s";
        }

        private void RemoveToast(Form mainForm)
        {
            if (_toast != null)
            {
                mainForm.Controls.Remove(_toast);
                _toast.Dispose();
                _toast = null;
                _toastLabel = null;
                _lastShownSeconds = int.MaxValue;
            }
        }
    }
}
