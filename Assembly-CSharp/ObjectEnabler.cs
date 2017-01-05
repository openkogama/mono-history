using UnityEngine;

public class ObjectEnabler : MonoBehaviour, IUpdatecontrollerSubscriber
{
	public MVObjectEnabler woObjectEnabler;

	private bool isEnabled;

	private float currentAlpha;

	private Material objectMaterial;

	private int nameToLayer;

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

	public void Initialize()
	{
		Material objectHiddenMaterial = PrefabPool.Instance.ObjectHiddenMaterial;
		objectMaterial = Object.Instantiate(objectHiddenMaterial);
		UpdateController.AddUpdateObject(this, UpdatePriority.UPDATEBUCKET_STANDARD);
		nameToLayer = LayerMask.NameToLayer("Default");
	}

	private void DrawObject(MeshFilter[] previewMeshes)
	{
		foreach (MeshFilter meshFilter in previewMeshes)
		{
			for (int j = 0; j < meshFilter.sharedMesh.subMeshCount; j++)
			{
				Graphics.DrawMesh(meshFilter.sharedMesh, meshFilter.transform.localToWorldMatrix, objectMaterial, nameToLayer, Camera.main, j);
			}
		}
	}

	private void OnDestroy()
	{
		UpdateController.RemoveUpdateObject(this);
		Object.Destroy(objectMaterial);
	}

	public void UpdateControllerUpdate()
	{
		float num = ((!isEnabled) ? 0.15f : 0.05f);
		currentAlpha = Mathf.Lerp(currentAlpha, num, Time.deltaTime * 1f);
		Color color = objectMaterial.color;
		color.a = currentAlpha;
		objectMaterial.color = color;
		if (!(isEnabled | (currentAlpha != num)) || !woObjectEnabler.ShowingOutline)
		{
			return;
		}
		for (int i = 0; i < woObjectEnabler.ObjectLinkRefs.Count; i++)
		{
			MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(woObjectEnabler.ObjectLinkRefs[i].objectWOID);
			if (worldObjectClient is MVCubeModelInstance && (worldObjectClient as MVCubeModelInstance).IsVisibleSet)
			{
				DrawObject((worldObjectClient as MVCubeModelInstance).MeshFilters);
			}
		}
	}

	public void UpdateControllerFixedUpdate()
	{
	}
}
