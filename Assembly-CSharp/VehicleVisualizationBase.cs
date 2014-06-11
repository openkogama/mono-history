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
				foreach (Renderer val in array)
				{
					val.enabled = true;
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
				foreach (Renderer val2 in array2)
				{
					val2.enabled = false;
				}
			}
		}
		if (!isInSpawner)
		{
			if (((Behaviour)this).enabled && distance > disableVisualizationDistance)
			{
				((Behaviour)this).enabled = false;
			}
			if (!((Behaviour)this).enabled && distance <= disableVisualizationDistance)
			{
				((Behaviour)this).enabled = true;
			}
		}
	}

	protected static void ParentHullTransformToVisualizationRoot(Transform hullTransform, Transform visualizationRoot)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		Vector3 localPosition = hullTransform.localPosition;
		Quaternion localRotation = hullTransform.localRotation;
		hullTransform.parent = visualizationRoot;
		hullTransform.localPosition = localPosition;
		hullTransform.localRotation = localRotation;
	}
}
