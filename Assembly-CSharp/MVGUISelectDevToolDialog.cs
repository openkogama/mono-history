using System;
using System.Collections;
using System.Collections.Generic;
using Localize;
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
		Debug.Log((object)"OpenImageOfWOButtion");
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
		Hashtable hashtable = (Hashtable)dialogBox.GetResult();
		string name = (string)hashtable["name"];
		int woId = (int)hashtable["woId"];
		int itemCategory = (int)hashtable["itemCategory"];
		bool overWrite = (bool)hashtable["overWrite"];
		string message = string.Format("Add with following data?:\n\nItemTypeName: {0}\nID: {1}\nItemCategory: {2}\nOverWrite: {3}", new object[4]
		{
			name,
			woId,
			MVGameController.Instance.Game.ItemCategories.IDToName(itemCategory),
			overWrite
		});
		DialogFactory.CreateDevelopmentDialog(message, "Notice", UXDialogType.Simple, noButtons: false, stackDialog: false, canClose: false).AddPositiveButton(TextSlotIndex.Confirm).AddNegativeButton(TextSlotIndex.Reject)
			.SetOnResultCallback((UXDialogBox dialog) =>
			{
				if (dialog.DialogResult == UXDialogResult.Positive)
				{
					Debug.Log((object)string.Format("Adding to INV with name: {0} - id: {1} - itemType: {2} - overWrite: {3}", new object[4] { name, woId, itemCategory, overWrite }));
					Action<byte[]> callback = (byte[] imageData) =>
					{
						MVGameController.Instance.Game.AddWorldObjectToInventorDev(woId, imageData, name, itemCategory, overWrite);
					};
					Coroutines.StartCoroutine(AEditController.CreateTextureFromData(MVGameController.Instance.WOCM.GetWorldObjectClient(woId), callback));
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
		Hashtable hashtable = (Hashtable)dialogBox.GetResult();
		int woId = (int)hashtable["woId"];
		DialogFactory.CreateDevelopmentDialog("Are you sure you want to delete the following woid:\n\n" + woId, "Delete?").AddPositiveButton(TextSlotIndex.Confirm).AddNegativeButton(TextSlotIndex.Reject)
			.SetOnResultCallback((UXDialogBox dialog) =>
			{
				if (dialog.DialogResult == UXDialogResult.Positive)
				{
					HashSet<MVWorldObjectClient> deleteSet = new HashSet<MVWorldObjectClient> { MVGameController.Instance.WOCM.GetWorldObjectClient(woId) };
					MVGameController.Instance.EditController.Delete(deleteSet);
				}
			})
			.Show();
	}
}
