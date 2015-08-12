using System;
using System.Collections.Generic;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class MVGUIGameInfo : UXViewScript
{
	public UXText playersText;

	public UXIconButton teamIcon;

	public Color blueTeamColor;

	public Color redTeamColor;

	public Color greenTeamColor;

	public Color yellowTeamColor;

	public Material blueTeamMaterial;

	public Material redTeamMaterial;

	public Material greenTeamMaterial;

	public Material yellowTeamMaterial;

	public Material neutralTeamMaterial;

	[SerializeField]
	private ScaleAnimationBase scaleAnimationBase;

	[SerializeField]
	private MVGUIMenu menu;

	public override void OnInitialize()
	{
		base.OnInitialize();
		teamIcon.OnClick = () =>
		{
			menu.ToggleMenu();
		};
		InitializePlayersText();
	}

	private void InitializePlayersText()
	{
		MVNetworkGame game = MVGameController.Game;
		game.onPlayerListChanged = (MVNetworkGame.OnPlayerListChangedDelegate)Delegate.Combine(game.onPlayerListChanged, new MVNetworkGame.OnPlayerListChangedDelegate(UpdateGameInfoText));
		MVTeamManager teamManager = MVGameController.Game.TeamManager;
		teamManager.OnTeamsUpdated = (MVTeamManager.OnTeamsUpdatedDelegate)Delegate.Combine(teamManager.OnTeamsUpdated, new MVTeamManager.OnTeamsUpdatedDelegate(UpdateGameInfoText));
		MVNetworkGame game2 = MVGameController.Game;
		game2.OnReceivedGameMsg = (MVNetworkGame.OnReceivedGameMsgDelegate)Delegate.Combine(game2.OnReceivedGameMsg, new MVNetworkGame.OnReceivedGameMsgDelegate(PlayScaleAnimation));
		scaleAnimationBase.OnScaleAnimationStopped = (float x) =>
		{
			scaleAnimationBase.ResetScaleAnimation();
		};
		UpdatePlayersText();
		UpdateTeamText();
	}

	private void PlayScaleAnimation(MVGameMsgType msgType, Dictionary<object, object> message)
	{
		if (msgType == MVGameMsgType.UserJoined || msgType == MVGameMsgType.UserLeft)
		{
			UpdateGameInfoText();
			scaleAnimationBase.Play();
		}
	}

	private void UpdateGameInfoText()
	{
		UpdatePlayersText();
		UpdateTeamText();
	}

	private void UpdatePlayersText()
	{
		playersText.Text = MVGameController.Game.Players.Count.ToString();
	}

	private void UpdateTeamText()
	{
		if (MVGameController.Game.TeamManager.TeamCount() == 1)
		{
			teamIcon.ChangeMaterial(neutralTeamMaterial);
			return;
		}
		switch (MVGameController.Game.LocalPlayer.Team)
		{
		case MVTeam.Blue:
			teamIcon.ChangeMaterial(blueTeamMaterial);
			break;
		case MVTeam.Red:
			teamIcon.ChangeMaterial(redTeamMaterial);
			break;
		case MVTeam.Green:
			teamIcon.ChangeMaterial(greenTeamMaterial);
			break;
		case MVTeam.Yellow:
			teamIcon.ChangeMaterial(yellowTeamMaterial);
			break;
		}
	}
}
