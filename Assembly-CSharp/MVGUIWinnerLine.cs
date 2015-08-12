public class MVGUIWinnerLine : UXLine
{
	public UXText winnerName;

	public UXText score;

	private int maxNameChars = 13;

	public void BuildLine(int num, int scoreCount, string entryName, GameStatCounterType gameStatCounterType)
	{
		if (entryName.Length >= maxNameChars)
		{
			entryName = entryName.Substring(0, maxNameChars) + "...";
		}
		winnerName.Text = "#" + num + " - " + entryName;
		if (gameStatCounterType == GameStatCounterType.Kill)
		{
			score.Text = "Score: " + scoreCount;
		}
		else
		{
			score.Text = FormatWinnerTime(scoreCount);
		}
	}

	private string FormatWinnerTime(int timeInMS)
	{
		float num = (float)timeInMS / 1000f;
		return $"{num / 60f:00}:{num % 60f:00}.{(float)timeInMS % 1000f:000}";
	}
}
