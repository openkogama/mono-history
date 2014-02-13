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
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected Obj, but got Unknown
		if ((Object)(object)this == (Object)null || !((Component)this).gameObject.active)
		{
			Object.Destroy((Object)(object)((Component)createdAvatarAccessory).gameObject);
			return;
		}
		previewItemRoot = new GameObject("Preview Items Root - Avatar Accessory Shop Preview");
		((MonoBehaviour)this).StartCoroutine(ItemViewRoutine(createdAvatarAccessory));
	}

	private IEnumerator ItemViewRoutine(AvatarAccessory createdAvatarAccessory)
	{
		objectPreviewer = ObjectPreviewer.Create(512, (CameraClearFlags)2, LayerFlags.Default | LayerFlags.CamRotateTarget, previewItemRoot.transform, "Preview Item", ((Component)createdAvatarAccessory).gameObject);
		Material previewMaterial = new Material(ItemPreviewMaterial);
		((Object)previewMaterial).hideFlags = (HideFlags)13;
		previewMaterial.mainTexture = (Texture)(object)objectPreviewer.PreviewTexture;
		((Component)ItemImagePlane).renderer.material = previewMaterial;
		loadingCircle.SetVisible(visible: false);
		ItemImagePlane.SetVisible(visible: true);
		yield return 0;
	}

	private void Update()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (loadingCircle.Visible)
		{
			((Component)loadingCircle).transform.RotateAroundLocal(Vector3.forward, 20f * Time.deltaTime);
		}
		if ((Object)(object)objectPreviewer != (Object)null)
		{
			objectPreviewer.UpdateRotation();
		}
	}

	private void OnDestroy()
	{
		if ((Object)(object)objectPreviewer != (Object)null)
		{
			objectPreviewer.Destroy();
		}
		if (Object.op_Implicit((Object)(object)previewItemRoot))
		{
			Object.Destroy((Object)(object)previewItemRoot);
		}
	}
}
