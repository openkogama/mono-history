using System;
using UnityEngine;

[RequireComponent(typeof(Projector))]
public class AvatarBlobShadowController : MonoBehaviour
{
	public Projector blobProjector;

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
		((Behaviour)blobProjector).enabled = false;
	}

	private void OnDestroy()
	{
		MVQualitySettings.onQualityLevelChanged = (MVQualitySettings.OnQualityLevedChanged)Delegate.Remove(MVQualitySettings.onQualityLevelChanged, new MVQualitySettings.OnQualityLevedChanged(OnQualityLevelChanged));
	}

	private void OnQualityLevelChanged(int level)
	{
		switch (level)
		{
		case 0:
			((Behaviour)blobProjector).enabled = true;
			break;
		case 1:
			((Behaviour)blobProjector).enabled = false;
			break;
		}
	}
}
