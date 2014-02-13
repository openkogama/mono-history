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

	public MVGUITeamList()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
	}

	public void InitializeTeamList(MVTeam team)
	{
		this.team = team;
		teamManager = MVGameController.Instance.Game.TeamManager;
		MVNetworkGame game = MVGameController.Instance.Game;
		game.onPlayerListChanged = (MVNetworkGame.OnPlayerListChangedDelegate)Delegate.Combine(game.onPlayerListChanged, new MVNetworkGame.OnPlayerListChangedDelegate(UpdatePlayerNumber));
		UpdatePlayerNumber();
		MVTeamManager mVTeamManager = teamManager;
		mVTeamManager.OnTeamScoreUpdate = (MVTeamManager.OnTeamScoreUpdateDelegate)Delegate.Combine(mVTeamManager.OnTeamScoreUpdate, new MVTeamManager.OnTeamScoreUpdateDelegate(UpdateTeamScore));
		UpdateTeamScore();
		ColorizeHeader();
	}

	public Vector2 GetFullSize()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		return new Vector2(Width + 1f, Height + 2f);
	}

	private void ColorizeHeader()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		if (team == MVTeam.None)
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
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		header.SetColor(color, string.Empty);
	}

	private void ColorizeHeaderText(Color color)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		foreach (UXText uiText in uiTexts)
		{
			uiText.Color = color;
		}
	}

	private void UpdatePlayerNumber()
	{
		if ((Object)(object)playerNumber != (Object)null)
		{
			playerNumber.Text = teamManager.GetNoOfPlayersInTeam(team) + string.Empty;
		}
	}

	private void UpdateTeamScore()
	{
		if ((Object)(object)teamScore != (Object)null)
		{
			teamScore.Text = teamManager.GetScore(team) + string.Empty;
		}
	}
}
