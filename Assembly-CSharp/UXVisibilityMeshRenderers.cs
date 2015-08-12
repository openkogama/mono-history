using System;
using UnityEngine;

[RequireComponent(typeof(UXVisibility))]
public class UXVisibilityMeshRenderers : MonoBehaviour
{
	public string materialAlphaVariable = "_MainColor";

	public Color color = Color.white;

	private MeshRenderer[] meshRenderers;

	private void Awake()
	{
		meshRenderers = GetComponentsInChildren<MeshRenderer>();
		UXVisibility component = GetComponent<UXVisibility>();
		component.OnVisibilityChange = (UXVisibility.VisibilityChangeDelegate)Delegate.Combine(component.OnVisibilityChange, new UXVisibility.VisibilityChangeDelegate(HandleVisibilityChange));
	}

	private void HandleVisibilityChange(float visibility)
	{
		MeshRenderer[] array = meshRenderers;
		foreach (MeshRenderer meshRenderer in array)
		{
			meshRenderer.material.SetColor(materialAlphaVariable, new Color(color.r, color.g, color.b, color.a * visibility));
		}
	}
}
