using System.Collections;
using UnityEngine;

public abstract class SizeModifier : AvatarModifier
{
	protected delegate void ActionDelegate(float time);

	protected float timeToSize = 1.5f;

	protected float sizeModifier = 0.25f;

	protected float sizeUnstableAfterSeconds = 15f;

	protected float unstableSpeed = 10f;

	protected float sineStrength = 14f;

	protected bool isDeactivating;

	[SerializeField]
	protected AudioSource audioSource;

	public AudioClip growSound;

	public AudioClip shrinkSound;

	protected override void OnActivated(Avatar target)
	{
		isDeactivating = false;
		SetSizeModifier();
		timeStamp = Time.time;
		owner = target;
		owner.mvAvatar.Body.BlobShadow.ScaleShadow(sizeModifier);
		Scale();
	}

	public override void ResetTimeStamp()
	{
		timeStamp = Time.time;
		owner.mvAvatar.Scale = Vector3.one * sizeModifier;
	}

	protected override void OnDeactivated(Avatar target)
	{
		isDeactivating = true;
		owner = target;
		owner.mvAvatar.Body.BlobShadow.ScaleShadow(1f);
		UnScale();
	}

	protected virtual void SetSizeModifier()
	{
	}

	protected virtual void UnScale()
	{
	}

	protected virtual void Scale()
	{
	}

	protected float BlockStep(float t, float steps, float clampMin, float clampMax)
	{
		return Mathf.Clamp(Mathf.Round(t * steps) / steps, clampMin, clampMax);
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
