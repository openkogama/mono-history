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
		scoreText.Text = data.player.GetGameStat(GameStatCounterType.Kill).ToString();
		InitializeListeners();
	}

	public void UpdateLine(PlayerData data)
	{
		player = data.player;
		friend = data.friend;
		scoreText.Text = data.Score.ToString();
		if (friend != null && friend.status == FriendStatus.Accepted)
		{
			playersText.Color = Color.green;
		}
		if (player.IsAnonymous && player != MVGameControllerBase.Game.LocalPlayer)
		{
			playersText.Color = Color.gray;
		}
		UpdateButtons();
	}

	public MVPlayer GetPlayer()
	{
		return player;
	}

	private void UpdateScore(int score)
	{
		scoreText.Text = score.ToString();
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
			try
			{
				ValidateFriendRequest();
				MVGameControllerBase.Game.RequestFriendShipByID(player.ProfileID);
			}
			catch (Exception ex)
			{
				Debug.Log(ex.Message);
				UXUtils.FindGUIObjectOfType<MVGUIChatWindow>().AddLine(ex.Message, Color.red);
			}
		}));
		UXIconButton uXIconButton2 = acceptButton;
		uXIconButton2.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXIconButton2.OnClick, (UXBaseButton.OnClickDelegate)(() =>
		{
			try
			{
				ValidateFriendRequest();
				MVGameControllerBase.Game.RequestAcceptFriendShip(friend.friendID);
			}
			catch (Exception ex)
			{
				Debug.Log(ex.Message);
				UXUtils.FindGUIObjectOfType<MVGUIChatWindow>().AddLine(ex.Message, Color.red);
			}
		}));
		UXIconButton uXIconButton3 = cancelButton;
		uXIconButton3.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXIconButton3.OnClick, (UXBaseButton.OnClickDelegate)(() =>
		{
			MVGameControllerBase.Game.RequestRejectFriendShip(friend.friendID);
		}));
	}

	private void ValidateFriendRequest()
	{
		int level = MVGameControllerBase.Game.LocalPlayer.Level;
		int friendsLimit = BadgeManager.GetFriendsLimit(level);
		int count = MVGameControllerBase.Game.Friends.Friends.Count;
		if (count < friendsLimit)
		{
			return;
		}
		int num = level + 1;
		int friendsLimit2 = BadgeManager.GetFriendsLimit(num);
		string format = TM._("You can only have {0} friends at level {1}. Get to level {2} and you can have {3} friends.");
		throw new Exception(string.Format(format, friendsLimit, level, num, friendsLimit2));
	}

	private void UpdateButtons()
	{
		showAcceptButton = false;
		showRequestButton = false;
		showCancelButton = false;
		showPendingButton = false;
		if (player.IsAnonymous || MVGameControllerBase.Game.LocalPlayer.IsAnonymous)
		{
			return;
		}
		if (friend == null && player != MVGameControllerBase.Game.LocalPlayer)
		{
			showRequestButton = true;
		}
		else if (friend != null && friend.status == FriendStatus.Pending)
		{
			if (MVGameControllerBase.Game.Friends.Friends.ContainsValue(friend))
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
