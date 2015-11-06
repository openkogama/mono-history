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

	public Rect previewPosition = new Rect(220f, 208f, 200f, 200f);

	private Action<Texture2D> callback;

	public void TakeScreenShot(Action<Texture2D> callback, bool ignoreAccessories = false)
	{
		if (isMakingScreenShot)
		{
			return;
		}
		this.callback = callback;
		EditorStateMachine editorStateMachine = MVGameControllerLegacyUI.CharacterEditorController.EditorStateMachine;
		if (editorStateMachine.ParentGroup is MVBody mVBody)
		{
			bodyCloneGO = UnityEngine.Object.Instantiate(mVBody.GameObject);
			if (ignoreAccessories)
			{
				AvatarAccessory[] componentsInChildren = bodyCloneGO.GetComponentsInChildren<AvatarAccessory>();
				AvatarAccessory[] array = componentsInChildren;
				foreach (AvatarAccessory avatarAccessory in array)
				{
					UnityEngine.Object.Destroy(avatarAccessory.gameObject);
				}
			}
			AvatarScreenshotGenerator.Generate(bodyCloneGO, ScreenShotDataTexHandler);
		}
		isMakingScreenShot = true;
	}

	private void ScreenShotDataTexHandler(Texture2D screenshotTex)
	{
		UnityEngine.Object.Destroy(bodyCloneGO);
		bodyCloneGO = null;
		isMakingScreenShot = false;
		callback(screenshotTex);
	}

	private void ShowScreenShot(Texture2D screenshotTex)
	{
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
