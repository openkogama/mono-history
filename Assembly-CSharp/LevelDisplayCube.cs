using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

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

	private void OnBadgeTextureReceived(UnityWebRequest www)
	{
		if (!string.IsNullOrEmpty(www.error))
		{
			return;
		}
		badgeTextureAsset = DownloadHandlerTexture.GetContent(www);
		Renderer[] array = Renderers;
		foreach (Renderer renderer in array)
		{
			if (!(renderer == null))
			{
				renderer.material.mainTexture = badgeTextureAsset;
			}
		}
	}

	public void Destroy()
	{
		BadgeManager.UnsubscribeGetBadgeRequest(OnBadgeTextureReceived);
		for (int i = 0; i < Renderers.Length; i++)
		{
			UnityEngine.Object.Destroy(Renderers[i].gameObject);
		}
		UnityEngine.Object.Destroy(cube);
		badgeTextureAsset = null;
	}

	public void SetScale(Vector3 size)
	{
		transform.localScale = size;
	}
}
