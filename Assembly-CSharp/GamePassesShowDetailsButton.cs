using System;
using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;

public class GamePassesShowDetailsButton : MonoBehaviour
{
	[SerializeField]
	private GameObject disabledButton;

	[SerializeField]
	private GamePassesTextBubble disabledButtonToolTip;

	[SerializeField]
	private GamePassesTextBubble OnActivatedToolTip;

	[SerializeField]
	private GamePassesShopDetails shopDetails;

	private void Start()
	{
		GamePassProgressionController.OnGamePassesProgressionUpdate = (Action)Delegate.Combine(GamePassProgressionController.OnGamePassesProgressionUpdate, new Action(OnProgressionUpdate));
		UpdateButtonVisibility();
	}

	private void OnDestroy()
	{
		GamePassProgressionController.OnGamePassesProgressionUpdate = (Action)Delegate.Remove(GamePassProgressionController.OnGamePassesProgressionUpdate, new Action(OnProgressionUpdate));
	}

	private void OnProgressionUpdate()
	{
		UpdateButtonVisibility();
	}

	private void UpdateButtonVisibility()
	{
		if (MVClientSettings.IsFlagSet(ClientSettingFlags.GamePassSilentReleaseEnabled))
		{
			if (gameObject.activeSelf)
			{
				gameObject.SetActive(value: false);
			}
			if (disabledButton.activeSelf)
			{
				disabledButton.SetActive(value: false);
			}
			return;
		}
		bool isProgressionEnabled = GamePassProgressionController.IsProgressionEnabled;
		if (gameObject.activeSelf != isProgressionEnabled)
		{
			gameObject.SetActive(isProgressionEnabled);
			if (isProgressionEnabled)
			{
				OnActivatedToolTip.Activate("Game Tiers Activated");
			}
		}
		if (disabledButton.activeSelf == isProgressionEnabled)
		{
			disabledButton.SetActive(!isProgressionEnabled);
		}
	}

	public void OnButtonPressed()
	{
		GamePassesShopDetails shopDetail = UnityEngine.Object.Instantiate(shopDetails);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(shopDetail.gameObject, UIPushOption.InvisibleBlocker, null, UIGroupFlags.InventoryUI);
		});
	}

	public void OnDisabledButtonPressed()
	{
		disabledButtonToolTip.Activate("Add Crystals to unlock Tiers");
	}
}
