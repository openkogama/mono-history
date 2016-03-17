using UnityEngine;

public class PreviewObject : MonoBehaviour
{
	[SerializeField]
	private Camera previewCamera;

	[SerializeField]
	private Transform previewTransform;

	[SerializeField]
	private int width;

	[SerializeField]
	private int height;

	[SerializeField]
	private int depth;

	[SerializeField]
	private int antiAliasing;

	[SerializeField]
	private FilterMode filterMode;

	[SerializeField]
	private RenderTexture renderTexture;

	public RenderTexture RenderTexture => renderTexture;

	private void Awake()
	{
		renderTexture = new RenderTexture(width, height, depth);
		renderTexture.antiAliasing = antiAliasing;
		renderTexture.filterMode = filterMode;
		renderTexture.hideFlags = HideFlags.DontSave;
		previewCamera.targetTexture = renderTexture;
	}

	private void OnDestroy()
	{
		previewCamera.targetTexture = null;
		renderTexture.Release();
	}
}
