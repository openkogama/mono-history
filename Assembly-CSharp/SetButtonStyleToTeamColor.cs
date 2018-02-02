using System;
using MV.WorldObject;
using UnityEngine;
using UnityEngine.UI;

public class SetButtonStyleToTeamColor : MonoBehaviour
{
	[SerializeField]
	private Button button;

	[SerializeField]
	private bool shouldRetainAlpha;

	private void Start()
	{
		MVTeamManager teamManager = MVGameControllerBase.Game.TeamManager;
		teamManager.OnTeamsUpdated = (MVTeamManager.OnTeamsUpdatedDelegate)Delegate.Combine(teamManager.OnTeamsUpdated, new MVTeamManager.OnTeamsUpdatedDelegate(UpdateColor));
		MVPlayerContainer mVPlayerContainer = MVGameControllerBase.Game.MVPlayerContainer;
		mVPlayerContainer.OnPlayerListChanged = (Action)Delegate.Combine(mVPlayerContainer.OnPlayerListChanged, new Action(UpdateColor));
		UpdateColor();
	}

	private void UpdateColor()
	{
		float alpha = 1f;
		if (shouldRetainAlpha)
		{
			alpha = button.colors.normalColor.a;
		}
		if (MVGameControllerBase.Game.TeamManager.TeamCount() <= 1)
		{
			Styles.SetStyle(button, ButtonStyle.RegularButton, MVTeam.None);
		}
		else
		{
			MVTeam team = MVGameControllerBase.Game.LocalPlayer.Team;
			Styles.SetStyle(button, ButtonStyle.RegularButton, team);
		}
		if (shouldRetainAlpha)
		{
			ResetAlpha(alpha);
		}
	}

	private void ResetAlpha(float alpha)
	{
		Color normalColor = button.colors.normalColor;
		normalColor.a = alpha;
		ColorBlock colors = button.colors;
		colors.normalColor = normalColor;
		button.colors = colors;
	}
}
