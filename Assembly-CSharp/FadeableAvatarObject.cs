using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FadeableAvatarObject : MonoBehaviour
{
	private GameObject avatarObject;

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
		if (avatarObject != null)
		{
			AddMaterialsToAvatarFader();
		}
	}

	public void Initialize(GameObject avatarObject)
	{
		this.avatarObject = avatarObject;
		AddMaterialsToAvatarFader();
	}

	private void AddMaterialsToAvatarFader()
	{
		int i;
		for (i = 0; i < materials.Count; i++)
		{
			ExecuteEvents.ExecuteHierarchy(avatarObject, null, (IFadeParent x, BaseEventData y) =>
			{
				x.AddFadeMaterial(materials[i]);
			});
		}
	}

	private void OnDestroy()
	{
		if (avatarObject != null)
		{
			int i;
			for (i = 0; i < materials.Count; i++)
			{
				ExecuteEvents.ExecuteHierarchy(avatarObject, null, (IFadeParent x, BaseEventData y) =>
				{
					x.RemoveFadeMaterial(materials[i]);
				});
			}
		}
		materials.Clear();
	}
}
