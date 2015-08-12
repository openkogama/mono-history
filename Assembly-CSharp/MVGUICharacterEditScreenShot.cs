using System;
using MV.Common;
using UnityEngine;

public class MVGUICharacterEditScreenShot : UXViewScript
{
	public AvatarScreenShooter avatarScreenShooter;

	public UXIconButton takeScreenshotButton;

	private AudioBankSound screenshotSound;

	private MVNetworkGame Game => MVGameController.Game;

	public override void OnInitialize()
	{
		UXIconButton uXIconButton = takeScreenshotButton;
		uXIconButton.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXIconButton.OnClick, new UXBaseButton.OnClickDelegate(TakeScreenShot));
		screenshotSound = GUIAudioBank.Instance.GetSound("screenShot");
	}

	private void TakeScreenShot()
	{
		avatarScreenShooter.TakeScreenShot(ScreenShotCallback);
	}

	private void ScreenShotCallback(Texture2D screenshotTex)
	{
		if (screenshotSound != null)
		{
			screenshotSound.Play();
		}
		int profileID = MVGameController.Game.LocalPlayer.ProfileID;
		if (Game.UploadScreenshot(screenshotTex.EncodeToPNG(), ImageType.Avatar, profileID))
		{
			Game.ScreenshotUploaded += MVNetworGame_ScreenshotUploadedHandler;
		}
	}

	private void MVNetworGame_ScreenshotUploadedHandler(object sender, ScreenshotUploadedEventArgs e)
	{
		MVGameController.Game.ScreenshotUploaded -= MVNetworGame_ScreenshotUploadedHandler;
		if (e.Uploaded)
		{
			UXUtils.UXDialogFactory.CreateDialog(TM._("Screenshot Taken!"), string.Empty).Show();
		}
		else
		{
			UXUtils.UXDialogFactory.CreateDialog(TM._("There was a server communication problem. Try again!"), string.Empty).Show();
		}
	}
}
