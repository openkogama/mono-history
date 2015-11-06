using System;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class MVGUIItemActionReselling : MVGUIItemAction
{
	private const float LOADING_CIRCLE_SPEED = 12f;

	private const int SILVER_PRICE_MINIMUM = 10;

	private const int SILVER_PRICE_DEFAULT = 500;

	public Color inputValidColor;

	public Color inputInvalidColor;

	public UXPlane nameValidToggle;

	public UXTextField nameTextField;

	public UXPlane silverPriceValidToggle;

	public UXTextField silverPriceTextField;

	public UXText goldPriceText;

	public UXPlane descriptionValidToggle;

	public UXTextBox descriptionTextBox;

	public UXText sellText;

	public UXTextButton removeFromMarketplaceButton;

	public UXTextButton sellItemButton;

	public UXText compareHeader;

	public MVGUICompareSlider compareSlider;

	public UXGroup loadingGroup;

	public UXPlane loadingCircle;

	public UXGroup CannotEditBGGroup;

	private bool doingCompare;

	private bool canSell;

	private bool differentEnough;

	private bool showRemoveButton;

	private bool itemUpdate;

	private UXDialogFactory dialogFactory;

	private bool UserIsAuthor => item.authorProfileID == MVGameControllerBase.Game.LocalPlayer.ProfileID;

	private bool ItemOnMarketplace => item.shopInventoryID != 0;

	public override void UpdateItemAction(MVItem item)
	{
		base.UpdateItemAction(item);
		if (ItemOnMarketplace && UserIsAuthor)
		{
			sellItemButton.Text = TM._("Update Item");
			showRemoveButton = true;
		}
		nameTextField.Text = item.name;
		if ((float)base.item.priceSilver == 0f)
		{
			base.item.priceSilver = 500;
		}
		silverPriceTextField.Text = string.Empty + item.priceSilver;
		goldPriceText.Text = string.Empty + item.priceGold;
		descriptionTextBox.Text = item.description;
		if (!UserIsAuthor)
		{
			DoCompare();
		}
		else
		{
			differentEnough = true;
		}
		ValidateInput();
	}

	protected override void Initialize()
	{
		UXTextField uXTextField = nameTextField;
		uXTextField.OnValueChanged = (UXTextInputElement.OnValueChangedDelegate)Delegate.Combine(uXTextField.OnValueChanged, (UXTextInputElement.OnValueChangedDelegate)((string value) =>
		{
			ValidateInput();
		}));
		UXTextField uXTextField2 = silverPriceTextField;
		uXTextField2.OnValueChanged = (UXTextInputElement.OnValueChangedDelegate)Delegate.Combine(uXTextField2.OnValueChanged, (UXTextInputElement.OnValueChangedDelegate)((string value) =>
		{
			ValidateInput();
		}));
		UXTextBox uXTextBox = descriptionTextBox;
		uXTextBox.OnValueChanged = (UXTextInputElement.OnValueChangedDelegate)Delegate.Combine(uXTextBox.OnValueChanged, (UXTextInputElement.OnValueChangedDelegate)((string value) =>
		{
			ValidateInput();
		}));
		UXTextButton uXTextButton = sellItemButton;
		uXTextButton.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXTextButton.OnClick, new UXBaseButton.OnClickDelegate(SellItem));
		UXTextButton uXTextButton2 = removeFromMarketplaceButton;
		uXTextButton2.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXTextButton2.OnClick, new UXBaseButton.OnClickDelegate(RemoveFromMarketplace));
		UXGroup actionGroup = ActionGroup;
		actionGroup.OnShowGroup = (UXGroup.OnGroupEventDelegate)Delegate.Combine(actionGroup.OnShowGroup, new UXGroup.OnGroupEventDelegate(OnShowResellGroup));
		dialogFactory = UXUtils.UXDialogFactory;
		base.Initialize();
	}

	private void OnShowResellGroup()
	{
		loadingGroup.SetVisible(doingCompare);
		compareSlider.SetVisible(!doingCompare);
		removeFromMarketplaceButton.SetVisible(showRemoveButton);
		sellItemButton.SetVisible(canSell && (UserIsAuthor || differentEnough));
		if (UserIsAuthor)
		{
			compareHeader.SetVisible(visible: false);
			compareSlider.SetVisible(visible: false);
		}
		CannotEditBGGroup.SetVisible(!differentEnough && !UserIsAuthor);
		silverPriceTextField.SetVisible(differentEnough || UserIsAuthor);
		nameTextField.SetVisible(differentEnough || UserIsAuthor);
		descriptionTextBox.SetVisible(differentEnough || UserIsAuthor);
	}

	private void DoCompare()
	{
		doingCompare = true;
		compareSlider.SetVisible(visible: false);
		loadingGroup.SetVisible(ActionGroup.Visible);
		MVGameControllerBase.Game.ReceivedItemFromQuery += OnLoadMarketPlaceItem;
		MVGameControllerBase.Game.RequestMarketPlaceItem(item.itemID);
	}

	private void OnLoadMarketPlaceItem(object sender, ReceivedItemFromQueryEventArgs e)
	{
		MVGameControllerBase.Game.ReceivedItemFromQuery -= OnLoadMarketPlaceItem;
		BytePacker koGaMaData = e.KoGaMaData;
		KoGaMaPackageClient koGaMaPackageClient = new KoGaMaPackageClient(new BytePacker(item.data), readRuntimeValues: false);
		KoGaMaPackageClient koGaMaPackageClient2 = new KoGaMaPackageClient(koGaMaData, readRuntimeValues: false);
		float num = KoGaMaPackageClient.Compare(koGaMaPackageClient2, koGaMaPackageClient);
		koGaMaPackageClient.Destroy();
		koGaMaPackageClient2.Destroy();
		compareSlider.SetVisible(ActionGroup.Visible);
		compareSlider.Ratio = 1f - num;
		compareSlider.RequiredRatio = 1f - CommonValues.CompareThreshold;
		if (num <= CommonValues.CompareThreshold)
		{
			differentEnough = true;
		}
		loadingGroup.SetVisible(visible: false);
		doingCompare = false;
		if (ActionGroup.Visible)
		{
			CannotEditBGGroup.SetVisible(!differentEnough && !UserIsAuthor);
			silverPriceTextField.SetVisible(differentEnough || UserIsAuthor);
			nameTextField.SetVisible(differentEnough || UserIsAuthor);
			descriptionTextBox.SetVisible(differentEnough || UserIsAuthor);
		}
	}

	private void OnDestroy()
	{
		MVGameControllerBase.Game.ReceivedItemFromQuery -= OnLoadMarketPlaceItem;
	}

	private void Update()
	{
		if (loadingCircle.Visible)
		{
			loadingCircle.transform.Rotate(Vector3.forward, 12f * Time.deltaTime * 57.29578f);
		}
		if (ActionGroup.Visible)
		{
			ValidateInput();
		}
	}

	private void ValidateInput()
	{
		bool flag = nameTextField.Text.Length > 0;
		bool flag2 = descriptionTextBox.Text.Length > 0;
		int result = 0;
		bool flag3 = int.TryParse(silverPriceTextField.Text, out result) && result >= 10;
		nameValidToggle.SetColor((!differentEnough || !flag) ? inputInvalidColor : inputValidColor, string.Empty);
		descriptionValidToggle.SetColor((!differentEnough || !flag2) ? inputInvalidColor : inputValidColor, string.Empty);
		silverPriceValidToggle.SetColor((!differentEnough || !flag3) ? inputInvalidColor : inputValidColor, string.Empty);
		canSell = flag && flag2 && flag3;
		string text;
		if (!differentEnough && !UserIsAuthor)
		{
			text = TM._("Your model is not different enough compared to the original model");
		}
		else
		{
			text = (canSell ? string.Empty : TM._("Please make sure all the required information is entered"));
		}
		sellText.Text = text;
		sellItemButton.SetVisible(canSell && (UserIsAuthor || differentEnough));
	}

	private void RemoveFromMarketplace()
	{
		dialogFactory.CreateCustomDialog("Prefabs/GUI/Item Action/MarketPlaceActionDialog", TM._("Remove Item"), noButtons: true, stackDialog: true, canClose: false).SetOnResultCallback(OnRemoveFromMarketplaceReturn);
		(dialogFactory.CurrentlyBuildingDialogBox as MVGUIMarketPlaceActionDialog).RemoveItem(item.itemID);
		dialogFactory.Show();
	}

	private void SellItem()
	{
		if (MVGameControllerBase.Game.LocalPlayer.Level < MVGameControllerBase.Game.MarketPlaceLevel)
		{
			dialogFactory.CreateDialog(TM._("You can not add to marketplace before reaching level: ") + MVGameControllerBase.Game.MarketPlaceLevel, string.Empty, UXDialogType.Simple, noButtons: true, stackDialog: true).Show();
			return;
		}
		string text = nameTextField.Text;
		string text2 = descriptionTextBox.Text;
		int silverPrice = int.Parse(silverPriceTextField.Text);
		itemUpdate = ItemOnMarketplace;
		dialogFactory.CreateCustomDialog("Prefabs/GUI/Item Action/MarketPlaceActionDialog", TM._("Sell Item"), noButtons: true, stackDialog: true, canClose: false).SetOnResultCallback(OnAddToMarketplaceReturn);
		(dialogFactory.CurrentlyBuildingDialogBox as MVGUIMarketPlaceActionDialog).SellItem(item.itemID, text, text2, silverPrice);
		dialogFactory.Show();
	}

	private void OnRemoveFromMarketplaceReturn(UXDialogBox dialogBox)
	{
		string txt;
		if (dialogBox.DialogResult == UXDialogResult.Positive)
		{
			MVGameControllerBase.Game.ItemBusinessLogic.GetItem(item.itemID).shopInventoryID = 0;
			MVGameControllerBase.Game.PlayerRepository.PlayerInventory[item.itemID].shopInventoryID = 0;
			txt = TM._("Removed '{0}' from marketplace.");
		}
		else
		{
			txt = TM._("Failed to remove '{0}' from marketplace.");
		}
		ShowResultDialog(txt);
	}

	private void OnAddToMarketplaceReturn(UXDialogBox dialogBox)
	{
		string txt;
		if (dialogBox.DialogResult == UXDialogResult.Positive)
		{
			string text = nameTextField.Text;
			string text2 = descriptionTextBox.Text;
			int priceSilver = int.Parse(silverPriceTextField.Text);
			item.name = text;
			item.description = text2;
			item.priceSilver = priceSilver;
			item.authorProfileID = MVGameControllerBase.Game.LocalPlayer.ProfileID;
			MVGameControllerBase.Game.ItemBusinessLogic.GetItem(item.itemID).name = text;
			txt = ((!itemUpdate) ? TM._("'{0}' is now on the marketplace.") : TM._("'{0}' updated on marketplace."));
		}
		else
		{
			txt = ((!itemUpdate) ? TM._("Failed to put '{0}' on the marketplace.") : TM._("Failed to update '{0}'."));
		}
		ShowResultDialog(txt);
	}

	private void ShowResultDialog(string txt)
	{
		ValueInsert valueInsert = new ValueInsert();
		valueInsert.AddString(item.name);
		dialogFactory.CreateDialog(TM.GetTextWithValues(txt, valueInsert), TM._("Notice"), UXDialogType.Simple, noButtons: false, stackDialog: true, canClose: true, valueInsert).SetOnResultCallback(OnCloseActionCompleteDialog).Show();
	}

	private void OnCloseActionCompleteDialog(UXDialogBox dialogBox)
	{
		FireOnActionCompleted();
	}
}
