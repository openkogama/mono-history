using System;
using UnityEngine;

public class MVGUIMuteToggle : UXViewScript
{
	[SerializeField]
	private UXToggleIconButton muteButton;

	public override void OnInitialize()
	{
		base.OnInitialize();
		Debug.Log("Mute " + MVCameraController.Mute);
		muteButton.SetToggleState(MVCameraController.Mute);
		UXToggleIconButton uXToggleIconButton = muteButton;
		uXToggleIconButton.OnToggle = (UXToggleIconButton.OnToggleDelegate)Delegate.Combine(uXToggleIconButton.OnToggle, new UXToggleIconButton.OnToggleDelegate(HandleOnToggle));
		MVCameraController.OnMuteChange = (Action<bool>)Delegate.Combine(MVCameraController.OnMuteChange, (Action<bool>)((bool mute) =>
		{
			muteButton.SetToggleState(mute);
		}));
	}

	public void HandleOnToggle(bool mute)
	{
		MVCameraController.Mute = mute;
	}
}
