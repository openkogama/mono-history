using System;
using Localize;
using MV.Common;
using UnityEngine;

public class MVGUICharacterEditScreenShot : UXViewScript
{
	public AvatarScreenShooter avatarScreenShooter;

	public UXIconButton takeScreenshotButton;

	private AudioBankSound screenshotSound;

	private MVNetworkGame Game => MVGameController.Instance.Game;

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
		int profileID = MVGameController.Instance.Game.LocalPlayer.ProfileID;
		Game.ScreenshotUploaded += MVNetworGame_ScreenshotUploadedHandler;
		Game.UploadScreenshot(screenshotTex.EncodeToPNG(), ImageType.Avatar, profileID);
	}

	private void MVNetworGame_ScreenshotUploadedHandler(object sender, ScreenshotUploadedEventArgs e)
	{
		MVGameController.Instance.Game.ScreenshotUploaded -= MVNetworGame_ScreenshotUploadedHandler;
		if (e.Uploaded)
		{
			UXUtils.FindGUIObjectOfType<UXDialogFactory>().CreateDialog(TextSlotIndex.ScreenshotTaken).Show();
		}
		else
		{
			UXUtils.FindGUIObjectOfType<UXDialogFactory>().CreateDialog(TextSlotIndex.ScreenshotNotTaken).Show();
		}
	}
}
