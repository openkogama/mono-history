using System;
using System.Collections;
using UnityEngine;

public class AvatarScreenshotGenerator : MonoBehaviour
{
	public Vector3 cameraOffset = new Vector3(-1f, 0.5f, 2f);

	public Vector3 lookAtOffset = new Vector3(0f, 0f, 0f);

	public string animationToShoot = "Walk";

	public float animationTime = 0.16f;

	private ParticleSystem[] particleSystems;

	private BoneAnimation boneAnimation;

	private GameObject bodyCloneGO;

	private Action<Texture2D> screenShotDataTexHandler;

	public static void Generate(GameObject bodyCloneGO, Action<Texture2D> screenShotDataTexHandler)
	{
		bodyCloneGO.transform.position = new Vector3(1000f, 1000f, 1000f);
		bodyCloneGO.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
		LayerUtil.SetLayerRecursively(bodyCloneGO.transform, "Preview");
		bodyCloneGO.ScaleBounds(1f);
		GameObject gameObject = new GameObject("AvatarScreenShotGenerator");
		AvatarScreenshotGenerator avatarScreenshotGenerator = gameObject.AddComponent<AvatarScreenshotGenerator>();
		avatarScreenshotGenerator.screenShotDataTexHandler = screenShotDataTexHandler;
		avatarScreenshotGenerator.bodyCloneGO = bodyCloneGO;
		avatarScreenshotGenerator.boneAnimation = bodyCloneGO.GetComponentInChildren<BoneAnimation>();
		avatarScreenshotGenerator.particleSystems = bodyCloneGO.GetComponentsInChildren<ParticleSystem>();
		avatarScreenshotGenerator.boneAnimation.PlayAndPauseAt(avatarScreenshotGenerator.animationToShoot, avatarScreenshotGenerator.animationTime);
		avatarScreenshotGenerator.StartCoroutine(avatarScreenshotGenerator.GenerateScreenshot());
	}

	public IEnumerator GenerateScreenshot()
	{
		if (particleSystems != null)
		{
			ParticleSystem[] array = particleSystems;
			foreach (ParticleSystem particleSystem in array)
			{
				if (particleSystem != null)
				{
					particleSystem.Simulate(2f, withChildren: true);
				}
			}
		}
		yield return new WaitForEndOfFrame();
		AvatarEditModeBodyController.Theme.Deactivate();
		ScreenShotGenerator.Generate(bodyCloneGO, cameraOffset, lookAtOffset, ScreenShotDataTexHandler);
	}

	private void ScreenShotDataTexHandler(Texture2D screenshotTex)
	{
		AvatarEditModeBodyController.Theme.Activate();
		if (screenShotDataTexHandler != null)
		{
			screenShotDataTexHandler(screenshotTex);
		}
		UnityEngine.Object.Destroy(bodyCloneGO);
		bodyCloneGO = null;
		boneAnimation = null;
		UnityEngine.Object.Destroy(gameObject);
	}
}
