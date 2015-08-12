using System.Collections;
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
		GhostBody.transform.localScale = Vector3.one;
		foreach (Transform item in GhostBody.transform)
		{
			Object.Destroy(item.gameObject);
		}
		foreach (KeyValuePair<IntVector, GameObject> item2 in (IEnumerable)body.ChunkInstances)
		{
			GameObject gameObject = Object.Instantiate(item2.Value);
			gameObject.transform.parent = GhostBody.transform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.gameObject.layer = GhostBody.gameObject.layer;
			gameObject.gameObject.SetActive(GhostBody.gameObject.activeSelf);
		}
		GhostBody.transform.localScale = body.Transform.localScale;
	}

	private void body_Changed(object sender, CubeModelChangedEventArgs e)
	{
		CloneCubeMeshes((MVCubeModelBase)sender);
	}

	private void AddSphereVolumeIndicator()
	{
		sphereVolumeIndicator = Object.Instantiate(Resources.Load("Prefabs/Effects/RangeVisualization", typeof(SphereVolumeIndicator))) as SphereVolumeIndicator;
		sphereVolumeIndicator.transform.parent = gameObject.transform;
		sphereVolumeIndicator.transform.localPosition = Vector3.zero;
		sphereVolumeIndicator.transform.localRotation = Quaternion.identity;
	}
}
