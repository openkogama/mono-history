using System.Collections;
using MV.WorldObject;
using UnityEngine;

public class MVGUIItemShopPreview : MonoBehaviour
{
	public UXPlane ItemImagePlane;

	private ObjectPreviewer objectPreviewer;

	private Transform itemPreviewRoot;

	private MVWorldObjectClient WO;

	public void BuildItemShopPreview(MVItem item, float width = 12f, float height = 12f)
	{
		ItemImagePlane.SetSize(width, height);
		((MonoBehaviour)this).StartCoroutine(ItemViewRoutine(item, 512, 512));
	}

	private IEnumerator ItemViewRoutine(MVItem item, int previewWidth, int previewHeight)
	{
		KoGaMaPackageClient koGaMaPackageClient = ARepository.GetKoGaMaPackageFromItem(item);
		WO = koGaMaPackageClient.worldObjects[koGaMaPackageClient.worldObjectRoot];
		itemPreviewRoot = new GameObject("Item preview root - " + ((Object)((Component)this).gameObject).name).transform;
		objectPreviewer = ObjectPreviewer.Create(previewWidth, previewHeight, (CameraClearFlags)2, WO.PreviewLayerMask, Vector3.zero, itemPreviewRoot, new Vector3(0f, 0f, 10f), item.name, WO, WO.GameObject);
		((Component)ItemImagePlane).renderer.material = new Material(((Component)ItemImagePlane).renderer.material);
		((Object)((Component)ItemImagePlane).renderer.material).hideFlags = (HideFlags)13;
		((Component)ItemImagePlane).renderer.material.mainTexture = (Texture)(object)objectPreviewer.PreviewTexture;
		yield return null;
	}

	private void Update()
	{
		if ((Object)(object)objectPreviewer != (Object)null)
		{
			objectPreviewer.UpdateRotation();
		}
	}

	public void OnDestroy()
	{
		if (WO != null)
		{
			WO.Destroy();
		}
		if ((Object)(object)objectPreviewer != (Object)null)
		{
			Object.Destroy((Object)(object)((Component)objectPreviewer).gameObject);
		}
		if ((Object)(object)itemPreviewRoot != (Object)null)
		{
			Object.Destroy((Object)(object)((Component)itemPreviewRoot).gameObject);
		}
	}
}
