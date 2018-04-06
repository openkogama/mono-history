using System.Collections.Generic;
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
	private List<Image> darkTeamColoredImages;

	[SerializeField]
	private List<Image> teamColoredImages;

	[SerializeField]
	private Image scoreIcon;

	public void Initialize(MVTeam team, GameStatCounterType statToDisplay)
	{
		if (team == MVTeam.None)
		{
			MVTeam team2 = MVGameControllerBase.Game.TeamManager.GetTeamList()[0];
			playerCount.text = MVGameControllerBase.Game.TeamManager.GetPlayersInTeam(team2).Count.ToString();
			score.text = WinningConditionControl.MakeIntoScoreText(MVGameControllerBase.Game.TeamManager.GetScore(team2, statToDisplay), statToDisplay);
		}
		else
		{
			playerCount.text = MVGameControllerBase.Game.TeamManager.GetPlayersInTeam(team).Count.ToString();
			score.text = WinningConditionControl.MakeIntoScoreText(MVGameControllerBase.Game.TeamManager.GetScore(team, statToDisplay), statToDisplay);
		}
		Color teamColor = Styles.GetTeamColor(team);
		Color color = teamColor - new Color(0.1f, 0.1f, 0.1f, 0f);
		for (int i = 0; i < teamColoredImages.Count; i++)
		{
			teamColoredImages[i].color = teamColor;
		}
		for (int j = 0; j < darkTeamColoredImages.Count; j++)
		{
			darkTeamColoredImages[j].color = color;
		}
		Styles.TeamToSprite(teamImage, team);
	}
}
