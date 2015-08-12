using System;
using UnityEngine;

public class MVGUIPlayButton : UXViewScript
{
	[SerializeField]
	public UXToggleIconButton playButton;

	public override void OnInitialize()
	{
		base.OnInitialize();
		UXToggleIconButton uXToggleIconButton = playButton;
		uXToggleIconButton.OnToggle = (UXToggleIconButton.OnToggleDelegate)Delegate.Combine(uXToggleIconButton.OnToggle, (UXToggleIconButton.OnToggleDelegate)((bool toggle) =>
		{
			playButton.GetComponent<UXToolTip>().toolTipText = ((!toggle) ? TM._("Play Mode <P>") : TM._("Edit Mode <P>"));
		}));
	}
}
