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

	private void Awake()
	{
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
		UXMouseOverObject component = GetComponent<UXMouseOverObject>();
		component.OnMouseOverEnter = (UXMouseOverObject.OnMouseOverDelegate)Delegate.Combine(component.OnMouseOverEnter, new UXMouseOverObject.OnMouseOverDelegate(OnMouseOverEnter));
		component.OnMouseOverExit = (UXMouseOverObject.OnMouseOverDelegate)Delegate.Combine(component.OnMouseOverExit, new UXMouseOverObject.OnMouseOverDelegate(OnMouseOverExit));
		if (GetComponent<UXDropObject>() == null)
		{
			gameObject.AddComponent<UXDropObject>();
		}
		UXDropObject component2 = GetComponent<UXDropObject>();
		component2.OnDragOverEnter = (UXDropObject.OnDragOverEnterDelegate)Delegate.Combine(component2.OnDragOverEnter, new UXDropObject.OnDragOverEnterDelegate(OnDragOverEnter));
		component2.OnDragOverExit = (UXDropObject.OnDragOverExitDelegate)Delegate.Combine(component2.OnDragOverExit, new UXDropObject.OnDragOverExitDelegate(OnDragOverExit));
	}

	private void Start()
	{
		UpdateColor(ratio);
	}

	private IEnumerator Fade(float target, float duration)
	{
		yield return StartCoroutine(pTween.To(duration, ratio, target, (float t) =>
		{
			ratio = t;
			UpdateColor(Mathf.SmoothStep(0f, 1f, ratio));
		}));
	}

	private void UpdateColor(float ratio)
	{
		Color color = Color.Lerp(minColor, maxColor, ratio);
		if (useAlpha)
		{
			color.a = Mathf.Lerp(minAlpha, maxAlpha, ratio);
		}
		foreach (Material material in materials)
		{
			material.SetColor(shaderColorProperty, color);
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
		if (gameObject.activeInHierarchy)
		{
			StopAllCoroutines();
			StartCoroutine(Fade(1f, fadeInTime));
		}
		else
		{
			ratio = 1f;
			UpdateColor(ratio);
		}
	}

	public void OnMouseOverExit(UXMouseOverObject mouseOverObject)
	{
		if (gameObject.activeInHierarchy)
		{
			StopAllCoroutines();
			StartCoroutine(Fade(0f, fadeOutTime));
		}
		else
		{
			ratio = 0f;
			UpdateColor(ratio);
		}
	}

	public void UpdateMaterials()
	{
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
