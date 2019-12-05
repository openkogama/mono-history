using System.Collections;
using UnityEngine;

public abstract class SizeModifier : AvatarModifier
{
	protected delegate void ActionDelegate(float time);

	[SerializeField]
	protected float timeToSize = 1.5f;

	[SerializeField]
	protected float sizeModifier = 1f;

	[SerializeField]
	protected float sizeUnstableAfterSeconds = 28f;

	[SerializeField]
	protected float unstableSpeed = 10f;

	[SerializeField]
	protected float sineStrength = 14f;

	[SerializeField]
	protected AudioSource audioSource;

	protected Vector3 defaultScale = Vector3.one;

	protected bool isDeactivating;

	public AudioClip growSound;

	public AudioClip shrinkSound;

	protected override void OnActivated(Avatar target)
	{
		isDeactivating = false;
		timeStamp = Time.time;
		owner = target;
		owner.mvAvatar.Body.BlobShadow.ScaleShadow(sizeModifier);
		defaultScale = MVGameControllerBase.Game.LocalPlayer.SpawnRoleDataMediator.Scale;
		Scale();
	}

	public override void ResetTimeStamp()
	{
		timeStamp = Time.time;
		owner.mvAvatar.Scale = defaultScale * sizeModifier;
	}

	protected override void OnDeactivated(Avatar target)
	{
		isDeactivating = true;
		owner = target;
		owner.mvAvatar.Body.BlobShadow.ScaleShadow(1f);
		UnScale();
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
		if (!gameObject.activeInHierarchy)
		{
			body(timeToSize);
		}
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
			owner.mvAvatar.Scale = defaultScale * sizeModifier + defaultScale * 0.03f * (1f - Mathf.Sin((num - sizeUnstableAfterSeconds) * unstableSpeed));
		}
	}
}
