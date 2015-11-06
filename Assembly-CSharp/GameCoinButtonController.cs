using System;
using System.Collections.Generic;
using UnityEngine;

public class GameCoinButtonController : UXViewScript
{
	[SerializeField]
	private UXToggleIconButton startStopBoostButton;

	[SerializeField]
	private UXBaseButton purchaseBoostButton;

	[SerializeField]
	private UXText boostLeftText;

	public override void OnInitialize()
	{
		base.OnInitialize();
		startStopBoostButton.OnToggle = HandleOnToggle;
		purchaseBoostButton.OnClick = () =>
		{
			CreatePurchaseDialog();
		};
		MVGameCoinManager gameCoinManager = MVGameControllerBase.Game.GameCoinManager;
		gameCoinManager.BoostStateChanged = (Action<int, bool>)Delegate.Combine(gameCoinManager.BoostStateChanged, new Action<int, bool>(OnBoostingChanged));
	}

	public override void OnShow()
	{
		base.OnShow();
		OnBoostingChanged(MVGameControllerBase.Game.GameCoinManager.BoostLeft, MVGameControllerBase.Game.GameCoinManager.BoostEnabled);
	}

	private void Update()
	{
		if (boostLeftText.Visible)
		{
			TimeSpan timeSpan = new TimeSpan(0, 0, 0, 0, MVGameControllerBase.Game.GameCoinManager.BoostLeft);
			string text = $"{timeSpan.Minutes:00}:{timeSpan.Seconds:00}";
			boostLeftText.Text = text;
		}
	}

	private void HandleOnToggle(bool boost)
	{
		if (!MVGameControllerBase.Game.GameCoinManager.Active && boost)
		{
			startStopBoostButton.SetToggleState(toggle: false);
			UXUtils.UXDialogFactory.BuildDialog("To use game coin boost go to a game where game coins are used", "No game coins", UXDialogType.Simple, noButtons: true, stackDialog: true).Show();
		}
		else
		{
			MVGameControllerBase.Game.SetGameCoinBoostState(boost);
		}
	}

	private void CreatePurchaseDialog()
	{
		UXUtils.UXDialogFactory.CreateCustomDialog("Prefabs/GUI/Dialogs/GameCoinBoostDialog", string.Empty, noButtons: true, stackDialog: true).SetOnResultCallback(OnShopDialogResult).Show();
		MVGUIGameCoinBoostShopDialog mVGUIGameCoinBoostShopDialog = (MVGUIGameCoinBoostShopDialog)UXUtils.UXDialogFactory.CurrentDialogBox;
		mVGUIGameCoinBoostShopDialog.OnTryPurchaseProduct = () =>
		{
			Debug.Log("Trying to purchase");
			MVGameControllerBase.Game.PurchaseGameCoinBooster();
		};
	}

	private void OnBoostingChanged(int boostTimeLeft, bool boosting)
	{
		SetAllToNotVisible();
		if (boostTimeLeft == 0)
		{
			purchaseBoostButton.SetVisible(visible: true);
			return;
		}
		boostLeftText.SetVisible(visible: true);
		startStopBoostButton.SetVisible(visible: true);
		startStopBoostButton.SetToggleState(boosting);
		if (boosting)
		{
			AwayMonitor.IdleKickEnabled = false;
		}
	}

	private void SetAllToNotVisible()
	{
		purchaseBoostButton.SetVisible(visible: false);
		startStopBoostButton.SetVisible(visible: false);
		boostLeftText.SetVisible(visible: false);
	}

	private void OnShopDialogResult(UXDialogBox dialogBox)
	{
		if (dialogBox.DialogResult == UXDialogResult.Positive)
		{
			Dictionary<object, object> dictionary = (Dictionary<object, object>)dialogBox.GetResult();
			int boostLeft = (int)dictionary[(byte)180];
			MVGameControllerBase.Game.GameCoinManager.OnGameBoostChanged(boostLeft, boostEnabled: false);
		}
	}
}
