using System.Collections.Generic;
using UnityEngine;

public class VehicleVisualizationBase : MonoBehaviour
{
	protected float disableVisualizationDistance = 40f;

	protected float cullDistance = 145f;

	private bool disabledByLod;

	protected bool isInSpawner;

	public List<GameObject> lodGameObjects = new List<GameObject>();

	public virtual void ChangeLOD(float distance)
	{
		if (disabledByLod && distance < cullDistance)
		{
			disabledByLod = false;
			foreach (GameObject lodGameObject in lodGameObjects)
			{
				Renderer[] componentsInChildren = lodGameObject.GetComponentsInChildren<Renderer>();
				Renderer[] array = componentsInChildren;
				foreach (Renderer renderer in array)
				{
					renderer.enabled = true;
				}
			}
		}
		else if (!disabledByLod && distance >= cullDistance)
		{
			disabledByLod = true;
			foreach (GameObject lodGameObject2 in lodGameObjects)
			{
				Renderer[] componentsInChildren2 = lodGameObject2.GetComponentsInChildren<Renderer>();
				Renderer[] array2 = componentsInChildren2;
				foreach (Renderer renderer2 in array2)
				{
					renderer2.enabled = false;
				}
			}
		}
		if (!isInSpawner)
		{
			if (enabled && distance > disableVisualizationDistance)
			{
				enabled = false;
			}
			if (!enabled && distance <= disableVisualizationDistance)
			{
				enabled = true;
			}
		}
	}

	protected static void ParentHullTransformToVisualizationRoot(Transform hullTransform, Transform visualizationRoot)
	{
		Vector3 localPosition = hullTransform.localPosition;
		Quaternion localRotation = hullTransform.localRotation;
		hullTransform.parent = visualizationRoot;
		hullTransform.localPosition = localPosition;
		hullTransform.localRotation = localRotation;
	}
}
