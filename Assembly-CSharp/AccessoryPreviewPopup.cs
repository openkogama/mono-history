using System.Collections.Generic;
using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AccessoryPreviewPopup : MonoBehaviour, IAccessoryClicked, IEventSystemHandler
{
	[SerializeField]
	private AccessoryInventoryViewItem accessoryPopupItemPrefab;

	[SerializeField]
	private HorizontalLayoutGroup layoutGroup;

	private Transform tempTransform;

	private MVBody body;

	public void Initialize(List<AccessoryDataClient> previewedAccessories)
	{
		tempTransform = new GameObject().transform;
		tempTransform.gameObject.SetActive(value: false);
		if (MVGameControllerBase.GameMode == MVGameMode.CharacterEditor)
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IGetCurrentBody x, BaseEventData y) =>
			{
				x.GetCurrentBody(SetBody);
			});
		}
		else
		{
			body = MVGameControllerBase.WOCM.AvatarLocal.Body;
		}
		for (int num = 0; num < previewedAccessories.Count; num++)
		{
			AccessoryInventoryViewItem accessoryInventoryViewItem = Object.Instantiate(accessoryPopupItemPrefab);
			accessoryInventoryViewItem.Initialize(previewedAccessories[num], tempTransform, body);
			accessoryInventoryViewItem.transform.SetParent(layoutGroup.transform, worldPositionStays: false);
		}
	}

	private void SetBody(MVBody avatarBody)
	{
		body = avatarBody;
	}

	public void OpenAccessoryManagementScreen(AccessoryDataClient accessoryData)
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IAccessoryPopupHandler x, BaseEventData y) =>
		{
			x.OpenInventoryAtItem(UIPushOption.Blocking, accessoryData);
		});
	}

	public void DisplayCategoryFeatures(AccessoryCategoryClient category)
	{
	}

	public void OpenCategoryScreen(bool canSortByInventory)
	{
	}

	public void UpdateHighlightedTab(AccessoryCategoryClient category)
	{
	}

	private void OnDestroy()
	{
		if (tempTransform != null)
		{
			Object.Destroy(tempTransform.gameObject);
			tempTransform = null;
		}
	}
}
