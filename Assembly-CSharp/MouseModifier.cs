using System.Collections.Generic;
using UnityEngine;

public class MouseModifier : SizeModifier
{
	public override AvatarModifierPackageType ModifierType => AvatarModifierPackageType.Shrunken;

	public override bool EvaluateShouldBeAdded(Dictionary<AvatarModifierPackageType, AvatarModifier> modifiers)
	{
		return true;
	}

	protected override void SetSizeModifier()
	{
		sizeModifier = 0.25f;
	}

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
			audioSource.PlayOneShot(shrinkSound);
		}
		StartCoroutine(DoForSeconds(timeToSize, (float t) =>
		{
			owner.mvAvatar.Scale = Vector3.one * (1f - BlockStep(t, 40f, 0f, 1f - sizeModifier)) + Vector3.one * sizeModifier * (1f - Mathf.Sin(t * sineStrength));
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
			audioSource.PlayOneShot(growSound);
		}
		StartCoroutine(DoForSeconds(timeToSize, (float t) =>
		{
			owner.mvAvatar.Scale = Vector3.one * BlockStep(t, 40f, sizeModifier, 1f) + Vector3.one * sizeModifier * (1f - Mathf.Sin(t * sineStrength));
			if (t == timeToSize)
			{
				Destroy();
			}
		}));
	}

	private void OnDisable()
	{
		StopAllCoroutines();
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

	private void Destroy()
	{
		owner.mvAvatar.Scale = Vector3.one;
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
