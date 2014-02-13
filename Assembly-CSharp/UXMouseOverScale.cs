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
		UXMouseOverObject component = ((Component)this).GetComponent<UXMouseOverObject>();
		component.OnMouseOverEnter = (UXMouseOverObject.OnMouseOverDelegate)Delegate.Combine(component.OnMouseOverEnter, new UXMouseOverObject.OnMouseOverDelegate(OnMouseOverEnter));
		component.OnMouseOverExit = (UXMouseOverObject.OnMouseOverDelegate)Delegate.Combine(component.OnMouseOverExit, new UXMouseOverObject.OnMouseOverDelegate(OnMouseOverExit));
		screen = UXUtils.FindGUIObjectOfType<UXScreen>();
		UXScreen uXScreen = screen;
		uXScreen.OnFullScreenChange = (UXScreen.OnFullScreenChangeDelegate)Delegate.Combine(uXScreen.OnFullScreenChange, new UXScreen.OnFullScreenChangeDelegate(OnFullScreen));
		RefreshScaleValues();
	}

	public void RefreshScaleValues(bool resetScale = false)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		if (resetScale)
		{
			ratio = 0f;
			UpdateScale(ratio);
		}
		minScale = minScaleFactor * ((Component)this).transform.localScale;
		midScale = midScaleFactor * ((Component)this).transform.localScale;
		maxScale = maxScaleFactor * ((Component)this).transform.localScale;
		pos = ((Component)this).transform.localPosition;
		if (Object.op_Implicit((Object)(object)scaleWith))
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
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		if (ignoreFullScreen)
		{
			full = false;
		}
		fullScreen = full;
		ratio = 0f;
		UpdateScale(ratio);
		if (!freezePos)
		{
			((Component)this).transform.localPosition = pos + ((!full) ? scaledOffsetWindowed : scaledOffsetFull);
		}
		if ((Object)(object)scaleWith != (Object)null && !freezePos)
		{
			scaleWith.localPosition = scaleWithPos + ((!full) ? scaledOffsetWindowed : (-scaledOffsetFull));
		}
	}

	private IEnumerator Scale(float target, float duration)
	{
		yield return ((MonoBehaviour)this).StartCoroutine(pTween.To(duration, ratio, target, (float t) =>
		{
			ratio = t;
			UpdateScale(Mathf.SmoothStep(0f, 1f, ratio));
		}));
	}

	private void UpdateScale(float ratio)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
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
		((Component)this).transform.localScale = zero;
		if ((Object)(object)scaleWith != (Object)null)
		{
			scaleWith.localScale = zero2;
		}
	}

	public void OnMouseOverEnter(UXMouseOverObject mouseOverObject)
	{
		((MonoBehaviour)this).StopAllCoroutines();
		if (((Component)this).gameObject.active)
		{
			((MonoBehaviour)this).StartCoroutine(Scale(1f, 0.1f));
		}
	}

	public void OnMouseOverExit(UXMouseOverObject mouseOverObject)
	{
		((MonoBehaviour)this).StopAllCoroutines();
		if (((Component)this).gameObject.active)
		{
			((MonoBehaviour)this).StartCoroutine(Scale(0f, 0.1f));
		}
	}

	public void Reset()
	{
		ratio = 0f;
		UpdateScale(ratio);
	}
}
