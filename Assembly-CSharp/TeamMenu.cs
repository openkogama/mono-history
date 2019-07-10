using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TeamMenu : MonoBehaviour
{
	[SerializeField]
	private VerticalLayoutGroup teamLayoutGroup;

	[SerializeField]
	private TeamSelectButton teamPrefab;

	public void Start()
	{
		List<TeamData> teamDatas = MVGameControllerBase.Game.TeamManager.GetTeamDatas(GameStatCounterType.Kill);
		List<TeamData> list = teamDatas.OrderBy((TeamData teamData) => teamData.playersCount).ToList();
		for (int num = 0; num < list.Count; num++)
		{
			TeamSelectButton teamSelectButton = Object.Instantiate(teamPrefab);
			teamSelectButton.Initialize(list[num]);
			teamSelectButton.transform.SetParent(teamLayoutGroup.transform, worldPositionStays: false);
		}
	}

	private void Update()
	{
		if (MVGameControllerBase.MainCameraManager.BlueModeEnabled)
		{
			MVGameControllerBase.MainCameraManager.CamMaskMode = MaskMode.SkyBoxOnly;
		}
	}

	private void OnDestroy()
	{
		if (!WinningConditionControl.TryGetPrioritizedWinCondition(out var _))
		{
			MVGameControllerBase.MainCameraManager.CamMaskMode = MaskMode.Default;
		}
	}
}
