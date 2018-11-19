using System;
using System.Collections.Generic;
using MV.WorldObject.Accessories;
using MV.WorldObject.HighlightSystem;
using MV.WorldObject.HighlightSystem.HighlightPayloads;
using MV.WorldObject.MetaData;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class AccessoryMenuButton : MonoBehaviour
{
	[SerializeField]
	private GameObject redDotNotification;

	[SerializeField]
	private AccessoryShinyButton shineEffect;

	[SerializeField]
	private Text redDotNotificationText;

	[SerializeField]
	private AccessoryPreviewPopup accessoryPreviewPopup;

	private bool playerReady;

	private void Start()
	{
		if (MVGameControllerBase.Game.LocalPlayer.IsReady)
		{
			PlayerReady();
		}
		else
		{
			MVGameControllerBase.OnJoinStateChanged = (Action<MVJoinState>)Delegate.Combine(MVGameControllerBase.OnJoinStateChanged, new Action<MVJoinState>(OnJoinChanged));
		}
	}

	private void OnJoinChanged(MVJoinState joinState)
	{
		if (joinState == MVJoinState.Playing)
		{
			MVGameControllerBase.OnJoinStateChanged = (Action<MVJoinState>)Delegate.Remove(MVGameControllerBase.OnJoinStateChanged, new Action<MVJoinState>(OnJoinChanged));
			PlayerReady();
		}
	}

	private void OnEnable()
	{
		if (playerReady && redDotNotification.activeInHierarchy)
		{
			CalculateShouldShowHighlightIcon();
		}
	}

	private void PlayerReady()
	{
		if (!MVGameControllerBase.Game.LocalPlayer.IsTourist)
		{
			AccessoryDataManager.readyCallback = (UnityAction)Delegate.Combine(AccessoryDataManager.readyCallback, new UnityAction(OnAccessoryDataReady));
			AccessoryDataManager.SetReady();
		}
		else
		{
			enabled = false;
		}
	}

	private void OnAccessoryDataReady()
	{
		AccessoryDataManager.readyCallback = (UnityAction)Delegate.Remove(AccessoryDataManager.readyCallback, new UnityAction(OnAccessoryDataReady));
		playerReady = true;
		CalculateShouldShowHighlightIcon();
		CalculateShouldShowBundleAd();
		CalculateShouldShowAccessoryPopup();
	}

	private void CalculateShouldShowAccessoryPopup()
	{
		bool uiBlocked = false;
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			uiBlocked = x.IsUIElementBlocked(gameObject);
		});
		if (uiBlocked || MVGameControllerBase.IEditModeUI != null)
		{
			return;
		}
		List<Highlight<HighlightAccessory>> highLights = HighlightManager.GetHighLights<HighlightAccessory>(HighlightType.AccessoryPopup);
		List<AccessoryDataClient> list = new List<AccessoryDataClient>();
		for (int num = 0; num < highLights.Count; num++)
		{
			AccessoryDataClient accessoryDataByMetaDataId = AccessoryDataManager.GetAccessoryDataByMetaDataId(highLights[num].highlightData.accessoryMetaDataId);
			if (accessoryDataByMetaDataId != null && accessoryDataByMetaDataId.GetShowInShop() && !accessoryDataByMetaDataId.owns && accessoryDataByMetaDataId.isAvailable)
			{
				HighlightManager.SetHighlightToSeen(highLights[num].id);
				list.Add(accessoryDataByMetaDataId);
				if (list.Count > 2)
				{
					break;
				}
			}
		}
		if (list.Count > 0)
		{
			AccessoryPreviewPopup popup = UnityEngine.Object.Instantiate(accessoryPreviewPopup);
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.Push(popup.gameObject, UIPushOption.InvisibleBlocker, null, UIGroupFlags.Popup);
			});
			popup.Initialize(list);
		}
	}

	private void CalculateShouldShowHighlightIcon()
	{
		List<Highlight<HighlightAccessory>> highLights = HighlightManager.GetHighLights<HighlightAccessory>(HighlightType.Accessory);
		int num = 0;
		for (int i = 0; i < highLights.Count; i++)
		{
			AccessoryDataClient accessoryDataByMetaDataId = AccessoryDataManager.GetAccessoryDataByMetaDataId(highLights[i].highlightData.accessoryMetaDataId);
			if (accessoryDataByMetaDataId != null && accessoryDataByMetaDataId.GetShowInShop() && !accessoryDataByMetaDataId.owns)
			{
				num++;
			}
		}
		shineEffect.gameObject.SetActive(num > 0);
		redDotNotification.SetActive(num > 0);
		redDotNotificationText.text = num.ToString();
	}

	private void CalculateShouldShowBundleAd()
	{
		List<Highlight<HighlightAccessoryBundle>> highLights = HighlightManager.GetHighLights<HighlightAccessoryBundle>(HighlightType.AccessoryBundle);
		int num = -1;
		for (int i = 0; i < highLights.Count; i++)
		{
			if (highLights[i].highlightData.bundleId == AccessoryDataManager.GetAccessoryBundleId())
			{
				num = highLights[i].id;
				break;
			}
		}
		if (num == -1)
		{
			return;
		}
		List<AccessoryBundleItem> accessoryBundleItems = AccessoryDataManager.GetAccessoryBundleClient().accessoryBundleItems;
		for (int j = 0; j < accessoryBundleItems.Count; j++)
		{
			AccessoryDataClient accessoryDataByMetaDataId = AccessoryDataManager.GetAccessoryDataByMetaDataId(accessoryBundleItems[j].accessoryMetaDataID);
			if (accessoryDataByMetaDataId.owns)
			{
				continue;
			}
			redDotNotification.SetActive(value: true);
			shineEffect.gameObject.SetActive(value: true);
			if (FirstTimeEventManager.HasFirstTimeEventOccured(FirstTimeEvent.PM_AccessoryShop))
			{
				HighlightManager.SetHighlightToSeen(num);
				ExecuteEvents.ExecuteHierarchy(gameObject, null, (IActivateUIElement x, BaseEventData y) =>
				{
					x.Activate(ActivateUIElement.AvatarAccessoryShopBundles);
				});
			}
			break;
		}
	}
}
