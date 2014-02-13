using System;
using UnityEngine;

[AddComponentMenu("UX/Audio/Text Input")]
public class UXTextInputAudio : MonoBehaviour
{
	private AudioBankSound entrySound;

	private AudioBankSound deleteSound;

	private void Awake()
	{
		entrySound = GUIAudioBank.Instance.GetSound("textField_characterEntry");
		deleteSound = GUIAudioBank.Instance.GetSound("textField_characterDelete");
		UXTextInputElement component = ((Component)this).GetComponent<UXTextInputElement>();
		component.OnCharacterEntry = (UXTextInputElement.OnCharacterEntryDelegate)Delegate.Combine(component.OnCharacterEntry, new UXTextInputElement.OnCharacterEntryDelegate(HandleOnCharacterEntry));
		component.OnCharacterDelete = (UXTextInputElement.OnCharacterDeleteDelegate)Delegate.Combine(component.OnCharacterDelete, new UXTextInputElement.OnCharacterDeleteDelegate(HandleOnCharacterDelete));
	}

	private void HandleOnCharacterEntry(char c)
	{
		entrySound.Play();
	}

	private void HandleOnCharacterDelete()
	{
		deleteSound.Play();
	}
}
