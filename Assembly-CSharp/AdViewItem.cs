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
		((MonoBehaviour)this).StartCoroutine(ItemViewRoutine(mvItem, 512, 512, previewItemsRoot));
	}

	private void BuildImagePlane()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected Obj, but got Unknown
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = new GameObject("Image Plane");
		val.layer = LayerMask.NameToLayer("UXElement");
		val.transform.parent = ((Component)this).transform;
		val.transform.localScale = Vector3.one;
		val.transform.localPosition = new Vector3(0f, 0f, -0.01f);
		ItemImagePlane = val.AddComponent<UXPlane>();
		ItemImagePlane.SetSize(Width, Height);
	}

	private IEnumerator ItemViewRoutine(MVItem item, int previewWidth, int previewHeight, Transform previewItemsRoot)
	{
		KoGaMaPackageClient koGaMaPackageClient = ARepository.GetKoGaMaPackageFromItem(item);
		WO = koGaMaPackageClient.worldObjects[koGaMaPackageClient.worldObjectRoot];
		objectPreviewer = ObjectPreviewer.Create(previewWidth, previewHeight, (CameraClearFlags)2, WO.PreviewLayerMask, Vector3.zero, previewItemsRoot, new Vector3(0f, 0f, 10f), item.name, WO, WO.GameObject);
		Material previewMaterial = new Material(ItemPreviewMaterial);
		((Object)previewMaterial).hideFlags = (HideFlags)13;
		previewMaterial.mainTexture = (Texture)(object)objectPreviewer.PreviewTexture;
		MeshRenderer meshRenderer = ((Component)ItemImagePlane).gameObject.AddComponent<MeshRenderer>();
		((Renderer)meshRenderer).material = previewMaterial;
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
			objectPreviewer.Destroy();
		}
	}
}
