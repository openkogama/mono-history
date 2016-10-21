using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(WaterBase))]
public class SpecularLighting : MonoBehaviour
{
	public Transform specularLight;

	private WaterBase waterBase;

	public void Start()
	{
		waterBase = GetComponent<WaterBase>();
	}

	public void Update()
	{
		if (!waterBase)
		{
			waterBase = GetComponent<WaterBase>();
		}
		if ((bool)specularLight && (bool)waterBase.sharedMaterial)
		{
			waterBase.sharedMaterial.SetVector("_WorldLightDir", specularLight.transform.forward);
		}
	}
}
