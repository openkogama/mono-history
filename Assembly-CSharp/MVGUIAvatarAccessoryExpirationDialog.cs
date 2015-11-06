using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class MVGUIAvatarAccessoryExpirationDialog : UXCustomDialogBox
{
	public UXTextButton removeButton;

	public UXTextButton okButton;

	public UXText itemNameText;

	public Transform previewRoot;

	private ProductInventoryInfo productInventoryInfo;

	private StreamingAssetInfo streamingAssetInfo;

	private int newInventoryID;

	private bool _isInitialized;

	private MVNetworkGame Game => MVGameControllerBase.Game;

	public override void OnShowDialog()
	{
		base.OnShowDialog();
		if (!_isInitialized)
		{
			UXTextButton uXTextButton = removeButton;
			uXTextButton.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXTextButton.OnClick, (UXBaseButton.OnClickDelegate)(() =>
			{
				OnNegativeClose();
				DialogFactory.CloseDialog();
			}));
			UXTextButton uXTextButton2 = okButton;
			uXTextButton2.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXTextButton2.OnClick, (UXBaseButton.OnClickDelegate)(() =>
			{
				OnNegativeClose();
				DialogFactory.CloseDialog();
			}));
			_isInitialized = true;
		}
		okButton.SetVisible(visible: false);
	}

	public void BuildExpirationDialog(ProductInventoryInfo productInventoryInfo)
	{
		this.productInventoryInfo = productInventoryInfo;
		streamingAssetInfo = productInventoryInfo.ProductInfo;
		if (Game.StreamingAssetShopInventory.Contains(streamingAssetInfo.ProductID))
		{
			okButton.SetVisible(visible: false);
		}
		else
		{
			okButton.SetVisible(visible: true);
			removeButton.SetVisible(visible: false);
		}
		itemNameText.Text = streamingAssetInfo.Name;
		MVGUIAvatarAccessoryShopPreview component = (UnityEngine.Object.Instantiate(Resources.Load("Prefabs/GUI/ShopPreview/AvatarAccessoryShopPreview")) as GameObject).GetComponent<MVGUIAvatarAccessoryShopPreview>();
		component.CreateNewViewItem(streamingAssetInfo);
		component.transform.parent = previewRoot;
		component.transform.localScale = Vector3.one;
		component.transform.localPosition = Vector3.zero;
	}

	public override object GetResult()
	{
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary.Add("oldInventoryID", productInventoryInfo.InventoryID);
		dictionary.Add("newInventoryID", newInventoryID);
		return dictionary;
	}

	private void OpenShopDialog()
	{
		UXDialogFactory uXDialogFactory = UXUtils.UXDialogFactory;
		uXDialogFactory.CreateCustomDialog("Prefabs/GUI/Dialogs/AvatarAccessoryShopDialog", string.Empty, noButtons: true, stackDialog: true).SetOnResultCallback(OnPurchaseDialogResult).SetValues(BuildDialogData())
			.Show();
		MVGUIAvatarAccessoryShopDialog mVGUIAvatarAccessoryShopDialog = (MVGUIAvatarAccessoryShopDialog)uXDialogFactory.CurrentDialogBox;
		mVGUIAvatarAccessoryShopDialog.BuildShopDialogForRentRenewal(streamingAssetInfo, productInventoryInfo.InventoryID);
	}

	private Dictionary<string, DialogData> BuildDialogData()
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

	private void OnPurchaseDialogResult(UXDialogBox dialogBox)
	{
		if (dialogBox.DialogResult == UXDialogResult.Positive)
		{
			Dictionary<object, object> dictionary = (Dictionary<object, object>)dialogBox.GetResult();
			bool flag = (bool)dictionary["isRenting"];
			int num = (int)dictionary[(byte)73];
			long ticks = (long)dictionary[(byte)83];
			DateTime purchaseTime = new DateTime(ticks);
			if (!Game.StreamingAssetInventory.Contains(num))
			{
				AddToInventory(num, purchaseTime, dictionary, flag);
			}
			if (flag)
			{
				HandleRent(num, purchaseTime, dictionary);
			}
			newInventoryID = num;
			OnPositiveClose();
			DialogFactory.CloseDialog();
		}
	}

	private void HandleRent(int invID, DateTime purchaseTime, Dictionary<object, object> purchaseResponse)
	{
		int rentExpireSeconds = (int)purchaseResponse[(byte)80];
		ProductInventoryInfo productInventoryInfo = Game.StreamingAssetInventory.Get(invID);
		productInventoryInfo.Renew(purchaseTime, rentExpireSeconds);
		InventoryExpirationInfo expirationInfo = Game.StreamingAssetExpirationChecker.GetExpirationInfo(invID);
		if (expirationInfo != null)
		{
			expirationInfo.Renew(purchaseTime, rentExpireSeconds);
			return;
		}
		expirationInfo = new InventoryExpirationInfo(MVProductType.StreamingAsset, invID, ProductExpirationState.Expiring, purchaseTime, rentExpireSeconds);
		Game.StreamingAssetExpirationChecker.AddExpirationInfo(expirationInfo);
	}

	private void AddToInventory(int invID, DateTime purchaseTime, Dictionary<object, object> purchaseResponse, bool isRenting)
	{
		ProductInventoryInfo invInfo = new ProductInventoryInfo(invID, streamingAssetInfo, purchaseTime, isRenting);
		Game.StreamingAssetInventory.Add(invInfo);
		Game.StreamingAssetInventory.NotifyProductInventoryChange();
	}
}
