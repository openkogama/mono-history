using System.Collections;
using UnityEngine;

public class MVGUIAvatarAccessoryShopPreview : MonoBehaviour
{
	private const float LOADING_CIRCLE_SPEED = 20f;

	public UXPlane loadingCircle;

	public UXPlane ItemImagePlane;

	public Material ItemPreviewMaterial;

	private ObjectPreviewer objectPreviewer;

	private GameObject previewItemRoot;

	public void CreateNewViewItem(StreamingAssetInfo streamingAssetInfo)
	{
		loadingCircle.SetVisible(visible: true);
		ItemImagePlane.SetVisible(visible: false);
		AvatarAccessory.Create(streamingAssetInfo, OnAvatarAccessoryCreated);
	}

	private void OnAvatarAccessoryCreated(AvatarAccessory createdAvatarAccessory)
	{
		if (this == null || !gameObject.activeInHierarchy)
		{
			Object.Destroy(createdAvatarAccessory.gameObject);
			return;
		}
		previewItemRoot = new GameObject("Preview Items Root - Avatar Accessory Shop Preview");
		StartCoroutine(ItemViewRoutine(createdAvatarAccessory));
	}

	private IEnumerator ItemViewRoutine(AvatarAccessory createdAvatarAccessory)
	{
		objectPreviewer = ObjectPreviewer.Create(512, CameraClearFlags.Color, LayerFlags.Default | LayerFlags.CamRotateTarget, previewItemRoot.transform, "Preview Item", createdAvatarAccessory.gameObject);
		Material previewMaterial = new Material(ItemPreviewMaterial)
		{
			hideFlags = HideFlags.HideAndDontSave,
			mainTexture = objectPreviewer.PreviewTexture
		};
		ItemImagePlane.GetComponent<Renderer>().material = previewMaterial;
		loadingCircle.SetVisible(visible: false);
		ItemImagePlane.SetVisible(visible: true);
		yield return 0;
	}

	private void Update()
	{
		if (loadingCircle.Visible)
		{
			loadingCircle.transform.Rotate(Vector3.forward, 20f * Time.deltaTime * 57.29578f);
		}
		if (objectPreviewer != null)
		{
			objectPreviewer.UpdateRotation();
		}
	}

	private void OnDestroy()
	{
		if (objectPreviewer != null)
		{
			objectPreviewer.Destroy();
		}
		if ((bool)previewItemRoot)
		{
			Object.Destroy(previewItemRoot);
		}
	}
}
