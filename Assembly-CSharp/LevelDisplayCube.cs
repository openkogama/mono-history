using UnityEngine;

public class LevelDisplayCube : MonoBehaviour
{
	private static readonly float visibilityDistance = 25f;

	public GameObject cube;

	private Renderer[] renderers;

	private bool visible;

	public void Initialize()
	{
		renderers = gameObject.GetComponentsInChildren<Renderer>();
		Renderer[] array = renderers;
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
		BadgeManager.GetBadgeTexture(levelAmount, StreamingAssetCallback);
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
		Renderer[] array = renderers;
		foreach (Renderer renderer in array)
		{
			if (!(renderer == null))
			{
				if (renderer.material.mainTexture != null)
				{
					Object.Destroy(renderer.material.mainTexture);
				}
				renderer.material.mainTexture = www.texture;
			}
		}
	}

	public void Destroy()
	{
		Renderer[] array = renderers;
		foreach (Renderer renderer in array)
		{
			if (renderer.material.mainTexture != null)
			{
				Object.Destroy(renderer.material.mainTexture);
			}
		}
		for (int j = 0; j < renderers.Length; j++)
		{
			Object.Destroy(renderers[j].gameObject);
		}
		Object.Destroy(cube);
	}

	public void Show()
	{
		visible = true;
		for (int i = 0; i < renderers.Length; i++)
		{
			renderers[i].enabled = true;
		}
	}

	public void Hide()
	{
		visible = false;
		for (int i = 0; i < renderers.Length; i++)
		{
			renderers[i].enabled = false;
		}
	}
}
