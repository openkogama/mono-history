using UnityEngine;

public class ObjectPreviewer : MonoBehaviour
{
	private const float previewObjMaxSize = 2f;

	private const float previewCamAdditionalHeight = 1.2f;

	private const float previewCamDist = 2.2f;

	private const float previewItemRotateSpeed = 9.3f;

	private static int previewerIndex = 1;

	private Vector3 previewCamOffset = Vector3.zero;

	public LayerFlags layersToRender;

	private Camera previewCam;

	private Vector3 pivotPoint;

	public RenderTexture PreviewTexture { get; private set; }

	public GameObject PreviewGameObject { get; private set; }

	private ObjectPreviewer()
	{
	}

	public static ObjectPreviewer Create(int textureSize, CameraClearFlags clearFlags, LayerFlags layersToRender, Transform previewItemsRoot, string name, GameObject woGameObjectCopy)
	{
		Vector3 previewPosition = new Vector3(10 * previewerIndex++, 300f, 0f);
		return Create(textureSize, textureSize, clearFlags, layersToRender, Vector3.zero, previewItemsRoot, previewPosition, name, null, woGameObjectCopy);
	}

	public static ObjectPreviewer Create(int textureSize, CameraClearFlags clearFlags, LayerFlags layersToRender, Vector3 cameraOffset, Transform previewItemsRoot, Vector3 previewPosition, string name, MVWorldObjectClient wo, GameObject woGameObjectCopy)
	{
		return Create(textureSize, textureSize, clearFlags, layersToRender, cameraOffset, previewItemsRoot, previewPosition, name, wo, woGameObjectCopy);
	}

	public static ObjectPreviewer Create(int textureWidth, int textureHeight, CameraClearFlags clearFlags, LayerFlags layersToRender, Vector3 cameraOffset, Transform previewItemsRoot, Vector3 previewPosition, string name, MVWorldObjectClient wo, GameObject woGameObjectCopy)
	{
		GameObject gameObject = new GameObject();
		ObjectPreviewer objectPreviewer = gameObject.AddComponent<ObjectPreviewer>();
		objectPreviewer.layersToRender = layersToRender | LayerFlags.Hidden;
		objectPreviewer.previewCamOffset = cameraOffset;
		gameObject.transform.parent = previewItemsRoot;
		gameObject.name = $"Preview_{name}_RenderCam";
		gameObject.layer = LayerMask.NameToLayer("Preview");
		objectPreviewer.PreviewTexture = new RenderTexture(textureWidth, textureHeight, 16, RenderTextureFormat.ARGB32);
		objectPreviewer.PreviewTexture.name = name;
		objectPreviewer.PreviewTexture.antiAliasing = 2;
		objectPreviewer.PreviewTexture.filterMode = FilterMode.Bilinear;
		objectPreviewer.PreviewTexture.hideFlags = HideFlags.DontSave;
		objectPreviewer.previewCam = gameObject.AddComponent<Camera>();
		objectPreviewer.previewCam.clearFlags = clearFlags;
		objectPreviewer.previewCam.backgroundColor = new Color(0f, 0f, 0f, 0f);
		objectPreviewer.previewCam.fieldOfView = 35f;
		objectPreviewer.previewCam.aspect = (float)textureWidth / (float)textureHeight;
		objectPreviewer.previewCam.cullingMask = 1 << LayerMask.NameToLayer("Preview");
		objectPreviewer.previewCam.nearClipPlane = 0.05f;
		objectPreviewer.previewCam.farClipPlane = 100f;
		objectPreviewer.previewCam.targetTexture = objectPreviewer.PreviewTexture;
		objectPreviewer.PreviewGameObject = woGameObjectCopy;
		if (wo != null)
		{
			objectPreviewer.PreviewGameObject.name = "Preview_" + name + "_Item_" + wo.ItemId + "_woID_" + wo.Id;
		}
		else
		{
			objectPreviewer.PreviewGameObject.name = "Preview_" + name;
		}
		objectPreviewer.PreviewGameObject.transform.parent = previewItemsRoot;
		objectPreviewer.PreviewGameObject.transform.localRotation = Quaternion.identity;
		objectPreviewer.PreviewGameObject.transform.position = previewPosition;
		Renderer[] componentsInChildren = objectPreviewer.PreviewGameObject.GetComponentsInChildren<Renderer>();
		Renderer[] array = componentsInChildren;
		foreach (Renderer renderer in array)
		{
			if (renderer.GetComponent<TriggerBoxEvents>() != null)
			{
				renderer.enabled = false;
			}
		}
		SelectionBox[] componentsInChildren2 = objectPreviewer.PreviewGameObject.GetComponentsInChildren<SelectionBox>();
		SelectionBox[] array2 = componentsInChildren2;
		foreach (SelectionBox selectionBox in array2)
		{
			selectionBox.GetComponent<Renderer>().enabled = false;
		}
		Bounds bounds;
		if (wo != null)
		{
			bounds = wo.GetLocalBounds(BoundsContext.Preview);
		}
		else
		{
			AvatarAccessory component = objectPreviewer.PreviewGameObject.GetComponent<AvatarAccessory>();
			bounds = ((!(component != null)) ? ComputeLocalBounds(objectPreviewer.PreviewGameObject) : component.GetLocalBounds());
		}
		Vector3 localScale = objectPreviewer.PreviewGameObject.transform.localScale;
		float num = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
		num = Mathf.Max(bounds.size.x * localScale.x, bounds.size.y * localScale.y, bounds.size.z * localScale.z);
		float num2 = 2f / num;
		if (wo is MVMovingPlatformGroup)
		{
			LineRenderer componentInChildren = objectPreviewer.PreviewGameObject.GetComponentInChildren<LineRenderer>();
			float endWidth = (componentInChildren.startWidth = 0.3f * num2);
			componentInChildren.endWidth = endWidth;
		}
		objectPreviewer.PreviewGameObject.transform.localScale = localScale * num2;
		Vector3 vector = new Vector3(bounds.center.x * localScale.x, bounds.center.y * localScale.y, bounds.center.z * localScale.z);
		objectPreviewer.pivotPoint = vector * num2 + objectPreviewer.PreviewGameObject.transform.position;
		objectPreviewer.PreviewGameObject.transform.RotateAround(objectPreviewer.pivotPoint, Vector3.up, 180f);
		objectPreviewer.previewCam.transform.position = objectPreviewer.pivotPoint + (new Vector3(0f, 1.2f, -2.2f) + objectPreviewer.previewCamOffset) * 2f;
		objectPreviewer.previewCam.transform.LookAt(objectPreviewer.pivotPoint);
		return objectPreviewer;
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
			PreviewGameObject.transform.RotateAround(pivotPoint, Vector3.up, ((!(rotateSpeed > 0f)) ? 9.3f : rotateSpeed) * Time.deltaTime);
		}
	}

	public void Destroy()
	{
		if (previewCam != null)
		{
			RenderTexture targetTexture = previewCam.targetTexture;
			previewCam.targetTexture = null;
			targetTexture.Release();
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
