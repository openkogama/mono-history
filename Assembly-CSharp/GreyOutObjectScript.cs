using System;
using System.Collections.Generic;
using UnityEngine;

public class GreyOutObjectScript : MonoBehaviour
{
	private class PickupOriginalMaterials
	{
		public MeshRenderer meshRenderer;

		public Material[] originalMaterials;

		public bool meshRendererEnabled = true;

		public PickupOriginalMaterials(MeshRenderer meshRenderer)
		{
			this.meshRenderer = meshRenderer;
			originalMaterials = meshRenderer.sharedMaterials;
		}

		public override string ToString()
		{
			string empty = string.Empty;
			empty += $"GameObject: {meshRenderer.gameObject.name}.";
			empty += $"meshRenderer.enabled: {meshRenderer.enabled}.";
			return empty + $"meshRendererEnabled: {meshRendererEnabled}.";
		}
	}

	public GameObject pickupObject;

	public Shader hiddenShader;

	private List<PickupOriginalMaterials> pickupOriginalMaterials = new List<PickupOriginalMaterials>();

	private void Awake()
	{
		enabled = false;
		if ((bool)pickupObject)
		{
			InitializeOriginalMaterials();
		}
	}

	public void Hide()
	{
		ExecuteOnMaterials(HideExec);
	}

	public void GreyIn()
	{
		ExecuteOnMaterials(GreyInExec);
	}

	public void GreyOut()
	{
		ExecuteOnMaterials(GreyOutExec);
	}

	public void InitializeOriginalMaterials()
	{
		pickupOriginalMaterials.Clear();
		MeshRenderer[] componentsInChildren = pickupObject.GetComponentsInChildren<MeshRenderer>();
		foreach (MeshRenderer meshRenderer in componentsInChildren)
		{
			PickupOriginalMaterials item = new PickupOriginalMaterials(meshRenderer);
			pickupOriginalMaterials.Add(item);
		}
	}

	private static void GreyInExec(PickupOriginalMaterials pickupOriginalMaterial)
	{
		pickupOriginalMaterial.meshRenderer.sharedMaterials = pickupOriginalMaterial.originalMaterials;
		pickupOriginalMaterial.meshRenderer.enabled = pickupOriginalMaterial.meshRendererEnabled;
	}

	private void GreyOutExec(PickupOriginalMaterials pickupOriginalMaterial)
	{
		Material[] materials = pickupOriginalMaterial.meshRenderer.materials;
		foreach (Material material in materials)
		{
			material.shader = hiddenShader;
		}
	}

	private void HideExec(PickupOriginalMaterials pickupOriginalMaterial)
	{
		pickupOriginalMaterial.meshRendererEnabled = pickupOriginalMaterial.meshRenderer.enabled;
		pickupOriginalMaterial.meshRenderer.enabled = false;
	}

	private void ExecuteOnMaterials(Action<PickupOriginalMaterials> action)
	{
		for (int num = pickupOriginalMaterials.Count - 1; num >= 0; num--)
		{
			if (pickupOriginalMaterials[num].meshRenderer == null || pickupOriginalMaterials[num].meshRenderer.gameObject == null)
			{
				pickupOriginalMaterials.RemoveAt(num);
			}
			else
			{
				action(pickupOriginalMaterials[num]);
			}
		}
	}
}
