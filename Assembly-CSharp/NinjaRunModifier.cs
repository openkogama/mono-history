using System.Collections;
using UnityEngine;

public class NinjaRunModifier : AvatarModifier
{
	public TrailRenderer trailRenderer;

	[SerializeField]
	private AudioSource soundEffect;

	private Vector3 oldPosition;

	private Vector3 oldScale;

	private bool isDestroying;

	private float minMagnitudeValue = 0.1f;

	[SerializeField]
	private float startWidth;

	[SerializeField]
	private float endWidth;

	[SerializeField]
	private float trailHeight;

	private Transform ownerTransform;

	public override AvatarModifierPackageType ModifierType => AvatarModifierPackageType.NinjaRun;

	protected override void OnActivated(Avatar target)
	{
		owner = target;
		ownerTransform = owner.transform;
		oldScale = owner.mvAvatar.Transform.localScale;
		trailRenderer.startWidth = startWidth * ownerTransform.localScale.x;
		trailRenderer.endWidth = endWidth * ownerTransform.localScale.x;
		Vector3 localPosition = trailRenderer.transform.localPosition;
		localPosition.y = trailHeight * ownerTransform.localScale.x;
		trailRenderer.transform.localPosition = localPosition;
	}

	protected override void OnDeactivated(Avatar target)
	{
		if (gameObject.activeInHierarchy)
		{
			StartCoroutine(DoFadeAndDestroy());
		}
		else
		{
			Object.Destroy(gameObject);
		}
	}

	private IEnumerator DoFadeAndDestroy()
	{
		isDestroying = true;
		soundEffect.volume = 0f;
		trailRenderer.gameObject.transform.SetParent(null);
		yield return new WaitForSeconds(trailRenderer.time);
		Object.Destroy(trailRenderer.gameObject);
		Object.Destroy(gameObject);
	}

	private void FixedUpdate()
	{
		if (!isDestroying && oldPosition != ownerTransform.position)
		{
			float magnitude = (ownerTransform.position - oldPosition).magnitude;
			if (ownerTransform.localScale != oldScale)
			{
				oldScale = ownerTransform.localScale;
				trailRenderer.startWidth = startWidth * ownerTransform.localScale.x;
				trailRenderer.endWidth = endWidth * ownerTransform.localScale.x;
			}
			if (owner.IsLocal)
			{
				soundEffect.volume = ((!(magnitude > 1f)) ? magnitude : 1f);
			}
			else
			{
				soundEffect.volume = ((!(magnitude > minMagnitudeValue)) ? 0f : 0.5f);
			}
			oldPosition = ownerTransform.position;
		}
	}
}
