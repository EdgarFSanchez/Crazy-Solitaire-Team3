namespace CrazySolitaire
{
    public static class ScoreManager
    {
        public static int Score { get; private set; } = 0;
        public static int Multiplier { get; private set; } = 1;

        // NEW: tracks if 2x was bought (session-wide)
        public static bool PermanentDoubleCredits { get; private set; } = false;

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

        // NEW: buy-and-keep 2x for the whole session
        public static void PurchaseDoubleCredits()
        {
            PermanentDoubleCredits = true;
            if (Multiplier < 2) SetMultiplier(2);
        }

        // NEW: reset only the score (keep multiplier as-is)
        public static void ResetScoreOnly()
        {
            Score = 0;
            OnScoreChanged?.Invoke(Score);
        }

        // Keep this if other places rely on Reset(); respect permanent 2x.
        public static void Reset()
        {
            Score = 0;
            Multiplier = PermanentDoubleCredits ? 2 : 1;
            OnScoreChanged?.Invoke(Score);
            OnMultiplierChanged?.Invoke(Multiplier);
        }
    }
}
