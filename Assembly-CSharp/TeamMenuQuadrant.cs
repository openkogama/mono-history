using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TeamMenuQuadrant : MonoBehaviour
{
	private TeamData teamData;

	[SerializeField]
	private Text players;

	[SerializeField]
	private Text score;

	[SerializeField]
	private Image header;

	[SerializeField]
	private Image buttonImage;

	[SerializeField]
	private Image teamImage;

	public void Initialize(TeamData teamData)
	{
		this.teamData = teamData;
		players.text = teamData.playersCount.ToString();
		score.text = teamData.score.ToString();
		Styles.SetStyle(header, teamData.team);
		Styles.SetStyle(buttonImage, teamData.team);
		Styles.TeamToSprite(teamImage, teamData.team);
	}

	public void TeamSelected()
	{
		MVGameControllerBase.Game.SetTeam(teamData.team);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
	}
}
