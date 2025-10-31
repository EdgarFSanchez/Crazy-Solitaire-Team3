namespace CrazySolitaire
{
	public static class ScoreManager
	{
		public static int Score { get; private set; } = 0;
        public static int Multiplier { get; private set; } = 1;

        // Event that fires whenever the score changes
        public static event Action<int> OnScoreChanged;			// Event that fires whenever the score changes
        public static event Action<int> OnMultiplierChanged;	// Event that fires whenever the multiplier changes

        public static void AddPoints(int points)
		{
			Score += points * Multiplier;
			OnScoreChanged?.Invoke(Score);						// Notify listeners that the score has changed
        }

		/// <summary>
		/// Substracts points from the player's score.
		/// Ensures the score never drops below zero.
		/// </summary>
		/// <param name="points">The number of points to subtract</param>
		public static void SubtractPoints(int points)
		{
			Score -= points;
			if (Score < 0) Score = 0;
			OnScoreChanged?.Invoke(Score);
        }

        public static void SetMultiplier(int newMultiplier)
        {
            Multiplier = newMultiplier;
            OnMultiplierChanged?.Invoke(Multiplier);			// Notify listeners that the multiplier has changed
        }

        public static void Reset()
		{
			Score = 0;
			Multiplier = 1;
			OnScoreChanged?.Invoke(Score);
            OnMultiplierChanged?.Invoke(Multiplier);
        }
	}
}
