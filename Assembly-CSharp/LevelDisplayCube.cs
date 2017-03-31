using System;
using UnityEngine;
using UnityEngine.Events;

public class LevelDisplayCube : MonoBehaviour
{
	public GameObject cube;

	private Renderer[] renderers;

	private bool waitingForBadgeTexture;

	public Renderer[] Renderers
	{
		get
		{
			if (renderers == null)
			{
				renderers = gameObject.GetComponentsInChildren<Renderer>();
			}
			return renderers;
		}
	}

	public void Initialize()
	{
		Renderer[] array = Renderers;
		foreach (Renderer renderer in array)
		{
			renderer.material.mainTexture = null;
		}
	}

	public void SetAmount(int levelAmount)
	{
		if (!LevelingManager.IsInitialized)
		{
			if (!waitingForBadgeTexture)
			{
				LevelingManager.OnLevelingInitialized = (UnityAction)Delegate.Combine(LevelingManager.OnLevelingInitialized, (UnityAction)(() =>
				{
					BadgeManager.GetBadgeTexture(levelAmount, StreamingAssetCallback);
					waitingForBadgeTexture = false;
				}));
				waitingForBadgeTexture = true;
			}
		}
		else
		{
			BadgeManager.GetBadgeTexture(levelAmount, StreamingAssetCallback);
		}
	}

	private void StreamingAssetCallback(WWW www)
	{
		if (!string.IsNullOrEmpty(www.error))
		{
			return;
		}
		Renderer[] array = Renderers;
		foreach (Renderer renderer in array)
		{
			if (!(renderer == null))
			{
				if (renderer.material.mainTexture != null)
				{
					UnityEngine.Object.Destroy(renderer.material.mainTexture);
				}
				renderer.material.mainTexture = www.texture;
			}
		}
	}

	public void Destroy()
	{
		BadgeManager.UnsubscribeGetBadgeRequest(StreamingAssetCallback);
		Renderer[] array = Renderers;
		foreach (Renderer renderer in array)
		{
			if (renderer.material.mainTexture != null)
			{
				UnityEngine.Object.Destroy(renderer.material.mainTexture);
			}
		}
		for (int j = 0; j < Renderers.Length; j++)
		{
			UnityEngine.Object.Destroy(Renderers[j].gameObject);
		}
		UnityEngine.Object.Destroy(cube);
	}

	public void SetScale(Vector3 size)
	{
		transform.localScale = size;
	}
}
