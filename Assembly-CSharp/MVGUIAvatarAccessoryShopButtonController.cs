using System;
using UnityEngine;

public class MVGUIAvatarAccessoryShopButtonController : UXViewScript
{
	[SerializeField]
	private UXBaseButton accessoryShopButton;

	public override void OnInitialize()
	{
		base.OnInitialize();
		UXBaseButton uXBaseButton = accessoryShopButton;
		uXBaseButton.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXBaseButton.OnClick, (UXBaseButton.OnClickDelegate)(() =>
		{
			UXUtils.FindGUIObjectOfType<AvatarAccessoryController>().OpenAvatarAccessoryShop();
		}));
	}
}
