using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UXMouseOverObject))]
public class UXMouseOverColorFade : MonoBehaviour
{
	public HashSet<Material> materials = new HashSet<Material>();

	public string shaderColorProperty = "_MainColor";

	public Color color = Color.white;

	public float minColorRatio = 0.5f;

	public float maxColorRatio = 0.6f;

	public bool useAlpha;

	public float minAlpha = 1f;

	public float maxAlpha = 1f;

	private float ratio;

	private Color minColor;

	private Color maxColor;

	public UXMouseOverColorFade()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
	}

	private void Awake()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		minColor = minColorRatio * color;
		maxColor = maxColorRatio * color;
		UXMouseOverObject component = ((Component)this).GetComponent<UXMouseOverObject>();
		component.OnMouseOverEnter = (UXMouseOverObject.OnMouseOverEnterDelegate)Delegate.Combine(component.OnMouseOverEnter, new UXMouseOverObject.OnMouseOverEnterDelegate(OnMouseOverEnter));
		component.OnMouseOverExit = (UXMouseOverObject.OnMouseOverExitDelegate)Delegate.Combine(component.OnMouseOverExit, new UXMouseOverObject.OnMouseOverExitDelegate(OnMouseOverExit));
	}

	private void Start()
	{
		UpdateColor(ratio);
	}

	private IEnumerator Fade(float target, float duration)
	{
		yield return ((MonoBehaviour)this).StartCoroutine(pTween.To(duration, ratio, target, (float t) =>
		{
			ratio = t;
			UpdateColor(Mathf.SmoothStep(0f, 1f, ratio));
		}));
	}

	private void UpdateColor(float ratio)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		Color val = Color.Lerp(minColor, maxColor, ratio);
		if (useAlpha)
		{
			val.a = Mathf.Lerp(minAlpha, maxAlpha, ratio);
		}
		foreach (Material material in materials)
		{
			material.SetColor(shaderColorProperty, val);
		}
	}

	public void OnMouseOverEnter(UXMouseOverObject mouseOverObject)
	{
		if (((Component)this).gameObject.active)
		{
			((MonoBehaviour)this).StopAllCoroutines();
			((MonoBehaviour)this).StartCoroutine(Fade(1f, 0.1f));
		}
		else
		{
			ratio = 1f;
			UpdateColor(ratio);
		}
	}

	public void OnMouseOverExit(UXMouseOverObject mouseOverObject)
	{
		if (((Component)this).gameObject.active)
		{
			((MonoBehaviour)this).StopAllCoroutines();
			((MonoBehaviour)this).StartCoroutine(Fade(0f, 0.3f));
		}
		else
		{
			ratio = 0f;
			UpdateColor(ratio);
		}
	}

	public void UpdateMaterials()
	{
		UpdateColor(ratio);
	}
}
