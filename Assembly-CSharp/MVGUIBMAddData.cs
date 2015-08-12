using System;
using System.Collections.Generic;
using UnityEngine;

public class MVGUIBMAddData : UXCustomDialogBox
{
	public UXTextField nameTextField;

	public UXComboBox typeComboBox;

	public List<UXGroup> stepGroups;

	public UXTextButton prevStep;

	public UXTextButton nextStep;

	public UXTextButton finishButton;

	private int _currentStep;

	private bool _isInitialized;

	public override void OnShowDialog()
	{
		base.OnShowDialog();
		if (!_isInitialized)
		{
			UXTextButton uXTextButton = nextStep;
			uXTextButton.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXTextButton.OnClick, (UXBaseButton.OnClickDelegate)(() =>
			{
				ChangeStep(1);
			}));
			UXTextButton uXTextButton2 = prevStep;
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
			_isInitialized = true;
		}
		ChangeStep(0);
		typeComboBox.Close();
	}

	public override void OnCloseDialog()
	{
		base.OnCloseDialog();
		(UXUtils.FindComponentInParents(typeof(UXView), transform.parent) as UXView).ReleaseFocus();
	}

	public void ChangeStep(int stepChange)
	{
		int currentStep = _currentStep;
		_currentStep += stepChange;
		_currentStep = Mathf.Clamp(_currentStep, 0, stepGroups.Count - 1);
		stepGroups[currentStep].Hide();
		stepGroups[_currentStep].Show();
		prevStep.SetVisible(_currentStep != 0);
		nextStep.SetVisible(_currentStep != stepGroups.Count - 1);
		finishButton.SetVisible(_currentStep == stepGroups.Count - 1);
		nameTextField.ReleaseFocus();
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
		if (flag)
		{
			DialogFactory.CreateDialog("You forgot these things:\n\nNo name entered.", TM._("Error"), UXDialogType.Simple, noButtons: false, stackDialog: true).Show();
		}
		return !flag;
	}

	public override object GetResult()
	{
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary.Add("name", nameTextField.Text);
		string value = (string)typeComboBox.CurrentlySelectedItem.GetValue();
		dictionary.Add("type", (BlueprintManagerDataType)(int)Enum.Parse(typeof(BlueprintManagerDataType), value));
		return dictionary;
	}
}
