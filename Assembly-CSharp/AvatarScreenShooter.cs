using System;
using System.IO;
using UnityEngine;

public class AvatarScreenShooter : MonoBehaviour
{
	private bool isMakingScreenShot;

	[SerializeField]
	private GameObject bodyCloneGO;

	public Vector3 cameraOffset = new Vector3(-1f, 0.5f, 2f);

	public Vector3 lookAtOffset = new Vector3(0f, 0f, 0f);

	[SerializeField]
	private Texture2D screenshotTex;

	public Rect previewPosition = new Rect(220f, 208f, 200f, 200f);

	private Action<Texture2D> callback;

	public AvatarScreenShooter()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
	}

	public void TakeScreenShot(Action<Texture2D> callback, bool ignoreAccessories = false)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Expected Obj, but got Unknown
		if (isMakingScreenShot)
		{
			return;
		}
		this.callback = callback;
		EditorStateMachine editorStateMachine = MVGameController.Instance.CharacterEditorController.EditorStateMachine;
		if (editorStateMachine.ParentGroup is MVBody mVBody)
		{
			bodyCloneGO = (GameObject)Object.Instantiate((Object)(object)mVBody.GameObject);
			if (ignoreAccessories)
			{
				AvatarAccessory[] componentsInChildren = bodyCloneGO.GetComponentsInChildren<AvatarAccessory>();
				AvatarAccessory[] array = componentsInChildren;
				foreach (AvatarAccessory avatarAccessory in array)
				{
					Object.Destroy((Object)(object)((Component)avatarAccessory).gameObject);
				}
			}
			AvatarScreenshotGenerator.Generate(bodyCloneGO, ScreenShotDataTexHandler);
		}
		isMakingScreenShot = true;
	}

	private void ScreenShotDataTexHandler(Texture2D screenshotTex)
	{
		Object.Destroy((Object)(object)bodyCloneGO);
		bodyCloneGO = null;
		isMakingScreenShot = false;
		callback(screenshotTex);
	}

	private void ShowScreenShot(Texture2D screenshotTex)
	{
		this.screenshotTex = screenshotTex;
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
