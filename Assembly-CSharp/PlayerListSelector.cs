using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public class PlayerListSelector : MonoBehaviour
{
	[SerializeField]
	private PlayerListSelectorButton teamSelectButtonPrefab;

	private Dictionary<MVTeam, PlayerList> playerLists;

	public void Setup(Dictionary<MVTeam, PlayerList> playerLists)
	{
		this.playerLists = playerLists;
		List<MVTeam> list = new List<MVTeam>();
		if (MVGameControllerBase.Game.TeamManager.TeamCount() == 1)
		{
			list.Add(MVTeam.None);
		}
		else
		{
			list = MVGameControllerBase.Game.TeamManager.GetTeamList();
		}
		foreach (MVTeam item in list)
		{
			PlayerListSelectorButton playerListSelectorButton = Object.Instantiate(teamSelectButtonPrefab);
			playerListSelectorButton.transform.SetParent(transform, worldPositionStays: false);
			playerListSelectorButton.gameObject.SetActive(value: true);
			playerListSelectorButton.Initialize(item);
			if (item != MVTeam.None)
			{
				MVTeam teamCallbackVal = item;
				playerListSelectorButton.button.onClick.AddListener(() =>
				{
					Debug.Log("Set active");
					SetActiveTeam(teamCallbackVal);
				});
			}
		}
	}

	public void SetActiveTeam(MVTeam team)
	{
		Debug.Log(team);
		foreach (KeyValuePair<MVTeam, PlayerList> playerList in playerLists)
		{
			playerList.Value.gameObject.SetActive(value: false);
		}
		playerLists[team].gameObject.SetActive(value: true);
		playerLists[team].transform.SetAsLastSibling();
	}
}
