using System;
using System.Collections.Generic;
using UnityEngine;

public class MVGUIBlueprintManagerPickExisting : UXCustomDialogBox
{
	public UXTextField woIdTextField;

	public UXText woIdInfo;

	public UXText parentInfo;

	public UXTextButton pickButton;

	public UXIconButton refreshInfoButton;

	public UXTextButton useParentButton;

	public UXTextButton okButton;

	private bool _isInitialized;

	private MVWorldObjectClientManager WOCM => MVGameController.WOCM;

	public override void OnShowDialog()
	{
		base.OnShowDialog();
		if (!_isInitialized)
		{
			UXTextButton uXTextButton = pickButton;
			uXTextButton.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXTextButton.OnClick, (UXBaseButton.OnClickDelegate)(() =>
			{
				PickChild();
			}));
			UXIconButton uXIconButton = refreshInfoButton;
			uXIconButton.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXIconButton.OnClick, (UXBaseButton.OnClickDelegate)(() =>
			{
				RefreshInfo();
			}));
			UXTextButton uXTextButton2 = useParentButton;
			uXTextButton2.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXTextButton2.OnClick, (UXBaseButton.OnClickDelegate)(() =>
			{
				GoToParent();
			}));
			UXTextButton uXTextButton3 = okButton;
			uXTextButton3.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXTextButton3.OnClick, (UXBaseButton.OnClickDelegate)(() =>
			{
				OpenBluePrintOverview();
			}));
			_isInitialized = true;
		}
	}

	public override void OnCloseDialog()
	{
		base.OnCloseDialog();
		woIdTextField.ReleaseFocus();
	}

	private void PickChild()
	{
		Dictionary<string, DialogData> dictionary = new Dictionary<string, DialogData>();
		dictionary.Add("PickText", new TextData
		{
			text = "Select Blueprint"
		});
		DialogFactory.CreateCustomDialog("Prefabs/GUI/Dev Tools/PickDialog", string.Empty, noButtons: true, stackDialog: true).SetValues(dictionary).SetOnResultCallback(OnPickChildResponse)
			.Show();
		(DialogFactory.CurrentDialogBox as MVGUIPickDialog).SetPickType(typeof(MVBlueprintBase));
	}

	private void OnPickChildResponse(UXDialogBox dialogBox)
	{
		if (dialogBox.DialogResult == UXDialogResult.Positive)
		{
			int num = (int)dialogBox.GetResult();
			woIdTextField.Text = num + string.Empty;
			RefreshInfo();
		}
	}

	private void RefreshInfo()
	{
		int result = 0;
		string empty = string.Empty;
		string text = string.Empty;
		if (int.TryParse(woIdTextField.Text, out result))
		{
			MVWorldObjectClient worldObjectClient = WOCM.GetWorldObjectClient(result);
			if (worldObjectClient != null)
			{
				if (worldObjectClient.GroupId == -1)
				{
					woIdTextField.Text = string.Empty;
					woIdInfo.Text = "Can't choose WO Root Group";
					return;
				}
				empty = worldObjectClient.GetType().ToString();
				int num = FindParentID(worldObjectClient.Transform);
				if (num != -1)
				{
					MVWorldObjectClient worldObjectClient2 = WOCM.GetWorldObjectClient(num);
					text = ((worldObjectClient2.GroupId == -1) ? "WO Root Group" : worldObjectClient2.GetType().ToString());
				}
				else
				{
					text = "Parent is not valid wo";
				}
			}
			else
			{
				empty = "No wo with id: " + result;
			}
		}
		else
		{
			empty = "Failed to parse woid";
		}
		woIdInfo.Text = empty;
		parentInfo.Text = text;
	}

	private void GoToParent()
	{
		int result = 0;
		if (!int.TryParse(woIdTextField.Text, out result))
		{
			return;
		}
		MVWorldObjectClient worldObjectClient = WOCM.GetWorldObjectClient(result);
		if (worldObjectClient == null)
		{
			return;
		}
		int num = FindParentID(worldObjectClient.Transform);
		if (num != -1)
		{
			MVWorldObjectClient worldObjectClient2 = WOCM.GetWorldObjectClient(num);
			if (worldObjectClient2.GroupId != -1)
			{
				woIdTextField.Text = worldObjectClient2.Id + string.Empty;
				RefreshInfo();
			}
			else
			{
				DialogFactory.CreateDevelopmentDialog("Can't use root group", "Error", UXDialogType.Simple, noButtons: false, stackDialog: true).Show();
			}
		}
	}

	private int FindParentID(Transform t)
	{
		if (t.parent != null)
		{
			MVWorldObjectClient worldObjectByGoId = WOCM.GetWorldObjectByGoId(t.parent.gameObject.GetInstanceID());
			if (worldObjectByGoId != null)
			{
				return worldObjectByGoId.Id;
			}
		}
		return -1;
	}

	private void OpenBluePrintOverview()
	{
		int pickedWo = 0;
		if (!ValidateWOID(out pickedWo))
		{
			DialogFactory.CreateDevelopmentDialog("Woid is not valid or not blueprint", "Error", UXDialogType.Simple, noButtons: false, stackDialog: true).Show();
			return;
		}
		(UXUtils.FindComponentInParents(typeof(UXView), transform.parent) as UXView).ReleaseFocus();
		OnPositiveClose();
		DialogFactory.CloseDialog();
		DialogFactory.CreateCustomDevelopmentDialog("Prefabs/GUI/Dev Tools/BlueprintManager/BlueprintManagerOverview", "Blueprint Manager", noButtons: true).Show();
		(DialogFactory.CurrentDialogBox as MVGUIBlueprintManagerOverview).SetWoid(pickedWo);
	}

	private bool ValidateWOID(out int pickedWo)
	{
		if (int.TryParse(woIdTextField.Text, out pickedWo))
		{
			MVWorldObjectClient worldObjectClient = WOCM.GetWorldObjectClient(pickedWo);
			if (worldObjectClient != null)
			{
				return typeof(MVBlueprintBase).IsAssignableFrom(worldObjectClient.GetType());
			}
		}
		return false;
	}
}
