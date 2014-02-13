using System;
using Localize;
using UnityEngine;

public class MVGUIPlayButton : UXViewScript
{
	public UXToggleIconButton playButton;

	public override void OnInitialize()
	{
		UXToggleIconButton uXToggleIconButton = playButton;
		uXToggleIconButton.OnToggle = (UXToggleIconButton.OnToggleDelegate)Delegate.Combine(uXToggleIconButton.OnToggle, (UXToggleIconButton.OnToggleDelegate)((bool toggle) =>
		{
			((Component)playButton).GetComponent<UXToolTip>().toolTipTextID = ((!toggle) ? TextSlotIndex.PlayToggle : TextSlotIndex.EditToggle);
		}));
	}
}
