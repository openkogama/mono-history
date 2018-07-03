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
	private Text redDotNotificationText;

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
	}

	private void CalculateShouldShowHighlightIcon()
	{
		List<Highlight<HighlightAccessory>> highLights = HighlightManager.GetHighLights<HighlightAccessory>(HighlightType.Accessory);
		int num = 0;
		for (int i = 0; i < highLights.Count; i++)
		{
			AccessoryDataClient accessoryDataByMetaDataId = AccessoryDataManager.GetAccessoryDataByMetaDataId(highLights[i].highlightData.accessoryMetaDataId);
			if (accessoryDataByMetaDataId != null && accessoryDataByMetaDataId.GetShowInShop())
			{
				num++;
			}
		}
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
