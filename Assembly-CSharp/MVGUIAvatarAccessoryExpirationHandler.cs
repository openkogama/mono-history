using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Localize;
using MV.Common;
using UnityEngine;

public class MVGUIAvatarAccessoryExpirationHandler : MonoBehaviour
{
	public float dialogWaitDelay = 5f;

	private Queue<InventoryExpirationInfo> expiringAccessories = new Queue<InventoryExpirationInfo>();

	private int inventoryIDOfOpenExpirationDialog = -1;

	private float lastDialogTime;

	private bool subscribedToAvatar;

	private bool respawned;

	private MVNetworkGame Game => MVGameController.Instance.Game;

	private MVWorldObjectClientManager WOCM => MVGameController.Instance.WOCM;

	private PlayController PlayController => MVGameController.Instance.PlayController;

	private EditorController EditorController => MVGameController.Instance.EditorController;

	private CharacterEditorController CharacterEditorController => MVGameController.Instance.CharacterEditorController;

	private UXDialogFactory DialogFactory => UXUtils.FindGUIObjectOfType<UXDialogFactory>();

	private void Start()
	{
		Game.StreamingAssetsExpired += HandleGameStreamingAssetsExpired;
		lastDialogTime = Time.time;
	}

	private void HandleGameStreamingAssetsExpired(object sender, ProductsExpiringEventArgs e)
	{
		InventoryExpirationInfo inventoryExpirationInfo;
		foreach (InventoryExpirationInfo expiringProduct in e.ExpiringProducts)
		{
			inventoryExpirationInfo = expiringProduct;
			if (!expiringAccessories.Any((InventoryExpirationInfo exp) => exp.InventoryID == inventoryExpirationInfo.InventoryID) && inventoryIDOfOpenExpirationDialog != inventoryExpirationInfo.InventoryID)
			{
				expiringAccessories.Enqueue(inventoryExpirationInfo);
			}
		}
	}

	private void Update()
	{
		if (!subscribedToAvatar && Game.JoinState == MVJoinState.Playing)
		{
			WOCM.AvatarLocal.Respawned += AvatarController_Respawned;
			subscribedToAvatar = true;
		}
		if (!((Object)(object)MVGameController.Instance == (Object)null) && expiringAccessories.Count != 0 && !(Time.time - lastDialogTime < dialogWaitDelay))
		{
			ProcessOneFromExpiredQueue();
		}
	}

	private void ProcessOneFromExpiredQueue()
	{
		if (expiringAccessories.Count != 0)
		{
			InventoryExpirationInfo inventoryExpirationInfo = expiringAccessories.Dequeue();
			if (CanShowPopup(inventoryExpirationInfo))
			{
				ShowExpirationPopup(inventoryExpirationInfo);
			}
			else
			{
				expiringAccessories.Enqueue(inventoryExpirationInfo);
			}
		}
	}

	private void AvatarController_Respawned(object source, EventArgs e)
	{
		respawned = true;
		ProcessOneFromExpiredQueue();
		respawned = false;
	}

	private bool CanShowPopup(InventoryExpirationInfo expirationInfo)
	{
		if (PlayController != null)
		{
			return CanShowExpirationPopup(PlayController, expirationInfo);
		}
		if (EditorController != null)
		{
			return CanShowExpirationPopup(EditorController, expirationInfo);
		}
		if (CharacterEditorController != null)
		{
			return CanShowExpirationPopup(CharacterEditorController, expirationInfo);
		}
		return false;
	}

	private bool CanShowExpirationPopup(PlayController playController, InventoryExpirationInfo expirationInfo)
	{
		if (!((Object)(object)DialogFactory.CurrentDialogBox == (Object)null) || playController.IsChatShown() || playController.IsMenuShown() || (!WOCM.AvatarLocal.IsDead && !respawned))
		{
			return false;
		}
		foreach (AvatarAccessory accessory in WOCM.AvatarLocal.Body.GetAccessories())
		{
			if (accessory.InventoryID == expirationInfo.InventoryID)
			{
				return true;
			}
		}
		return false;
	}

	private bool CanShowExpirationPopup(EditorController playController, InventoryExpirationInfo expirationInfo)
	{
		if ((Object)(object)DialogFactory.CurrentDialogBox != (Object)null || playController.IsMenuShown())
		{
			return false;
		}
		foreach (AvatarAccessory accessory in WOCM.AvatarLocal.Body.GetAccessories())
		{
			if (accessory.InventoryID == expirationInfo.InventoryID)
			{
				return true;
			}
		}
		return false;
	}

	private bool CanShowExpirationPopup(CharacterEditorController playController, InventoryExpirationInfo expirationInfo)
	{
		return (Object)(object)DialogFactory.CurrentDialogBox == (Object)null && !playController.IsMenuShown() && !playController.IsAvatarShopShown();
	}

	private void ShowExpirationPopup(InventoryExpirationInfo expirationInfo)
	{
		if (!expirationInfo.IsExpiredNotRenewed)
		{
			ProductInventoryInfo<StreamingAssetInfo> productInventoryInfo = Game.StreamingAssetInventory.Get(expirationInfo.InventoryID);
			if (productInventoryInfo == null)
			{
				Debug.LogWarning((object)("Expired accessory " + expirationInfo.InventoryID + " that is not in inventory, remove from any body it is attached on"));
				RemoveAccessoryFromPlayerBodies(expirationInfo.InventoryID);
			}
			else if (productInventoryInfo.ProductInfo.ShopInfo == null)
			{
				inventoryIDOfOpenExpirationDialog = expirationInfo.InventoryID;
				DialogFactory.CreateCustomDialog("Prefabs/GUI/AvatarAccessory/AvatarAccessoryExpirationDialog", TextSlotIndex.ItemExpiredHeader, noButtons: true, stackDialog: true, canClose: false).SetOnResultCallback(OnExpirationPopupReturn).Show();
				(DialogFactory.CurrentDialogBox as MVGUIAvatarAccessoryExpirationDialog).BuildExpirationDialog(productInventoryInfo);
			}
			else
			{
				DialogFactory.CreateCustomDialog("Prefabs/GUI/Dialogs/AvatarAccessoryShopDialog", TextSlotIndex.Empty, noButtons: true, stackDialog: true, canClose: false).SetOnResultCallback(OnExpirationPopupReturn).SetValues(BuildDialogData(productInventoryInfo.ProductInfo))
					.Show();
				MVGUIAvatarAccessoryShopDialog mVGUIAvatarAccessoryShopDialog = (MVGUIAvatarAccessoryShopDialog)DialogFactory.CurrentDialogBox;
				mVGUIAvatarAccessoryShopDialog.BuildShopDialogForRentRenewal(productInventoryInfo.ProductInfo, productInventoryInfo.InventoryID);
			}
		}
	}

	private Dictionary<string, DialogData> BuildDialogData(StreamingAssetInfo streamingAssetInfo)
	{
		Dictionary<string, DialogData> dictionary = new Dictionary<string, DialogData>();
		dictionary.Add("AccessoryName", new TextData
		{
			text = streamingAssetInfo.Name,
			useWordWrap = true
		});
		dictionary.Add("AccessoryDescription", new TextData
		{
			text = streamingAssetInfo.Desc,
			useWordWrap = true
		});
		Object val = Object.Instantiate(Resources.Load("Prefabs/GUI/ShopPreview/AvatarAccessoryShopPreview"));
		MVGUIAvatarAccessoryShopPreview component = ((GameObject)((val is GameObject) ? val : null)).GetComponent<MVGUIAvatarAccessoryShopPreview>();
		component.CreateNewViewItem(streamingAssetInfo);
		dictionary.Add("AccessoryPreview", new ProductPreviewData
		{
			productPreview = ((Component)component).gameObject
		});
		return dictionary;
	}

	private void OnExpirationPopupReturn(UXDialogBox dialogBox)
	{
		inventoryIDOfOpenExpirationDialog = -1;
		Hashtable hashtable = (Hashtable)dialogBox.GetResult();
		int num = (int)hashtable["oldInventoryID"];
		if (dialogBox.DialogResult == UXDialogResult.Positive)
		{
			int num2 = (int)hashtable["newInventoryID"];
			if (num != num2)
			{
				HandleExpireAvatarAccessory(num);
			}
		}
		else
		{
			HandleExpireAvatarAccessory(num);
		}
		lastDialogTime = Time.time;
	}

	private void HandleExtendAvatarAccessory(int inventoryID)
	{
	}

	private void HandlePurchaseAvatarAccessory(int oldInventoryID, int newInventoryID)
	{
		HandleExpireAvatarAccessory(oldInventoryID);
	}

	private void HandleExpireAvatarAccessory(int inventoryID)
	{
		ProductInventoryInfo<StreamingAssetInfo> productInventoryInfo = Game.StreamingAssetInventory.Get(inventoryID);
		if (productInventoryInfo == null)
		{
			Debug.LogWarning((object)$"Item with inventoryID '{inventoryID}' was removed from inventory, but not from avatar");
		}
		else
		{
			Game.ExpireAvatarAccessory(inventoryID, GetBodyIDOnEquippedItem(inventoryID));
		}
	}

	private int GetBodyIDOnEquippedItem(int inventoryID)
	{
		int result = 0;
		if (MVGameController.Instance.Game.GameMode == MVGameMode.CharacterEditor)
		{
			CharacterEditorController characterEditorController = MVGameController.Instance.IngameController as CharacterEditorController;
			foreach (MVBody body in characterEditorController.Bodies)
			{
				MVBody mVBody = body;
				if (mVBody.HasAccessoryWithID(inventoryID))
				{
					result = mVBody.Id;
					break;
				}
			}
		}
		else
		{
			result = MVGameController.Instance.WOCM.AvatarLocal.Body.Id;
		}
		return result;
	}

	private void RemoveAccessoryFromPlayerBodies(int accessoryInventoryID)
	{
		if (MVGameController.Instance.Game.GameMode == MVGameMode.CharacterEditor)
		{
			CharacterEditorController characterEditorController = MVGameController.Instance.IngameController as CharacterEditorController;
			{
				foreach (MVBody body2 in characterEditorController.Bodies)
				{
					MVBody mVBody = body2;
					if (mVBody.HasAccessoryWithID(accessoryInventoryID))
					{
						Game.SetAvatarAccessorySlot(mVBody.Id, accessoryInventoryID, AvatarAccessorySlot.Undefined, 0f);
					}
				}
				return;
			}
		}
		MVBody body = MVGameController.Instance.WOCM.AvatarLocal.Body;
		if (body.HasAccessoryWithID(accessoryInventoryID))
		{
			Game.SetAvatarAccessorySlot(body.Id, accessoryInventoryID, AvatarAccessorySlot.Undefined, 0f);
		}
	}
}
