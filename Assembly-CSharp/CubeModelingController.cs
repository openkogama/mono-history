using System;
using UnityEngine;

public class CubeModelingController
{
	public delegate void OnToggleEditCubesDelegate(bool active);

	public delegate void OnToggleDeleteCubesDelegate(bool active);

	public delegate void OnTogglePaintCubesDelegate(bool active);

	public delegate void OnWorkPlaneAltitudeChangedDelegate(int altitude);

	public OnToggleEditCubesDelegate OnToggleEditCubes;

	public OnToggleDeleteCubesDelegate OnToggleDeleteCubes;

	public OnTogglePaintCubesDelegate OnTogglePaintCubes;

	public OnWorkPlaneAltitudeChangedDelegate OnDrawplaneAltitudeChanged;

	protected MVGUIMaterialSelectionWindow materialSelectionWindow;

	protected MVGUIDrawplaneControls workplaneArrows;

	protected MVGUICurrentSelectedMaterialCube currentSelectedMaterialCube;

	private readonly AIngameController ingameController;

	private readonly CubeModelingStateMachine cubeModelingStateMachine;

	public MVGUICubeTools CubeTools { get; private set; }

	public WorldEditorDrawPlane WorldEditorDrawPlane { get; private set; }

	public bool IsDrawPlaneActive => WorldEditorDrawPlane.Active;

	public CubeModelingController(AIngameController ingameController, CubeModelingStateMachine cubeModelingStateMachine)
	{
		this.ingameController = ingameController;
		this.cubeModelingStateMachine = cubeModelingStateMachine;
	}

	public void ResolveCubeTools(GameObject gui)
	{
		CubeTools = AIngameController.FindGUIObjectOfType<MVGUICubeTools>(gui);
		materialSelectionWindow = AIngameController.FindGUIObjectOfType<MVGUIMaterialSelectionWindow>(gui);
		workplaneArrows = AIngameController.FindGUIObjectOfType<MVGUIDrawplaneControls>(gui);
		currentSelectedMaterialCube = AIngameController.FindGUIObjectOfType<MVGUICurrentSelectedMaterialCube>(gui);
		InitializeSelectMaterialUI();
		InitializeCubeToolsUI();
		CubeTools.View.Show();
		CreateDrawPlane();
	}

	public void Update()
	{
		if (WorldEditorDrawPlane != null && WorldEditorDrawPlane.Active)
		{
			WorldEditorDrawPlane.UpdateDrawPlane();
		}
	}

	public void HandleInput()
	{
		if (MVInputWrapper.GetBooleanControlUp(KogamaControls.ChangeMaterial))
		{
			ShowMaterialChangeWindow();
		}
		if (MVInputWrapper.GetBooleanControlDown(KogamaControls.ToggleDrawPlane))
		{
			ToggleDrawPlane();
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

	public void CreateDrawPlane()
	{
		if (!(WorldEditorDrawPlane != null))
		{
			WorldEditorDrawPlane = (UnityEngine.Object.Instantiate(Resources.Load("Prefabs/GUI/DrawPlane")) as GameObject).GetComponent<WorldEditorDrawPlane>();
			WorldEditorDrawPlane.TargetGameObject = MVGameController.WOCM.GetSingletonWorldObject<MVCubeModelPrototypeTerrain>().GameObject;
			WorldEditorDrawPlane.Active = false;
			WorldEditorDrawPlane worldEditorDrawPlane = WorldEditorDrawPlane;
			worldEditorDrawPlane.OnAltitudeChanged = (WorldEditorDrawPlane.AltitudeChangedDelegate)Delegate.Combine(worldEditorDrawPlane.OnAltitudeChanged, new WorldEditorDrawPlane.AltitudeChangedDelegate(NotifyAltitudeUpdate));
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
			currentSelectedMaterialCube.CurrentMaterial = material;
			MVGameController.WOCM.AvatarLocal.LaserPointer.CurrentCubeMaterial = materialId;
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
		SetCubeModelingTool(CubeModelingEvent.EditCubes);
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

	public void DrawPlaneToModel(GameObject gameObject)
	{
		WorldEditorDrawPlane.CachePos();
		WorldEditorDrawPlane.TargetGameObject = gameObject;
		WorldEditorDrawPlane.SetToTargetGameObjectZero();
	}

	private void NotifyAltitudeUpdate(int altitude)
	{
		if (OnDrawplaneAltitudeChanged != null)
		{
			OnDrawplaneAltitudeChanged(altitude);
		}
	}

	public void ToggleDrawPlane()
	{
		if (!(WorldEditorDrawPlane == null))
		{
			if (WorldEditorDrawPlane.IsOnLandscape)
			{
				WorldEditorDrawPlane.SetToCameraPos();
			}
			else
			{
				WorldEditorDrawPlane.SetToTargetGameObjectZero();
			}
			WorldEditorDrawPlane.Active = !WorldEditorDrawPlane.Active;
			workplaneArrows.View.SetVisible(WorldEditorDrawPlane.Active);
		}
	}

	public void ReturnDrawPlaneToLandscape()
	{
		WorldEditorDrawPlane.TargetGameObject = MVGameController.WOCM.GetSingletonWorldObject<MVCubeModelPrototypeTerrain>().GameObject;
		WorldEditorDrawPlane.RestorePos();
	}

	public void HideEditorTools()
	{
		CubeTools.View.Hide();
		if (WorldEditorDrawPlane.Active)
		{
			ToggleDrawPlane();
		}
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
