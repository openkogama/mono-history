using UnityEngine;

public class GrowthModifier : SizeModifier
{
	public override AvatarModifierPackageType ModifierType => AvatarModifierPackageType.Enlarged;

	protected override void Scale()
	{
		owner.mvAvatar.Scale = defaultScale;
		if (!owner.gameObject.activeInHierarchy)
		{
			owner.mvAvatar.Scale = defaultScale * sizeModifier;
			return;
		}
		if (audioSource.gameObject.activeInHierarchy)
		{
			audioSource.PlayOneShot(growSound);
		}
		StartCoroutine(DoForSeconds(timeToSize, (float t) =>
		{
			owner.mvAvatar.Scale = defaultScale * (1f + BlockStep(t, 40f, 0f, sizeModifier)) + defaultScale * 0.25f * (1f - Mathf.Sin(t * sineStrength));
			if (t == timeToSize)
			{
				owner.mvAvatar.Scale = defaultScale * sizeModifier;
			}
		}));
	}

	protected override void UnScale()
	{
		owner.mvAvatar.Scale = defaultScale * sizeModifier;
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
			owner.mvAvatar.Scale = defaultScale * (sizeModifier - BlockStep(t, 40f, 0f, sizeModifier - 1f)) + defaultScale * 0.25f * (1f - Mathf.Sin(t * sineStrength));
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
				owner.mvAvatar.Scale = defaultScale;
				Destroy();
			}
			else
			{
				owner.mvAvatar.Scale = defaultScale * sizeModifier;
			}
		}
	}

	private void Destroy()
	{
		owner.mvAvatar.Scale = defaultScale;
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
