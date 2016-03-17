using MV.WorldObject;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListSelectorButton : MonoBehaviour
{
	[SerializeField]
	private Image buttonImage;

	[SerializeField]
	public Button button;

	[SerializeField]
	private Text playerCount;

	[SerializeField]
	private Text score;

	[SerializeField]
	private Image teamImage;

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
		Styles.SetStyle(button, ButtonStyle.TabButton, team);
		Styles.TeamToSprite(teamImage, team);
	}
}
