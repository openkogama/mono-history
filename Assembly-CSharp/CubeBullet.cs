using UnityEngine;

public class CubeBullet : MonoBehaviour
{
	private void Update()
	{
		transform.Rotate(Vector3.up, 300f * Time.deltaTime);
	}

	public void SetCubeMaterial(byte id)
	{
		GetComponent<Renderer>().sharedMaterial = MVGameController.Game.MaterialRepository.GetMaterial(id).material;
	}
}
