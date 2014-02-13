using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public class AdvancedGhostIcon : MonoBehaviour
{
	private SphereVolumeIndicator sphereVolumeIndicator;

	public GhostBody GhostBody;

	public float Radius
	{
		set
		{
			sphereVolumeIndicator.Radius = value;
		}
	}

	public void Init(MVCubeModelBase body)
	{
		AddSphereVolumeIndicator();
		body.Changed += body_Changed;
		CloneCubeMeshes(body);
	}

	private void CloneCubeMeshes(MVCubeModelBase body)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Expected Obj, but got Unknown
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Expected Obj, but got Unknown
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		((Component)GhostBody).transform.localScale = Vector3.one;
		foreach (Transform item in ((Component)GhostBody).transform)
		{
			Transform val = item;
			Object.Destroy((Object)(object)((Component)val).gameObject);
		}
		foreach (KeyValuePair<IntVector, GameObject> chunkInstance in body.chunkInstances)
		{
			GameObject val2 = (GameObject)Object.Instantiate((Object)(object)chunkInstance.Value);
			val2.transform.parent = ((Component)GhostBody).transform;
			val2.transform.localPosition = Vector3.zero;
			val2.transform.localRotation = Quaternion.identity;
			val2.gameObject.layer = ((Component)GhostBody).gameObject.layer;
			val2.gameObject.SetActiveRecursively(((Component)GhostBody).gameObject.active);
		}
		((Component)GhostBody).transform.localScale = body.Transform.localScale;
	}

	private void body_Changed(object sender, CubeModelChangedEventArgs e)
	{
		CloneCubeMeshes((MVCubeModelBase)sender);
	}

	private void AddSphereVolumeIndicator()
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		sphereVolumeIndicator = Object.Instantiate(Resources.Load("Prefabs/Effects/RangeVisualization", typeof(SphereVolumeIndicator))) as SphereVolumeIndicator;
		((Component)sphereVolumeIndicator).transform.parent = ((Component)this).gameObject.transform;
		((Component)sphereVolumeIndicator).transform.localPosition = Vector3.zero;
		((Component)sphereVolumeIndicator).transform.localRotation = Quaternion.identity;
	}
}
