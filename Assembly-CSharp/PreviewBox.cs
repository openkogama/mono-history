using UnityEngine;

public class PreviewBox : MonoBehaviour
{
	private void Start()
	{
		gameObject.layer = LayerMask.NameToLayer("UIItems");
	}

	public void Show(Material material, Vector3[] corners)
	{
		MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
		if (meshRenderer == null)
		{
			meshRenderer = gameObject.AddComponent<MeshRenderer>();
		}
		meshRenderer.material = material;
		MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
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
