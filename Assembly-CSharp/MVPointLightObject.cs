using UnityEngine;

public class MVPointLightObject : ObjectPrefab
{
	[SerializeField]
	private Light pointLight;

	public Light PointLight => pointLight;
}
