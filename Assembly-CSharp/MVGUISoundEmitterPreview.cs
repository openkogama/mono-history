using UnityEngine;

public class MVGUISoundEmitterPreview : MonoBehaviour
{
	public void Update()
	{
		transform.Rotate(Vector3.up, -60f * Time.deltaTime);
	}
}
