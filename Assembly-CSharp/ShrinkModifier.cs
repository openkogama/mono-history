using System.Collections;
using UnityEngine;

public class ShrinkModifier : AvatarModifier
{
	private delegate void ActionDelegate(float time);

	private MVTriggerHandler handler;

	private float timeToShrink = 2f;

	public override AvatarModifierPackageType ModifierType => AvatarModifierPackageType.Shrunken;

	protected override void OnActivated(Avatar target)
	{
		Debug.Log("Shrink started");
		owner = target;
		Shrink();
	}

	protected override void OnDeactivated(Avatar target)
	{
		Debug.Log("Shrink ended");
		owner = target;
		UnShrink();
	}

	private void Shrink()
	{
		StartCoroutine(DoForSeconds(timeToShrink, (float t) =>
		{
			owner.mvAvatar.Scale = Vector3.one * (1f - BlockStep(t, 40f, 0f, 0.75f)) + Vector3.one * 0.25f * (1f - Mathf.Sin(t * 14f));
		}));
	}

	private void UnShrink()
	{
		StartCoroutine(DoForSeconds(timeToShrink, (float t) =>
		{
			owner.mvAvatar.Scale = Vector3.one * BlockStep(t, 40f, 0.25f, 1f);
		}));
	}

	private IEnumerator DoForSeconds(float duration, ActionDelegate body)
	{
		float t = 0f;
		while (t < duration)
		{
			body(t / duration);
			t += Time.deltaTime;
			yield return 0f;
		}
		body(1f);
	}

	private float BlockStep(float t, float steps, float clampMin, float clampMax)
	{
		return Mathf.Clamp(Mathf.Round(t * steps) / steps, clampMin, clampMax);
	}
}
