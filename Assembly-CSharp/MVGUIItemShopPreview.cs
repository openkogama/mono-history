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
		StartCoroutine(ItemViewRoutine(item, 512, 512));
	}

	private IEnumerator ItemViewRoutine(MVItem item, int previewWidth, int previewHeight)
	{
		KoGaMaPackageClient koGaMaPackageClient = ARepository.GetKoGaMaPackageFromItem(item);
		WO = koGaMaPackageClient.worldObjects[koGaMaPackageClient.worldObjectRoot];
		itemPreviewRoot = new GameObject("Item preview root - " + gameObject.name).transform;
		objectPreviewer = ObjectPreviewer.Create(previewWidth, previewHeight, CameraClearFlags.Color, WO.PreviewLayerMask, Vector3.zero, itemPreviewRoot, new Vector3(0f, 0f, 10f), item.name, WO, WO.GameObject);
		ItemImagePlane.GetComponent<Renderer>().material = new Material(ItemImagePlane.GetComponent<Renderer>().material);
		ItemImagePlane.GetComponent<Renderer>().material.hideFlags = HideFlags.HideAndDontSave;
		ItemImagePlane.GetComponent<Renderer>().material.mainTexture = objectPreviewer.PreviewTexture;
		yield return null;
	}

	private void Update()
	{
		if (objectPreviewer != null)
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
		if (objectPreviewer != null)
		{
			Object.Destroy(objectPreviewer.gameObject);
		}
		if (itemPreviewRoot != null)
		{
			Object.Destroy(itemPreviewRoot.gameObject);
		}
	}
}
