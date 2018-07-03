using UnityEngine;

public class AccessoryPreviewer : MonoBehaviour
{
	private RenderTexture previewTexture;

	private Camera previewCam;

	private GameObject previewGameObject;

	private LayerFlags layersToRender;

	private static Vector3 previewPosition = new Vector3(0f, 0f, 0f);

	private Transform rootTransform;

	private Vector3 pivotPoint;

	public RenderTexture PreviewTexture => previewTexture;

	public void Initialize(int textureWidth, int textureHeight, LayerFlags layersToRender, CameraClearFlags clearFlags, Vector3 cameraPosOffset, Vector3 cameraRotOffset, GameObject woGameObjectCopy, Transform rootTransform)
	{
		GameObject gameObject = base.gameObject;
		this.rootTransform = rootTransform;
		this.layersToRender = layersToRender | LayerFlags.Hidden;
		gameObject.transform.parent = rootTransform;
		gameObject.name = $"RenderCam_Preview_{woGameObjectCopy.name}";
		gameObject.layer = LayerMask.NameToLayer("Preview");
		previewTexture = new RenderTexture(textureWidth, textureHeight, 16, RenderTextureFormat.ARGB32);
		previewTexture.name = woGameObjectCopy.name;
		previewTexture.antiAliasing = 2;
		previewTexture.filterMode = FilterMode.Bilinear;
		previewTexture.hideFlags = HideFlags.DontSave;
		previewCam = gameObject.AddComponent<Camera>();
		previewCam.clearFlags = clearFlags;
		previewCam.backgroundColor = new Color(0f, 0f, 0f, 0f);
		previewCam.fieldOfView = 35f;
		previewCam.aspect = (float)textureWidth / (float)textureHeight;
		previewCam.cullingMask = 1 << LayerMask.NameToLayer("Preview");
		previewCam.nearClipPlane = 0.05f;
		previewCam.farClipPlane = 100f;
		previewCam.targetTexture = previewTexture;
		previewCam.orthographic = true;
		previewCam.orthographicSize = 0.88f;
		previewGameObject = woGameObjectCopy;
		previewGameObject.name = "Preview_" + woGameObjectCopy.name;
		previewGameObject.transform.parent = rootTransform;
		previewGameObject.transform.localRotation = Quaternion.identity;
		previewGameObject.transform.position = previewPosition;
		previewPosition += new Vector3(30f, 0f, 0f);
		previewGameObject.transform.RotateAround(previewGameObject.transform.position, Vector3.up, 215f);
		Bounds bounds = ComputeLocalBounds(previewGameObject);
		Vector3 localScale = previewGameObject.transform.localScale;
		float num = Mathf.Max(bounds.size.x * localScale.x, bounds.size.y * localScale.y, bounds.size.z * localScale.z);
		float num2 = 2f / num;
		previewGameObject.transform.localScale = Vector3.Min(localScale * num2, new Vector3(2f, 2f, 2f));
		previewGameObject.transform.position = previewGameObject.transform.position - new Vector3(0f, bounds.min.y, 0f);
		previewCam.transform.position = previewGameObject.transform.position + cameraPosOffset;
		Vector3 position = previewCam.transform.position;
		position.y = cameraPosOffset.y;
		previewCam.transform.position = position;
		previewCam.transform.LookAt(previewGameObject.transform);
		previewCam.transform.Rotate(cameraRotOffset);
	}

	private static Bounds ComputeLocalBounds(GameObject go)
	{
		Renderer[] componentsInChildren = go.GetComponentsInChildren<Renderer>();
		Bounds result = new Bounds(Vector3.zero, Vector3.zero);
		if (componentsInChildren.Length > 0)
		{
			Bounds bounds = componentsInChildren[0].bounds;
			bounds.center -= go.transform.position;
			result = bounds;
			for (int i = 1; i < componentsInChildren.Length; i++)
			{
				bounds = componentsInChildren[i].bounds;
				bounds.center -= go.transform.position;
				result.Encapsulate(bounds);
			}
		}
		else
		{
			Debug.LogWarning("Renderers required for correct bounds");
		}
		return result;
	}

	public void Destroy()
	{
		if (previewCam != null)
		{
			RenderTexture targetTexture = previewCam.targetTexture;
			previewCam.targetTexture = null;
			targetTexture.Release();
		}
		if (rootTransform != null)
		{
			Object.Destroy(rootTransform.gameObject);
			rootTransform = null;
		}
		if (previewTexture != null)
		{
			Object.Destroy(previewTexture);
		}
		Object.Destroy(gameObject);
	}

	private void OnPreCull()
	{
		LayerUtil.SetLayerRecursively(previewGameObject.transform, (int)layersToRender, LayerMask.NameToLayer("Preview"));
	}

	private void OnPostRender()
	{
		LayerUtil.SetLayerRecursively(previewGameObject.transform, "Preview", "Hidden");
	}
}
