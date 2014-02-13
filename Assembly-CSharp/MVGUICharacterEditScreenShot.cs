using System;
using System.IO;
using Localize;
using MV.Common;
using UnityEngine;

public class MVGUICharacterEditScreenShot : UXViewScript
{
	private bool isMakingScreenShot;

	public UXIconButton takeScreenshotButton;

	private AudioBankSound screenshotSound;

	private GameObject bodyCloneGO;

	public Vector3 cameraOffset = new Vector3(-1f, 0.5f, 2f);

	public Vector3 lookAtOffset = new Vector3(0f, 0f, 0f);

	public string animationToShoot = "Walk";

	public float animationTime = 0.16f;

	private Texture2D screenshotTex;

	private bool showingScreenshot;

	public float previewTime;

	public float previewPeriod = 2f;

	public float fadeInPeriod = 0.1f;

	public AnimationCurve fadeInCurve;

	public float fadeOutPeriod = 0.1f;

	public AnimationCurve fadeOutCurve;

	public Rect previewPosition = new Rect(220f, 208f, 200f, 200f);

	private MVNetworkGame Game => MVGameController.Instance.Game;

	public MVGUICharacterEditScreenShot()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
	}

	public override void OnInitialize()
	{
		UXIconButton uXIconButton = takeScreenshotButton;
		uXIconButton.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXIconButton.OnClick, new UXBaseButton.OnClickDelegate(TakeScreenShot));
		screenshotSound = GUIAudioBank.Instance.GetSound("screenShot");
	}

	private void TakeScreenShot()
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Expected Obj, but got Unknown
		if (!isMakingScreenShot)
		{
			EditorStateMachine editorStateMachine = MVGameController.Instance.CharacterEditorController.EditorStateMachine;
			if (editorStateMachine.ParentGroup is MVBody mVBody)
			{
				bodyCloneGO = (GameObject)Object.Instantiate((Object)(object)mVBody.GameObject);
				AvatarScreenshotGenerator.Generate(bodyCloneGO, ScreenShotDataTexHandler);
			}
			isMakingScreenShot = true;
		}
	}

	private void Update()
	{
		if (showingScreenshot && (Object)(object)screenshotTex != (Object)null)
		{
			if (previewTime < fadeInPeriod + previewPeriod + fadeOutPeriod)
			{
				previewTime += Time.deltaTime;
				return;
			}
			showingScreenshot = false;
			Object.Destroy((Object)(object)screenshotTex);
		}
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
		isMakingScreenShot = false;
	}

	private void ScreenShotDataTexHandler(Texture2D screenshotTex)
	{
		if (screenshotSound != null)
		{
			screenshotSound.Play();
		}
		int profileID = MVGameController.Instance.Game.LocalPlayer.ProfileID;
		Game.ScreenshotUploaded += MVNetworGame_ScreenshotUploadedHandler;
		Game.UploadScreenshot(screenshotTex.EncodeToPNG(), ImageType.Avatar, profileID);
		Object.Destroy((Object)(object)bodyCloneGO);
		bodyCloneGO = null;
	}

	private void ShowScreenShot(Texture2D screenshotTex)
	{
		this.screenshotTex = screenshotTex;
		showingScreenshot = true;
		previewTime = 0f;
	}

	private void WriteToDisk(byte[] pngData)
	{
		string path = "C:\\dev\\Screenshots\\test.png";
		FileStream fileStream = File.Create(path);
		if (fileStream != null)
		{
			fileStream.Write(pngData, 0, pngData.Length);
			fileStream.Close();
		}
	}
}
