using UnityEngine;

public class XpBoostParticlePreviewer : MonoBehaviour
{
	private LayerFlags layersToRender;

	[SerializeField]
	public Camera previewCam;

	[SerializeField]
	private ParticleSystem xpBoostParticles;

	private bool isParticlesPlaying;

	private RenderTexture previewTexture;

	private const int lowResRT = 256;

	public bool IsParticlesPlaying => isParticlesPlaying;

	public RenderTexture PreviewTexture => previewTexture;

	public GameObject PreviewGameObject { get; private set; }

	public void FaceGameObject(GameObject go)
	{
		previewCam.transform.LookAt(go.transform);
	}

	public void OverrideCameraForPreviewer(Vector3 cameraAngle, Vector3 cameraOffset)
	{
		Transform transform = previewCam.transform;
		transform.rotation = Quaternion.Euler(cameraAngle);
		transform.position = cameraOffset;
	}

	public void Initialize(int textureWidth, int textureHeight, CameraClearFlags clearFlags, LayerFlags layersToRender, Vector3 cameraOffset, Vector3 previewPosition)
	{
		xpBoostParticles.Stop();
		this.layersToRender = layersToRender | LayerFlags.Hidden;
		gameObject.name = $"Preview_{name}_RenderCam";
		gameObject.layer = LayerMask.NameToLayer("Preview");
		int antiAliasing = 2;
		previewTexture = RenderTexture.GetTemporary(textureWidth, textureHeight, 16, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default, antiAliasing);
		previewCam.targetTexture = previewTexture;
		PreviewGameObject = xpBoostParticles.gameObject;
		transform.position = previewPosition;
		previewCam.transform.LookAt(xpBoostParticles.transform.position);
	}

	public void StartParticleSystem()
	{
		isParticlesPlaying = true;
		xpBoostParticles.gameObject.SetActive(value: true);
		xpBoostParticles.Play();
	}

	public void StopParticleSystem()
	{
		isParticlesPlaying = false;
		xpBoostParticles.Stop();
		xpBoostParticles.gameObject.SetActive(value: false);
	}

	private void OnPreCull()
	{
		LayerUtil.SetLayerRecursively(PreviewGameObject.transform, (int)layersToRender, LayerMask.NameToLayer("Preview"));
	}

	private void OnPostRender()
	{
		LayerUtil.SetLayerRecursively(PreviewGameObject.transform, "Preview", "Hidden");
	}

	private void OnDestroy()
	{
		if (previewCam != null)
		{
			previewCam.targetTexture = null;
		}
		if (previewTexture != null)
		{
			RenderTexture.ReleaseTemporary(previewTexture);
			previewTexture = null;
		}
		Object.Destroy(gameObject);
	}
}
