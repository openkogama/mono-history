using UnityEngine;

public class GrowthModifier : SizeModifier
{
	public override AvatarModifierPackageType ModifierType => AvatarModifierPackageType.Enlarged;

	protected override void Scale()
	{
		owner.mvAvatar.Scale = Vector3.one;
		if (!owner.gameObject.activeInHierarchy)
		{
			owner.mvAvatar.Scale = Vector3.one * sizeModifier;
			return;
		}
		if (audioSource.gameObject.activeInHierarchy)
		{
			audioSource.PlayOneShot(growSound);
		}
		StartCoroutine(DoForSeconds(timeToSize, (float t) =>
		{
			owner.mvAvatar.Scale = Vector3.one * (1f + BlockStep(t, 40f, 0f, sizeModifier)) + Vector3.one * 0.25f * (1f - Mathf.Sin(t * sineStrength));
			if (t == timeToSize)
			{
				owner.mvAvatar.Scale = Vector3.one * sizeModifier;
			}
		}));
	}

	protected override void UnScale()
	{
		owner.mvAvatar.Scale = Vector3.one * sizeModifier;
		if (!owner.gameObject.activeInHierarchy)
		{
			Destroy();
			return;
		}
		if (audioSource.gameObject.activeInHierarchy)
		{
			audioSource.PlayOneShot(shrinkSound);
		}
		StartCoroutine(DoForSeconds(timeToSize, (float t) =>
		{
			owner.mvAvatar.Scale = Vector3.one * (sizeModifier - BlockStep(t, 40f, 0f, sizeModifier - 1f)) + Vector3.one * 0.25f * (1f - Mathf.Sin(t * sineStrength));
			owner.mvAvatar.SetTransparency = 1f;
			if (t == timeToSize)
			{
				Destroy();
			}
		}));
	}

	private void OnDisable()
	{
		StopAllCoroutines();
		if (owner != null)
		{
			if (isDeactivating)
			{
				owner.mvAvatar.Scale = Vector3.one;
				Destroy();
			}
			else
			{
				owner.mvAvatar.Scale = Vector3.one * sizeModifier;
			}
		}
	}

	private void Destroy()
	{
		owner.mvAvatar.Scale = Vector3.one;
		if (owner.mvAvatar is MVAvatarLocal mVAvatarLocal)
		{
			mVAvatarLocal.RigidBody.GetComponent<AvatarMotor>().GetSizeState.ScaleChanged();
		}
		Object.Destroy(gameObject);
	}

	private void Update()
	{
		if (!isDeactivating)
		{
			Unstablize();
		}
	}
}
