using System;
using MV.WorldObject;
using UnityEngine;

public class InventoryViewItem : UXCollectionViewItem
{
	public GameObject deleteButtonPrefab;

	private IUXCollectionItem item;

	private int textureSize = 128;

	private GameObject previewItemsRoot;

	private UXIconButton deleteButton;

	private static float curX;

	private float previewCamAdditionalHeight = 2.5f;

	private float previewCamDist = 2.5f;

	private float previewItemRotateSpeed = 9.3f;

	private GameObject previewCamObject;

	private Camera previewCam;

	private GameObject previewGameObject;

	public override IUXCollectionItem Item
	{
		get
		{
			return item;
		}
		set
		{
			item = value;
		}
	}

	public GameObject PreviewItemsRoot
	{
		get
		{
			return previewItemsRoot;
		}
		set
		{
			previewItemsRoot = value;
		}
	}

	private Mesh BuildMesh()
	{
		return UXUtils.BuildPlaneMesh();
	}

	public void Initialize()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Expected Obj, but got Unknown
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Expected Obj, but got Unknown
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		//IL_035e: Expected Obj, but got Unknown
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_037b: Expected Obj, but got Unknown
		//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03af: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
		MVItem mVItem = item.Object as MVItem;
		previewCamObject = new GameObject();
		previewCamObject.transform.parent = previewItemsRoot.transform;
		((Object)previewCamObject).name = $"Preview_{mVItem.name}_RenderCam";
		previewCamObject.layer = LayerMask.NameToLayer("Preview");
		RenderTexture val = new RenderTexture(textureSize, textureSize, 16);
		((Texture)val).filterMode = (FilterMode)1;
		previewCam = previewCamObject.AddComponent<Camera>();
		previewCam.clearFlags = (CameraClearFlags)2;
		previewCam.backgroundColor = new Color(0f, 0f, 0f, 0f);
		previewCam.fieldOfView = 35f;
		previewCam.aspect = 1f;
		previewCam.cullingMask = 1 << LayerMask.NameToLayer("Preview");
		previewCam.near = 0.05f;
		previewCam.far = 100f;
		previewCam.targetTexture = val;
		previewGameObject = new RuntimePrototypeCubeModel(mVItem).GetMesh();
		previewGameObject.transform.parent = previewItemsRoot.transform;
		((Object)previewGameObject).name = "Preview_" + mVItem.name + "_Item";
		HelperFunctions.SetLayerRecursively(previewGameObject.transform, "Preview");
		previewGameObject.transform.position = new Vector3(curX, 0f, 10f);
		Bounds? axisAlignedBoundsRecursively = SharedCubeFunctions.GetAxisAlignedBoundsRecursively(previewGameObject.transform);
		Bounds val2 = new Bounds(Vector3.zero, Vector3.one);
		if (axisAlignedBoundsRecursively.HasValue)
		{
			val2 = axisAlignedBoundsRecursively.Value;
		}
		else
		{
			Debug.Log((object)"Failed to find bounds!");
		}
		float num = Mathf.Max(val2.size.x, Mathf.Max(val2.size.y, val2.size.z));
		float num2 = 1f / num;
		previewGameObject.transform.localScale = new Vector3(num2, num2, num2);
		Bounds? axisAlignedBoundsRecursively2 = SharedCubeFunctions.GetAxisAlignedBoundsRecursively(previewGameObject.transform);
		Bounds val3 = new Bounds(Vector3.zero, Vector3.one);
		if (axisAlignedBoundsRecursively2.HasValue)
		{
			val3 = axisAlignedBoundsRecursively2.Value;
		}
		else
		{
			Debug.Log((object)"Failed to find bounds!");
		}
		float x = val3.center.x;
		float y = val3.center.y;
		float z = val3.center.z;
		((Component)previewCam).transform.position = new Vector3(x, y + previewCamAdditionalHeight * num2, z - previewCamDist);
		Vector3 center = val3.center;
		((Component)previewCam).transform.LookAt(center);
		curX += val2.size.x + 100f;
		Object val4 = Resources.Load("Materials/ItemPreview");
		Material val5 = new Material((Material)(object)((val4 is Material) ? val4 : null));
		((Object)val5).hideFlags = (HideFlags)13;
		val5.mainTexture = (Texture)(object)val;
		GameObject val6 = new GameObject("Image Plane");
		val6.layer = LayerMask.NameToLayer("UXElement");
		val6.transform.parent = ((Component)this).transform;
		val6.transform.localScale = Vector3.one * 6f;
		val6.transform.localPosition = Vector3.zero;
		MeshFilter val7 = val6.AddComponent<MeshFilter>();
		val7.mesh = BuildMesh();
		MeshRenderer val8 = val6.AddComponent<MeshRenderer>();
		((Renderer)val8).material = val5;
		UXMouseOverObject uXMouseOverObject = val6.AddComponent<UXMouseOverObject>();
		uXMouseOverObject.OnMouseOverEnter = (UXMouseOverObject.OnMouseOverEnterDelegate)Delegate.Combine(uXMouseOverObject.OnMouseOverEnter, (UXMouseOverObject.OnMouseOverEnterDelegate)((UXMouseOverObject moo) =>
		{
			NotifyMouseOver();
		}));
		UXMouseClickObject uXMouseClickObject = val6.AddComponent<UXMouseClickObject>();
		uXMouseClickObject.OnClick = (UXMouseClickObject.OnClickDelegate)Delegate.Combine(uXMouseClickObject.OnClick, (UXMouseClickObject.OnClickDelegate)((UXMouseClickObject clickObject, Vector3 mousePositionWorld) =>
		{
			NotifySelection();
		}));
		UXVisibility uXVisibility = val6.AddComponent<UXVisibility>();
		val6.AddComponent<UXVisibilityMeshRenderers>();
		val6.AddComponent<BoxCollider>();
		val6.AddComponent<UXVisibilityCollider>();
		InitializeDeleteButton();
		bool mouseOver = false;
		bool visible = false;
		uXMouseOverObject.OnMouseOverEnter = (UXMouseOverObject.OnMouseOverEnterDelegate)Delegate.Combine(uXMouseOverObject.OnMouseOverEnter, (UXMouseOverObject.OnMouseOverEnterDelegate)((UXMouseOverObject obj) =>
		{
			mouseOver = true;
			((Component)deleteButton).renderer.enabled = mouseOver && visible;
		}));
		uXMouseOverObject.OnMouseOverExit = (UXMouseOverObject.OnMouseOverExitDelegate)Delegate.Combine(uXMouseOverObject.OnMouseOverExit, (UXMouseOverObject.OnMouseOverExitDelegate)((UXMouseOverObject obj) =>
		{
			mouseOver = false;
			((Component)deleteButton).renderer.enabled = mouseOver && visible;
		}));
		uXVisibility.OnVisibilityChange = (UXVisibility.VisibilityChangeDelegate)Delegate.Combine(uXVisibility.OnVisibilityChange, (UXVisibility.VisibilityChangeDelegate)((float v) =>
		{
			visible = v > 0f;
			((Component)deleteButton).renderer.enabled = mouseOver && visible;
		}));
	}

	private void InitializeDeleteButton()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		Object val = Object.Instantiate((Object)(object)deleteButtonPrefab);
		deleteButton = ((GameObject)((val is GameObject) ? val : null)).GetComponent<UXIconButton>();
		((Component)deleteButton).transform.localScale = Vector3.one;
		((Component)deleteButton).transform.parent = ((Component)this).transform;
		((Component)deleteButton).transform.localPosition = new Vector3(1.3f, 1.3f, -2f);
		((Component)deleteButton).transform.localScale = Vector3.one;
		UXIconButton uXIconButton = deleteButton;
		uXIconButton.OnClick = (UXIconButton.OnClickDelegate)Delegate.Combine(uXIconButton.OnClick, (UXIconButton.OnClickDelegate)(() =>
		{
			NotifyRemove();
		}));
	}

	private void NotifySelection()
	{
		if (OnSelection != null)
		{
			OnSelection(this);
		}
	}

	private void NotifyRemove()
	{
		if (OnRemove != null)
		{
			OnRemove(this);
		}
	}

	private void NotifyMouseOver()
	{
		if (OnMouseOver != null)
		{
			OnMouseOver(this);
		}
	}

	public void Update()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)previewGameObject != (Object)null)
		{
			previewGameObject.transform.RotateAround(SharedCubeFunctions.GetWorldCenter(previewGameObject), Vector3.up, previewItemRotateSpeed * Time.deltaTime);
		}
	}

	public void OnDestroy()
	{
		Object.Destroy((Object)(object)previewCamObject);
		Object.Destroy((Object)(object)previewGameObject);
	}
}
