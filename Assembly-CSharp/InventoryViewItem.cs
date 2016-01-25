using System;
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
		if (InfoButton != null)
		{
			InfoButton.SetVisible(visible: false);
		}
	}

	public override void Initialize()
	{
		if (!_isBuilding)
		{
			_isBuilding = true;
			gameObject.SetActive(value: true);
			BuildImagePlane();
			MVItem item = Item.Object as MVItem;
			if (InfoButton != null)
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
		ItemViewRoutine(item, previewWidth, previewHeight);
	}

	private void ItemViewRoutine(MVItem item, int previewWidth, int previewHeight)
	{
		KoGaMaPackageClient koGaMaPackageFromItem = ARepository.GetKoGaMaPackageFromItem(item);
		WO = koGaMaPackageFromItem.worldObjects[koGaMaPackageFromItem.worldObjectRoot];
		ObjectPreviewer = ObjectPreviewer.Create(previewWidth, previewHeight, CameraClearFlags.Color, WO.PreviewLayerMask, Vector3.zero, PreviewItemsRoot, new Vector3(100f, 100f, 10f * (float)Item.Index), item.name, WO, WO.GameObject);
		Material material = new Material(ItemPreviewMaterial);
		material.hideFlags = HideFlags.HideAndDontSave;
		material.mainTexture = ObjectPreviewer.PreviewTexture;
		MeshRenderer meshRenderer = ItemImagePlane.gameObject.GetComponent<MeshRenderer>();
		if (meshRenderer == null)
		{
			meshRenderer = ItemImagePlane.gameObject.AddComponent<MeshRenderer>();
		}
		meshRenderer.material = material;
		OnInventoryViewItemBuilt();
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
			AddTooltip(ItemNameToLocalizedString.GetToolTipTextFromItemName(mVItem.name));
		}
	}

	private void OnOpenItemActionDialog()
	{
		MVGameControllerLegacyUI.IngameController.HideCurrentWindow();
		UXDialogFactory uXDialogFactory = UXUtils.UXDialogFactory;
		uXDialogFactory.CreateCustomDialog("Prefabs/GUI/Item Action/ItemActionDialog", string.Empty, noButtons: true).SetOnResultCallback(OnCloseItemActionDialog);
		(uXDialogFactory.CurrentlyBuildingDialogBox as MVGUIItemActionDialog).SetMVItem(Item.Object as MVItem);
		uXDialogFactory.Show();
	}

	private void OnCloseItemActionDialog(UXDialogBox dialogBox)
	{
		MVGameControllerLegacyUI.EditorController.ShowInventory();
	}

	public override void OnMouseSlotOver(int slotIndex, int visiblePage)
	{
		base.OnMouseSlotOver(slotIndex, visiblePage);
		if (PageIndex == visiblePage && InfoButton != null)
		{
			InfoButton.SetVisible(visible: true);
		}
	}

	public override void OnMouseSlotOverExit(int slotIndex, int visiblePage)
	{
		base.OnMouseSlotOverExit(slotIndex, visiblePage);
		if (InfoButton != null)
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
		if (ObjectPreviewer != null)
		{
			ObjectPreviewer.Destroy();
		}
	}
}
