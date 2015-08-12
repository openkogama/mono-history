using System.Collections;
using MV.WorldObject;
using UnityEngine;

public class AdViewItem : MonoBehaviour
{
	public float Width;

	public float Height;

	public Material ItemPreviewMaterial;

	[HideInInspector]
	public UXPlane ItemImagePlane;

	[HideInInspector]
	public ObjectPreviewer objectPreviewer;

	[HideInInspector]
	public MVWorldObjectClient WO;

	public void BuildViewItem(MVItem mvItem, Transform previewItemsRoot)
	{
		BuildImagePlane();
		StartCoroutine(ItemViewRoutine(mvItem, 512, 512, previewItemsRoot));
	}

	private void BuildImagePlane()
	{
		GameObject gameObject = new GameObject("Image Plane");
		gameObject.layer = LayerMask.NameToLayer("UXElement");
		gameObject.transform.parent = transform;
		gameObject.transform.localScale = Vector3.one;
		gameObject.transform.localPosition = new Vector3(0f, 0f, -0.01f);
		ItemImagePlane = gameObject.AddComponent<UXPlane>();
		ItemImagePlane.SetSize(Width, Height);
	}

	private IEnumerator ItemViewRoutine(MVItem item, int previewWidth, int previewHeight, Transform previewItemsRoot)
	{
		KoGaMaPackageClient koGaMaPackageClient = ARepository.GetKoGaMaPackageFromItem(item);
		WO = koGaMaPackageClient.worldObjects[koGaMaPackageClient.worldObjectRoot];
		objectPreviewer = ObjectPreviewer.Create(previewWidth, previewHeight, CameraClearFlags.Color, WO.PreviewLayerMask, Vector3.zero, previewItemsRoot, new Vector3(0f, 0f, 10f), item.name, WO, WO.GameObject);
		Material previewMaterial = new Material(ItemPreviewMaterial)
		{
			hideFlags = HideFlags.HideAndDontSave,
			mainTexture = objectPreviewer.PreviewTexture
		};
		MeshRenderer meshRenderer = ItemImagePlane.gameObject.AddComponent<MeshRenderer>();
		meshRenderer.material = previewMaterial;
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
			objectPreviewer.Destroy();
		}
	}
}
