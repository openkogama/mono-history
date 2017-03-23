using System.Collections.Generic;
using UnityEngine;

public class InventoryItemPreviewer : MonoBehaviour
{
	private float previewObjMaxSize = 2f;

	private float previewItemRotateSpeed = 9.3f;

	private Vector3 previewCamOffset = Vector3.zero;

	private LayerFlags layersToRender;

	[SerializeField]
	public Camera previewCam;

	private RenderTexture previewTexture;

	private Vector3 pivotPoint;

	private readonly Dictionary<string, float> WorldObjectCameraFOVOverload = new Dictionary<string, float> { { "HamsterWheel", 50f } };

	public RenderTexture PreviewTexture => previewTexture;

	public GameObject PreviewGameObject { get; private set; }

	public void Initialize(int textureWidth, int textureHeight, CameraClearFlags clearFlags, LayerFlags layersToRender, Vector3 cameraOffset, Transform previewItemsRoot, Vector3 previewPosition, string name, MVWorldObjectClient wo, GameObject woGameObjectCopy)
	{
		this.layersToRender = layersToRender | LayerFlags.Hidden;
		if (WorldObjectCameraFOVOverload.ContainsKey(name))
		{
			previewCam.fieldOfView = WorldObjectCameraFOVOverload[name];
		}
		previewCamOffset = cameraOffset;
		transform.parent = previewItemsRoot;
		gameObject.name = $"Preview_{name}_RenderCam";
		gameObject.layer = LayerMask.NameToLayer("Preview");
		previewTexture = new RenderTexture(textureWidth, textureHeight, 16, RenderTextureFormat.Default);
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
		Renderer[] componentsInChildren = PreviewGameObject.GetComponentsInChildren<Renderer>();
		Renderer[] array = componentsInChildren;
		foreach (Renderer renderer in array)
		{
			if (renderer.GetComponent<TriggerBoxEvents>() != null)
			{
				renderer.enabled = false;
			}
		}
		SelectionBox[] componentsInChildren2 = PreviewGameObject.GetComponentsInChildren<SelectionBox>();
		SelectionBox[] array2 = componentsInChildren2;
		foreach (SelectionBox selectionBox in array2)
		{
			selectionBox.GetComponent<Renderer>().enabled = false;
		}
		Bounds localBounds = wo.GetLocalBounds(BoundsContext.Preview);
		Vector3 localScale = PreviewGameObject.transform.localScale;
		float num = Mathf.Max(localBounds.size.x, localBounds.size.y, localBounds.size.z);
		num = Mathf.Max(localBounds.size.x * localScale.x, localBounds.size.y * localScale.y, localBounds.size.z * localScale.z);
		float num2 = previewObjMaxSize / num;
		if (wo is MVMovingPlatformGroup)
		{
			LineRenderer componentInChildren = PreviewGameObject.GetComponentInChildren<LineRenderer>();
			componentInChildren.SetWidth(0.3f * num2, 0.3f * num2);
		}
		PreviewGameObject.transform.localScale = localScale * num2;
		Vector3 vector = new Vector3(localBounds.center.x * localScale.x, localBounds.center.y * localScale.y, localBounds.center.z * localScale.z);
		pivotPoint = vector * num2 + PreviewGameObject.transform.position;
		PreviewGameObject.transform.RotateAround(pivotPoint, Vector3.up, 180f);
		previewCam.transform.position = pivotPoint + previewCamOffset;
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
