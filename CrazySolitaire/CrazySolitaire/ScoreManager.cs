namespace CrazySolitaire
{
	public static class ScoreManager
	{
		public static int Score { get; private set; } = 0;
		public static event Action<int> OnScoreChanged;

		public static void AddPoints(int points)
		{
			Score += points;
			OnScoreChanged?.Invoke(Score);
		}

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
