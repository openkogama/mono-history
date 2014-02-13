using System.Collections.Generic;
using UnityEngine;

public class PickupItemObjectScript : MonoBehaviour
{
	internal class PickupOriginalMaterials
	{
		public MeshRenderer meshRenderer;

		public Material[] originalMaterials;

		public PickupOriginalMaterials(MeshRenderer meshRenderer)
		{
			this.meshRenderer = meshRenderer;
			originalMaterials = ((Renderer)meshRenderer).sharedMaterials;
		}
	}

	public GameObject pickupObject;

	public Shader hiddenShader;

	private List<PickupOriginalMaterials> pickupOriginalMaterials = new List<PickupOriginalMaterials>();

	private void Awake()
	{
		((Behaviour)this).enabled = false;
		if (Object.op_Implicit((Object)(object)pickupObject))
		{
			InitializeOriginalMaterials();
		}
	}

	public void Take()
	{
		for (int num = pickupOriginalMaterials.Count - 1; num >= 0; num--)
		{
			if ((Object)(object)pickupOriginalMaterials[num].meshRenderer == (Object)null || (Object)(object)((Component)pickupOriginalMaterials[num].meshRenderer).gameObject == (Object)null)
			{
				pickupOriginalMaterials.RemoveAt(num);
			}
			else
			{
				Material[] materials = ((Renderer)pickupOriginalMaterials[num].meshRenderer).materials;
				foreach (Material val in materials)
				{
					val.shader = hiddenShader;
				}
			}
		}
	}

	public void InitializeOriginalMaterials()
	{
		pickupOriginalMaterials.Clear();
		MeshRenderer[] componentsInChildren = pickupObject.GetComponentsInChildren<MeshRenderer>();
		foreach (MeshRenderer meshRenderer in componentsInChildren)
		{
			pickupOriginalMaterials.Add(new PickupOriginalMaterials(meshRenderer));
		}
	}

	public void Respawn()
	{
		for (int num = pickupOriginalMaterials.Count - 1; num >= 0; num--)
		{
			if ((Object)(object)pickupOriginalMaterials[num].meshRenderer == (Object)null || (Object)(object)((Component)pickupOriginalMaterials[num].meshRenderer).gameObject == (Object)null)
			{
				pickupOriginalMaterials.RemoveAt(num);
			}
			else
			{
				((Renderer)pickupOriginalMaterials[num].meshRenderer).sharedMaterials = pickupOriginalMaterials[num].originalMaterials;
			}
		}
	}
}
