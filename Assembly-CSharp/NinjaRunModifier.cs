using System.Collections;
using UnityEngine;

public class NinjaRunModifier : AvatarModifier
{
	public TrailRenderer trailRenderer;

	[SerializeField]
	private AudioSource soundEffect;

	private Vector3 oldPosition;

	private bool isDestroying;

	private float minMagnitudeValue = 0.1f;

	public override AvatarModifierPackageType ModifierType => AvatarModifierPackageType.NinjaRun;

	protected override void OnActivated(Avatar target)
	{
		owner = target;
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
		if (!isDestroying && oldPosition != owner.transform.position)
		{
			float magnitude = (owner.transform.position - oldPosition).magnitude;
			if (owner.IsLocal)
			{
				soundEffect.volume = ((!(magnitude > 1f)) ? magnitude : 1f);
			}
			else
			{
				soundEffect.volume = ((!(magnitude > minMagnitudeValue)) ? 0f : 0.5f);
			}
			oldPosition = owner.transform.position;
		}
	}
}
