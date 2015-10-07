using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseModifier : AvatarModifier
{
	protected delegate void ActionDelegate(float time);

	protected float timeToSize = 1.5f;

	protected float sizeModifier = 0.25f;

	protected float sizeUnstableAfterSeconds = 15f;

	protected float unstableSpeed = 10f;

	protected float sineStrength = 14f;

	protected bool isDeactivating;

	protected AudioSource audioSource;

	public AudioClip growSound;

	public AudioClip shrinkSound;

	public override AvatarModifierPackageType ModifierType => AvatarModifierPackageType.Shrunken;

	public override void ResetTimeStamp()
	{
		timeStamp = Time.time;
		owner.mvAvatar.Scale = Vector3.one * sizeModifier;
	}

	public override bool EvaluateShouldBeAdded(Dictionary<AvatarModifierPackageType, AvatarModifier> modifiers)
	{
		return true;
	}

	protected virtual void SetSizeModifier()
	{
		sizeModifier = 0.25f;
	}

	protected override void OnActivated(Avatar target)
	{
		audioSource = gameObject.GetComponent<AudioSource>();
		isDeactivating = false;
		SetSizeModifier();
		timeStamp = Time.time;
		owner = target;
		owner.mvAvatar.Body.BlobShadow.ScaleShadow(sizeModifier);
		Scale();
	}

	protected override void OnDeactivated(Avatar target)
	{
		isDeactivating = true;
		owner = target;
		owner.mvAvatar.Body.BlobShadow.ScaleShadow(1f);
		UnScale();
	}

	protected virtual void Scale()
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

	protected virtual void UnScale()
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

	protected IEnumerator DoForSeconds(float duration, ActionDelegate body)
	{
		float t = 0f;
		while (t < duration)
		{
			body(t / duration);
			t += Time.deltaTime;
			yield return 0f;
		}
		body(timeToSize);
	}

	protected float BlockStep(float t, float steps, float clampMin, float clampMax)
	{
		return Mathf.Clamp(Mathf.Round(t * steps) / steps, clampMin, clampMax);
	}

	private void Update()
	{
		if (!isDeactivating)
		{
			Unstablize();
		}
	}

	protected void Unstablize()
	{
		float num = Time.time - timeStamp;
		if (num > sizeUnstableAfterSeconds)
		{
			unstableSpeed += Time.deltaTime;
			owner.mvAvatar.Scale = Vector3.one * sizeModifier + Vector3.one * 0.03f * (1f - Mathf.Sin((num - sizeUnstableAfterSeconds) * unstableSpeed));
		}
	}
}
