using UnityEngine;

public class FlareLight : MonoBehaviour
{
	[SerializeField]
	private Light light;

	[SerializeField]
	private LensFlare lensFlare;

	public Light Light => light;

	public LensFlare LensFlare => lensFlare;

	protected void OnEnable()
	{
		SetEnabled(b: true);
	}

	protected void OnDisable()
	{
		SetEnabled(b: false);
	}

	private void SetEnabled(bool b)
	{
		lensFlare.enabled = b;
	}

	protected void Reset()
	{
		light = GetComponent<Light>();
		lensFlare = GetComponent<LensFlare>();
	}
}
