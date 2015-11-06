using MV.WorldObject;
using UnityEngine;

public class ObjectEnabler : MonoBehaviour
{
	public MVObjectEnabler woObjectEnabler;

	private bool isEnabled;

	private float currentAlpha;

	private Material blah;

	public bool IsDrawingEnabled
	{
		get
		{
			return isEnabled;
		}
		set
		{
			isEnabled = value;
		}
	}

	private void DrawObject(MeshFilter[] previewMeshes)
	{
		foreach (MeshFilter meshFilter in previewMeshes)
		{
			for (int j = 0; j < meshFilter.sharedMesh.subMeshCount; j++)
			{
				Graphics.DrawMesh(meshFilter.sharedMesh, meshFilter.transform.localToWorldMatrix, blah, LayerMask.NameToLayer("Default"), Camera.main, j);
			}
		}
	}

	private void Awake()
	{
		Material original = Resources.Load("Materials/ObjectHidden", typeof(Material)) as Material;
		blah = Object.Instantiate(original);
	}

	private void OnDestroy()
	{
		Object.Destroy(blah);
	}

	private void Update()
	{
		float num = ((!isEnabled) ? 0.15f : 0.05f);
		currentAlpha = Mathf.Lerp(currentAlpha, num, Time.deltaTime * 1f);
		Color color = blah.color;
		color.a = currentAlpha;
		blah.color = color;
		if (!(isEnabled | (currentAlpha != num)) || !woObjectEnabler.ShowingOutline)
		{
			return;
		}
		foreach (ObjectLink objectLinkRef in woObjectEnabler.ObjectLinkRefs)
		{
			MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(objectLinkRef.objectWOID);
			if (worldObjectClient is MVCubeModelInstance)
			{
				MVCubeModelInstance mVCubeModelInstance = worldObjectClient as MVCubeModelInstance;
				DrawObject(mVCubeModelInstance.MeshFilters);
			}
		}
	}
}
