using System;
using UnityEngine;

[AddComponentMenu("UX/Audio/Mouse Over Object")]
public class UXMouseOverAudio : MonoBehaviour
{
	private AudioBankSound sound;

	public string soundId = "menu_hover";

	private void Awake()
	{
		sound = GUIAudioBank.Instance.GetSound("hover");
		UXMouseOverObject uXMouseOverObject = UXUtils.AddComponentIfNotExists<UXMouseOverObject>(gameObject);
		uXMouseOverObject.OnMouseOverEnter = (UXMouseOverObject.OnMouseOverDelegate)Delegate.Combine(uXMouseOverObject.OnMouseOverEnter, new UXMouseOverObject.OnMouseOverDelegate(HandleMouseOverEnter));
	}

	private void HandleMouseOverEnter(UXMouseOverObject mouseOverObject)
	{
		sound.Play();
	}
}
