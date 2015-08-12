using System;
using MV.WorldObject;
using UnityEngine;

public class MVGUIDebriefingTeam : MVGUIDebriefing
{
	[SerializeField]
	protected UXText winnerName;

	[SerializeField]
	protected UXText winnerValue;

	[SerializeField]
	private UXPlane teamIcon;

	[SerializeField]
	private Material teamBlue;

	[SerializeField]
	private Material teamRed;

	[SerializeField]
	private Material teamYellow;

	[SerializeField]
	private Material teamGreen;

	public void SetWinValue(string winValue)
	{
		winnerValue.Text = winValue;
	}

	public void SetTeam(MVTeam team)
	{
		switch (team)
		{
		case MVTeam.Blue:
			teamIcon.SetMaterial(teamBlue);
			break;
		case MVTeam.Red:
			teamIcon.SetMaterial(teamRed);
			break;
		case MVTeam.Green:
			teamIcon.SetMaterial(teamGreen);
			break;
		case MVTeam.Yellow:
			teamIcon.SetMaterial(teamYellow);
			break;
		default:
			throw new Exception("Unknown team");
		}
	}

	private void Update()
	{
	}

	private void Tests()
	{
		if (MVInputWrapper.DebugGetKeyUp(KeyCode.B))
		{
			SetTeam(MVTeam.Blue);
			SetWinValue(203.ToString());
		}
	}
}
