using System;
using UnityEngine;

public class AvatarScreenShooter : MonoBehaviour
{
	private bool isMakingScreenShot;

	[SerializeField]
	private GameObject bodyCloneGO;

	public Vector3 cameraOffset = new Vector3(-1f, 0.5f, 2f);

	public Vector3 lookAtOffset = new Vector3(0f, 0f, 0f);

	public Rect previewPosition = new Rect(220f, 208f, 200f, 200f);

	private Action<Texture2D, string> callback;

	private string successMessage;

	public void TakeScreenShot(Action<Texture2D, string> callback, MVBody body, bool ignoreAccessories = false, string successMessage = "Screenshot taken successfully!")
	{
		Debug.Log("screenshot generation of avatar initiated");
		if (isMakingScreenShot)
		{
			return;
		}
		this.callback = callback;
		this.successMessage = successMessage;
		if (body != null)
		{
			bodyCloneGO = UnityEngine.Object.Instantiate(body.GameObject);
			if (ignoreAccessories)
			{
				AvatarAccessory[] componentsInChildren = bodyCloneGO.GetComponentsInChildren<AvatarAccessory>();
				AvatarAccessory[] array = componentsInChildren;
				foreach (AvatarAccessory avatarAccessory in array)
				{
					UnityEngine.Object.Destroy(avatarAccessory.gameObject);
				}
			}
			else
			{
				SkinnedMeshOptimizer[] componentsInChildren2 = bodyCloneGO.GetComponentsInChildren<SkinnedMeshOptimizer>();
				for (int j = 0; j < componentsInChildren2.Length; j++)
				{
					componentsInChildren2[j].TurnOffMesh();
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
		callback(screenshotTex, successMessage);
	}
}
