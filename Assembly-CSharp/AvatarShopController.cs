using System;
using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class AvatarShopController : MonoBehaviour, IPurchaseAvatar, IEventSystemHandler
{
	private InventoryController inventoryController;

	private TabState tab;

	[SerializeField]
	private int numberOfSlotsPrPage;

	[SerializeField]
	private InventoryController inventoryControllerPrefab;

	[SerializeField]
	private AvatarShopPreviewItem previewItemPrefab;

	private Transform previewRootTransform;

	private AvatarEditModeBodyController avatarEditModeBodyController;

	private AvatarRepository avatarRepository;

	public void Initialize(AvatarEditModeBodyController editModeBodyController)
	{
		avatarEditModeBodyController = editModeBodyController;
		avatarRepository = MVGameControllerBase.Game.AvatarShopRepository;
		tab = new TabState(TM._("Avatars"), numberOfSlotsPrPage);
		tab.highestSlotIndex = avatarRepository.Count - 1;
	}

	private static MVWorldObjectClient GetWorldObjectFromItemData(AvatarRepositoryItem item)
	{
		byte[] data = item.data;
		BytePacker koGaMaData = new BytePacker(data);
		KoGaMaPackageClient koGaMaPackageClient = new KoGaMaPackageClient(koGaMaData, readRuntimeValues: false);
		koGaMaPackageClient.InventoryInitialize();
		return koGaMaPackageClient.worldObjects[koGaMaPackageClient.worldObjectRoot];
	}

	public void Activate(UIPushOption pushOption)
	{
		this.inventoryController = UnityEngine.Object.Instantiate(inventoryControllerPrefab);
		this.inventoryController.Initialize(numberOfSlotsPrPage);
		this.inventoryController.AddTab(0, tab.name);
		InventoryController inventoryController = this.inventoryController;
		inventoryController.OnPageTurned = (UnityAction<int>)Delegate.Combine(inventoryController.OnPageTurned, new UnityAction<int>(PageTurned));
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack handler, BaseEventData data) =>
		{
			handler.PopGroups(UIGroupFlags.InventoryUI | UIGroupFlags.InventoryUISubMenu);
		});
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(this.inventoryController.gameObject, pushOption, OnPop, UIGroupFlags.InventoryUI);
		});
		UpdateContent();
	}

	private void UpdateContent()
	{
		if (previewRootTransform != null)
		{
			UnityEngine.Object.Destroy(previewRootTransform.gameObject);
		}
		previewRootTransform = new GameObject("Preview Root - AvatarShopController").transform;
		inventoryController.Clear();
		inventoryController.SelectTab(0, tab.currentPage, tab.MaxPages);
		List<AvatarRepositoryItem> avatars = avatarRepository.GetAvatars();
		for (int i = 0; i < avatars.Count; i++)
		{
			if (tab.SlotIndexIsInRange(i))
			{
				AddPreviewObjectForIndex(i);
			}
		}
	}

	private void AddPreviewObjectForIndex(int index)
	{
		AvatarRepositoryItem avatar = avatarRepository.GetAvatar(index);
		MVWorldObjectClient worldObjectFromItemData = GetWorldObjectFromItemData(avatar);
		AvatarShopPreviewItem avatarShopPreviewItem = UnityEngine.Object.Instantiate(previewItemPrefab);
		avatarShopPreviewItem.InitializeObjectPreview(avatar, worldObjectFromItemData, previewRootTransform);
		inventoryController.AddObject(avatarShopPreviewItem.gameObject, index % numberOfSlotsPrPage);
	}

	private void OnPop()
	{
		if (previewRootTransform != null)
		{
			UnityEngine.Object.Destroy(previewRootTransform.gameObject);
		}
	}

	public void PageTurned(int dir)
	{
		if (tab.UpdatePage(dir))
		{
			UpdateContent();
		}
	}

	public void PurchaseAvatar(AvatarRepositoryItem item)
	{
		avatarEditModeBodyController.PurchaseAvatar(item);
	}
}
