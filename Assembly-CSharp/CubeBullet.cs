using UnityEngine;

public class CubeBullet : MonoBehaviour
{
	[SerializeField]
	private MeshFilter meshFilter;

	[SerializeField]
	private MeshRenderer meshRenderer;

	public MeshFilter MeshFilter => meshFilter;

	public MeshRenderer MeshRenderer => meshRenderer;

	private void Update()
	{
		transform.Rotate(Vector3.up, 300f * Time.deltaTime);
	}

	public void SetCubeMaterial(byte id)
	{
		meshFilter.sharedMesh = MVGameControllerBase.Game.MaterialRepository.GetMaterial(id).mesh;
		meshRenderer.sharedMaterial = MVGameControllerBase.MaterialLoader.CubeModelMaterial;
	}
}
