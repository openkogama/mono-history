using System;
using System.Collections;
using MV.WorldObject.MetaData;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class DesktopCubeModelingControllerCubeTutorial : MonoBehaviour
{
	private byte defaultMaterial;

	private CubeModelingStateMachine cubeModelingStateMachine;

	private MaterialsController materialsController;

	[SerializeField]
	private RawImage materialsButtonImage;

	[SerializeField]
	private DesktopCubeModelingToolsController desktopCubeModelingController;

	[SerializeField]
	private GameObject deleteTool;

	[SerializeField]
	private GameObject paintTool;

	[SerializeField]
	private GameObject cubeTool;

	[SerializeField]
	private FirstTimeCubeEditFadeButtons materialButton;

	[SerializeField]
	private DesktopCubeModelingToolsController desktopCubeModelingToolsController;

	[SerializeField]
	private FirstTimeEvent exitFirstTimeEvent;

	private bool paintHasBeenActivated;

	public void Initialize(CubeModelingStateMachine cubeModelingStateMachine, MaterialsController materialsController)
	{
		this.materialsController = materialsController;
		this.cubeModelingStateMachine = cubeModelingStateMachine;
		defaultMaterial = cubeModelingStateMachine.CurrentMaterialId;
		desktopCubeModelingController.Initialize(cubeModelingStateMachine);
		FirstTimeEventManager.SubscribeToFirstTimeState(OnFirstTimeState);
		materialsController.materialChange = (UnityAction<byte>)Delegate.Combine(materialsController.materialChange, new UnityAction<byte>(SetMaterial));
		materialsController.materialsPop = (UnityAction)Delegate.Combine(materialsController.materialsPop, new UnityAction(MaterialsPop));
	}

	private void OnFirstTimeState(FirstTimeState firstTimeState, FirstTimeEvent firstTimeEvent)
	{
		if (firstTimeEvent == exitFirstTimeEvent)
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				if (x.PopToStackElement(gameObject))
				{
					x.Pop();
				}
			});
			Debug.Log("Trying to pop");
		}
		UpdateDelete();
		UpdatePaint();
	}

	private void UpdateDelete()
	{
		if (!deleteTool.activeSelf && FirstTimeEventManager.HasFirstTimeEventOccured(FirstTimeEvent.BM_CubeTutorialPaintedCubes))
		{
			DeactivateAllToolButtons();
			Debug.Log("delete tool set active");
			deleteTool.SetActive(value: true);
			desktopCubeModelingToolsController.SetAllToTransparent();
		}
	}

	private void UpdatePaint()
	{
		if (FirstTimeEventManager.HasFirstTimeEventOccured(FirstTimeEvent.BM_CubeTutorialAddedCubes))
		{
			if (FirstTimeEventManager.HasFirstTimeEventOccured(FirstTimeEvent.BM_ChangeToolToPaint) && !materialButton.IsEnabled())
			{
				Debug.Log("Activate materials");
				materialButton.ActivateImmediate();
			}
			if (!paintTool.activeSelf && !paintHasBeenActivated)
			{
				DeactivateAllToolButtons();
				paintTool.SetActive(value: true);
				paintHasBeenActivated = true;
				desktopCubeModelingToolsController.SetAllToTransparent();
			}
		}
	}

	private void DeactivateAllToolButtons()
	{
		cubeTool.SetActive(value: false);
		paintTool.SetActive(value: false);
		deleteTool.SetActive(value: false);
	}

	public void SetMaterial(byte materialId)
	{
		DoSetMaterial(cubeModelingStateMachine.CurrentMaterialId);
	}

	private void DoSetMaterial(byte materialId)
	{
		materialsButtonImage.texture = MVGameControllerBase.Game.MaterialRepository.GetMaterial(materialId).buttonTexture;
		MVGameControllerBase.WOCM.AvatarLocal.LaserPointer.CurrentCubeMaterial = materialId;
	}

	public void MaterialsPop()
	{
		StartCoroutine(OverRideIfDefaultMaterial());
	}

	private IEnumerator OverRideIfDefaultMaterial()
	{
		yield return 0;
		if (cubeModelingStateMachine.CurrentMaterialId == defaultMaterial)
		{
			cubeModelingStateMachine.CurrentMaterialId = 23;
			DoSetMaterial(cubeModelingStateMachine.CurrentMaterialId);
		}
	}

	private void OnDestroy()
	{
		cubeModelingStateMachine = null;
		FirstTimeEventManager.UnSubscribeToFirstTimeState(OnFirstTimeState);
		MaterialsController materialsController = this.materialsController;
		materialsController.materialChange = (UnityAction<byte>)Delegate.Remove(materialsController.materialChange, new UnityAction<byte>(SetMaterial));
		MaterialsController materialsController2 = this.materialsController;
		materialsController2.materialsPop = (UnityAction)Delegate.Remove(materialsController2.materialsPop, new UnityAction(MaterialsPop));
		FirstTimeEventManager.SetFirstTimeEvent(FirstTimeEvent.BM_CubeTutorialDone);
	}
}
