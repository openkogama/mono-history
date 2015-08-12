using System;
using System.Collections.Generic;
using UnityEngine;

public class MVGUISelectDevToolDialog : UXCustomDialogBox
{
	public UXTextButton openBlueprintButton;

	public UXTextButton openAddToInventoryButton;

	public UXTextButton openDeleteButton;

	public UXTextButton openImageOfWOButton;

	private bool _isInitialized;

	public override void OnShowDialog()
	{
		base.OnShowDialog();
		if (!_isInitialized)
		{
			UXTextButton uXTextButton = openBlueprintButton;
			uXTextButton.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXTextButton.OnClick, (UXBaseButton.OnClickDelegate)(() =>
			{
				OpenBlueprintManager();
			}));
			UXTextButton uXTextButton2 = openAddToInventoryButton;
			uXTextButton2.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXTextButton2.OnClick, (UXBaseButton.OnClickDelegate)(() =>
			{
				OpenAddToInventory();
			}));
			UXTextButton uXTextButton3 = openDeleteButton;
			uXTextButton3.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXTextButton3.OnClick, (UXBaseButton.OnClickDelegate)(() =>
			{
				OpenDeleteWoid();
			}));
			UXTextButton uXTextButton4 = openImageOfWOButton;
			uXTextButton4.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXTextButton4.OnClick, (UXBaseButton.OnClickDelegate)(() =>
			{
				OpenImageOfWOButtion();
			}));
			_isInitialized = true;
		}
	}

	private void OpenImageOfWOButtion()
	{
		Debug.Log("OpenImageOfWOButtion");
		DialogFactory.CloseDialog();
		DialogFactory.CreateCustomDevelopmentDialog("Prefabs/GUI/Dev Tools/ImageOfWO", "Image Of WO", noButtons: true).Show();
	}

	private void OpenBlueprintManager()
	{
		DialogFactory.CloseDialog();
		DialogFactory.CreateCustomDevelopmentDialog("Prefabs/GUI/Dev Tools/BlueprintManager/BlueprintManagerEntry", "Blueprint Manager", noButtons: true).Show();
	}

	private void OpenAddToInventory()
	{
		DialogFactory.CloseDialog();
		DialogFactory.CreateCustomDevelopmentDialog("Prefabs/GUI/Dev Tools/AddToInventoryDevDialog", "Add To Inventory", noButtons: true).SetOnResultCallback(OnAddToInventoryResult).Show();
	}

	private void OnAddToInventoryResult(UXDialogBox dialogBox)
	{
		if (dialogBox.DialogResult != UXDialogResult.Positive)
		{
			return;
		}
		Dictionary<object, object> dictionary = (Dictionary<object, object>)dialogBox.GetResult();
		string name = (string)dictionary["name"];
		int woId = (int)dictionary["woId"];
		int itemCategory = (int)dictionary["itemCategory"];
		bool overWrite = (bool)dictionary["overWrite"];
		string message = $"Add with following data?:\n\nItemTypeName: {name}\nID: {woId}\nItemCategory: {MVGameController.Game.ItemCategories.IDToName(itemCategory)}\nOverWrite: {overWrite}";
		DialogFactory.CreateDevelopmentDialog(message, "Notice", UXDialogType.Simple, noButtons: false, stackDialog: false, canClose: false).AddPositiveButton(TM._("Yes")).AddNegativeButton(TM._("No"))
			.SetOnResultCallback((UXDialogBox dialog) =>
			{
				if (dialog.DialogResult == UXDialogResult.Positive)
				{
					Debug.Log($"Adding to INV with name: {name} - id: {woId} - itemType: {itemCategory} - overWrite: {overWrite}");
					Action<byte[]> callback = (byte[] imageData) =>
					{
						MVGameController.Game.AddWorldObjectToInventorDev(woId, imageData, name, itemCategory, overWrite);
					};
					Coroutines.StartCoroutine(ImageGenerator.CreateTextureFromData(MVGameController.WOCM.GetWorldObjectClient(woId), callback));
				}
			})
			.Show();
	}

	private void OpenDeleteWoid()
	{
		DialogFactory.CloseDialog();
		DialogFactory.CreateCustomDevelopmentDialog("Prefabs/GUI/Dev Tools/DeleteWoidDevDialog", "Delete Woid", noButtons: true).SetOnResultCallback(OnDeleteWoidResult).Show();
	}

	private void OnDeleteWoidResult(UXDialogBox dialogBox)
	{
		if (dialogBox.DialogResult != UXDialogResult.Positive)
		{
			return;
		}
		Dictionary<object, object> dictionary = (Dictionary<object, object>)dialogBox.GetResult();
		int woId = (int)dictionary["woId"];
		DialogFactory.CreateDevelopmentDialog("Are you sure you want to delete the following woid:\n\n" + woId, "Delete?").AddPositiveButton(TM._("Yes")).AddNegativeButton(TM._("No"))
			.SetOnResultCallback((UXDialogBox dialog) =>
			{
				if (dialog.DialogResult == UXDialogResult.Positive)
				{
					HashSet<MVWorldObjectClient> deleteSet = new HashSet<MVWorldObjectClient> { MVGameController.WOCM.GetWorldObjectClient(woId) };
					MVGameController.EditorController.Delete(deleteSet);
				}
			})
			.Show();
	}
}
