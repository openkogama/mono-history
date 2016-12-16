using UnityEngine;

public class GodzillaHover : MonoBehaviour
{
	[SerializeField]
	private float waveMagnitude = 1f;

	[SerializeField]
	private float waveFrequency = 1f;

	[SerializeField]
	private float startHeight;

	private void OnValidate()
	{
		transform.localPosition = new Vector3(transform.localPosition.x, startHeight, transform.localPosition.z);
	}

	private void Update()
	{
		float y = startHeight + Mathf.Sin(Time.time * waveFrequency) * waveMagnitude;
		transform.localPosition = new Vector3(transform.localPosition.x, y, transform.localPosition.z);
	}
}
