using System;
using MV.WorldObject;
using UnityEngine;

public class MVGUIGameInfo : UXViewScript
{
	public UXText playersText;

	public UXPlane teamIcon;

	public Color blueTeamColor;

	public Color redTeamColor;

	public Color greenTeamColor;

	public Color yellowTeamColor;

	public Material blueTeamMaterial;

	public Material redTeamMaterial;

	public Material greenTeamMaterial;

	public Material yellowTeamMaterial;

	public Material neutralTeamMaterial;

	private bool _isInitialized;

	public override void OnShow()
	{
		base.OnShow();
		if (!_isInitialized)
		{
			InitializePlayersText();
		}
		UpdatePlayersText();
		UpdateTeamText();
	}

	private void InitializePlayersText()
	{
		MVNetworkGame game = MVGameController.Instance.Game;
		game.onPlayerListChanged = (MVNetworkGame.OnPlayerListChangedDelegate)Delegate.Combine(game.onPlayerListChanged, new MVNetworkGame.OnPlayerListChangedDelegate(UpdateGameInfoText));
		MVTeamManager teamManager = MVGameController.Instance.Game.TeamManager;
		teamManager.OnTeamsUpdated = (MVTeamManager.OnTeamsUpdatedDelegate)Delegate.Combine(teamManager.OnTeamsUpdated, new MVTeamManager.OnTeamsUpdatedDelegate(UpdateGameInfoText));
		_isInitialized = true;
	}

	private void UpdateGameInfoText()
	{
		UpdatePlayersText();
		UpdateTeamText();
	}

	private void UpdatePlayersText()
	{
		playersText.Text = MVGameController.Instance.Game.Players.Count.ToString();
	}

	private void UpdateTeamText()
	{
		switch (MVGameController.Instance.Game.LocalPlayer.Team)
		{
		case MVTeam.Blue:
			teamIcon.SetMaterial(blueTeamMaterial);
			break;
		case MVTeam.Red:
			teamIcon.SetMaterial(redTeamMaterial);
			break;
		case MVTeam.Green:
			teamIcon.SetMaterial(greenTeamMaterial);
			break;
		case MVTeam.Yellow:
			teamIcon.SetMaterial(yellowTeamMaterial);
			break;
		default:
			teamIcon.SetMaterial(neutralTeamMaterial);
			break;
		}
	}
}
