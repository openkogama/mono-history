using UnityEngine;

public class MVPointLightObject : ObjectPrefab
{
	[SerializeField]
	private Light pointLight;

	[SerializeField]
	private GameObject visualObject;

	[SerializeField]
	private StreamedTextureToMaterial streamedTexture;

	[SerializeField]
	private MeshRenderer pointLightPlaneMesh;

	[SerializeField]
	private Transform pointLightPlaneTransform;

	public GameObject VisualObject => visualObject;

	public Light PointLight => pointLight;

	public StreamedTextureToMaterial StreamedTexture => streamedTexture;

	public MeshRenderer PointLightPlaneMesh => pointLightPlaneMesh;

	public Transform PointLightPlaneTransform => pointLightPlaneTransform;
}
