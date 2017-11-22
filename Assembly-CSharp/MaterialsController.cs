using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class MaterialsController : MonoBehaviour, IMaterialClicked, IHandleMaterial, IEventSystemHandler
{
	protected CubeModelingStateMachine cubeModelingStateMachine;

	private TabState tab;

	private InventoryController inventoryController;

	protected DesktopCubeModelingController desktopCubeModelingController;

	[SerializeField]
	private InventoryController inventoryControllerPrefab;

	[SerializeField]
	private DesktopCubeModelingController desktopCubeModelingControllerPrefab;

	[SerializeField]
	private int numberOfSlotsPrPage;

	[SerializeField]
	private MaterialViewItem materialViewItemPrefab;

	public UnityAction<byte> materialChange;

	public UnityAction materialsPop;

	public void Initialize(CubeModelingStateMachine cubeModelingStateMachine)
	{
		this.cubeModelingStateMachine = cubeModelingStateMachine;
		InitializeMaterialInventoryData();
	}

	public virtual Transform SetActive()
	{
		desktopCubeModelingController = UnityEngine.Object.Instantiate(desktopCubeModelingControllerPrefab);
		desktopCubeModelingController.Initialize(cubeModelingStateMachine);
		cubeModelingStateMachine.CurrentMaterialId = cubeModelingStateMachine.CurrentMaterialId;
		OnMaterialChanged(cubeModelingStateMachine.CurrentMaterialId);
		return desktopCubeModelingController.transform;
	}

	public void Push(UIPushOption pushOption, UnityAction onPop)
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(desktopCubeModelingController.gameObject, pushOption, onPop);
		});
	}

	public virtual void OnMaterialChanged(byte id)
	{
		if (desktopCubeModelingController != null)
		{
			desktopCubeModelingController.SetMaterial(id);
		}
		if (materialChange != null)
		{
			materialChange(id);
		}
	}

	public void ShowMaterialInventory()
	{
		if (!(inventoryController != null))
		{
			ShowInventory(UIPushOption.Blocking, tab.currentPage);
		}
	}

	public void ShowMaterialInventoryFirstPage()
	{
		if (!(inventoryController != null))
		{
			ShowInventory(UIPushOption.Blocking);
		}
	}

	public void OnMaterialClicked(byte materialID)
	{
		cubeModelingStateMachine.CurrentMaterialId = materialID;
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.PopGroups(UIGroupFlags.InventoryUI | UIGroupFlags.InventoryUISubMenu);
		});
	}

	private void ShowInventory(UIPushOption pushOption, int page = 1)
	{
		this.inventoryController = UnityEngine.Object.Instantiate(inventoryControllerPrefab);
		InventoryController inventoryController = this.inventoryController;
		inventoryController.OnPageTurned = (UnityAction<int>)Delegate.Combine(inventoryController.OnPageTurned, new UnityAction<int>(PageTurned));
		this.inventoryController.Initialize(numberOfSlotsPrPage);
		tab.currentPage = page;
		this.inventoryController.AddTab(0, tab.name);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack handler, BaseEventData data) =>
		{
			handler.PopGroups(UIGroupFlags.InventoryUI | UIGroupFlags.InventoryUISubMenu);
		});
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(this.inventoryController.gameObject, pushOption, OnPop, UIGroupFlags.InventoryUI);
		});
		UpdateContent();
	}

	public void PageTurned(int dir)
	{
		if (tab.UpdatePage(dir))
		{
			UpdateContent();
		}
	}

	public void UpdateContent()
	{
		inventoryController.Clear();
		inventoryController.SelectTab(0, tab.currentPage, tab.MaxPages);
		for (byte b = 0; b < MVGameControllerBase.Game.MaterialRepository.MaterialCount; b++)
		{
			if (tab.SlotIndexIsInRange(b))
			{
				MVMaterial material = MVGameControllerBase.Game.MaterialRepository.GetMaterial(b);
				MaterialViewItem materialViewItem = UnityEngine.Object.Instantiate(materialViewItemPrefab);
				materialViewItem.Initialize(b, !material.isUnlocked, material.buttonTexture, material.IsAvailable, b == cubeModelingStateMachine.CurrentMaterialId);
				inventoryController.AddObject(materialViewItem.gameObject, b % numberOfSlotsPrPage);
			}
		}
	}

	private void InitializeMaterialInventoryData()
	{
		tab = new TabState(TM._("Materials"), numberOfSlotsPrPage);
		tab.highestSlotIndex = MVGameControllerBase.Game.MaterialRepository.MaterialCount - 1;
	}

	private void OnPop()
	{
		if (materialsPop != null)
		{
			materialsPop();
		}
	}
}
