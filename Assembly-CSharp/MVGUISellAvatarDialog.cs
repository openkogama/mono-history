using System;
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
		dialogFactory = UXUtils.UXDialogFactory;
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
			priceTextField.Text = avatarMetaData.priceSilver.ToString();
		}
		avatarName.Text = avatarMetaData.name;
	}

	public void SellClicked()
	{
		Debug.Log("SellClick");
		int num = Convert.ToInt32(priceTextField.Text);
		if (num > 0 && avatarName.Text != string.Empty)
		{
			avatarScreenShooter.TakeScreenShot(ScreenShotCallback, ignoreAccessories: true);
		}
	}

	private void ScreenShotCallback(Texture2D screenshotTex)
	{
		Debug.Log("Screenshot taken");
		int priceSilver = Convert.ToInt32(priceTextField.Text);
		dialogFactory.CreateCustomDialog("Prefabs/GUI/Avatar Action/AvatarMarketPlaceActionDialog", TM._("Sell Item"), noButtons: true, stackDialog: true, canClose: false).SetOnResultCallback(OnAddToMarketplaceReturn);
		(dialogFactory.CurrentlyBuildingDialogBox as MVGUIAvatarMarketPlaceActionDialog).SellAvatar(woID, priceSilver, avatarName.Text, screenshotTex.EncodeToPNG());
		dialogFactory.Show();
		View.Hide();
	}

	private void OnAddToMarketplaceReturn(UXDialogBox dialogBox)
	{
		string txt;
		if (dialogBox.DialogResult != UXDialogResult.Positive)
		{
			txt = ((!avatarMetaData.isOnMarketPlace) ? "Failed to put '{0}' on the marketplace." : "Failed to update '{0}'.");
		}
		else
		{
			txt = ((!avatarMetaData.isOnMarketPlace) ? "'{0}' is now on the marketplace." : "'{0}' updated on marketplace.");
		}
		ShowResultDialog(txt);
	}

	private void OnDeleteFromMarketplaceReturn(UXDialogBox dialogBox)
	{
		string txt = ((dialogBox.DialogResult != UXDialogResult.Positive) ? TM._("Failed to remove '{0}' from marketplace.") : TM._("Removed '{0}' from marketplace."));
		ShowResultDialog(txt);
	}

	private void ShowResultDialog(string txt)
	{
		Debug.Log(txt);
		ValueInsert valueInsert = new ValueInsert();
		valueInsert.AddString(avatarName.Text);
		dialogFactory.CreateDialog(TM.GetTextWithValues(txt, valueInsert), TM._("Notice"), UXDialogType.Simple, noButtons: false, stackDialog: true, canClose: true, valueInsert).Show();
	}

	public void RemoveClicked()
	{
		Debug.Log("RemoveClick");
		dialogFactory.CreateCustomDialog("Prefabs/GUI/Avatar Action/AvatarMarketPlaceActionDialog", TM._("Remove Item"), noButtons: true, stackDialog: true, canClose: false).SetOnResultCallback(OnDeleteFromMarketplaceReturn);
		(dialogFactory.CurrentlyBuildingDialogBox as MVGUIAvatarMarketPlaceActionDialog).DeleteAvatar(woID);
		dialogFactory.Show();
		View.Hide();
	}

	public void UpdateClicked()
	{
		Debug.Log("UpdateClick");
		SellClicked();
	}

	public override void OnHide()
	{
		base.OnHide();
		UXFullscreenColliderBox.Instance.RemoveBlockingObject(this);
		MVInputWrapper.ignoreAllKeys = false;
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
