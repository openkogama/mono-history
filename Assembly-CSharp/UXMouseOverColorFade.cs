using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UXMouseOverObject))]
[RequireComponent(typeof(UXDropObject))]
public class UXMouseOverColorFade : MonoBehaviour
{
	public bool HitOnDragOver;

	public HashSet<Material> materials = new HashSet<Material>();

	public string shaderColorProperty = "_MainColor";

	public Color color = Color.white;

	public Color toColor = Color.white;

	public bool useMaterialColor;

	public bool fadeChangeColor;

	public float minColorRatio = 0.9f;

	public float maxColorRatio = 1f;

	public bool useAlpha;

	public float minAlpha = 1f;

	public float maxAlpha = 1f;

	public float fadeInTime = 0.1f;

	public float fadeOutTime = 0.3f;

	private float ratio;

	private Color minColor;

	private Color maxColor;

	public UXMouseOverColorFade()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
	}

	private void Awake()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		if (fadeChangeColor)
		{
			minColor = color;
			maxColor = toColor;
		}
		else
		{
			minColor = minColorRatio * color;
			maxColor = maxColorRatio * color;
		}
		UXMouseOverObject component = ((Component)this).GetComponent<UXMouseOverObject>();
		component.OnMouseOverEnter = (UXMouseOverObject.OnMouseOverDelegate)Delegate.Combine(component.OnMouseOverEnter, new UXMouseOverObject.OnMouseOverDelegate(OnMouseOverEnter));
		component.OnMouseOverExit = (UXMouseOverObject.OnMouseOverDelegate)Delegate.Combine(component.OnMouseOverExit, new UXMouseOverObject.OnMouseOverDelegate(OnMouseOverExit));
		if ((Object)(object)((Component)this).GetComponent<UXDropObject>() == (Object)null)
		{
			((Component)this).gameObject.AddComponent<UXDropObject>();
		}
		UXDropObject component2 = ((Component)this).GetComponent<UXDropObject>();
		component2.OnDragOverEnter = (UXDropObject.OnDragOverEnterDelegate)Delegate.Combine(component2.OnDragOverEnter, new UXDropObject.OnDragOverEnterDelegate(OnDragOverEnter));
		component2.OnDragOverExit = (UXDropObject.OnDragOverExitDelegate)Delegate.Combine(component2.OnDragOverExit, new UXDropObject.OnDragOverExitDelegate(OnDragOverExit));
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

	private void OnDragOverEnter(GameObject dropObject)
	{
		if (HitOnDragOver)
		{
			OnMouseOverEnter(null);
		}
	}

	private void OnDragOverExit(GameObject dropObject)
	{
		if (HitOnDragOver)
		{
			OnMouseOverExit(null);
		}
	}

	public void OnMouseOverEnter(UXMouseOverObject mouseOverObject)
	{
		if (((Component)this).gameObject.active)
		{
			((MonoBehaviour)this).StopAllCoroutines();
			((MonoBehaviour)this).StartCoroutine(Fade(1f, fadeInTime));
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
			((MonoBehaviour)this).StartCoroutine(Fade(0f, fadeOutTime));
		}
		else
		{
			ratio = 0f;
			UpdateColor(ratio);
		}
	}

	public void UpdateMaterials()
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		if (useMaterialColor && materials.Count > 0)
		{
			foreach (Material material in materials)
			{
				color = material.GetColor("_MainColor");
			}
		}
		UpdateColor(ratio);
	}

	public void SetNewColor(Color color)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		this.color = color;
		if (fadeChangeColor)
		{
			minColor = color;
			maxColor = toColor;
		}
		else
		{
			minColor = minColorRatio * color;
			maxColor = maxColorRatio * color;
		}
	}
}
