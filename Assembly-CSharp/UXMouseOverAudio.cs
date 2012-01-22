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
		if ((Object)(object)((Component)this).GetComponent<UXMouseOverObject>() == (Object)null)
		{
			Debug.LogError((object)((Object)((Component)this).gameObject).name);
		}
		UXMouseOverObject component = ((Component)this).GetComponent<UXMouseOverObject>();
		component.OnMouseOverEnter = (UXMouseOverObject.OnMouseOverEnterDelegate)Delegate.Combine(component.OnMouseOverEnter, new UXMouseOverObject.OnMouseOverEnterDelegate(HandleMouseOverEnter));
	}

	private void HandleMouseOverEnter(UXMouseOverObject mouseOverObject)
	{
		sound.Play();
	}
}
