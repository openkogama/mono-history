using UnityEngine;

public class PreviewBox : MonoBehaviour
{
	private MeshRenderer meshRenderer;

	private MeshFilter meshFilter;

	private void Start()
	{
		gameObject.layer = LayerMask.NameToLayer("UIItems");
		meshRenderer = gameObject.GetComponent<MeshRenderer>();
		meshFilter = gameObject.GetComponent<MeshFilter>();
	}

	public void Show(Material material, Vector3[] corners)
	{
		if (meshRenderer == null)
		{
			meshRenderer = gameObject.AddComponent<MeshRenderer>();
		}
		meshRenderer.material = material;
		if (meshFilter == null)
		{
			meshFilter = gameObject.AddComponent<MeshFilter>();
		}
		meshRenderer.material.hideFlags = HideFlags.DontSave;
		meshFilter.mesh.Clear();
		SharedCubeFunctions.AddCubeMeshCubeLines(meshFilter.mesh, corners, 0.2f);
	}

	public void DestroyBox()
	{
		Object.Destroy(gameObject);
	}
}
