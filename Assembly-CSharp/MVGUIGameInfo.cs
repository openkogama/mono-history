using System;
using Localize;
using MV.WorldObject;
using UnityEngine;

public class MVGUIGameInfo : UXViewScript
{
	public UXText playersText;

	public UXText teamText;

	public Color blueTeamColor;

	public Color redTeamColor;

	public Color greenTeamColor;

	public Color yellowTeamColor;

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
		playersText.Text = string.Format(Localization.Instance.GetText(TextSlotIndex.NumberofPlayers), MVGameController.Instance.Game.Players.Count);
	}

	private void UpdateTeamText()
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		switch (MVGameController.Instance.Game.LocalPlayer.Team)
		{
		case MVTeam.Blue:
			teamText.Text = Localization.Instance.GetText(TextSlotIndex.BlueTeam);
			teamText.Color = blueTeamColor;
			break;
		case MVTeam.Red:
			teamText.Text = Localization.Instance.GetText(TextSlotIndex.RedTeam);
			teamText.Color = redTeamColor;
			break;
		case MVTeam.Green:
			teamText.Text = Localization.Instance.GetText(TextSlotIndex.GreenTeam);
			teamText.Color = greenTeamColor;
			break;
		case MVTeam.Yellow:
			teamText.Text = Localization.Instance.GetText(TextSlotIndex.YellowTeam);
			teamText.Color = yellowTeamColor;
			break;
		default:
			teamText.Text = string.Empty;
			break;
		}
	}
}
