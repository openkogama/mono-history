using System;
using MV.Common;
using UnityEngine;

public class CubeModelingController
{
	public delegate void OnToggleEditCubesDelegate(bool active);

	public delegate void OnToggleDeleteCubesDelegate(bool active);

	public delegate void OnTogglePaintCubesDelegate(bool active);

	public OnToggleEditCubesDelegate OnToggleEditCubes;

	public OnToggleDeleteCubesDelegate OnToggleDeleteCubes;

	public OnTogglePaintCubesDelegate OnTogglePaintCubes;

	protected MVGUIMaterialSelectionWindow materialSelectionWindow;

	protected MVGUICurrentSelectedMaterialCube currentSelectedMaterialCube;

	private readonly AIngameController ingameController;

	private readonly CubeModelingStateMachine cubeModelingStateMachine;

	public MVGUICubeTools CubeTools { get; private set; }

	public CubeModelingController(AIngameController ingameController, CubeModelingStateMachine cubeModelingStateMachine)
	{
		this.ingameController = ingameController;
		this.cubeModelingStateMachine = cubeModelingStateMachine;
	}

	public void ResolveCubeTools(GameObject gui)
	{
		CubeTools = AIngameController.FindGUIObjectOfType<MVGUICubeTools>(gui);
		materialSelectionWindow = AIngameController.FindGUIObjectOfType<MVGUIMaterialSelectionWindow>(gui);
		currentSelectedMaterialCube = AIngameController.FindGUIObjectOfType<MVGUICurrentSelectedMaterialCube>(gui);
		InitializeSelectMaterialUI();
		InitializeCubeToolsUI();
		CubeTools.View.Show();
	}

	public void HandleInput()
	{
		if (MVInputWrapper.GetBooleanControlUp(KogamaControls.ChangeMaterial))
		{
			ShowMaterialChangeWindow();
		}
		if (MVInputWrapper.GetBooleanControlUp(KogamaControls.ActivateEditCubeTool))
		{
			CubeTools.editCubeButton.Toggle();
		}
		if (MVInputWrapper.GetBooleanControlUp(KogamaControls.ActivateDeleteCubeTool))
		{
			CubeTools.deleteCubeButton.Toggle();
		}
		if (MVInputWrapper.GetBooleanControlUp(KogamaControls.ActivetaPaintCubeTool))
		{
			CubeTools.paintCubeButton.Toggle();
		}
	}

	private void InitializeCubeToolsUI()
	{
		UXToggleIconButton editCubeButton = CubeTools.editCubeButton;
		editCubeButton.OnToggle = (UXToggleIconButton.OnToggleDelegate)Delegate.Combine(editCubeButton.OnToggle, (UXToggleIconButton.OnToggleDelegate)((bool active) =>
		{
			ToggleEditCubes();
		}));
		CubeTools.editCubeButton.Toggle();
		UXToggleIconButton deleteCubeButton = CubeTools.deleteCubeButton;
		deleteCubeButton.OnToggle = (UXToggleIconButton.OnToggleDelegate)Delegate.Combine(deleteCubeButton.OnToggle, (UXToggleIconButton.OnToggleDelegate)((bool active) =>
		{
			ToggleDeleteCubes();
		}));
		UXToggleIconButton paintCubeButton = CubeTools.paintCubeButton;
		paintCubeButton.OnToggle = (UXToggleIconButton.OnToggleDelegate)Delegate.Combine(paintCubeButton.OnToggle, (UXToggleIconButton.OnToggleDelegate)((bool active) =>
		{
			TogglePaintCubes();
		}));
		UXToggleIconButton sprayPaintCubeButton = CubeTools.sprayPaintCubeButton;
		sprayPaintCubeButton.OnToggle = (UXToggleIconButton.OnToggleDelegate)Delegate.Combine(sprayPaintCubeButton.OnToggle, (UXToggleIconButton.OnToggleDelegate)((bool active) =>
		{
			ToggleSprayCubes();
		}));
	}

	public MVGUIMaterialSelectionWindow ShowMaterialChangeWindow()
	{
		ingameController.ShowSingleWindow(materialSelectionWindow.View);
		return materialSelectionWindow;
	}

	private void InitializeSelectMaterialUI()
	{
		CubeModelingStateMachine cubeModelingStateMachine = this.cubeModelingStateMachine;
		cubeModelingStateMachine.OnCurrentMaterialChange = (CubeModelingStateMachine.OnCurrentMaterialChangeDelegate)Delegate.Combine(cubeModelingStateMachine.OnCurrentMaterialChange, (CubeModelingStateMachine.OnCurrentMaterialChangeDelegate)((byte materialId, Material material) =>
		{
			currentSelectedMaterialCube.CurrentMaterial = materialId;
			MVGameControllerBase.WOCM.AvatarLocal.LaserPointer.CurrentCubeMaterial = materialId;
		}));
		materialSelectionWindow.OnMaterialSelection = (byte materialId) =>
		{
			this.cubeModelingStateMachine.CurrentMaterialId = materialId;
		};
		currentSelectedMaterialCube.OnClick = () =>
		{
			ShowMaterialChangeWindow();
		};
		CubeModelingStateMachine cubeModelingStateMachine2 = this.cubeModelingStateMachine;
		cubeModelingStateMachine2.OnCurrentMaterialChange = (CubeModelingStateMachine.OnCurrentMaterialChangeDelegate)Delegate.Combine(cubeModelingStateMachine2.OnCurrentMaterialChange, (CubeModelingStateMachine.OnCurrentMaterialChangeDelegate)((byte materialId, Material material) =>
		{
			materialSelectionWindow.SetSelectedMaterial(materialId);
		}));
		this.cubeModelingStateMachine.CurrentMaterialId = 21;
	}

	public void ToggleTool(CubeModelingEvent tool)
	{
		cubeModelingStateMachine.Event = tool;
	}

	private void SetCubeModelingTool(CubeModelingEvent toolState)
	{
		cubeModelingStateMachine.Event = toolState;
	}

	public void ToggleEditCubes()
	{
		if (MVGameControllerBase.Game.GameType == MVGameType.Platformer && cubeModelingStateMachine.TargetCubeModel is MVCubeModelPrototypeTerrain)
		{
			SetCubeModelingTool(CubeModelingEvent.EditCubes2D);
		}
		else
		{
			SetCubeModelingTool(CubeModelingEvent.EditCubes);
		}
	}

	public void ToggleDeleteCubes()
	{
		SetCubeModelingTool(CubeModelingEvent.DeleteCubes);
	}

	public void TogglePaintCubes()
	{
		SetCubeModelingTool(CubeModelingEvent.PaintCubes);
	}

	public void ToggleSprayCubes()
	{
		SetCubeModelingTool(CubeModelingEvent.SprayCubes);
	}

	public void HideCubeTools()
	{
		CubeTools.View.Hide();
	}

	public void ShowCurrentSelectedMaterial()
	{
		currentSelectedMaterialCube.View.Show();
	}

	public void HideCurrentSelectedMaterial()
	{
		currentSelectedMaterialCube.View.Hide();
	}

	public virtual void ShowEditorTools()
	{
		CubeTools.View.Show();
	}
}
