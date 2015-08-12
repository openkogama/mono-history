using System;
using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public class MVGUITeamList : UXScrollableBox
{
	public Color blueTeamColor = new Color(0.14f, 0.45f, 0.7f, 1f);

	public Color blueHeaderTextColor;

	public Color redTeamColor = new Color(0.67f, 0.07f, 0f, 1f);

	public Color redHeaderTextColor;

	public Color greenTeamColor = new Color(0.55f, 0.745f, 0.28f, 1f);

	public Color greenHeaderTextColor;

	public Color yellowTeamColor = new Color(1f, 0.6f, 0f, 1f);

	public Color yellowHeaderTextColor;

	public Color noneTeamColor = new Color(0.31f, 0.31f, 0.4f, 1f);

	public List<UXText> uiTexts;

	public UXText playerNumber;

	public UXText teamScore;

	public UXPlane header;

	private MVTeam team;

	private MVTeamManager teamManager;

	public void InitializeTeamList(MVTeam team)
	{
		this.team = team;
		teamManager = MVGameController.Game.TeamManager;
		MVNetworkGame game = MVGameController.Game;
		game.onPlayerListChanged = (MVNetworkGame.OnPlayerListChangedDelegate)Delegate.Combine(game.onPlayerListChanged, new MVNetworkGame.OnPlayerListChangedDelegate(UpdatePlayerNumber));
		UpdatePlayerNumber();
		ColorizeHeader();
	}

	public Vector2 GetFullSize()
	{
		return new Vector2(Width + 1f, Height + 2f);
	}

	private void ColorizeHeader()
	{
		if (teamManager.TeamCount() == 1)
		{
			ColorizeHeader(noneTeamColor);
		}
		else if (team == MVTeam.Blue)
		{
			ColorizeHeader(blueTeamColor);
			ColorizeHeaderText(blueHeaderTextColor);
		}
		else if (team == MVTeam.Red)
		{
			ColorizeHeader(redTeamColor);
			ColorizeHeaderText(redHeaderTextColor);
		}
		else if (team == MVTeam.Green)
		{
			ColorizeHeader(greenTeamColor);
			ColorizeHeaderText(greenHeaderTextColor);
		}
		else if (team == MVTeam.Yellow)
		{
			ColorizeHeader(yellowTeamColor);
			ColorizeHeaderText(yellowHeaderTextColor);
		}
	}

	private void ColorizeHeader(Color color)
	{
		header.SetColor(color, string.Empty);
	}

	private void ColorizeHeaderText(Color color)
	{
		foreach (UXText uiText in uiTexts)
		{
			uiText.Color = color;
		}
	}

	private void UpdatePlayerNumber()
	{
		if (playerNumber != null)
		{
			playerNumber.Text = teamManager.GetNoOfPlayersInTeam(team) + string.Empty;
		}
	}

	public override void Update()
	{
		base.Update();
		if (!(teamScore == null))
		{
			string text = teamManager.GetScore(team, GameStatCounterType.Kill).ToString();
			if (teamScore.Text != text)
			{
				teamScore.Text = text;
			}
		}
	}
}
