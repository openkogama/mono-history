using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(UXMouseOverObject))]
public class UXSimpleMouseOverScale : MonoBehaviour
{
	public float minScale = 0.7f;

	public float maxScale = 1.3f;

	public float scaleUpDuration = 0.1f;

	public float scaleDownDuration = 0.1f;

	private float ratio;

	private void Awake()
	{
		UXMouseOverObject component = GetComponent<UXMouseOverObject>();
		component.OnMouseOverEnter = (UXMouseOverObject.OnMouseOverDelegate)Delegate.Combine(component.OnMouseOverEnter, new UXMouseOverObject.OnMouseOverDelegate(OnMouseOverEnter));
		component.OnMouseOverExit = (UXMouseOverObject.OnMouseOverDelegate)Delegate.Combine(component.OnMouseOverExit, new UXMouseOverObject.OnMouseOverDelegate(OnMouseOverExit));
		RefreshScaleValues();
	}

	public void RefreshScaleValues(bool resetScale = false)
	{
		if (resetScale)
		{
			ratio = 0f;
			UpdateScale(ratio);
		}
	}

	private IEnumerator Scale(float target, float duration)
	{
		yield return StartCoroutine(pTween.To(duration, ratio, target, (float t) =>
		{
			ratio = t;
			UpdateScale(Mathf.SmoothStep(0f, 1f, ratio));
		}));
	}

	private void UpdateScale(float ratio)
	{
		transform.localScale = Vector3.one * Mathf.Lerp(minScale, maxScale, ratio);
	}

	public void OnMouseOverEnter(UXMouseOverObject mouseOverObject)
	{
		StopAllCoroutines();
		if (gameObject.activeInHierarchy)
		{
			StartCoroutine(Scale(1f, scaleUpDuration));
		}
	}

	public void OnMouseOverExit(UXMouseOverObject mouseOverObject)
	{
		StopAllCoroutines();
		if (gameObject.activeInHierarchy)
		{
			StartCoroutine(Scale(0f, scaleDownDuration));
		}
	}

	public void Reset()
	{
		ratio = 0f;
		UpdateScale(ratio);
	}
}
