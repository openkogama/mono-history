using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(UXMouseOverObject))]
public class UXMouseOverScale : MonoBehaviour
{
	public float minScaleFactor = 0.7f;

	public float midScaleFactor = 1f;

	public float maxScaleFactor = 1.3f;

	public Transform scaleWith;

	public bool freezePos;

	public bool ignoreFullScreen;

	public Vector3 scaledOffsetWindowed;

	public Vector3 scaledOffsetFull;

	private Vector3 minScale;

	private Vector3 midScale;

	private Vector3 maxScale;

	private Vector3 minScaleWith;

	private Vector3 midScaleWith;

	private Vector3 maxScaleWith;

	private UXScreen screen;

	private float ratio;

	private bool fullScreen;

	private Vector3 pos;

	private Vector3 scaleWithPos;

	private void Awake()
	{
		UXMouseOverObject component = GetComponent<UXMouseOverObject>();
		component.OnMouseOverEnter = (UXMouseOverObject.OnMouseOverDelegate)Delegate.Combine(component.OnMouseOverEnter, new UXMouseOverObject.OnMouseOverDelegate(OnMouseOverEnter));
		component.OnMouseOverExit = (UXMouseOverObject.OnMouseOverDelegate)Delegate.Combine(component.OnMouseOverExit, new UXMouseOverObject.OnMouseOverDelegate(OnMouseOverExit));
		screen = UXUtils.UXScreen;
		UXScreen uXScreen = screen;
		uXScreen.OnFullScreenChange = (UXScreen.OnFullScreenChangeDelegate)Delegate.Combine(uXScreen.OnFullScreenChange, new UXScreen.OnFullScreenChangeDelegate(OnFullScreen));
		RefreshScaleValues();
	}

	public void RefreshScaleValues(bool resetScale = false)
	{
		if (resetScale)
		{
			ratio = 0f;
			UpdateScale(ratio);
		}
		minScale = minScaleFactor * transform.localScale;
		midScale = midScaleFactor * transform.localScale;
		maxScale = maxScaleFactor * transform.localScale;
		pos = transform.localPosition;
		if ((bool)scaleWith)
		{
			minScaleWith = minScaleFactor * scaleWith.localScale;
			midScaleWith = midScaleFactor * scaleWith.localScale;
			maxScaleWith = maxScaleFactor * scaleWith.localScale;
			scaleWithPos = scaleWith.localPosition;
		}
	}

	private void Start()
	{
		OnFullScreen(fullScreen);
	}

	private void OnDestroy()
	{
		UXScreen uXScreen = screen;
		uXScreen.OnFullScreenChange = (UXScreen.OnFullScreenChangeDelegate)Delegate.Remove(uXScreen.OnFullScreenChange, new UXScreen.OnFullScreenChangeDelegate(OnFullScreen));
	}

	private void OnFullScreen(bool full)
	{
		if (ignoreFullScreen)
		{
			full = false;
		}
		fullScreen = full;
		ratio = 0f;
		UpdateScale(ratio);
		if (!freezePos)
		{
			transform.localPosition = pos + ((!full) ? scaledOffsetWindowed : scaledOffsetFull);
		}
		if (scaleWith != null && !freezePos)
		{
			scaleWith.localPosition = scaleWithPos + ((!full) ? scaledOffsetWindowed : (-scaledOffsetFull));
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
		Vector3 zero = Vector3.zero;
		Vector3 zero2 = Vector3.zero;
		if (fullScreen && !ignoreFullScreen)
		{
			zero = Vector3.Lerp(midScale, maxScale, ratio);
			zero2 = Vector3.Lerp(midScaleWith, maxScaleWith, ratio);
		}
		else
		{
			zero = Vector3.Lerp(minScale, midScale, ratio);
			zero2 = Vector3.Lerp(minScaleWith, midScaleWith, ratio);
		}
		transform.localScale = zero;
		if (scaleWith != null)
		{
			scaleWith.localScale = zero2;
		}
	}

	public void OnMouseOverEnter(UXMouseOverObject mouseOverObject)
	{
		StopAllCoroutines();
		if (gameObject.activeInHierarchy)
		{
			StartCoroutine(Scale(1f, 0.1f));
		}
	}

	public void OnMouseOverExit(UXMouseOverObject mouseOverObject)
	{
		StopAllCoroutines();
		if (gameObject.activeInHierarchy)
		{
			StartCoroutine(Scale(0f, 0.1f));
		}
	}

	public void Reset()
	{
		ratio = 0f;
		UpdateScale(ratio);
	}
}
