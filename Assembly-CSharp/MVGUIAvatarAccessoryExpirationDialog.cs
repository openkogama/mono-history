using System;
using System.Collections;
using System.Collections.Generic;
using Localize;
using MV.Common;
using UnityEngine;

public class MVGUIAvatarAccessoryExpirationDialog : UXCustomDialogBox
{
	public UXTextButton extendButton;

	public UXTextButton removeButton;

	public UXTextButton okButton;

	public UXText expirationText;

	public UXText itemNameText;

	public Transform previewRoot;

	private ProductInventoryInfo<StreamingAssetInfo> productInventoryInfo;

	private StreamingAssetInfo streamingAssetInfo;

	private int newInventoryID;

	private bool _isInitialized;

	private MVNetworkGame Game => MVGameController.Instance.Game;

	public override void OnShowDialog()
	{
		base.OnShowDialog();
		if (!_isInitialized)
		{
			UXTextButton uXTextButton = extendButton;
			uXTextButton.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXTextButton.OnClick, new UXBaseButton.OnClickDelegate(OpenShopDialog));
			UXTextButton uXTextButton2 = removeButton;
			uXTextButton2.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXTextButton2.OnClick, (UXBaseButton.OnClickDelegate)(() =>
			{
				OnNegativeClose();
				DialogFactory.CloseDialog();
			}));
			UXTextButton uXTextButton3 = okButton;
			uXTextButton3.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXTextButton3.OnClick, (UXBaseButton.OnClickDelegate)(() =>
			{
				OnNegativeClose();
				DialogFactory.CloseDialog();
			}));
			_isInitialized = true;
		}
		okButton.SetVisible(visible: false);
	}

	public void BuildExpirationDialog(ProductInventoryInfo<StreamingAssetInfo> productInventoryInfo)
	{
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		this.productInventoryInfo = productInventoryInfo;
		streamingAssetInfo = productInventoryInfo.ProductInfo;
		if (Game.StreamingAssetShopInventory.Contains(streamingAssetInfo.ProductID))
		{
			okButton.SetVisible(visible: false);
		}
		else
		{
			expirationText.index = TextSlotIndex.Empty;
			expirationText.Text = Localization.Instance.GetText(TextSlotIndex.ItemExpiredCantExtend);
			okButton.SetVisible(visible: true);
			extendButton.SetVisible(visible: false);
			removeButton.SetVisible(visible: false);
		}
		itemNameText.Text = streamingAssetInfo.Name;
		Object val = Object.Instantiate(Resources.Load("Prefabs/GUI/ShopPreview/AvatarAccessoryShopPreview"));
		MVGUIAvatarAccessoryShopPreview component = ((GameObject)((val is GameObject) ? val : null)).GetComponent<MVGUIAvatarAccessoryShopPreview>();
		component.CreateNewViewItem(streamingAssetInfo);
		((Component)component).transform.parent = previewRoot;
		((Component)component).transform.localScale = Vector3.one;
		((Component)component).transform.localPosition = Vector3.zero;
	}

	public override object GetResult()
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add("oldInventoryID", productInventoryInfo.InventoryID);
		hashtable.Add("newInventoryID", newInventoryID);
		return hashtable;
	}

	private void OpenShopDialog()
	{
		UXDialogFactory uXDialogFactory = UXUtils.FindGUIObjectOfType<UXDialogFactory>();
		uXDialogFactory.CreateCustomDialog("Prefabs/GUI/Dialogs/AvatarAccessoryShopDialog", TextSlotIndex.Empty, noButtons: true, stackDialog: true).SetOnResultCallback(OnPurchaseDialogResult).SetValues(BuildDialogData())
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
		Object val = Object.Instantiate(Resources.Load("Prefabs/GUI/ShopPreview/AvatarAccessoryShopPreview"));
		MVGUIAvatarAccessoryShopPreview component = ((GameObject)((val is GameObject) ? val : null)).GetComponent<MVGUIAvatarAccessoryShopPreview>();
		component.CreateNewViewItem(streamingAssetInfo);
		dictionary.Add("AccessoryPreview", new ProductPreviewData
		{
			productPreview = ((Component)component).gameObject
		});
		return dictionary;
	}

	private void OnPurchaseDialogResult(UXDialogBox dialogBox)
	{
		if (dialogBox.DialogResult == UXDialogResult.Positive)
		{
			Hashtable hashtable = (Hashtable)dialogBox.GetResult();
			bool flag = (bool)hashtable["isRenting"];
			int num = (int)hashtable[(byte)74];
			long ticks = (long)hashtable[(byte)84];
			DateTime purchaseTime = new DateTime(ticks);
			if (!Game.StreamingAssetInventory.Contains(num))
			{
				AddToInventory(num, purchaseTime, hashtable, flag);
			}
			if (flag)
			{
				HandleRent(num, purchaseTime, hashtable);
			}
			newInventoryID = num;
			OnPositiveClose();
			DialogFactory.CloseDialog();
		}
	}

	private void HandleRent(int invID, DateTime purchaseTime, Hashtable purchaseResponse)
	{
		int rentExpireSeconds = (int)purchaseResponse[(byte)81];
		ProductInventoryInfo<StreamingAssetInfo> productInventoryInfo = Game.StreamingAssetInventory.Get(invID);
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

	private void AddToInventory(int invID, DateTime purchaseTime, Hashtable purchaseResponse, bool isRenting)
	{
		ProductInventoryInfo<StreamingAssetInfo> invInfo = new ProductInventoryInfo<StreamingAssetInfo>(invID, streamingAssetInfo, purchaseTime, isRenting);
		Game.StreamingAssetInventory.Add(invInfo);
		Game.StreamingAssetInventory.NotifyProductInventoryChange();
	}
}
