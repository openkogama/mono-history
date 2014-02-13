using MV.Common;
using MV.WorldObject;

public class MVGUIWinnerLine : UXLine
{
	public UXText winnerName;

	public UXText score;

	private int maxNameChars = 13;

	public void BuildLine(int num, WinnerListNode winnerNode, MVWinningCondition winningType, bool isTeamGame)
	{
		string empty = string.Empty;
		if (isTeamGame)
		{
			MVTeam actorNr = (MVTeam)winnerNode.actorNr;
			empty = string.Concat(actorNr, " team");
		}
		else
		{
			MVPlayer mVPlayer = MVGameController.Instance.Game.Players[winnerNode.actorNr];
			empty = mVPlayer.Username;
		}
		if (empty.Length >= maxNameChars)
		{
			empty = empty.Substring(0, maxNameChars) + "...";
		}
		winnerName.Text = "#" + num + " - " + empty;
		if (winningType == MVWinningCondition.MostKills)
		{
			score.Text = "Score: " + winnerNode.data;
		}
		else
		{
			score.Text = FormatWinnerTime(winnerNode.data);
		}
	}

	private string FormatWinnerTime(int timeInMS)
	{
		float num = (float)timeInMS / 1000f;
		return $"{num / 60f:00}:{num % 60f:00}.{(float)timeInMS % 1000f:000}";
	}
}
