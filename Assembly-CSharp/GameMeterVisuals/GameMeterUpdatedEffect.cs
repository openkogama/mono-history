using System.Collections;
using UnityEngine;

namespace GameMeterVisuals;

public class GameMeterUpdatedEffect : GameMeterVisualEffect
{
	[SerializeField]
	private float scaleStrength = 0.08f;

	[SerializeField]
	private float scaleTime = 0.1f;

	[SerializeField]
	private RectTransform scaleTarget;

	private Vector3 startSize;

	private void OnEnable()
	{
		startSize = scaleTarget.transform.localScale;
	}

	public override void ExecuteEffect()
	{
		if (gameObject.activeInHierarchy)
		{
			scaleTarget.transform.localScale = startSize;
			StopAllCoroutines();
			StartCoroutine(AnimateScale());
		}
	}

	private IEnumerator AnimateScale()
	{
		for (float i = 0f; i < scaleTime; i += Time.deltaTime)
		{
			scaleTarget.transform.localScale = startSize + Vector3.one * (i / scaleTime * scaleStrength);
			yield return 0;
		}
		scaleTarget.transform.localScale = startSize + Vector3.one * scaleStrength;
		for (float i2 = 0f; i2 < scaleTime; i2 += Time.deltaTime)
		{
			Vector2.Lerp(scaleTarget.transform.localScale, startSize, i2 / scaleTime);
			yield return 0;
		}
		scaleTarget.transform.localScale = startSize;
		yield return 0;
	}

	private void OnDisable()
	{
		StopAllCoroutines();
		scaleTarget.transform.localScale = startSize;
	}
}
