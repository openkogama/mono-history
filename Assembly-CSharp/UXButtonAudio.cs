using System;
using UnityEngine;

[AddComponentMenu("UX/Audio/Button")]
public class UXButtonAudio : MonoBehaviour
{
	private AudioBankSound clickSound;

	private void Awake()
	{
		clickSound = GUIAudioBank.Instance.GetSound("buttonClick");
		UXTextButton component = ((Component)this).GetComponent<UXTextButton>();
		component.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(component.OnClick, new UXBaseButton.OnClickDelegate(HandleOnClick));
	}

	private void HandleOnClick()
	{
		clickSound.Play();
	}
}
