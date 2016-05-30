using UnityEngine;

public class AvatarPreviewer : MonoBehaviour
{
	private float previewObjMaxSize = 2f;

	private float previewCamAdditionalHeight = 0.5f;

	private float previewCamDist = 1.5f;

	private float previewItemRotateSpeed = 9.3f;

	private LayerFlags layersToRender;

	[SerializeField]
	public Camera previewCam;

	private RenderTexture previewTexture;

	private Vector3 pivotPoint;

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

	public void Initialize(int textureWidth, int textureHeight, CameraClearFlags clearFlags, LayerFlags layersToRender, Vector3 cameraOffset, Transform previewItemsRoot, Vector3 previewPosition, string name, MVWorldObjectClient wo, GameObject woGameObjectCopy)
	{
		this.layersToRender = layersToRender | LayerFlags.Hidden;
		transform.parent = previewItemsRoot;
		gameObject.name = $"Preview_{name}_RenderCam";
		gameObject.layer = LayerMask.NameToLayer("Preview");
		previewTexture = new RenderTexture(textureWidth, textureHeight, 16, RenderTextureFormat.ARGB32);
		previewTexture.name = name;
		previewTexture.filterMode = FilterMode.Bilinear;
		previewTexture.hideFlags = HideFlags.DontSave;
		previewTexture.antiAliasing = 2;
		previewCam.targetTexture = previewTexture;
		PreviewGameObject = woGameObjectCopy;
		PreviewGameObject.name = "Preview_" + name + "_Item_" + wo.ItemId + "_woID_" + wo.Id;
		PreviewGameObject.transform.parent = previewItemsRoot;
		PreviewGameObject.transform.localRotation = Quaternion.identity;
		PreviewGameObject.transform.position = previewPosition;
		Bounds localBounds = wo.GetLocalBounds(BoundsContext.Preview);
		Vector3 localScale = PreviewGameObject.transform.localScale;
		float num = Mathf.Max(localBounds.size.x, localBounds.size.y, localBounds.size.z);
		num = Mathf.Max(localBounds.size.x * localScale.x, localBounds.size.y * localScale.y, localBounds.size.z * localScale.z);
		float num2 = previewObjMaxSize / num;
		PreviewGameObject.transform.localScale = localScale * num2;
		Vector3 vector = new Vector3(localBounds.center.x * localScale.x, localBounds.center.y * localScale.y, localBounds.center.z * localScale.z);
		pivotPoint = vector * num2 + PreviewGameObject.transform.position;
		PreviewGameObject.transform.RotateAround(pivotPoint, Vector3.up, 180f);
		previewCam.transform.position = pivotPoint + (new Vector3(0f, previewCamAdditionalHeight, 0f - previewCamDist) + cameraOffset) * previewObjMaxSize;
		previewCam.transform.LookAt(pivotPoint);
	}

	private void OnPreCull()
	{
		LayerUtil.SetLayerRecursively(PreviewGameObject.transform, (int)layersToRender, LayerMask.NameToLayer("Preview"));
	}

	private void OnPostRender()
	{
		LayerUtil.SetLayerRecursively(PreviewGameObject.transform, "Preview", "Hidden");
	}

	public void UpdateRotation(float rotateSpeed = 0f)
	{
		if (PreviewGameObject != null)
		{
			PreviewGameObject.transform.RotateAround(pivotPoint, Vector3.up, ((!(rotateSpeed > 0f)) ? previewItemRotateSpeed : rotateSpeed) * Time.deltaTime);
		}
	}

	private void OnDestroy()
	{
		if (previewCam != null)
		{
			RenderTexture targetTexture = previewCam.targetTexture;
			previewCam.targetTexture = null;
			targetTexture.Release();
		}
		if (previewTexture != null)
		{
			Object.Destroy(previewTexture);
		}
		Object.Destroy(gameObject);
	}
}
