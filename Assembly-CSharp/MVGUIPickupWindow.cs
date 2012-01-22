using System;
using UnityEngine;

public class MVGUIPickupWindow : UXViewScript
{
	public UXIconButton healthPickupButton;

	public UXIconButton centerGunPickupButton;

	public UXIconButton impulseGunPickupButton;

	public override void OnInitialize()
	{
		HideViewOnClick(healthPickupButton);
		HideViewOnClick(centerGunPickupButton);
		HideViewOnClick(impulseGunPickupButton);
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
