using System;
using System.Collections.Generic;
using UnityEngine;

public class MVGUIWaterPlaneSettingsBox : UXCustomDialogBox
{
	public UXPlane colorCube;

	public UXSlider waterRedSlider;

	public UXSlider waterGreenSlider;

	public UXSlider waterBlueSlider;

	public UXGroup modifierSettingsGroup;

	public UXText currentModifierText;

	public UXTextButton changeModifierButton;

	private float wr;

	private float wg;

	private float wb;

	private int modifierPackageType;

	private bool _isInitialized;

	public bool IsPreset { get; set; }

	public override void OnShowDialog()
	{
		base.OnShowDialog();
		wr = waterRedSlider.Value;
		wg = waterGreenSlider.Value;
		wb = waterBlueSlider.Value;
		modifierPackageType = (int)Enum.Parse(typeof(AvatarModifierPackageType), currentModifierText.Text);
		AddListeners();
		UpdateColor();
		modifierSettingsGroup.SetVisible(IsPreset);
	}

	private void AddListeners()
	{
		if (!_isInitialized)
		{
			UXSlider uXSlider = waterRedSlider;
			uXSlider.OnValueChangedIntermediate = (UXSlider.OnValueChangedIntermediateDelegate)Delegate.Combine(uXSlider.OnValueChangedIntermediate, (UXSlider.OnValueChangedIntermediateDelegate)((UXSlider s, float v) =>
			{
				wr = v;
				UpdateColor();
				FireIntermediateResult();
			}));
			UXSlider uXSlider2 = waterGreenSlider;
			uXSlider2.OnValueChangedIntermediate = (UXSlider.OnValueChangedIntermediateDelegate)Delegate.Combine(uXSlider2.OnValueChangedIntermediate, (UXSlider.OnValueChangedIntermediateDelegate)((UXSlider s, float v) =>
			{
				wg = v;
				UpdateColor();
				FireIntermediateResult();
			}));
			UXSlider uXSlider3 = waterBlueSlider;
			uXSlider3.OnValueChangedIntermediate = (UXSlider.OnValueChangedIntermediateDelegate)Delegate.Combine(uXSlider3.OnValueChangedIntermediate, (UXSlider.OnValueChangedIntermediateDelegate)((UXSlider s, float v) =>
			{
				wb = v;
				UpdateColor();
				FireIntermediateResult();
			}));
			UXSlider uXSlider4 = waterRedSlider;
			uXSlider4.OnValueChanged = (UXSlider.OnValueChangedDelegate)Delegate.Combine(uXSlider4.OnValueChanged, (UXSlider.OnValueChangedDelegate)((UXSlider s) =>
			{
				wr = s.Value;
				UpdateColor();
			}));
			UXSlider uXSlider5 = waterGreenSlider;
			uXSlider5.OnValueChanged = (UXSlider.OnValueChangedDelegate)Delegate.Combine(uXSlider5.OnValueChanged, (UXSlider.OnValueChangedDelegate)((UXSlider s) =>
			{
				wg = s.Value;
				UpdateColor();
			}));
			UXSlider uXSlider6 = waterBlueSlider;
			uXSlider6.OnValueChanged = (UXSlider.OnValueChangedDelegate)Delegate.Combine(uXSlider6.OnValueChanged, (UXSlider.OnValueChangedDelegate)((UXSlider s) =>
			{
				wb = s.Value;
				UpdateColor();
			}));
			UXTextButton uXTextButton = changeModifierButton;
			uXTextButton.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXTextButton.OnClick, (UXBaseButton.OnClickDelegate)(() =>
			{
				OpenChangeModifierDialog();
			}));
			_isInitialized = true;
		}
	}

	private void OpenChangeModifierDialog()
	{
		string[] names = Enum.GetNames(typeof(AvatarModifierPackageType));
		Dictionary<string, DialogData> dictionary = new Dictionary<string, DialogData>();
		dictionary.Add("ComboBox", new TextComboBoxData
		{
			items = names,
			currentlySelectedIndex = modifierPackageType
		});
		DialogFactory.CreateDialog(TM._("Select Modifier"), TM._("Modifier"), UXDialogType.ComboBox, noButtons: false, stackDialog: true, canClose: false).SetOnResultCallback(OnChangeModifierResult).SetValues(dictionary)
			.Show();
	}

	private void OnChangeModifierResult(UXDialogBox dialogBox)
	{
		if (dialogBox.DialogResult == UXDialogResult.Positive)
		{
			string text = (string)dialogBox.GetResult();
			if (text != string.Empty)
			{
				modifierPackageType = (int)Enum.Parse(typeof(AvatarModifierPackageType), text);
				currentModifierText.Text = text;
				FireIntermediateResult();
			}
		}
	}

	public override object GetResult()
	{
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary.Add("waterColor", new float[3] { wr, wg, wb });
		dictionary.Add("avatarModifierPackageType", modifierPackageType);
		return dictionary;
	}

	private void UpdateColor()
	{
		colorCube.SetColor(new Color(wr, wg, wb), string.Empty);
	}
}
