using System;
using System.Collections.Generic;
using System.Linq;
using MV.Common;
using UnityEngine;

public class MVGUIAvatarAccessoryExpirationHandler : MonoBehaviour
{
	public float dialogWaitDelay = 5f;

	private Queue<InventoryExpirationInfo> expiringAccessories = new Queue<InventoryExpirationInfo>();

	private int inventoryIDOfOpenExpirationDialog = -1;

	private float lastDialogTime;

	private bool subscribedToAvatar;

	private MVNetworkGame Game => MVGameControllerBase.Game;

	private MVWorldObjectClientManager WOCM => MVGameControllerBase.WOCM;

	private UXDialogFactory DialogFactory => UXUtils.UXDialogFactory;

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
		if (!subscribedToAvatar && MVGameControllerBase.JoinState == MVJoinState.Playing)
		{
			WOCM.AvatarLocal.Respawned += AvatarController_Respawned;
			subscribedToAvatar = true;
		}
		if (expiringAccessories.Count != 0 && !(Time.time - lastDialogTime < dialogWaitDelay))
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
		ProcessOneFromExpiredQueue();
	}

	private bool CanShowPopup(InventoryExpirationInfo expirationInfo)
	{
		return MVGameControllerBase.GameMode switch
		{
			MVGameMode.Edit => CanShowExpirationPopup(MVGameControllerLegacyUI.EditorController, expirationInfo), 
			MVGameMode.Play => CanShowExpirationPopup(MVGameControllerLegacyUI.PlayController, expirationInfo), 
			MVGameMode.CharacterEditor => CanShowExpirationPopup(MVGameControllerLegacyUI.CharacterEditorController, expirationInfo), 
			_ => throw new ArgumentOutOfRangeException(), 
		};
	}

	private bool CanShowExpirationPopup(PlayControllerBase playController, InventoryExpirationInfo expirationInfo)
	{
		if (!(DialogFactory.CurrentDialogBox == null) || playController.IsMenuShown() || Cursor.lockState == CursorLockMode.Locked)
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
		if (DialogFactory.CurrentDialogBox != null || playController.IsMenuShown())
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
		return DialogFactory.CurrentDialogBox == null && !playController.IsAvatarShopShown();
	}

	private void ShowExpirationPopup(InventoryExpirationInfo expirationInfo)
	{
		if (!expirationInfo.IsExpiredNotRenewed)
		{
			ProductInventoryInfo productInventoryInfo = Game.StreamingAssetInventory.Get(expirationInfo.InventoryID);
			if (productInventoryInfo == null)
			{
				Debug.LogWarning("Expired accessory " + expirationInfo.InventoryID + " that is not in inventory, remove from any body it is attached on");
				RemoveAccessoryFromPlayerBodies(expirationInfo.InventoryID);
			}
			else if (productInventoryInfo.ProductInfo.ShopInfo == null)
			{
				inventoryIDOfOpenExpirationDialog = expirationInfo.InventoryID;
				DialogFactory.CreateCustomDialog("Prefabs/GUI/AvatarAccessory/AvatarAccessoryExpirationDialog", TM._("Rent Period Expired"), noButtons: true, stackDialog: true, canClose: false).SetOnResultCallback(OnExpirationPopupReturn).Show();
				(DialogFactory.CurrentDialogBox as MVGUIAvatarAccessoryExpirationDialog).BuildExpirationDialog(productInventoryInfo);
			}
			else
			{
				DialogFactory.CreateCustomDialog("Prefabs/GUI/Dialogs/AvatarAccessoryShopDialog", string.Empty, noButtons: true, stackDialog: true, canClose: false).SetOnResultCallback(OnExpirationPopupReturn).SetValues(BuildDialogData(productInventoryInfo.ProductInfo))
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
		MVGUIAvatarAccessoryShopPreview component = (UnityEngine.Object.Instantiate(Resources.Load("Prefabs/GUI/ShopPreview/AvatarAccessoryShopPreview")) as GameObject).GetComponent<MVGUIAvatarAccessoryShopPreview>();
		component.CreateNewViewItem(streamingAssetInfo);
		dictionary.Add("AccessoryPreview", new ProductPreviewData
		{
			productPreview = component.gameObject
		});
		return dictionary;
	}

	private void OnExpirationPopupReturn(UXDialogBox dialogBox)
	{
		inventoryIDOfOpenExpirationDialog = -1;
		Dictionary<object, object> dictionary = (Dictionary<object, object>)dialogBox.GetResult();
		int num = (int)dictionary["oldInventoryID"];
		if (dialogBox.DialogResult == UXDialogResult.Positive)
		{
			int num2 = (int)dictionary["newInventoryID"];
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
		ProductInventoryInfo productInventoryInfo = Game.StreamingAssetInventory.Get(inventoryID);
		if (productInventoryInfo == null)
		{
			Debug.LogWarning($"Item with inventoryID '{inventoryID}' was removed from inventory, but not from avatar");
		}
		else
		{
			Game.ExpireAvatarAccessory(inventoryID, GetBodyIDOnEquippedItem(inventoryID));
		}
	}

	private int GetBodyIDOnEquippedItem(int inventoryID)
	{
		int result = 0;
		if (MVGameControllerBase.GameMode == MVGameMode.CharacterEditor)
		{
			CharacterEditorController characterEditorController = MVGameControllerLegacyUI.IngameController as CharacterEditorController;
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
			result = MVGameControllerBase.WOCM.AvatarLocal.Body.Id;
		}
		return result;
	}

	private void RemoveAccessoryFromPlayerBodies(int accessoryInventoryID)
	{
		if (MVGameControllerBase.GameMode == MVGameMode.CharacterEditor)
		{
			CharacterEditorController characterEditorController = MVGameControllerLegacyUI.IngameController as CharacterEditorController;
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
		MVBody body = MVGameControllerBase.WOCM.AvatarLocal.Body;
		if (body.HasAccessoryWithID(accessoryInventoryID))
		{
			Game.SetAvatarAccessorySlot(body.Id, accessoryInventoryID, AvatarAccessorySlot.Undefined, 0f);
		}
	}
}
