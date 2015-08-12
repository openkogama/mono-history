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
		if (createdAvatarAccessory != null)
		{
			ItemViewRoutine(createdAvatarAccessory);
		}
	}

	private void ItemViewRoutine(AvatarAccessory createdAvatarAccessory)
	{
		AvatarAccessory = createdAvatarAccessory;
		objectPreviewer = ObjectPreviewer.Create(256, CameraClearFlags.Color, LayerFlags.Default | LayerFlags.CamRotateTarget, PreviewItemsRoot, streamingAssetInfo.Name, createdAvatarAccessory.gameObject);
		Material material = new Material(ItemPreviewMaterial);
		material.hideFlags = HideFlags.HideAndDontSave;
		material.mainTexture = objectPreviewer.PreviewTexture;
		MeshRenderer meshRenderer = ItemImagePlane.gameObject.AddComponent<MeshRenderer>();
		meshRenderer.material = material;
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
		if (objectPreviewer != null)
		{
			objectPreviewer.Destroy();
		}
		if (AvatarAccessory != null)
		{
			Object.Destroy(AvatarAccessory.gameObject);
		}
	}
}
