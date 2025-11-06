using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CrazySolitaire
{
    /// <summary>
    /// Schedules and runs small random events based on draw count + simple cooldowns.
    /// </summary>
    public class RandomEventManager
    {
        private readonly Random _rng = new();
        private readonly List<RandomEventBase> _events = new();
        private readonly IGameApi _game;
        private readonly Form _mainForm;

        // Internal scheduling state
        private int _drawsSinceLastEvent = 0;
        private int _nextThreshold;
        private bool _active;

        /// <summary>
        /// Lower bound for the next draw threshold.
        /// </summary>
        public int MinDrawsBetweenEvents { get; set; } = 3;

        /// <summary>
        /// Upper bound for the next draw threshold.
        /// </summary>
        public int MaxDrawsBetweenEvents { get; set; } = 6;

        /// <summary>
        /// Chance to fire when the threshold is hit (0–100).
        /// </summary>
        public int PercentChanceWhenThresholdHit { get; set; } = 70;

        /// <summary>
        /// Hard cap on total events this game.
        /// </summary>
        public int MaxEventsPerGame { get; set; } = 6;

        /// <summary>
        /// Per-event cap this game.
        /// </summary>
        public int MaxPerEventPerGame { get; set; } = 3;

        /// <summary>
        /// Global spacing between any two events (seconds).
        /// </summary>
        public int MinSecondsBetweenEvents { get; set; } = 7;

        private int _totalEventsFired = 0;
        private DateTime _lastEventUtc = DateTime.MinValue;
        private readonly Dictionary<string, int> _perEventCounts = new();

        // Light anti-repeat: block only the immediately previous event
        private readonly Queue<string> _recent = new();
        private const int RecentWindow = 1;

        /// <summary>
        /// Binds the manager to the UI thread (form) and a small game API.
        /// </summary>
        public RandomEventManager(Form mainForm, IGameApi game)
        {
            _game = game;
            _mainForm = mainForm;
            ResetThreshold();
        }

        /// <summary>
        /// Makes an event eligible for selection.
        /// </summary>
        public void Register(RandomEventBase e)
        {
            _events.Add(e);
            if (!_perEventCounts.ContainsKey(e.Id))
                _perEventCounts[e.Id] = 0;
        }

        /// <summary>
        /// Call after a draw. May schedule and run an event if conditions line up.
        /// </summary>
        public void OnCardDrawn()
        {
            if (_active) return;
            if (_totalEventsFired >= MaxEventsPerGame) return;

            _drawsSinceLastEvent++;
            if (_drawsSinceLastEvent < _nextThreshold) return;
            if (_game.IsBusyAnimating) return;

            var sinceLast = (DateTime.UtcNow - _lastEventUtc).TotalSeconds;
            if (_lastEventUtc != DateTime.MinValue && sinceLast < MinSecondsBetweenEvents) return;

            // Threshold hit — roll to decide if we fire now
            if (_rng.Next(0, 100) >= PercentChanceWhenThresholdHit)
            {
                _nextThreshold += _rng.Next(2, 4);
                return;
            }

            var candidate = PickEvent();
            if (candidate == null)
            {
                _nextThreshold += _rng.Next(2, 4);
                return;
            }

            _active = true;
            _drawsSinceLastEvent = 0;
            ResetThreshold();

            if (candidate.IsBlocking) _game.PausePlayerInput();

            candidate.LastTriggeredUtc = DateTime.UtcNow;
            _perEventCounts[candidate.Id] = _perEventCounts.GetValueOrDefault(candidate.Id) + 1;
            _totalEventsFired++;
            _lastEventUtc = candidate.LastTriggeredUtc;

            _recent.Enqueue(candidate.Id);
            while (_recent.Count > RecentWindow) _recent.Dequeue();

            void done()
            {
                if (candidate.IsBlocking) _game.ResumePlayerInput();
                _active = false;
            }

            // Start on UI thread
            if (_mainForm.InvokeRequired)
                _mainForm.BeginInvoke(new Action(() => candidate.Start(_mainForm, _game, done)));
            else
                candidate.Start(_mainForm, _game, done);
        }

        // Weighted pick among currently eligible events.
        private RandomEventBase? PickEvent()
        {
            var now = DateTime.UtcNow;

            var eligible = _events.Where(e =>
            {
                _perEventCounts.TryGetValue(e.Id, out var count);
                if (count >= MaxPerEventPerGame) return false;
                if (_recent.Contains(e.Id)) return false;
                if (e.Cooldown.HasValue && now - e.LastTriggeredUtc < e.Cooldown.Value) return false;
                return e.CanTrigger(_game);
            }).ToList();

            if (eligible.Count == 0) return null;

            int total = eligible.Sum(e => Math.Max(1, e.Weight));
            int roll = _rng.Next(1, total + 1);
            int running = 0;

            foreach (var e in eligible)
            {
                running += Math.Max(1, e.Weight);
                if (roll <= running) return e;
            }
            return eligible[^1];
        }

        // Choose a fresh draw threshold within the configured range.
        private void ResetThreshold()
        {
            _nextThreshold = _rng.Next(MinDrawsBetweenEvents, MaxDrawsBetweenEvents + 1);
        }

        /// <summary>
        /// Stops any active event and restores input.
        /// </summary>
        public void CancelActiveEvents()
        {
            foreach (var e in _events) e.Cancel();
            _active = false;
            _game.ResumePlayerInput();
        }

        /// <summary>
        /// Clears counters for a new game; registered events remain.
        /// </summary>
        public void ResetForNewGame()
        {
            _totalEventsFired = 0;
            _lastEventUtc = DateTime.MinValue;
            _perEventCounts.Clear();
            foreach (var e in _events) _perEventCounts[e.Id] = 0;
            _recent.Clear();
            _drawsSinceLastEvent = 0;
            _active = false;
            ResetThreshold();
        }
    }

    // Small helper for older targets.
    static class DictExt
    {
        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue defaultValue = default!)
        {
            return dict.TryGetValue(key, out var v) ? v : defaultValue;
        }
    }
}
