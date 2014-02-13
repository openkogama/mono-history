using System;
using System.Collections;
using Localize;
using MV.WorldObject;
using UnityEngine;

public class InventoryViewItem : MVGUIBasicViewItem
{
	public UXIconButton InfoButton;

	protected bool _isBuilding;

	public MVWorldObjectClient WO { get; private set; }

	public ObjectPreviewer ObjectPreviewer { get; protected set; }

	public Transform PreviewItemsRoot { get; set; }

	public override void SetVisible(bool visible)
	{
		base.SetVisible(visible);
		if ((Object)(object)InfoButton != (Object)null)
		{
			InfoButton.SetVisible(visible: false);
		}
	}

	public override void Initialize()
	{
		if (!_isBuilding)
		{
			_isBuilding = true;
			((Component)this).gameObject.active = true;
			BuildImagePlane();
			MVItem item = Item.Object as MVItem;
			if ((Object)(object)InfoButton != (Object)null)
			{
				UXIconButton infoButton = InfoButton;
				infoButton.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(infoButton.OnClick, new UXBaseButton.OnClickDelegate(OnOpenItemActionDialog));
			}
			BuildViewItem(item, 128, 128);
		}
	}

	protected void BuildViewItem(MVItem item, int previewWidth, int previewHeight)
	{
		_loading = true;
		LoadingCircle.SetVisible(Visible);
		((MonoBehaviour)this).StartCoroutine(ItemViewRoutine(item, previewWidth, previewHeight));
	}

	private IEnumerator ItemViewRoutine(MVItem item, int previewWidth, int previewHeight)
	{
		KoGaMaPackageClient koGaMaPackageClient = ARepository.GetKoGaMaPackageFromItem(item);
		WO = koGaMaPackageClient.worldObjects[koGaMaPackageClient.worldObjectRoot];
		ObjectPreviewer = ObjectPreviewer.Create(previewWidth, previewHeight, (CameraClearFlags)2, WO.PreviewLayerMask, Vector3.zero, PreviewItemsRoot, new Vector3(100f, 100f, 10f * (float)Item.Index), item.name, WO, WO.GameObject);
		Material previewMaterial = new Material(ItemPreviewMaterial);
		((Object)previewMaterial).hideFlags = (HideFlags)13;
		previewMaterial.mainTexture = (Texture)(object)ObjectPreviewer.PreviewTexture;
		MeshRenderer meshRenderer = ((Component)ItemImagePlane).gameObject.AddComponent<MeshRenderer>();
		((Renderer)meshRenderer).material = previewMaterial;
		OnInventoryViewItemBuilt();
		yield return 0;
	}

	protected virtual void OnInventoryViewItemBuilt()
	{
		_loading = false;
		LoadingCircle.SetVisible(visible: false);
		AddListeners();
		IsInitialized = true;
		_isBuilding = false;
	}

	protected virtual void AddListeners()
	{
		MVItem mVItem = Item.Object as MVItem;
		if (mVItem.itemCategoryID == 1 || mVItem.itemCategoryID == 5 || mVItem.itemCategoryID == 8)
		{
			AddTooltip(mVItem.name);
		}
		if (mVItem.itemCategoryID == 6 || mVItem.itemCategoryID == 7 || mVItem.itemCategoryID == 10)
		{
			AddTooltip(ToolTipText.Instance.GetToolTipTextFromItemName(mVItem.name));
		}
	}

	private void OnOpenItemActionDialog()
	{
		MVGameController.Instance.EditController.HideCurrentWindow();
		UXDialogFactory uXDialogFactory = UXUtils.FindGUIObjectOfType<UXDialogFactory>();
		uXDialogFactory.CreateCustomDialog("Prefabs/GUI/Item Action/ItemActionDialog", TextSlotIndex.Empty, noButtons: true).SetOnResultCallback(OnCloseItemActionDialog);
		(uXDialogFactory.CurrentlyBuildingDialogBox as MVGUIItemActionDialog).SetMVItem(Item.Object as MVItem);
		uXDialogFactory.Show();
	}

	private void OnCloseItemActionDialog(UXDialogBox dialogBox)
	{
		MVGameController.Instance.EditController.ShowInventory();
	}

	public override void OnMouseSlotOver(int slotIndex, int visiblePage)
	{
		base.OnMouseSlotOver(slotIndex, visiblePage);
		if (PageIndex == visiblePage && (Object)(object)InfoButton != (Object)null)
		{
			InfoButton.SetVisible(visible: true);
		}
	}

	public override void OnMouseSlotOverExit(int slotIndex, int visiblePage)
	{
		base.OnMouseSlotOverExit(slotIndex, visiblePage);
		if ((Object)(object)InfoButton != (Object)null)
		{
			InfoButton.SetVisible(visible: false);
		}
	}

	public override void Update()
	{
		base.Update();
		if (IsInitialized)
		{
			ObjectPreviewer.UpdateRotation();
		}
	}

	public virtual void OnDestroy()
	{
		if (WO != null)
		{
			WO.Destroy();
		}
		if ((Object)(object)ObjectPreviewer != (Object)null)
		{
			ObjectPreviewer.Destroy();
		}
	}
}
