using UnityEngine;

public class MVPointLightObject : ObjectPrefab
{
	[SerializeField]
	private Light pointLight;

	[SerializeField]
	private GameObject visualObject;

	public GameObject VisualObject => visualObject;

	public Light PointLight => pointLight;
}
