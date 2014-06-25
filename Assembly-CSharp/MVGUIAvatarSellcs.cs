using System;
using MV.WorldObject;
using UnityEngine;

public class MVGUIAvatarSellcs : UXViewScript
{
	public UXIconButton avatarSellButton;

	public MVGUISellAvatarDialog sellAvatarDialog;

	private bool canBeVisible;

	private MvAvatarMetaData avatarMetaData;

	private int woID = -1;

	public void UpdateAvatarMetaData(int woID, MvAvatarMetaData avatarMetaData)
	{
		this.avatarMetaData = avatarMetaData;
		this.woID = woID;
		if (avatarMetaData.canBeSoldOnMarketPlace)
		{
			if (canBeVisible)
			{
				avatarSellButton.SetVisible(visible: true);
			}
		}
		else
		{
			avatarSellButton.SetVisible(visible: false);
		}
	}

	public override void OnInitialize()
	{
		UXIconButton uXIconButton = avatarSellButton;
		uXIconButton.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXIconButton.OnClick, (UXBaseButton.OnClickDelegate)(() =>
		{
			ShowSellMenu();
		}));
	}

	public override void OnShow()
	{
		if (avatarMetaData != null && avatarMetaData.canBeSoldOnMarketPlace)
		{
			base.OnShow();
		}
		canBeVisible = true;
	}

	public override void OnHide()
	{
		base.OnHide();
		canBeVisible = false;
	}

	private void ShowSellMenu()
	{
		Debug.Log((object)"Show sell menu");
		MVGameController.Instance.Game.AvatarMetaDataWoMap.TryGetValue(woID, out avatarMetaData);
		sellAvatarDialog.SetAvatarMetaData(woID, avatarMetaData);
		sellAvatarDialog.View.Show();
	}
}
