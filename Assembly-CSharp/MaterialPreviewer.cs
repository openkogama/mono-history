using UnityEngine;

public class MaterialPreviewer : MonoBehaviour
{
	public RenderTexture renderTexture;

	public int previewResolution = 128;

	[SerializeField]
	private MeshRenderer meshRenderer;

	[SerializeField]
	private MeshFilter meshFilter;

	[SerializeField]
	private Camera pictureCamera;

	[SerializeField]
	private Transform cube;

	private void Awake()
	{
		meshRenderer.sharedMaterial = MVGameControllerBase.MaterialLoader.CubeModelMaterial;
	}

	public void Initialize(Mesh mesh)
	{
		meshFilter.sharedMesh = mesh;
		pictureCamera.aspect = 1f;
		pictureCamera.enabled = true;
		renderTexture = new RenderTexture(previewResolution, previewResolution, 16);
		renderTexture.filterMode = FilterMode.Bilinear;
		renderTexture.hideFlags = HideFlags.DontSave;
		renderTexture.antiAliasing = 2;
		pictureCamera.targetTexture = renderTexture;
	}

	private void OnDestroy()
	{
		pictureCamera.targetTexture = null;
		renderTexture.Release();
		renderTexture = null;
	}

	private void Update()
	{
		cube.transform.Rotate(Vector3.forward, -60f * Time.deltaTime);
	}
}
