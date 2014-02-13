using UnityEngine;

public class ObjectPreviewer : MonoBehaviour
{
	private float previewObjMaxSize = 2f;

	private float previewCamAdditionalHeight = 1.2f;

	private float previewCamDist = 2.2f;

	private float previewItemRotateSpeed = 9.3f;

	private Vector3 previewCamOffset = Vector3.zero;

	private RenderTexture previewTexture;

	private GameObject previewGameObject;

	public LayerFlags layersToRender;

	private Camera previewCam;

	private Vector3 pivotPoint;

	private static int previewerIndex = 1;

	public RenderTexture PreviewTexture => previewTexture;

	private ObjectPreviewer()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
	}

	public static ObjectPreviewer Create(int textureSize, CameraClearFlags clearFlags, LayerFlags layersToRender, Transform previewItemsRoot, string name, GameObject woGameObjectCopy)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		Vector3 previewPosition = new Vector3((float)(10 * previewerIndex++), 300f, 0f);
		return Create(textureSize, textureSize, clearFlags, layersToRender, Vector3.zero, previewItemsRoot, previewPosition, name, null, woGameObjectCopy);
	}

	public static ObjectPreviewer Create(int textureSize, CameraClearFlags clearFlags, LayerFlags layersToRender, Vector3 cameraOffset, Transform previewItemsRoot, Vector3 previewPosition, string name, MVWorldObjectClient wo, GameObject woGameObjectCopy)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return Create(textureSize, textureSize, clearFlags, layersToRender, cameraOffset, previewItemsRoot, previewPosition, name, wo, woGameObjectCopy);
	}

	public static ObjectPreviewer Create(int textureWidth, int textureHeight, CameraClearFlags clearFlags, LayerFlags layersToRender, Vector3 cameraOffset, Transform previewItemsRoot, Vector3 previewPosition, string name, MVWorldObjectClient wo, GameObject woGameObjectCopy)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected Obj, but got Unknown
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Expected Obj, but got Unknown
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_0347: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_03af: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03da: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0403: Unknown result type (might be due to invalid IL or missing references)
		//IL_0409: Unknown result type (might be due to invalid IL or missing references)
		//IL_040d: Unknown result type (might be due to invalid IL or missing references)
		//IL_041d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0422: Unknown result type (might be due to invalid IL or missing references)
		//IL_0427: Unknown result type (might be due to invalid IL or missing references)
		//IL_0438: Unknown result type (might be due to invalid IL or missing references)
		//IL_043d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0458: Unknown result type (might be due to invalid IL or missing references)
		//IL_046f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0475: Unknown result type (might be due to invalid IL or missing references)
		//IL_047a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0485: Unknown result type (might be due to invalid IL or missing references)
		//IL_048a: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a0: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = new GameObject();
		ObjectPreviewer objectPreviewer = val.AddComponent<ObjectPreviewer>();
		objectPreviewer.layersToRender = layersToRender | LayerFlags.Hidden;
		objectPreviewer.previewCamOffset = cameraOffset;
		val.transform.parent = previewItemsRoot;
		((Object)val).name = $"Preview_{name}_RenderCam";
		val.layer = LayerMask.NameToLayer("Preview");
		objectPreviewer.previewTexture = new RenderTexture(textureWidth, textureHeight, 16);
		((Texture)objectPreviewer.previewTexture).filterMode = (FilterMode)1;
		((Object)objectPreviewer.previewTexture).hideFlags = (HideFlags)4;
		objectPreviewer.previewCam = val.AddComponent<Camera>();
		objectPreviewer.previewCam.clearFlags = clearFlags;
		objectPreviewer.previewCam.backgroundColor = new Color(0f, 0f, 0f, 0f);
		objectPreviewer.previewCam.fieldOfView = 35f;
		objectPreviewer.previewCam.aspect = (float)textureWidth / (float)textureHeight;
		objectPreviewer.previewCam.cullingMask = 1 << LayerMask.NameToLayer("Preview");
		objectPreviewer.previewCam.near = 0.05f;
		objectPreviewer.previewCam.far = 100f;
		objectPreviewer.previewCam.targetTexture = objectPreviewer.previewTexture;
		objectPreviewer.previewGameObject = woGameObjectCopy;
		if (wo != null)
		{
			((Object)objectPreviewer.previewGameObject).name = "Preview_" + name + "_Item_" + wo.ItemId + "_woID_" + wo.Id;
		}
		else
		{
			((Object)objectPreviewer.previewGameObject).name = "Preview_" + name;
		}
		objectPreviewer.previewGameObject.transform.parent = previewItemsRoot;
		objectPreviewer.previewGameObject.transform.localRotation = Quaternion.identity;
		objectPreviewer.previewGameObject.transform.position = previewPosition;
		Renderer[] componentsInChildren = objectPreviewer.previewGameObject.GetComponentsInChildren<Renderer>();
		Renderer[] array = componentsInChildren;
		foreach (Renderer val2 in array)
		{
			if ((Object)(object)((Component)val2).GetComponent<TriggerBoxEvents>() != (Object)null)
			{
				val2.enabled = false;
			}
			else
			{
				val2.enabled = true;
			}
		}
		SelectionBox[] componentsInChildren2 = objectPreviewer.previewGameObject.GetComponentsInChildren<SelectionBox>();
		SelectionBox[] array2 = componentsInChildren2;
		foreach (SelectionBox selectionBox in array2)
		{
			((Component)selectionBox).renderer.enabled = false;
		}
		Bounds val3;
		if (wo != null)
		{
			val3 = wo.GetLocalBounds(BoundsContext.Preview);
		}
		else
		{
			AvatarAccessory component = objectPreviewer.previewGameObject.GetComponent<AvatarAccessory>();
			val3 = ((!((Object)(object)component != (Object)null)) ? ComputeLocalBounds(objectPreviewer.previewGameObject) : component.GetLocalBounds());
		}
		Vector3 localScale = objectPreviewer.previewGameObject.transform.localScale;
		float num = Mathf.Max(new float[3]
		{
			val3.size.x,
			val3.size.y,
			val3.size.z
		});
		num = Mathf.Max(new float[3]
		{
			val3.size.x * localScale.x,
			val3.size.y * localScale.y,
			val3.size.z * localScale.z
		});
		float num2 = objectPreviewer.previewObjMaxSize / num;
		if (wo is MVMovingPlatformGroup)
		{
			LineRenderer componentInChildren = objectPreviewer.previewGameObject.GetComponentInChildren<LineRenderer>();
			componentInChildren.SetWidth(0.3f * num2, 0.3f * num2);
		}
		objectPreviewer.previewGameObject.transform.localScale = localScale * num2;
		Vector3 val4 = new Vector3(val3.center.x * localScale.x, val3.center.y * localScale.y, val3.center.z * localScale.z);
		objectPreviewer.pivotPoint = val4 * num2 + objectPreviewer.previewGameObject.transform.position;
		objectPreviewer.previewGameObject.transform.RotateAround(objectPreviewer.pivotPoint, Vector3.up, 180f);
		((Component)objectPreviewer.previewCam).transform.position = objectPreviewer.pivotPoint + (new Vector3(0f, objectPreviewer.previewCamAdditionalHeight, 0f - objectPreviewer.previewCamDist) + objectPreviewer.previewCamOffset) * objectPreviewer.previewObjMaxSize;
		((Component)objectPreviewer.previewCam).transform.LookAt(objectPreviewer.pivotPoint);
		return objectPreviewer;
	}

	private static Bounds ComputeLocalBounds(GameObject go)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
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
			Debug.LogWarning((object)"Renderers required for correct bounds");
		}
		return result;
	}

	private void OnPreCull()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		LayerUtil.SetLayerRecursively(previewGameObject.transform, LayerMask.op_Implicit((int)layersToRender), LayerMask.NameToLayer("Preview"));
	}

	private void OnPostRender()
	{
		LayerUtil.SetLayerRecursively(previewGameObject.transform, "Preview", "Hidden");
	}

	public void UpdateRotation(float rotateSpeed = 0f)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)previewGameObject != (Object)null)
		{
			previewGameObject.transform.RotateAround(pivotPoint, Vector3.up, ((!(rotateSpeed > 0f)) ? previewItemRotateSpeed : rotateSpeed) * Time.deltaTime);
		}
	}

	public void Destroy()
	{
		if ((Object)(object)previewCam != (Object)null)
		{
			previewCam.targetTexture = null;
		}
		if ((Object)(object)this != (Object)null)
		{
			Object.Destroy((Object)(object)((Component)this).gameObject);
		}
		if ((Object)(object)previewTexture != (Object)null)
		{
			Object.Destroy((Object)(object)previewTexture);
		}
	}
}
