using MV.WorldObject;
using UnityEngine;
using UnityEngine.UI;

public class TeamTab : MonoBehaviour
{
	[SerializeField]
	private Text playerCount;

	[SerializeField]
	private Text score;

	[SerializeField]
	private Image teamImage;

	[SerializeField]
	private Image background;

	public void Initialize(MVTeam team)
	{
		if (team == MVTeam.None)
		{
			MVTeam team2 = MVGameControllerBase.Game.TeamManager.GetTeamList()[0];
			playerCount.text = MVGameControllerBase.Game.TeamManager.GetPlayersInTeam(team2).Count.ToString();
			score.text = MVGameControllerBase.Game.TeamManager.GetScore(team2, GameStatCounterType.Kill).ToString();
		}
		else
		{
			playerCount.text = MVGameControllerBase.Game.TeamManager.GetPlayersInTeam(team).Count.ToString();
			score.text = MVGameControllerBase.Game.TeamManager.GetScore(team, GameStatCounterType.Kill).ToString();
		}
		background.color = Styles.GetTeamColor(team);
		Styles.TeamToSprite(teamImage, team);
	}
}
