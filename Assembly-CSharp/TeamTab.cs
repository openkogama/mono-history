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

	public void Initialize(MVTeam team, GameStatCounterType statToDisplay)
	{
		if (statToDisplay != GameStatCounterType.None && team != MVTeam.None)
		{
			score.gameObject.SetActive(value: true);
		}
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
		Color teamColor2 = Styles.GetTeamColor(team, darkTeam: true);
		for (int i = 0; i < teamColoredImages.Count; i++)
		{
			teamColoredImages[i].color = teamColor;
		}
		for (int j = 0; j < darkTeamColoredImages.Count; j++)
		{
			darkTeamColoredImages[j].color = teamColor2;
		}
		Styles.TeamToSprite(teamImage, team);
	}
}
