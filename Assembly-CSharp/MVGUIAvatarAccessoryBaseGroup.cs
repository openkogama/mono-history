using System;
using System.Linq;
using UnityEngine;

public abstract class MVGUIAvatarAccessoryBaseGroup : MonoBehaviour
{
	public UXCollectionView collectionView;

	public GameObject inventoryViewItemPrefab;

	protected Transform previewItemsRoot;

	protected UXGroup Group => ((Component)this).gameObject.GetComponent<UXGroup>();

	protected MVNetworkGame Game => MVGameController.Instance.Game;

	public virtual void Initialize()
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
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		previewItemsRoot = new GameObject("Preview Root - " + ((Object)((Component)this).gameObject).name).transform;
	}

	protected abstract void InitializeCollectionView();

	protected virtual void OnShow()
	{
		if ((Object)(object)previewItemsRoot != (Object)null)
		{
			((Component)previewItemsRoot).GetComponentsInChildren<Camera>().ToList().ForEach((Camera c) =>
			{
				((Behaviour)c).enabled = true;
			});
		}
	}

	protected virtual void OnHide()
	{
		if ((Object)(object)previewItemsRoot != (Object)null)
		{
			((Component)previewItemsRoot).GetComponentsInChildren<Camera>().ToList().ForEach((Camera c) =>
			{
				((Behaviour)c).enabled = false;
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
