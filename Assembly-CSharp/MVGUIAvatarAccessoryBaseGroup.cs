using System;
using System.Linq;
using UnityEngine;

public abstract class MVGUIAvatarAccessoryBaseGroup : MonoBehaviour
{
	public UXCollectionView collectionView;

	public GameObject inventoryViewItemPrefab;

	protected Transform previewItemsRoot;

	protected UXGroup Group => gameObject.GetComponent<UXGroup>();

	protected MVNetworkGame Game => MVGameController.Game;

	public virtual void Initialize(AvatarAccessoryController avatarAccessoryController)
	{
		CreatePreviewItemRoot();
		InitializeCollectionView();
		UXGroup uXGroup = Group;
		uXGroup.OnShowGroup = (UXGroup.OnGroupEventDelegate)Delegate.Combine(uXGroup.OnShowGroup, new UXGroup.OnGroupEventDelegate(OnShow));
		UXGroup uXGroup2 = Group;
		uXGroup2.OnHideGroup = (UXGroup.OnGroupEventDelegate)Delegate.Combine(uXGroup2.OnHideGroup, new UXGroup.OnGroupEventDelegate(OnHide));
	}

	private void CreatePreviewItemRoot()
	{
		previewItemsRoot = new GameObject("Preview Root - " + gameObject.name).transform;
	}

	protected abstract void InitializeCollectionView();

	protected virtual void OnShow()
	{
		if (previewItemsRoot != null)
		{
			previewItemsRoot.GetComponentsInChildren<Camera>().ToList().ForEach((Camera c) =>
			{
				c.enabled = true;
			});
		}
	}

	protected virtual void OnHide()
	{
		if (previewItemsRoot != null)
		{
			previewItemsRoot.GetComponentsInChildren<Camera>().ToList().ForEach((Camera c) =>
			{
				c.enabled = false;
			});
		}
	}

	public virtual void ResetInventoryGroup()
	{
		previewItemsRoot = null;
		collectionView.ResetViewItems();
	}

	public virtual void InitializeAfterReset()
	{
		CreatePreviewItemRoot();
	}

	protected abstract UXCollectionViewItem InstansiateViewItem(IUXCollectionItem item);
}
