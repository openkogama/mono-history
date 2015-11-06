using UnityEngine;

public class CubeBullet : MonoBehaviour
{
	private void Update()
	{
		transform.Rotate(Vector3.up, 300f * Time.deltaTime);
	}

	public void SetCubeMaterial(byte id)
	{
		GetComponent<Renderer>().sharedMaterial = MVGameControllerBase.Game.MaterialRepository.GetMaterial(id).material;
	}
}
