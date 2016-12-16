using System;
using UnityEngine;
using UnityEngine.Events;

public class PickupParticleScaler : MonoBehaviour
{
	[SerializeField]
	private PickupItem itemAttachedTo;

	[SerializeField]
	private ParticleSystem particleSysToScale;

	private float initialParticleStartSize;

	private float initialParticleStartSpeed;

	private void Start()
	{
		initialParticleStartSize = particleSysToScale.startSize;
		initialParticleStartSpeed = particleSysToScale.startSpeed;
		if (itemAttachedTo.owner != null)
		{
			MVWorldObjectClient worldObjectOwner = itemAttachedTo.owner.WorldObjectOwner;
			worldObjectOwner.ScaleChanged = (UnityAction<MVWorldObjectClient, ScaleChangedEventArgs>)Delegate.Combine(worldObjectOwner.ScaleChanged, new UnityAction<MVWorldObjectClient, ScaleChangedEventArgs>(OnScaleChange));
		}
	}

	private void OnDestroy()
	{
		if (itemAttachedTo.owner != null)
		{
			MVWorldObjectClient worldObjectOwner = itemAttachedTo.owner.WorldObjectOwner;
			worldObjectOwner.ScaleChanged = (UnityAction<MVWorldObjectClient, ScaleChangedEventArgs>)Delegate.Remove(worldObjectOwner.ScaleChanged, new UnityAction<MVWorldObjectClient, ScaleChangedEventArgs>(OnScaleChange));
		}
	}

	private void OnScaleChange(MVWorldObjectClient obj, ScaleChangedEventArgs args)
	{
		float y = transform.lossyScale.y;
		particleSysToScale.startSize = initialParticleStartSize * y;
		particleSysToScale.startSpeed = initialParticleStartSpeed * y;
	}
}
