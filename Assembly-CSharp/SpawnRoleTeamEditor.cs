using System;
using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SpawnRoleTeamEditor : MonoBehaviour
{
	[Serializable]
	private struct SpawnRoleTeamButton
	{
		public GameObject buttonSelected;

		public GameObject buttonNotSelected;

		public Text buttonSelectedText;

		public Text buttonNotSelectedText;

		public MVTeam team;
	}

	[SerializeField]
	private List<SpawnRoleTeamButton> teamButtons;

	private UnityAction<MVTeam> onTeamChangeCallback;

	public void Initialize(MVTeam spawnRolesTeam, UnityAction<MVTeam> onTeamChangeCallback)
	{
		this.onTeamChangeCallback = onTeamChangeCallback;
		for (int i = 0; i < teamButtons.Count; i++)
		{
			string text = MVGameControllerBase.Game.TeamManager.GetTeamNames()[teamButtons[i].team];
			teamButtons[i].buttonSelectedText.text = text;
			teamButtons[i].buttonNotSelectedText.text = text;
			bool flag = spawnRolesTeam == teamButtons[i].team;
			teamButtons[i].buttonSelected.SetActive(flag);
			teamButtons[i].buttonNotSelected.SetActive(!flag);
		}
	}

	public void SelectRedTeam()
	{
		SelectTeam(MVTeam.Red);
	}

	public void SelectBlueTeam()
	{
		SelectTeam(MVTeam.Blue);
	}

	public void SelectYellowTeam()
	{
		SelectTeam(MVTeam.Yellow);
	}

	public void SelectGreenTeam()
	{
		SelectTeam(MVTeam.Green);
	}

	private void SelectTeam(MVTeam teamSelected)
	{
		onTeamChangeCallback(teamSelected);
		for (int i = 0; i < teamButtons.Count; i++)
		{
			bool flag = teamSelected == teamButtons[i].team;
			teamButtons[i].buttonSelected.SetActive(flag);
			teamButtons[i].buttonNotSelected.SetActive(!flag);
		}
	}
}
