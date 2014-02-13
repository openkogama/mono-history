using System;
using UnityEngine;

public class MVGUIHDToggle : MonoBehaviour
{
	private void Awake()
	{
		MVQualitySettings.CurrentLevel = 0;
		UXToggleIconButton component = ((Component)this).GetComponent<UXToggleIconButton>();
		component.ToggleState = false;
		component.OnToggle = (UXToggleIconButton.OnToggleDelegate)Delegate.Combine(component.OnToggle, new UXToggleIconButton.OnToggleDelegate(HandleOnToggle));
	}

	public void HandleOnToggle(bool hd)
	{
		if (hd)
		{
			MVQualitySettings.CurrentLevel = 1;
		}
		else
		{
			MVQualitySettings.CurrentLevel = 0;
		}
	}
}
