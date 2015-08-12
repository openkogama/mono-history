using MV.WorldObject;
using UnityEngine;

public class MVGUITeamJoinButton : MonoBehaviour
{
	public UXText joinText;

	private MVTeam team;

	public void InitializeJoinButton(MVTeam team, MVGUITeamList playerList)
	{
		this.team = team;
		joinText.ShadowColor = GetShadowColor(playerList);
		GetComponent<Renderer>().material.SetColor("_Color", GetColor(playerList));
	}

	private Color GetColor(MVGUITeamList playerList)
	{
		Color result = Color.gray;
		if (team == MVTeam.Blue)
		{
			result = playerList.blueTeamColor;
		}
		else if (team == MVTeam.Red)
		{
			result = playerList.redTeamColor;
		}
		else if (team == MVTeam.Green)
		{
			result = playerList.greenTeamColor;
		}
		else if (team == MVTeam.Yellow)
		{
			result = playerList.yellowTeamColor;
		}
		return result;
	}

	private Color GetShadowColor(MVGUITeamList playerList)
	{
		Color result = Color.gray;
		if (team == MVTeam.Blue)
		{
			result = playerList.blueHeaderTextColor;
		}
		else if (team == MVTeam.Red)
		{
			result = playerList.redHeaderTextColor;
		}
		else if (team == MVTeam.Green)
		{
			result = playerList.greenHeaderTextColor;
		}
		else if (team == MVTeam.Yellow)
		{
			result = playerList.yellowHeaderTextColor;
		}
		return result;
	}
}
