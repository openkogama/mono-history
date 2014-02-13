using System;
using System.IO;
using UnityEngine;

public class AvatarScreenshotGenerator : MonoBehaviour
{
	public Vector3 cameraOffset = new Vector3(-1f, 0.5f, 2f);

	public Vector3 lookAtOffset = new Vector3(0f, 0f, 0f);

	public string animationToShoot = "Walk";

	public float animationTime = 0.16f;

	private bool generatingScreenshot;

	private int generateStartFrame = -1;

	private ParticleSystem[] particleSystems;

	private BoneAnimation boneAnimation;

	private GameObject bodyCloneGO;

	private Action<Texture2D> screenShotDataTexHandler;

	public AvatarScreenshotGenerator()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
	}

	public static void Generate(GameObject bodyCloneGO, Action<Texture2D> screenShotDataTexHandler)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Expected Obj, but got Unknown
		bodyCloneGO.transform.position = new Vector3(1000f, 1000f, 1000f);
		bodyCloneGO.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
		LayerUtil.SetLayerRecursively(bodyCloneGO.transform, "Preview");
		bodyCloneGO.ScaleBounds(1f);
		GameObject val = new GameObject("AvatarScreenShotGenerator");
		AvatarScreenshotGenerator avatarScreenshotGenerator = val.AddComponent<AvatarScreenshotGenerator>();
		avatarScreenshotGenerator.screenShotDataTexHandler = screenShotDataTexHandler;
		avatarScreenshotGenerator.bodyCloneGO = bodyCloneGO;
		avatarScreenshotGenerator.boneAnimation = bodyCloneGO.GetComponentInChildren<BoneAnimation>();
		avatarScreenshotGenerator.generateStartFrame = Time.frameCount;
		avatarScreenshotGenerator.generatingScreenshot = true;
		avatarScreenshotGenerator.particleSystems = bodyCloneGO.GetComponentsInChildren<ParticleSystem>();
		avatarScreenshotGenerator.boneAnimation.PlayAndPauseAt(avatarScreenshotGenerator.animationToShoot, avatarScreenshotGenerator.animationTime);
	}

	private void Update()
	{
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		if (!generatingScreenshot)
		{
			return;
		}
		if (Time.frameCount == generateStartFrame + 1)
		{
			if (particleSystems != null)
			{
				ParticleSystem[] array = particleSystems;
				foreach (ParticleSystem val in array)
				{
					val.Simulate(2f, true);
				}
			}
		}
		else if (Time.frameCount == generateStartFrame + 2)
		{
			ScreenShotGenerator.Generate(bodyCloneGO, cameraOffset, lookAtOffset, (Action<Texture2D>)ScreenShotDataTexHandler, false);
		}
	}

	private void ScreenShotDataTexHandler(Texture2D screenshotTex)
	{
		if (screenShotDataTexHandler != null)
		{
			screenShotDataTexHandler(screenshotTex);
		}
		Object.Destroy((Object)(object)bodyCloneGO);
		generatingScreenshot = false;
		generateStartFrame = -1;
		bodyCloneGO = null;
		boneAnimation = null;
		Object.Destroy((Object)(object)((Component)this).gameObject);
	}

	private void WriteToDisk(byte[] pngData)
	{
		if (Application.isEditor)
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
}
