using UnityEngine;

public class MVGUIMaterialPurchasePreviewCube : MonoBehaviour
{
	public void Update()
	{
		transform.Rotate(Vector3.forward, -60f * Time.deltaTime);
	}
}
