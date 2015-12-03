using UnityEngine;

public class CubeBullet : MonoBehaviour
{
	private void Update()
	{
		transform.Rotate(Vector3.up, 300f * Time.deltaTime);
	}

	public void SetCubeMaterial(byte id)
	{
		GetComponent<MeshFilter>().sharedMesh = MVGameControllerBase.Game.MaterialRepository.GetMaterial(id).mesh;
		GetComponent<Renderer>().sharedMaterial = MVGameControllerBase.MaterialLoader.CubeModelMaterial;
	}
}
