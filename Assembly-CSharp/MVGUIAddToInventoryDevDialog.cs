using System;
using System.Collections.Generic;
using UnityEngine;

public class MVGUIAddToInventoryDevDialog : UXCustomDialogBox
{
	public UXTextField nameTextField;

	public UXTextField woIdTextField;

	public UXToggleIconButton overwriteToggle;

	public UXText woIdInfo;

	public UXText parentInfo;

	public UXTextButton pickButton;

	public UXIconButton refreshInfoButton;

	public UXTextButton useParentButton;

	public UXComboBox itemTypeComboBox;

	public List<UXGroup> stepGroups;

	private int stepIndex;

	public UXTextButton nextStepButton;

	public UXTextButton prevStepButton;

	public UXTextButton finishButton;

	private bool _isInitialized;

	private MVWorldObjectClientManager WOCM => MVGameController.WOCM;

	public override void OnShowDialog()
	{
		base.OnShowDialog();
		if (!_isInitialized)
		{
			UXTextButton uXTextButton = nextStepButton;
			uXTextButton.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXTextButton.OnClick, (UXBaseButton.OnClickDelegate)(() =>
			{
				ChangeStep(1);
			}));
			UXTextButton uXTextButton2 = prevStepButton;
			uXTextButton2.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXTextButton2.OnClick, (UXBaseButton.OnClickDelegate)(() =>
			{
				ChangeStep(-1);
			}));
			UXTextButton uXTextButton3 = finishButton;
			uXTextButton3.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXTextButton3.OnClick, (UXBaseButton.OnClickDelegate)(() =>
			{
				if (CheckForError())
				{
					OnPositiveClose();
					DialogFactory.CloseDialog();
				}
			}));
			InitializeWoIDStep();
			itemTypeComboBox.Add(MVGameController.Game.ItemCategories.GetNames());
			_isInitialized = true;
		}
		itemTypeComboBox.Close();
		ChangeStep(0);
	}

	private void InitializeWoIDStep()
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
	}

	public override void OnCloseDialog()
	{
		base.OnCloseDialog();
		nameTextField.ReleaseFocus();
		woIdTextField.ReleaseFocus();
	}

	private void ChangeStep(int stepChange)
	{
		int index = stepIndex;
		stepIndex += stepChange;
		stepIndex = Mathf.Clamp(stepIndex, 0, stepGroups.Count - 1);
		stepGroups[index].Hide();
		stepGroups[stepIndex].Show();
		prevStepButton.SetVisible(stepIndex != 0);
		nextStepButton.SetVisible(stepIndex != stepGroups.Count - 1);
		finishButton.SetVisible(stepIndex == stepGroups.Count - 1);
		nameTextField.ReleaseFocus();
		woIdTextField.ReleaseFocus();
	}

	private void PickChild()
	{
		Dictionary<string, DialogData> dictionary = new Dictionary<string, DialogData>();
		dictionary.Add("PickText", new TextData
		{
			text = "Select Child"
		});
		DialogFactory.CreateCustomDevelopmentDialog("Prefabs/GUI/Dev Tools/PickDialog", string.Empty, noButtons: true, stackDialog: true).SetValues(dictionary).SetOnResultCallback(OnPickChildResponse)
			.Show();
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

	private bool CheckForError()
	{
		bool flag = false;
		string text = "You forgot these things:\n";
		if (nameTextField.Text == string.Empty)
		{
			text += "\nNo name entered.";
			flag = true;
		}
		if (woIdTextField.Text == string.Empty)
		{
			text += "\nNo woID entered.";
			flag = true;
		}
		if (itemTypeComboBox.CurrentlySelectedItem == null)
		{
			text += "\nNo ItemType chosen.";
			flag = true;
		}
		if (flag)
		{
			DialogFactory.CreateDevelopmentDialog(text, "Error", UXDialogType.Simple, noButtons: false, stackDialog: true).Show();
		}
		return !flag;
	}

	public override object GetResult()
	{
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary.Add("name", nameTextField.Text);
		dictionary.Add("woId", int.Parse(woIdTextField.Text));
		string text = (string)itemTypeComboBox.CurrentlySelectedItem.GetValue();
		dictionary.Add("itemCategory", MVGameController.Game.ItemCategories.NameToID(text));
		dictionary.Add("overWrite", overwriteToggle.ToggleState);
		return dictionary;
	}
}
