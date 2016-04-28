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
		audioSource.PlayOneShot(shrinkSound);
		owner.mvAvatar.Scale = Vector3.one;
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
		audioSource.PlayOneShot(growSound);
		owner.mvAvatar.Scale = Vector3.one * sizeModifier;
		StartCoroutine(DoForSeconds(timeToSize, (float t) =>
		{
			owner.mvAvatar.Scale = Vector3.one * BlockStep(t, 40f, sizeModifier, 1f) + Vector3.one * sizeModifier * (1f - Mathf.Sin(t * sineStrength));
			if (t == timeToSize)
			{
				owner.mvAvatar.Scale = Vector3.one;
				Object.Destroy(gameObject);
			}
		}));
	}

	private void Update()
	{
		if (!isDeactivating)
		{
			Unstablize();
		}
	}
}
