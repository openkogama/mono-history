using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UXMouseOverHighlight : MonoBehaviour
{
	private const float FADE_IN_TIME = 0.1f;

	private const float FADE_OUT_TIME = 0.3f;

	private const float MIN_COLOR_RATIO = 0.9f;

	private const float MAX_COLOR_RATIO = 1f;

	private float ratio;

	private Color minColor;

	private Color maxColor;

	public string shaderColorProperty = "_MainColor";

	private List<Material> materials = new List<Material>();

	public bool HitOnDragOver;

	public void Awake()
	{
		UXMouseOverObject uXMouseOverObject = UXUtils.AddComponentIfNotExists<UXMouseOverObject>(((Component)this).gameObject);
		uXMouseOverObject.OnMouseOverEnter = (UXMouseOverObject.OnMouseOverDelegate)Delegate.Combine(uXMouseOverObject.OnMouseOverEnter, new UXMouseOverObject.OnMouseOverDelegate(OnMouseOverEnter));
		uXMouseOverObject.OnMouseOverExit = (UXMouseOverObject.OnMouseOverDelegate)Delegate.Combine(uXMouseOverObject.OnMouseOverExit, new UXMouseOverObject.OnMouseOverDelegate(OnMouseOverExit));
		UXDropObject uXDropObject = UXUtils.AddComponentIfNotExists<UXDropObject>(((Component)this).gameObject);
		uXDropObject.OnDragOverEnter = (UXDropObject.OnDragOverEnterDelegate)Delegate.Combine(uXDropObject.OnDragOverEnter, (UXDropObject.OnDragOverEnterDelegate)((GameObject drop) =>
		{
			if (HitOnDragOver)
			{
				OnMouseOverEnter(null);
			}
		}));
		uXDropObject.OnDragOverExit = (UXDropObject.OnDragOverExitDelegate)Delegate.Combine(uXDropObject.OnDragOverExit, (UXDropObject.OnDragOverExitDelegate)((GameObject drop) =>
		{
			if (HitOnDragOver)
			{
				OnMouseOverExit(null);
			}
		}));
	}

	public void Start()
	{
		UpdateColor(ratio);
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
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		Color val = Color.Lerp(minColor, maxColor, ratio);
		foreach (Material material in materials)
		{
			material.SetColor(shaderColorProperty, val);
		}
	}

	public void AddMaterial(Material material)
	{
		materials.Add(material);
		UpdateMinMaxColor();
	}

	public void ClearMaterials()
	{
		materials.Clear();
	}

	private void UpdateMinMaxColor()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		if (materials.Count != 0)
		{
			Color color = materials[0].GetColor(shaderColorProperty);
			minColor = 0.9f * color;
			maxColor = 1f * color;
			minColor.a = (maxColor.a = 1f);
		}
	}
}
