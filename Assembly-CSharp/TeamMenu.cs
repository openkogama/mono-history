using System.Collections.Generic;
using System.Linq;
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
		List<TeamData> list = teamDatas.OrderBy((TeamData teamData) => teamData.playersCount).ToList();
		for (int num = 0; num < list.Count; num++)
		{
			TeamMenuQuadrant teamMenuQuadrant = Object.Instantiate(this.teamMenuQuadrant);
			teamMenuQuadrant.Initialize(list[num]);
			teamMenuQuadrant.transform.SetParent(teamsRoot, worldPositionStays: false);
		}
	}
}
