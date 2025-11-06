using System;

namespace CrazySolitaire
{
    /// <summary>
    /// Small surface that random events use to talk to the game.
    /// </summary>
    public interface IGameApi
    {
        /// <summary>
        /// True if the game is in the middle of an animation where starting a new event
        /// would look pretty bad. Event manager can check this before firing.
        /// </summary>
        bool IsBusyAnimating { get; }

        /// <summary>
        /// Temporarily disable player input if needed.
        /// </summary>
        void PausePlayerInput();

        /// <summary>
        /// Re-enable player input if it was paused
        /// </summary>
        void ResumePlayerInput();

        /// <summary>
        /// Adjust the player's score by a delta (positive or negative).
        /// Used by capcha for the reward/penalty.
        /// </summary>
        /// <param name="points">int: amount to add (use negative to subtract)</param>
        void AddScore(int points);
    }
}
