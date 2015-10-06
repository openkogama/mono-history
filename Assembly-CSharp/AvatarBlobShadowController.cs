using System;
using UnityEngine;

[RequireComponent(typeof(Projector))]
public class AvatarBlobShadowController : MonoBehaviour
{
	public Projector blobProjector;

	private float baseScale = 1f;

	private void Start()
	{
		MVQualitySettings.onQualityLevelChanged = (MVQualitySettings.OnQualityLevedChanged)Delegate.Combine(MVQualitySettings.onQualityLevelChanged, new MVQualitySettings.OnQualityLevedChanged(OnQualityLevelChanged));
		OnQualityLevelChanged(MVQualitySettings.CurrentLevel);
	}

	private void OnEnable()
	{
		OnQualityLevelChanged(MVQualitySettings.CurrentLevel);
	}

	private void OnDisable()
	{
		blobProjector.enabled = false;
	}

	private void OnDestroy()
	{
		MVQualitySettings.onQualityLevelChanged = (MVQualitySettings.OnQualityLevedChanged)Delegate.Remove(MVQualitySettings.onQualityLevelChanged, new MVQualitySettings.OnQualityLevedChanged(OnQualityLevelChanged));
	}

	public void ScaleShadow(float scale)
	{
		blobProjector.orthographicSize = baseScale * scale;
	}

	private void OnQualityLevelChanged(int level)
	{
		switch (level)
		{
		case 0:
			blobProjector.enabled = true;
			break;
		case 1:
			blobProjector.enabled = false;
			break;
		}
	}
}
