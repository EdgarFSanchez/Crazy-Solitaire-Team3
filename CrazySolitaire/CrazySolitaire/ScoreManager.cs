namespace CrazySolitaire
{
    /// <summary>
    /// Manages al scoring logic for the Crazy Solitare Game
    /// Handles adding, subtracting, and multiplying points
    /// </summary>
    public static class ScoreManager
    {

        /// <summary>
        /// Current total score for the player
        /// </summary>
        public static int Score { get; private set; } = 0;

        /// <summary>
        /// Current score multiplier, by default it's 1
        /// </summary>
        public static int Multiplier { get; private set; } = 1;

        /// <summary>
        /// Checks to see if player has purchased a double-points modifier.
        /// Persists for the entire round
        /// </summary>
        public static bool PermanentDoubleCredits { get; private set; } = false;

        // Events notifying the UI and other systems when values change
        public static event Action<int> OnScoreChanged;
        public static event Action<int> OnMultiplierChanged;


        public static void AddPoints(int points)
        {
            Score += points * Multiplier;
            OnScoreChanged?.Invoke(Score);
        }

        public static void SubtractPoints(int points)
        {
            Score -= points;
            if (Score < 0) Score = 0;
            OnScoreChanged?.Invoke(Score);
        }

        public static void SetMultiplier(int newMultiplier)
        {
            Multiplier = newMultiplier;
            OnMultiplierChanged?.Invoke(Multiplier);
        }

         /// <summary>
         /// Enables permanent 2x points for the round
         /// </summary>
        public static void PurchaseDoubleCredits()
        {
            PermanentDoubleCredits = true;                      // Mark upgrade as purchased    

            // Ensure active multiplier is at least 2x
            if (Multiplier < 2) SetMultiplier(2);
        }

        /// <summary>
        /// Resets only the player's score to 0
        /// Multiplier remains unchanged
        /// </summary>
        public static void ResetScoreOnly()
        {
            Score = 0;
            OnScoreChanged?.Invoke(Score);
        }

        /// <summary>
        /// Fully resets score and multiplier. 
        /// Keeps the permanent 2x multiplier active if purchased.
        /// </summary>
        public static void Reset()
        {
            Score = 0;
            Multiplier = PermanentDoubleCredits ? 2 : 1;        // Keep multiplier if the player bought it
            OnScoreChanged?.Invoke(Score);
            OnMultiplierChanged?.Invoke(Multiplier);
        }
    }
}
