using System;
using UnityEngine;

public class MVGUIEditModeScreenShot : UXViewScript
{
	[SerializeField]
	private UXIconButton takeScreenshotButton;

	private AudioBankSound screenshotSound;

	public override void OnInitialize()
	{
		base.OnInitialize();
		UXIconButton uXIconButton = takeScreenshotButton;
		uXIconButton.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXIconButton.OnClick, new UXBaseButton.OnClickDelegate(TakeScreenShot));
		screenshotSound = GUIAudioBank.Instance.GetSound("screenShot");
	}

	private void TakeScreenShot()
	{
		MVGameControllerBase.Game.UploadGameScreenShot();
		if (screenshotSound != null)
		{
			screenshotSound.Play();
		}
	}
}
