using System;
using UnityEngine;

[AddComponentMenu("UX/Audio/Text Field")]
public class UXTextFieldAudio : MonoBehaviour
{
	private AudioBankSound entrySound;

	private AudioBankSound deleteSound;

	private void Awake()
	{
		entrySound = GUIAudioBank.Instance.GetSound("textField_characterEntry");
		deleteSound = GUIAudioBank.Instance.GetSound("textField_characterDelete");
		UXTextField component = ((Component)this).GetComponent<UXTextField>();
		component.OnCharacterEntry = (UXTextField.OnCharacterEntryDelegate)Delegate.Combine(component.OnCharacterEntry, new UXTextField.OnCharacterEntryDelegate(HandleOnCharacterEntry));
		component.OnCharacterDelete = (UXTextField.OnCharacterDeleteDelegate)Delegate.Combine(component.OnCharacterDelete, new UXTextField.OnCharacterDeleteDelegate(HandleOnCharacterDelete));
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
