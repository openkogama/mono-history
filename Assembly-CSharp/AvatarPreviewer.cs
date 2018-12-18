using UnityEngine;

public class AvatarPreviewer : MonoBehaviour
{
	private float previewObjMaxSize = 2f;

	private float previewCamAdditionalHeight = 0.5f;

	private float previewCamDist = 1.5f;

	private LayerFlags layersToRender;

	[SerializeField]
	public Camera previewCam;

	private RenderTexture previewTexture;

	private Vector3 pivotPoint;

	private const int lowResRT = 256;

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

	public void Initialize(int textureWidth, int textureHeight, CameraClearFlags clearFlags, LayerFlags layersToRender, Vector3 cameraOffset, Transform previewItemsRoot, Vector3 previewPosition, string name, MVWorldObjectClient wo, GameObject woGameObjectCopy, Vector3 additionalCameraRotation)
	{
		this.layersToRender = layersToRender | LayerFlags.Hidden;
		transform.parent = previewItemsRoot;
		previewItemsRoot.gameObject.name = "Avatar_Previewer";
		gameObject.name = $"Preview_{name}_RenderCam";
		gameObject.layer = LayerMask.NameToLayer("Preview");
		int antiAliasing = 2;
		try
		{
			previewTexture = new RenderTexture(textureWidth, textureHeight, 16, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default)
			{
				antiAliasing = antiAliasing
			};
			if (!previewTexture.Create())
			{
				Object.Destroy(previewTexture);
				previewTexture = new RenderTexture(256 * (textureWidth / textureHeight), 256, 16, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default)
				{
					antiAliasing = antiAliasing
				};
				previewTexture.Create();
			}
		}
		catch
		{
			Debug.LogWarning("Rendertexture not created, it is likely not supported on target device.");
			if (previewTexture != null)
			{
				Object.Destroy(previewTexture);
			}
			previewTexture = null;
			return;
		}
		previewTexture = new RenderTexture(textureWidth, textureHeight, 16, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default)
		{
			antiAliasing = antiAliasing
		};
		previewCam.targetTexture = previewTexture;
		PreviewGameObject = woGameObjectCopy;
		PreviewGameObject.name = "Preview_" + name + "_Item_" + wo.ItemId + "_woID_" + wo.Id;
		PreviewGameObject.transform.parent = previewItemsRoot;
		PreviewGameObject.transform.rotation = Quaternion.identity;
		PreviewGameObject.transform.position = previewPosition;
		Bounds localBounds = wo.GetLocalBounds(BoundsContext.Preview);
		Vector3 localScale = PreviewGameObject.transform.localScale;
		float num = Mathf.Max(localBounds.size.x, localBounds.size.y, localBounds.size.z);
		num = Mathf.Max(localBounds.size.x * localScale.x, localBounds.size.y * localScale.y, localBounds.size.z * localScale.z);
		float num2 = previewObjMaxSize / num;
		PreviewGameObject.transform.localScale = localScale * num2;
		Vector3 vector = new Vector3(localBounds.center.x * localScale.x, localBounds.center.y * localScale.y, localBounds.center.z * localScale.z);
		pivotPoint = vector * num2 + PreviewGameObject.transform.position;
		previewCam.transform.position = pivotPoint + (new Vector3(0f, previewCamAdditionalHeight, 0f - previewCamDist) + cameraOffset) * previewObjMaxSize;
		previewCam.transform.LookAt(pivotPoint);
		previewCam.transform.Rotate(additionalCameraRotation);
	}

	private void OnPreCull()
	{
		LayerUtil.SetLayerRecursively(PreviewGameObject.transform, (int)layersToRender, LayerMask.NameToLayer("Preview"));
	}

	private void OnPostRender()
	{
		LayerUtil.SetLayerRecursively(PreviewGameObject.transform, "Preview", "Hidden");
	}

	public void UpdateRotation(float rotateSpeed)
	{
		if (rotateSpeed != 0f && PreviewGameObject != null)
		{
			PreviewGameObject.transform.RotateAround(pivotPoint, Vector3.up, rotateSpeed * Time.deltaTime);
		}
	}

	private void OnDestroy()
	{
		if (previewCam != null)
		{
			previewCam.targetTexture = null;
			previewTexture = null;
		}
		if (previewTexture != null)
		{
			previewTexture.Release();
			Object.Destroy(previewTexture);
			previewTexture = null;
		}
		Object.Destroy(gameObject);
	}
}
