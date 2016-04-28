using System.Collections.Generic;
using UnityEngine;

public class GrowthModifier : SizeModifier
{
	public override AvatarModifierPackageType ModifierType => AvatarModifierPackageType.Enlarged;

	public override bool EvaluateShouldBeAdded(Dictionary<AvatarModifierPackageType, AvatarModifier> modifiers)
	{
		return true;
	}

	protected override void SetSizeModifier()
	{
		sizeModifier = 2f;
	}

	protected override void Scale()
	{
		audioSource.PlayOneShot(growSound);
		owner.mvAvatar.Scale = Vector3.one;
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
		audioSource.PlayOneShot(shrinkSound);
		owner.mvAvatar.Scale = Vector3.one * sizeModifier;
		StartCoroutine(DoForSeconds(timeToSize, (float t) =>
		{
			owner.mvAvatar.Scale = Vector3.one * (sizeModifier - BlockStep(t, 40f, 0f, sizeModifier - 1f)) + Vector3.one * 0.25f * (1f - Mathf.Sin(t * sineStrength));
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
