using System;
using System.Collections.Generic;

public class MVGUIDeleteWoidDevDialog : UXCustomDialogBox
{
	public UXTextField woIdTextField;

	public UXTextButton pickButton;

	public UXTextButton okButton;

	private bool _isInitialized;

	public override void OnShowDialog()
	{
		base.OnShowDialog();
		if (_isInitialized)
		{
			return;
		}
		UXTextButton uXTextButton = okButton;
		uXTextButton.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXTextButton.OnClick, (UXBaseButton.OnClickDelegate)(() =>
		{
			if (CheckForError())
			{
				OnPositiveClose();
				DialogFactory.CloseDialog();
			}
		}));
		UXTextButton uXTextButton2 = pickButton;
		uXTextButton2.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXTextButton2.OnClick, (UXBaseButton.OnClickDelegate)(() =>
		{
			PickChild();
		}));
		_isInitialized = true;
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
		}
	}

	private bool CheckForError()
	{
		if (woIdTextField.Text == string.Empty)
		{
			DialogFactory.CreateDevelopmentDialog("You forgot to enter a woid", "Error", UXDialogType.Simple, noButtons: false, stackDialog: true).Show();
			return false;
		}
		return true;
	}

	public override object GetResult()
	{
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary.Add("woId", int.Parse(woIdTextField.Text));
		return dictionary;
	}
}
