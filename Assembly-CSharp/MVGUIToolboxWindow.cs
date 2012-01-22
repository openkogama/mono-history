using System;
using UnityEngine;

public class MVGUIToolboxWindow : UXViewScript
{
	public UXIconButton lightButton;

	public UXIconButton textMsgButton;

	public UXIconButton explositionButton;

	public UXIconButton fireButton;

	public UXIconButton smokeButton;

	public UXIconButton spawnPointButton;

	public UXIconButton triggerBoxButton;

	public UXIconButton flagButton;

	public UXIconButton testLogicButton;

	public UXIconButton batteryButton;

	public UXIconButton toggleBoxButton;

	public UXIconButton negateButton;

	public UXIconButton andButton;

	public UXIconButton timeTriggerButton;

	public UXIconButton teleporterButton;

	public UXIconButton goalButton;

	public UXIconButton pressurePlateButton;

	public override void OnInitialize()
	{
		HideViewOnClick(lightButton);
		HideViewOnClick(textMsgButton);
		HideViewOnClick(explositionButton);
		HideViewOnClick(smokeButton);
		HideViewOnClick(fireButton);
		HideViewOnClick(spawnPointButton);
		HideViewOnClick(triggerBoxButton);
		HideViewOnClick(flagButton);
		HideViewOnClick(testLogicButton);
		HideViewOnClick(batteryButton);
		HideViewOnClick(toggleBoxButton);
		HideViewOnClick(negateButton);
		HideViewOnClick(andButton);
		HideViewOnClick(timeTriggerButton);
		HideViewOnClick(teleporterButton);
		HideViewOnClick(goalButton);
		HideViewOnClick(pressurePlateButton);
	}

	private void HideViewOnClick(UXIconButton button)
	{
		if (Object.op_Implicit((Object)(object)button))
		{
			button.OnClick = (UXIconButton.OnClickDelegate)Delegate.Combine(button.OnClick, new UXIconButton.OnClickDelegate(View.Hide));
		}
	}

	private void OnClick()
	{
		View.Hide();
	}
}
