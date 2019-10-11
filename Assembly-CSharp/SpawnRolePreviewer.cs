using UnityEngine;

public class SpawnRolePreviewer : MonoBehaviour
{
	[SerializeField]
	private Camera previewCam;

	[SerializeField]
	private GrayscaleEffect grayScaleEffect;

	[SerializeField]
	private GameObject blobShadowPrefab;

	private RenderTexture previewTexture;

	private LayerFlags layersToRender;

	private Vector3 previewCamOffset = Vector3.zero;

	private Vector3 pivotPoint = Vector3.zero;

	private MVBodyObject body;

	private GameObject rootObject;

	public RenderTexture PreviewTexture => previewTexture;

	private GameObject PreviewGameObject { get; set; }

	public void Initialize(int textureWidth, int textureHeight, CameraClearFlags clearFlags, LayerFlags layersToRender, Vector3 cameraOffset, Transform previewSpawnRoleRoot, Vector3 previewPosition, string name, int spawnRoleId, GameObject woGameObjectCopy)
	{
		this.layersToRender = layersToRender | LayerFlags.Hidden;
		rootObject = previewSpawnRoleRoot.gameObject;
		previewCamOffset = cameraOffset;
		transform.parent = previewSpawnRoleRoot;
		base.gameObject.name = $"Preview_{name}_RenderCam";
		base.gameObject.layer = LayerMask.NameToLayer("Preview");
		previewTexture = RenderTexture.GetTemporary(textureWidth, textureHeight, 16, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default, 2);
		previewTexture.name = name;
		previewTexture.filterMode = FilterMode.Bilinear;
		previewTexture.hideFlags = HideFlags.DontSave;
		previewCam.targetTexture = previewTexture;
		PreviewGameObject = woGameObjectCopy;
		PreviewGameObject.name = "Preview_" + name + " " + spawnRoleId;
		PreviewGameObject.transform.parent = previewSpawnRoleRoot;
		PreviewGameObject.transform.localRotation = Quaternion.identity;
		PreviewGameObject.transform.position = previewPosition;
		PreviewGameObject.transform.RotateAround(pivotPoint, Vector3.up, 180f);
		previewCam.transform.position = PreviewGameObject.transform.position + previewCamOffset;
		GameObject gameObject = Object.Instantiate(blobShadowPrefab);
		gameObject.transform.SetParent(PreviewGameObject.transform, worldPositionStays: false);
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localScale = new Vector3(0.2f, 0.1f, 0.2f);
		body = PreviewGameObject.GetComponentInChildren<MVBodyObject>();
		PreviewGameObject.transform.SetLayerRecursively(LayerMask.NameToLayer("Preview"));
		SetRenderGrey(shouldRenderAsGrey: false);
		StartInactiveAnimation();
	}

	public void SetRenderGrey(bool shouldRenderAsGrey)
	{
		grayScaleEffect.enabled = shouldRenderAsGrey;
	}

	public void StartActiveAnimation()
	{
		if (body != null)
		{
			body.BoneAnimation.FallBackWalkSpeed = 0.7f;
			body.BoneAnimation.StartAnimation("Walk", MVGameControllerBase.Game.ServerTimeInMilliSeconds - 500);
		}
	}

	public void StartInactiveAnimation()
	{
		if (body != null)
		{
			int num = Random.Range(0, 80000);
			int timeStamp = MVGameControllerBase.Game.ServerTimeInMilliSeconds - 500 - num;
			body.BoneAnimation.StartAnimation("Idle", timeStamp);
		}
	}

	public void ActivatePreview()
	{
		previewCam.enabled = true;
		rootObject.SetActive(value: true);
	}

	public void DeactivatePreview()
	{
		previewCam.enabled = false;
		rootObject.SetActive(value: false);
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
			previewTexture.DiscardContents();
			RenderTexture.ReleaseTemporary(previewTexture);
			previewTexture = null;
		}
		Object.Destroy(PreviewGameObject);
		Object.Destroy(rootObject);
	}
}
