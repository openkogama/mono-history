using System;
using Localize;
using MV.WorldObject;
using UnityEngine;

public class MVGUISellAvatarDialog : UXViewScript
{
	[SerializeField]
	private UXWindow window;

	[SerializeField]
	private UXTextField priceTextField;

	[SerializeField]
	private UXTextButton remove;

	[SerializeField]
	private UXTextButton sell;

	[SerializeField]
	private UXTextButton update;

	[SerializeField]
	private UXTextField avatarName;

	[SerializeField]
	private AvatarScreenShooter avatarScreenShooter;

	private UXDialogFactory dialogFactory;

	private MvAvatarMetaData avatarMetaData;

	private int woID = -1;

	public override void OnInitialize()
	{
		UXTextButton uXTextButton = remove;
		uXTextButton.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXTextButton.OnClick, new UXBaseButton.OnClickDelegate(RemoveClicked));
		UXTextButton uXTextButton2 = sell;
		uXTextButton2.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXTextButton2.OnClick, new UXBaseButton.OnClickDelegate(SellClicked));
		UXTextButton uXTextButton3 = update;
		uXTextButton3.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXTextButton3.OnClick, new UXBaseButton.OnClickDelegate(UpdateClicked));
		dialogFactory = UXUtils.FindGUIObjectOfType<UXDialogFactory>();
		UXWindow uXWindow = window;
		uXWindow.OnExitButtonClick = (UXWindow.OnExitButtonClickDelegate)Delegate.Combine(uXWindow.OnExitButtonClick, (UXWindow.OnExitButtonClickDelegate)(() =>
		{
			View.Hide();
		}));
	}

	public void SetAvatarMetaData(int woID, MvAvatarMetaData avatarMetaData)
	{
		this.avatarMetaData = avatarMetaData;
		this.woID = woID;
		if (avatarMetaData.priceSilver <= 0)
		{
			UXTextField uXTextField = priceTextField;
			int priceSilver = avatarMetaData.priceSilver;
			uXTextField.Text = priceSilver.ToString();
		}
		avatarName.Text = avatarMetaData.name;
	}

	public void SellClicked()
	{
		Debug.Log((object)"SellClick");
		int num = Convert.ToInt32(priceTextField.Text);
		if (num > 0 && avatarName.Text != string.Empty)
		{
			avatarScreenShooter.TakeScreenShot(ScreenShotCallback, ignoreAccessories: true);
		}
	}

	private void ScreenShotCallback(Texture2D screenshotTex)
	{
		Debug.Log((object)"Screenshot taken");
		int priceSilver = Convert.ToInt32(priceTextField.Text);
		dialogFactory.CreateCustomDialog("Prefabs/GUI/Avatar Action/AvatarMarketPlaceActionDialog", TextSlotIndex.SellOnMarketplace, noButtons: true, stackDialog: true, canClose: false).SetOnResultCallback(OnAddToMarketplaceReturn);
		(dialogFactory.CurrentlyBuildingDialogBox as MVGUIAvatarMarketPlaceActionDialog).SellAvatar(woID, priceSilver, avatarName.Text, screenshotTex.EncodeToPNG());
		dialogFactory.Show();
		View.Hide();
	}

	private void OnAddToMarketplaceReturn(UXDialogBox dialogBox)
	{
		TextSlotIndex messageIndex;
		if (dialogBox.DialogResult != UXDialogResult.Positive)
		{
			messageIndex = ((!avatarMetaData.isOnMarketPlace) ? TextSlotIndex.SellItemOnMarketplaceFailed : TextSlotIndex.UpdateItemOnMarketplaceFailed);
		}
		else
		{
			messageIndex = ((!avatarMetaData.isOnMarketPlace) ? TextSlotIndex.SellItemOnMarketplaceSuccessful : TextSlotIndex.UpdateItemOnMarketplaceSuccessful);
		}
		ShowResultDialog(messageIndex);
	}

	private void OnDeleteFromMarketplaceReturn(UXDialogBox dialogBox)
	{
		TextSlotIndex messageIndex = ((dialogBox.DialogResult != UXDialogResult.Positive) ? TextSlotIndex.RemoveItemFromMarketplaceFailed : TextSlotIndex.RemoveItemFromMarketplaceSuccessful);
		ShowResultDialog(messageIndex);
	}

	private void ShowResultDialog(TextSlotIndex messageIndex)
	{
		ValueInsert valueInsert = new ValueInsert();
		valueInsert.AddString(avatarName.Text);
		dialogFactory.CreateDialog(messageIndex, TextSlotIndex.Notice, UXDialogType.Simple, noButtons: false, stackDialog: true, canClose: true, valueInsert).Show();
	}

	public void RemoveClicked()
	{
		Debug.Log((object)"RemoveClick");
		dialogFactory.CreateCustomDialog("Prefabs/GUI/Avatar Action/AvatarMarketPlaceActionDialog", TextSlotIndex.RemoveFromMarketplace, noButtons: true, stackDialog: true, canClose: false).SetOnResultCallback(OnDeleteFromMarketplaceReturn);
		(dialogFactory.CurrentlyBuildingDialogBox as MVGUIAvatarMarketPlaceActionDialog).DeleteAvatar(woID);
		dialogFactory.Show();
		View.Hide();
	}

	public void UpdateClicked()
	{
		Debug.Log((object)"UpdateClick");
		SellClicked();
	}

	public override void OnHide()
	{
		base.OnHide();
		UXFullscreenColliderBox.Instance.RemoveBlockingObject(this);
	}

	public override void OnShow()
	{
		base.OnShow();
		if (avatarMetaData.isOnMarketPlace)
		{
			sell.SetVisible(visible: false);
			remove.SetVisible(visible: true);
			update.SetVisible(visible: true);
		}
		else
		{
			sell.SetVisible(visible: true);
			remove.SetVisible(visible: false);
			update.SetVisible(visible: false);
		}
		UXFullscreenColliderBox.Instance.AddBlockingObject(this);
	}

	private void CreateAndHide(float size)
	{
		View.Hide();
	}
}
