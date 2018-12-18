using UnityEngine;

public class ObjectEnabler : MonoBehaviour, IUpdatecontrollerSubscriber
{
	public MVObjectEnabler woObjectEnabler;

	private Material objectMaterial;

	private int nameToLayer;

	private float currentAlpha;

	private Camera mainCamera;

	public bool IsDrawingEnabled { get; set; }

	protected void Awake()
	{
		objectMaterial = Object.Instantiate(PrefabPool.Instance.ObjectHiddenMaterial);
	}

	protected void OnDestroy()
	{
		UpdateController.RemoveUpdateObject(this);
		Object.Destroy(objectMaterial);
	}

	public void Initialize()
	{
		UpdateController.AddUpdateObject(this, UpdatePriority.UPDATEBUCKET_STANDARD);
		nameToLayer = LayerMask.NameToLayer("Default");
		mainCamera = Camera.main;
	}

	private void DrawObject(MeshFilter[] previewMeshes)
	{
		foreach (MeshFilter meshFilter in previewMeshes)
		{
			for (int j = 0; j < meshFilter.sharedMesh.subMeshCount; j++)
			{
				Graphics.DrawMesh(meshFilter.sharedMesh, meshFilter.transform.localToWorldMatrix, objectMaterial, nameToLayer, mainCamera, j);
			}
		}
	}

	public void UpdateControllerUpdate()
	{
		float num = ((!IsDrawingEnabled) ? 0.15f : 0.05f);
		currentAlpha = Mathf.Lerp(currentAlpha, num, Time.deltaTime * 1f);
		Color color = objectMaterial.color;
		color.a = currentAlpha;
		objectMaterial.color = color;
		if (!(IsDrawingEnabled | (currentAlpha != num)) || !woObjectEnabler.ShowingOutline)
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
