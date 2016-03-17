using System.Collections.Generic;
using UnityEngine;

public class TeamMenu : MonoBehaviour
{
	[SerializeField]
	private TeamMenuQuadrant teamMenuQuadrant;

	[SerializeField]
	private RectTransform teamsRoot;

	public void Start()
	{
		List<TeamData> teamDatas = MVGameControllerBase.Game.TeamManager.GetTeamDatas(GameStatCounterType.Kill);
		foreach (TeamData item in teamDatas)
		{
			TeamMenuQuadrant teamMenuQuadrant = Object.Instantiate(this.teamMenuQuadrant);
			teamMenuQuadrant.Initialize(item);
			teamMenuQuadrant.transform.SetParent(teamsRoot, worldPositionStays: false);
		}
	}
}
