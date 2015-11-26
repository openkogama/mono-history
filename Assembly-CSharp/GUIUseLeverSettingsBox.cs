using System;
using System.Collections.Generic;

public class GUIUseLeverSettingsBox : UXCustomDialogBox
{
	private bool activatedIntermediate;

	public UXToggleIconButton toggleButton;

	public UXText toggleText;

	public override object GetResult()
	{
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary.Add("beginActivated", activatedIntermediate);
		return dictionary;
	}

	public override void OnShowDialog()
	{
		base.OnShowDialog();
		activatedIntermediate = toggleButton.ToggleState;
		UXToggleIconButton uXToggleIconButton = toggleButton;
		uXToggleIconButton.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXToggleIconButton.OnClick, new UXBaseButton.OnClickDelegate(OnClick));
	}

	private void OnClick()
	{
		activatedIntermediate = !activatedIntermediate;
		FireIntermediateResult();
	}
}
