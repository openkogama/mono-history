using UnityEngine;

public class ScoreBoardController : MonoBehaviour
{
	[SerializeField]
	private ScoreBoardSingleBase scoreboardSingle;

	[SerializeField]
	private ScoreBoardTeamBase scoreboardTeam;

	public ScoreBoardBase GetInstantiatedScoreboard(WinningConditionType winConType)
	{
		if (MVGameControllerBase.Game.TeamManager.TeamCount() > 1)
		{
			return Object.Instantiate(scoreboardTeam);
		}
		return Object.Instantiate(scoreboardSingle);
	}
}
