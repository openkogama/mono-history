using System;
using MV.Common;
using UnityEngine;

public class MVGUIPlayerLine : UXLine
{
	public int maxNameChars = 20;

	private MVPlayer player;

	private Friend friend;

	public UXText levelText;

	public UXText playersText;

	public UXText scoreText;

	private bool mouseOver;

	public UXGroup buttonGroup;

	public UXIconButton requestButton;

	public UXIconButton cancelButton;

	public UXIconButton acceptButton;

	public UXIconButton pendingButton;

	private bool showAcceptButton;

	private bool showRequestButton;

	private bool showCancelButton;

	private bool showPendingButton;

	private bool showMouseOver => mouseOver && (showAcceptButton || showRequestButton || showCancelButton || showPendingButton);

	public void InitLine(PlayerData data)
	{
		player = data.player;
		friend = data.friend;
		string text = player.Username;
		if (text.Length >= maxNameChars)
		{
			text = text.Substring(0, maxNameChars) + "...";
		}
		levelText.Text = string.Empty;
		playersText.Text = text;
		MVPlayer mVPlayer = player;
		mVPlayer.OnScoreUpdated = (MVPlayer.OnScoreUpdatedDelegate)Delegate.Combine(mVPlayer.OnScoreUpdated, new MVPlayer.OnScoreUpdatedDelegate(UpdateScore));
		scoreText.Text = player.Score + string.Empty;
		InitializeListeners();
	}

	public void UpdateLine(PlayerData data)
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		player = data.player;
		friend = data.friend;
		scoreText.Text = player.Score + string.Empty;
		if (friend != null && friend.status == FriendStatus.Accepted)
		{
			playersText.Color = Color.green;
		}
		if (player.IsAnonymous && player != MVGameController.Instance.Game.LocalPlayer)
		{
			playersText.Color = Color.gray;
		}
		UpdateButtons();
	}

	public MVPlayer GetPlayer()
	{
		return player;
	}

	public override void DestroyLine()
	{
		MVPlayer mVPlayer = player;
		mVPlayer.OnScoreUpdated = (MVPlayer.OnScoreUpdatedDelegate)Delegate.Remove(mVPlayer.OnScoreUpdated, new MVPlayer.OnScoreUpdatedDelegate(UpdateScore));
	}

	private void UpdateScore(int score)
	{
		scoreText.Text = score + string.Empty;
	}

	public void OnMouseOver()
	{
		mouseOver = true;
	}

	public override void Update()
	{
		base.Update();
		if (mouseOver)
		{
			UpdateButtonVisibility();
			mouseOver = false;
		}
		else
		{
			UpdateButtonVisibility();
		}
	}

	private void InitializeListeners()
	{
		UXIconButton uXIconButton = requestButton;
		uXIconButton.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXIconButton.OnClick, (UXBaseButton.OnClickDelegate)(() =>
		{
			MVGameController.Instance.Game.RequestFriendShipByID(player.ProfileID);
		}));
		UXIconButton uXIconButton2 = acceptButton;
		uXIconButton2.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXIconButton2.OnClick, (UXBaseButton.OnClickDelegate)(() =>
		{
			MVGameController.Instance.Game.RequestAcceptFriendShip(friend.friendID);
		}));
		UXIconButton uXIconButton3 = cancelButton;
		uXIconButton3.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXIconButton3.OnClick, (UXBaseButton.OnClickDelegate)(() =>
		{
			MVGameController.Instance.Game.RequestRejectFriendShip(friend.friendID);
		}));
	}

	private void UpdateButtons()
	{
		showAcceptButton = false;
		showRequestButton = false;
		showCancelButton = false;
		showPendingButton = false;
		if (player.IsAnonymous || MVGameController.Instance.Game.LocalPlayer.IsAnonymous)
		{
			return;
		}
		if (friend == null && player != MVGameController.Instance.Game.LocalPlayer)
		{
			showRequestButton = true;
		}
		else if (friend != null && friend.status == FriendStatus.Pending)
		{
			if (MVGameController.Instance.Game.Friends.Friends.ContainsValue(friend))
			{
				showPendingButton = true;
			}
			else
			{
				showAcceptButton = true;
			}
			showCancelButton = true;
		}
	}

	public void UpdateButtonVisibility()
	{
		playersText.SetVisible(!showMouseOver);
		requestButton.SetVisible(showRequestButton && showMouseOver && Visible);
		cancelButton.SetVisible(showCancelButton && showMouseOver && Visible);
		acceptButton.SetVisible(showAcceptButton && showMouseOver && Visible);
		pendingButton.SetVisible(showPendingButton && showMouseOver && Visible);
	}
}
