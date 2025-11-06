using System;
using System.Drawing;
using System.Windows.Forms;

namespace CrazySolitaire
{
    /// <summary>
    /// Quick window shake: nudges the form around its starting position for about 0.9 s.
    /// Amplitude tapers over time, then the form snaps back to the original location.
    /// </summary>
    public class ScreenShakeEvent : RandomEventBase
    {
        public override string Id => "screen_shake";
        public override string DisplayName => "Rumble!";
        public override bool IsHelpful => false;
        public override bool IsBlocking => false;
        public override TimeSpan? Cooldown => TimeSpan.FromSeconds(20);

        private System.Windows.Forms.Timer? _timer;
        private Point _origin;   // form location before shaking starts
        private int _ticks;      // number of timer ticks since start
        private readonly Random _rng = new();

        public override void Start(System.Windows.Forms.Form mainForm, IGameApi game, System.Action onCompleted)
        {
            _origin = mainForm.Location;
            _ticks = 0;

            const int durationMs = 900; // total shake duration
            const int interval = 15;  // timer interval in ms

            _timer = new System.Windows.Forms.Timer { Interval = interval };
            _timer.Tick += (s, e) =>
            {
                _ticks++;

                // Linear falloff from 1 to 0 across the duration
                double t = 1.0 - Math.Min(1.0, (_ticks * interval) / (double)durationMs);

                // Start around 12 px and decay toward 1 px
                int amp = Math.Max(1, (int)(t * 12));

                // Random offset around the origin
                int dx = _rng.Next(-amp, amp + 1);
                int dy = _rng.Next(-amp, amp + 1);
                mainForm.Location = new Point(_origin.X + dx, _origin.Y + dy);

                // Stop and restore when time is up
                if (_ticks * interval >= durationMs)
                {
                    _timer!.Stop();
                    mainForm.Location = _origin;
                    _timer.Dispose();
                    _timer = null;
                    onCompleted();
                }
            };
            _timer.Start();
        }

        /// <summary>
        /// Stop the timer if active and clear references.
        /// </summary>
        public override void Cancel()
        {
            if (_timer is null) return;
            _timer.Stop();
            _timer.Dispose();
            _timer = null;
        }
    }
}
