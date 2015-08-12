using System;
using UnityEngine;

[AddComponentMenu("UX/Audio/Mouse Click Object")]
public class UXMouseClickObjectAudio : MonoBehaviour
{
	private AudioBankSound clickSound;

	public string soundId = "menu_click";

	private void Awake()
	{
		clickSound = GUIAudioBank.Instance.GetSound("buttonClick");
		UXMouseClickObject component = GetComponent<UXMouseClickObject>();
		component.OnClick = (UXMouseClickObject.OnClickDelegate)Delegate.Combine(component.OnClick, new UXMouseClickObject.OnClickDelegate(HandleOnClick));
	}

	private void HandleOnClick(UXMouseClickObject clickObject, Vector3 mousePositionWorld)
	{
		clickSound.Play();
	}
}
