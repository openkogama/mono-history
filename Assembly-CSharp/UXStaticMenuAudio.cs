using System;
using UnityEngine;

[AddComponentMenu("UX/Audio/Static Menu")]
public class UXStaticMenuAudio : MonoBehaviour
{
	private AudioBankSound openSound;

	private AudioBankSound closeSound;

	private void Awake()
	{
		openSound = GUIAudioBank.Instance.GetSound("staticMenu_open");
		closeSound = GUIAudioBank.Instance.GetSound("staticMenu_close");
		UXStaticMenu component = ((Component)this).GetComponent<UXStaticMenu>();
		component.OnOpen = (UXStaticMenu.OnOpenDelegate)Delegate.Combine(component.OnOpen, new UXStaticMenu.OnOpenDelegate(HandleOnOpen));
		component.OnClose = (UXStaticMenu.OnCloseDelegate)Delegate.Combine(component.OnClose, new UXStaticMenu.OnCloseDelegate(HandleOnClose));
	}

	private void HandleOnOpen()
	{
		openSound.Play();
	}

	private void HandleOnClose()
	{
		closeSound.Play();
	}
}
