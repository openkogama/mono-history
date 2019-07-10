using System;
using UnityEngine;
using UnityEngine.Events;

public class LevelDisplayCube : MonoBehaviour
{
	public GameObject cube;

	private Texture2D badgeTextureAsset;

	private bool waitingForBadgeTexture;

	private Renderer[] renderers;

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
					BadgeManager.GetBadgeTexture(levelAmount, OnBadgeTextureReceived);
					waitingForBadgeTexture = false;
				}));
				waitingForBadgeTexture = true;
			}
		}
		else
		{
			BadgeManager.GetBadgeTexture(levelAmount, OnBadgeTextureReceived);
		}
	}

	private void OnBadgeTextureReceived(WWW www)
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
				badgeTextureAsset = www.texture;
				renderer.material.mainTexture = badgeTextureAsset;
			}
		}
	}

	public void Destroy()
	{
		BadgeManager.UnsubscribeGetBadgeRequest(OnBadgeTextureReceived);
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
		UnityEngine.Object.Destroy(badgeTextureAsset);
	}

	public void SetScale(Vector3 size)
	{
		transform.localScale = size;
	}
}
