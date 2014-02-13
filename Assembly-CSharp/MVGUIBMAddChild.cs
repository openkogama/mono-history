using System;
using System.Collections;
using System.Collections.Generic;
using Localize;
using UnityEngine;

public class MVGUIBMAddChild : UXCustomDialogBox
{
	public UXTextField woidTextField;

	public UXTextButton pickButton;

	public UXTextField nameTextField;

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
			UXTextButton uXTextButton = pickButton;
			uXTextButton.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXTextButton.OnClick, (UXBaseButton.OnClickDelegate)(() =>
			{
				PickChild();
			}));
			UXTextButton uXTextButton2 = nextStep;
			uXTextButton2.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXTextButton2.OnClick, (UXBaseButton.OnClickDelegate)(() =>
			{
				ChangeStep(1);
			}));
			UXTextButton uXTextButton3 = prevStep;
			uXTextButton3.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXTextButton3.OnClick, (UXBaseButton.OnClickDelegate)(() =>
			{
				ChangeStep(-1);
			}));
			UXTextButton uXTextButton4 = finishButton;
			uXTextButton4.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXTextButton4.OnClick, (UXBaseButton.OnClickDelegate)(() =>
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
	}

	public override void OnCloseDialog()
	{
		base.OnCloseDialog();
		(UXUtils.FindComponentInParents(typeof(UXView), ((Component)this).transform.parent) as UXView).ReleaseFocus();
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
		woidTextField.ReleaseFocus();
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
			woidTextField.Text = num + string.Empty;
		}
	}

	private bool CheckForError()
	{
		bool flag = false;
		bool flag2 = false;
		if (nameTextField.Text == string.Empty)
		{
			flag = true;
		}
		if (woidTextField.Text == string.Empty)
		{
			flag2 = true;
		}
		bool flag3 = flag || flag2;
		if (flag3)
		{
			TextSlotIndex messageIndex = TextSlotIndex.ForgotNameAndWoid;
			if (!flag)
			{
				messageIndex = TextSlotIndex.ForgotWOid;
			}
			if (!flag2)
			{
				messageIndex = TextSlotIndex.ForgotName;
			}
			DialogFactory.CreateDialog(messageIndex, TextSlotIndex.ErrorHeadline, UXDialogType.Simple, noButtons: false, stackDialog: true).Show();
		}
		return !flag3;
	}

	public override object GetResult()
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add("name", nameTextField.Text);
		hashtable.Add("woId", int.Parse(woidTextField.Text.Split(new char[1] { '.' })[0]));
		return hashtable;
	}
}
