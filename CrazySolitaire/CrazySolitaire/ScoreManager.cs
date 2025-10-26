namespace CrazySolitaire
{
	public static class ScoreManager
	{
		public static int Score { get; private set; } = 0;

		/// <summary>
		/// Event that fires whenever the score changes
		/// The UI can use this event to automatically update the label
		/// </summary>
		public static event Action<int> OnScoreChanged;

		public static void AddPoints(int points)
		{
			Score += points;
			// Notify listeners that the score has changed
			OnScoreChanged?.Invoke(Score);
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

		public static void Reset()
		{
			Score = 0;
			OnScoreChanged?.Invoke(Score);
		}
	}
}
