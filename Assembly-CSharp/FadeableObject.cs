using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FadeableObject : MonoBehaviour
{
	private List<Material> materials = new List<Material>();

	private void Start()
	{
		Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			Material[] array = componentsInChildren[i].materials;
			for (int j = 0; j < array.Length; j++)
			{
				materials.Add(array[j]);
			}
		}
		int k;
		for (k = 0; k < materials.Count; k++)
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IFadeParent x, BaseEventData y) =>
			{
				x.AddFadeMaterial(materials[k]);
			});
		}
	}

	private void OnDestroy()
	{
		int i;
		for (i = 0; i < materials.Count; i++)
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IFadeParent x, BaseEventData y) =>
			{
				x.RemoveFadeMaterial(materials[i]);
			});
		}
		materials.Clear();
	}
}
