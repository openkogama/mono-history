using System;
using UnityEngine;

[RequireComponent(typeof(UXVisibility))]
public class UXVisibilityMeshRenderers : MonoBehaviour
{
	public string materialAlphaVariable = "_MainColor";

	public Color color = Color.white;

	private MeshRenderer[] meshRenderers;

	public UXVisibilityMeshRenderers()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
	}

	private void Awake()
	{
		meshRenderers = ((Component)this).GetComponentsInChildren<MeshRenderer>();
		UXVisibility component = ((Component)this).GetComponent<UXVisibility>();
		component.OnVisibilityChange = (UXVisibility.VisibilityChangeDelegate)Delegate.Combine(component.OnVisibilityChange, new UXVisibility.VisibilityChangeDelegate(HandleVisibilityChange));
	}

	private void HandleVisibilityChange(float visibility)
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		MeshRenderer[] array = meshRenderers;
		foreach (MeshRenderer val in array)
		{
			((Renderer)val).material.SetColor(materialAlphaVariable, new Color(color.r, color.g, color.b, color.a * visibility));
		}
	}
}
