using UnityEngine;

public class CubeBullet : MonoBehaviour
{
	private void Update()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).transform.Rotate(Vector3.up, 300f * Time.deltaTime);
	}

	public void SetCubeMaterial(byte id)
	{
		((Component)this).renderer.sharedMaterial = MVGameController.Instance.Game.MaterialRepository.GetMaterial(id).material;
	}
}
