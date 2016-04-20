using System;
using UnityEngine;
using UnityEngine.Events;

public class LevelDisplayCube : MonoBehaviour
{
	private static readonly float visibilityDistance = 25f;

	public GameObject cube;

	private Renderer[] renderers;

	private bool visible;

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
		Hide();
	}

	private void Update()
	{
		if (MVGameControllerBase.WOCM.AvatarLocal != null)
		{
			ChangeLOD((transform.position - MVGameControllerBase.WOCM.AvatarLocal.Transform.position).magnitude);
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

	public void ChangeLOD(float distance)
	{
		if (distance <= visibilityDistance)
		{
			if (!visible)
			{
				Show();
			}
		}
		else if (visible)
		{
			Hide();
		}
	}

	private void StreamingAssetCallback(WWW www)
	{
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

	public void Show()
	{
		visible = true;
		for (int i = 0; i < Renderers.Length; i++)
		{
			Renderers[i].enabled = true;
		}
	}

	public void Hide()
	{
		visible = false;
		for (int i = 0; i < Renderers.Length; i++)
		{
			Renderers[i].enabled = false;
		}
	}
}
