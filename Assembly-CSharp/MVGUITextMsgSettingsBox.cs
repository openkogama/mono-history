using System;
using System.Collections;
using UnityEngine;

public class MVGUITextMsgSettingsBox : UXCustomDialogBox
{
	public UXTextField textField;

	public UXSlider textSizeSlider;

	public UXText textSizeValueLabel;

	private string text;

	private float textSize;

	public override void OnShowDialog()
	{
		base.OnShowDialog();
		text = textField.Text;
		UpdateTextSize(textSizeSlider.Value);
		AddListeners();
	}

	private void AddListeners()
	{
		UXTextField uXTextField = textField;
		uXTextField.OnValueChanged = (UXTextInputElement.OnValueChangedDelegate)Delegate.Combine(uXTextField.OnValueChanged, (UXTextInputElement.OnValueChangedDelegate)((string textValue) =>
		{
			text = textValue;
			FireIntermediateResult();
		}));
		UXSlider uXSlider = textSizeSlider;
		uXSlider.OnValueChangedIntermediate = (UXSlider.OnValueChangedIntermediateDelegate)Delegate.Combine(uXSlider.OnValueChangedIntermediate, (UXSlider.OnValueChangedIntermediateDelegate)((UXSlider s, float v) =>
		{
			UpdateTextSize(v);
			FireIntermediateResult();
		}));
		UXSlider uXSlider2 = textSizeSlider;
		uXSlider2.OnValueChanged = (UXSlider.OnValueChangedDelegate)Delegate.Combine(uXSlider2.OnValueChanged, (UXSlider.OnValueChangedDelegate)((UXSlider s) =>
		{
			UpdateTextSize(s.Value);
		}));
	}

	private void UpdateTextSize(float textValue)
	{
		textSize = textValue;
		string text = string.Empty;
		if (textSize == 0.1f)
		{
			text = "Small";
		}
		else if (textSize == 0.2f)
		{
			text = "Medium";
		}
		else if (textSize == 0.3f)
		{
			text = "Large";
		}
		textSizeValueLabel.Text = text;
	}

	public override object GetResult()
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add("text", text);
		hashtable.Add("textSize", textSize);
		return hashtable;
	}

	public override void OnCloseDialog()
	{
		base.OnCloseDialog();
		(UXUtils.FindComponentInParents(typeof(UXView), ((Component)this).transform.parent) as UXView).ReleaseFocus();
	}
}
