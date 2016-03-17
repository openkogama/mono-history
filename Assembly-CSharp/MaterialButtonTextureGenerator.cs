using UnityEngine;

public class MaterialButtonTextureGenerator : MonoBehaviour
{
	public int previewResolution = 128;

	[SerializeField]
	private MeshRenderer meshRenderer;

	[SerializeField]
	private MeshFilter meshFilter;

	[SerializeField]
	private Camera pictureCamera;

	[SerializeField]
	private Texture2D testTexture2D;

	private void Awake()
	{
		Object.DontDestroyOnLoad(gameObject);
		meshRenderer.sharedMaterial = MVGameControllerBase.MaterialLoader.CubeModelMaterial;
	}

	public Texture2D TakePicture(Mesh mesh)
	{
		meshFilter.sharedMesh = mesh;
		pictureCamera.aspect = 1f;
		pictureCamera.enabled = true;
		RenderTexture renderTexture = new RenderTexture(previewResolution, previewResolution, 16);
		renderTexture.filterMode = FilterMode.Bilinear;
		renderTexture.hideFlags = HideFlags.DontSave;
		renderTexture.antiAliasing = 2;
		pictureCamera.targetTexture = renderTexture;
		pictureCamera.Render();
		pictureCamera.enabled = false;
		Texture2D texture2D = new Texture2D(previewResolution, previewResolution, TextureFormat.ARGB32, mipmap: false);
		RenderTexture.active = renderTexture;
		texture2D.ReadPixels(new Rect(0f, 0f, renderTexture.width, renderTexture.height), 0, 0);
		texture2D.Apply();
		RenderTexture.active = null;
		renderTexture.Release();
		transform.parent = null;
		testTexture2D = texture2D;
		return texture2D;
	}

	private void OnDestroy()
	{
		if (pictureCamera != null)
		{
			RenderTexture targetTexture = pictureCamera.targetTexture;
			pictureCamera.targetTexture = null;
			targetTexture.Release();
		}
		pictureCamera.targetTexture = null;
	}
}
