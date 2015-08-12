using System;
using UnityEngine;

[AddComponentMenu("UX/Audio/Toggle Icon Button")]
public class UXToggleIconButtonAudio : MonoBehaviour
{
	private AudioBankSound onSound;

	private AudioBankSound offSound;

	private AudioBankSound hoverSound;

	private void Awake()
	{
		onSound = GUIAudioBank.Instance.GetSound("toggle_on");
		offSound = GUIAudioBank.Instance.GetSound("toggle_off");
		hoverSound = GUIAudioBank.Instance.GetSound("hover");
		UXToggleIconButton component = GetComponent<UXToggleIconButton>();
		component.OnToggle = (UXToggleIconButton.OnToggleDelegate)Delegate.Combine(component.OnToggle, new UXToggleIconButton.OnToggleDelegate(HandleOnToggle));
		UXMouseOverObject component2 = GetComponent<UXMouseOverObject>();
		component2.OnMouseOverEnter = (UXMouseOverObject.OnMouseOverDelegate)Delegate.Combine(component2.OnMouseOverEnter, new UXMouseOverObject.OnMouseOverDelegate(HandleMouseOverEnter));
	}

	private void HandleOnToggle(bool state)
	{
		if (state)
		{
			onSound.Play();
		}
		else
		{
			offSound.Play();
		}
	}

	private void HandleMouseOverEnter(UXMouseOverObject mouseOverObject)
	{
		hoverSound.Play();
	}
}
