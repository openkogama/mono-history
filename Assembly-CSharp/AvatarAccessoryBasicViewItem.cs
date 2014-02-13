using UnityEngine;

public abstract class AvatarAccessoryBasicViewItem : MVGUIBasicViewItem
{
	protected StreamingAssetInfo streamingAssetInfo;

	protected ObjectPreviewer objectPreviewer;

	protected bool _isBuilding;

	public Transform PreviewItemsRoot { get; set; }

	public AvatarAccessory AvatarAccessory { get; private set; }

	public override void Initialize()
	{
		if (!_isBuilding)
		{
			_isBuilding = true;
			GetStreamingAssetInfo();
			BuildImagePlane();
			BuildViewItem();
		}
	}

	protected virtual void GetStreamingAssetInfo()
	{
		streamingAssetInfo = (StreamingAssetInfo)Item.Object;
	}

	protected virtual void BuildViewItem()
	{
		AddTooltip(streamingAssetInfo.Name);
		_loading = true;
		LoadingCircle.SetVisible(Visible);
		AvatarAccessory.Create(streamingAssetInfo, OnAvatarAccessoryCreated);
	}

	protected void OnAvatarAccessoryCreated(AvatarAccessory createdAvatarAccessory)
	{
		if ((Object)(object)createdAvatarAccessory != (Object)null)
		{
			ItemViewRoutine(createdAvatarAccessory);
		}
	}

	private void ItemViewRoutine(AvatarAccessory createdAvatarAccessory)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Expected Obj, but got Unknown
		AvatarAccessory = createdAvatarAccessory;
		objectPreviewer = ObjectPreviewer.Create(256, (CameraClearFlags)2, LayerFlags.Default | LayerFlags.CamRotateTarget, PreviewItemsRoot, streamingAssetInfo.Name, ((Component)createdAvatarAccessory).gameObject);
		Material val = new Material(ItemPreviewMaterial);
		((Object)val).hideFlags = (HideFlags)13;
		val.mainTexture = (Texture)(object)objectPreviewer.PreviewTexture;
		MeshRenderer val2 = ((Component)ItemImagePlane).gameObject.AddComponent<MeshRenderer>();
		((Renderer)val2).material = val;
		OnAvatarAccessoryViewItemBuilt();
	}

	private void OnAvatarAccessoryViewItemBuilt()
	{
		_loading = false;
		LoadingCircle.SetVisible(visible: false);
		IsInitialized = true;
		_isBuilding = false;
		if (OnViewItemBuilt != null)
		{
			OnViewItemBuilt(this);
		}
	}

	public override void Update()
	{
		base.Update();
		if (IsInitialized)
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
		if ((Object)(object)AvatarAccessory != (Object)null)
		{
			Object.Destroy((Object)(object)((Component)AvatarAccessory).gameObject);
		}
	}
}
