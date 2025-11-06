using System;
using System.Windows.Forms;

namespace CrazySolitaire
{
    /// <summary>
    /// Contrack for a small, self-contained "random event" that the game can trigger.
    /// Implemenations should be short-lived visual or UX effects that can run and then
    /// notify completion.
    /// </summary>
    public interface IRandomEvent
    {
        // Unique indentifier for the event (used by selection/cooldown logic).
        string Id { get; }

        // Player-facing label (used in simple UI like toasts).
        string DisplayName { get; }

        // Whether the event is helpful to the player.
        bool IsHelpful { get; }

        /// <summary>
        /// If true, the event should temporarily block normal input white it runs.
        /// Non-blocking events should leave the game interactive.
        /// </summary>
        bool IsBlocking { get; }

        /// <summary>
        /// Relative chance of being chosen compared to other events.
        /// </summary>
        int Weight { get; set; }

        // Optional per-event cooldown. If set, the event won't be eligible again
        // until the time has passed since cref = "LastTriggeredUtc"
        TimeSpan? Cooldown { get; }

        // When this event last successfully fired (UTC), used for cooldown checks.
        DateTime LastTriggeredUtc { get; set; }

        // Extra guard for eligibility beyond cooldown/recency
        bool CanTrigger(IGameApi game);

        /// <summary>
        /// Starts the event. Implementations should perform their effects and invoke
        /// paramref name="onCompleted" exactly once when finished.
        /// </summary>
        /// <param name="mainForm">Form: host form for any UI.</param>
        /// <param name="game">IGame: game API instance</param>
        /// <param name="onCompleted">Action: callback to run when done.</param>
        void Start(System.Windows.Forms.Form mainForm, IGameApi game, System.Action onCompleted);

        // Emergency cleanup hook. Should stop timers and remove transient UI if active.
        // Safe to call even if the event isn't running.
        void Cancel();
    }

    /// <summary>
    /// Convenience base class with defaults for simple events.
    /// Most events only need to override the abstact members and optionally cref = "Cooldown"
    /// </summary>
    public abstract class RandomEventBase : IRandomEvent
    {
        public abstract string Id { get; }
        public abstract string DisplayName { get; }
        public abstract bool IsHelpful { get; }
        public abstract bool IsBlocking { get; }
        public int Weight { get; set; } = 10;
        public virtual TimeSpan? Cooldown => TimeSpan.FromSeconds(30);
        public DateTime LastTriggeredUtc { get; set; } = DateTime.MinValue;

        public virtual bool CanTrigger(IGameApi game) => true;
        public abstract void Start(System.Windows.Forms.Form mainForm, IGameApi game, System.Action onCompleted);
        public virtual void Cancel() { }
    }
}
