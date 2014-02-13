using UnityEngine;

public class AvatarAccessoryPreviewer : MonoBehaviour
{
	private float previewCamAdditionalHeight = 1.2f;

	private float previewCamDist = 2.2f;

	private float previewItemRotateSpeed = 9.3f;

	private GameObject previewGameObject;

	private Camera previewCam;

	private Vector3 pivotPoint;

	public RenderTexture PreviewTexture { get; private set; }

	private AvatarAccessoryPreviewer()
	{
	}

	public static AvatarAccessoryPreviewer Create(int textureSize, CameraClearFlags clearFlags, Transform previewItemsRoot, string name, GameObject woGameObjectCopy)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return Create(textureSize, textureSize, clearFlags, previewItemsRoot, name, woGameObjectCopy);
	}

	public static AvatarAccessoryPreviewer Create(int textureWidth, int textureHeight, CameraClearFlags clearFlags, Transform previewItemsRoot, string name, GameObject woGameObjectCopy)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected Obj, but got Unknown
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Expected Obj, but got Unknown
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = new GameObject();
		AvatarAccessoryPreviewer avatarAccessoryPreviewer = val.AddComponent<AvatarAccessoryPreviewer>();
		val.transform.parent = previewItemsRoot;
		((Object)val).name = $"Preview_{name}_RenderCam";
		val.layer = LayerMask.NameToLayer("Preview");
		avatarAccessoryPreviewer.PreviewTexture = new RenderTexture(textureWidth, textureHeight, 16);
		((Texture)avatarAccessoryPreviewer.PreviewTexture).filterMode = (FilterMode)1;
		((Object)avatarAccessoryPreviewer.PreviewTexture).hideFlags = (HideFlags)4;
		avatarAccessoryPreviewer.previewCam = val.AddComponent<Camera>();
		avatarAccessoryPreviewer.previewCam.clearFlags = clearFlags;
		avatarAccessoryPreviewer.previewCam.backgroundColor = new Color(0f, 0f, 0f, 0f);
		avatarAccessoryPreviewer.previewCam.fieldOfView = 35f;
		avatarAccessoryPreviewer.previewCam.aspect = (float)textureWidth / (float)textureHeight;
		avatarAccessoryPreviewer.previewCam.cullingMask = 1 << LayerMask.NameToLayer("Preview");
		avatarAccessoryPreviewer.previewCam.near = 0.05f;
		avatarAccessoryPreviewer.previewCam.far = 100f;
		avatarAccessoryPreviewer.previewCam.targetTexture = avatarAccessoryPreviewer.PreviewTexture;
		avatarAccessoryPreviewer.previewGameObject = woGameObjectCopy;
		((Object)avatarAccessoryPreviewer.previewGameObject).name = "Preview_" + name + "_AvatarAccessory";
		avatarAccessoryPreviewer.previewGameObject.transform.parent = previewItemsRoot;
		avatarAccessoryPreviewer.previewGameObject.transform.localRotation = Quaternion.identity;
		avatarAccessoryPreviewer.previewGameObject.transform.position = Vector3.zero;
		MeshRenderer[] componentsInChildren = avatarAccessoryPreviewer.previewGameObject.GetComponentsInChildren<MeshRenderer>();
		MeshRenderer[] array = componentsInChildren;
		foreach (MeshRenderer val2 in array)
		{
			((Renderer)val2).enabled = true;
		}
		Bounds localBounds = woGameObjectCopy.GetComponent<AvatarAccessory>().GetLocalBounds();
		float num = Mathf.Max(new float[3]
		{
			localBounds.size.x,
			localBounds.size.y,
			localBounds.size.z
		});
		float num2 = 1f / num;
		avatarAccessoryPreviewer.previewGameObject.transform.localScale = new Vector3(num2, num2, num2);
		avatarAccessoryPreviewer.pivotPoint = localBounds.center * num2 + avatarAccessoryPreviewer.previewGameObject.transform.position;
		avatarAccessoryPreviewer.previewGameObject.transform.RotateAround(avatarAccessoryPreviewer.pivotPoint, Vector3.up, 180f);
		((Component)avatarAccessoryPreviewer.previewCam).transform.position = avatarAccessoryPreviewer.pivotPoint + new Vector3(0f, avatarAccessoryPreviewer.previewCamAdditionalHeight, 0f - avatarAccessoryPreviewer.previewCamDist);
		((Component)avatarAccessoryPreviewer.previewCam).transform.LookAt(avatarAccessoryPreviewer.pivotPoint);
		return avatarAccessoryPreviewer;
	}

	private void OnPreCull()
	{
		LayerUtil.SetLayerRecursively(previewGameObject.transform, "Preview");
	}

	private void OnPostRender()
	{
		LayerUtil.SetLayerRecursively(previewGameObject.transform, "Hidden");
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
		if ((Object)(object)PreviewTexture != (Object)null)
		{
			Object.Destroy((Object)(object)PreviewTexture);
		}
	}
}
