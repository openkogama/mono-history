using System;
using UnityEngine;

[AddComponentMenu("UX/Audio/Button")]
public class UXButtonAudio : MonoBehaviour
{
	private AudioBankSound clickSound;

	private void Awake()
	{
		clickSound = GUIAudioBank.Instance.GetSound("buttonClick");
		UXButton component = ((Component)this).GetComponent<UXButton>();
		component.OnClick = (UXButton.OnClickDelegate)Delegate.Combine(component.OnClick, new UXButton.OnClickDelegate(HandleOnClick));
	}

	private void HandleOnClick()
	{
		clickSound.Play();
	}
}
