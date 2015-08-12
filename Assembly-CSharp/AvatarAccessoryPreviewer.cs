using UnityEngine;

public class AvatarAccessoryPreviewer : MonoBehaviour
{
	private float previewCamAdditionalHeight = 1.2f;

	private float previewCamDist = 2.2f;

	private float previewItemRotateSpeed = 9.3f;

	private GameObject previewGameObject;

	private Camera previewCam;

	private Vector3 pivotPoint;

	public RenderTexture PreviewTexture { get; private set; }

	private AvatarAccessoryPreviewer()
	{
	}

	public static AvatarAccessoryPreviewer Create(int textureSize, CameraClearFlags clearFlags, Transform previewItemsRoot, string name, GameObject woGameObjectCopy)
	{
		return Create(textureSize, textureSize, clearFlags, previewItemsRoot, name, woGameObjectCopy);
	}

	public static AvatarAccessoryPreviewer Create(int textureWidth, int textureHeight, CameraClearFlags clearFlags, Transform previewItemsRoot, string name, GameObject woGameObjectCopy)
	{
		GameObject gameObject = new GameObject();
		AvatarAccessoryPreviewer avatarAccessoryPreviewer = gameObject.AddComponent<AvatarAccessoryPreviewer>();
		gameObject.transform.parent = previewItemsRoot;
		gameObject.name = $"Preview_{name}_RenderCam";
		gameObject.layer = LayerMask.NameToLayer("Preview");
		avatarAccessoryPreviewer.PreviewTexture = new RenderTexture(textureWidth, textureHeight, 16);
		avatarAccessoryPreviewer.PreviewTexture.filterMode = FilterMode.Bilinear;
		avatarAccessoryPreviewer.PreviewTexture.hideFlags = HideFlags.DontSave;
		avatarAccessoryPreviewer.previewCam = gameObject.AddComponent<Camera>();
		avatarAccessoryPreviewer.previewCam.clearFlags = clearFlags;
		avatarAccessoryPreviewer.previewCam.backgroundColor = new Color(0f, 0f, 0f, 0f);
		avatarAccessoryPreviewer.previewCam.fieldOfView = 35f;
		avatarAccessoryPreviewer.previewCam.aspect = (float)textureWidth / (float)textureHeight;
		avatarAccessoryPreviewer.previewCam.cullingMask = 1 << LayerMask.NameToLayer("Preview");
		avatarAccessoryPreviewer.previewCam.nearClipPlane = 0.05f;
		avatarAccessoryPreviewer.previewCam.farClipPlane = 100f;
		avatarAccessoryPreviewer.previewCam.targetTexture = avatarAccessoryPreviewer.PreviewTexture;
		avatarAccessoryPreviewer.previewGameObject = woGameObjectCopy;
		avatarAccessoryPreviewer.previewGameObject.name = "Preview_" + name + "_AvatarAccessory";
		avatarAccessoryPreviewer.previewGameObject.transform.parent = previewItemsRoot;
		avatarAccessoryPreviewer.previewGameObject.transform.localRotation = Quaternion.identity;
		avatarAccessoryPreviewer.previewGameObject.transform.position = Vector3.zero;
		MeshRenderer[] componentsInChildren = avatarAccessoryPreviewer.previewGameObject.GetComponentsInChildren<MeshRenderer>();
		MeshRenderer[] array = componentsInChildren;
		foreach (MeshRenderer meshRenderer in array)
		{
			meshRenderer.enabled = true;
		}
		Bounds localBounds = woGameObjectCopy.GetComponent<AvatarAccessory>().GetLocalBounds();
		float num = Mathf.Max(localBounds.size.x, localBounds.size.y, localBounds.size.z);
		float num2 = 1f / num;
		avatarAccessoryPreviewer.previewGameObject.transform.localScale = new Vector3(num2, num2, num2);
		avatarAccessoryPreviewer.pivotPoint = localBounds.center * num2 + avatarAccessoryPreviewer.previewGameObject.transform.position;
		avatarAccessoryPreviewer.previewGameObject.transform.RotateAround(avatarAccessoryPreviewer.pivotPoint, Vector3.up, 180f);
		avatarAccessoryPreviewer.previewCam.transform.position = avatarAccessoryPreviewer.pivotPoint + new Vector3(0f, avatarAccessoryPreviewer.previewCamAdditionalHeight, 0f - avatarAccessoryPreviewer.previewCamDist);
		avatarAccessoryPreviewer.previewCam.transform.LookAt(avatarAccessoryPreviewer.pivotPoint);
		return avatarAccessoryPreviewer;
	}

	private void OnPreCull()
	{
		LayerUtil.SetLayerRecursively(previewGameObject.transform, "Preview");
	}

	private void OnPostRender()
	{
		LayerUtil.SetLayerRecursively(previewGameObject.transform, "Hidden");
	}

	public void UpdateRotation(float rotateSpeed = 0f)
	{
		if (previewGameObject != null)
		{
			previewGameObject.transform.RotateAround(pivotPoint, Vector3.up, ((!(rotateSpeed > 0f)) ? previewItemRotateSpeed : rotateSpeed) * Time.deltaTime);
		}
	}

	public void Destroy()
	{
		if (previewCam != null)
		{
			previewCam.targetTexture = null;
		}
		if (this != null)
		{
			Object.Destroy(gameObject);
		}
		if (PreviewTexture != null)
		{
			Object.Destroy(PreviewTexture);
		}
	}
}
