using System;
using System.Collections;
using System.Collections.Generic;
using Localize;
using MV.WorldObject;
using UnityEngine;
using Util;

public abstract class AEditController : AIngameController
{
	public delegate void OnToggleEditCubesDelegate(bool active);

	public delegate void OnToggleDeleteCubesDelegate(bool active);

	public delegate void OnTogglePaintCubesDelegate(bool active);

	public delegate void OnWorkPlaneAltitudeChangedDelegate(int altitude);

	public OnToggleEditCubesDelegate OnToggleEditCubes;

	public OnToggleDeleteCubesDelegate OnToggleDeleteCubes;

	public OnTogglePaintCubesDelegate OnTogglePaintCubes;

	public OnWorkPlaneAltitudeChangedDelegate OnDrawplaneAltitudeChanged;

	private ILogger logger = LoggerManager.Instance.GetLogger(typeof(AEditController));

	private bool _isInEditMode;

	protected MVGUIEditorTools editorTools;

	protected MVGUIEditorToggles editorToggles;

	protected MVGUIAggregateInventory aggregateInventory;

	protected MVGUIShopView shopView;

	protected MVGUINewModelDialog newModelWindow;

	protected MVGUIMaterialSelectionWindow materialSelectionWindow;

	protected MVGUIDrawplaneControls workplaneArrows;

	protected MVGUIPlayButton playButton;

	private bool lockToggle = true;

	public EditorStateMachine EditorStateMachine { get; private set; }

	public EditorWorldObjectCreation EditorWorldObjectCreation { get; private set; }

	public WorldEditorDrawPlane WorldEditorDrawPlane { get; private set; }

	public bool PlayInEditor { get; private set; }

	public MVGUICubeTools CubeTools { get; private set; }

	public bool IsDrawPlaneActive => WorldEditorDrawPlane.Active;

	public AEditController()
	{
		EditorStateMachine = new EditorStateMachine();
		EditorWorldObjectCreation = new EditorWorldObjectCreation(EditorStateMachine);
	}

	public override void Initialize()
	{
		base.Initialize();
		InitializeSelectMaterialUI();
		InitializeToggleUI();
		InitializeCubeToolsUI();
		InitializePlayUI();
		InitializeNewModelUI();
		shopView.View.Show();
		editorTools.View.Show();
		CubeTools.View.Show();
		CreateDrawPlane();
		SetPlayInEditorMode(playInEditor: false);
	}

	public void CreateDrawPlane()
	{
		if (!((Object)(object)WorldEditorDrawPlane != (Object)null))
		{
			Object val = Object.Instantiate(Resources.Load("Prefabs/GUI/DrawPlane"));
			WorldEditorDrawPlane = ((GameObject)((val is GameObject) ? val : null)).GetComponent<WorldEditorDrawPlane>();
			WorldEditorDrawPlane.TargetGameObject = MVGameController.Instance.WOCM.GetSingletonWorldObject<MVCubeModelPrototypeTerrain>().GameObject;
			WorldEditorDrawPlane.Active = false;
			WorldEditorDrawPlane worldEditorDrawPlane = WorldEditorDrawPlane;
			worldEditorDrawPlane.OnAltitudeChanged = (WorldEditorDrawPlane.AltitudeChangedDelegate)Delegate.Combine(worldEditorDrawPlane.OnAltitudeChanged, new WorldEditorDrawPlane.AltitudeChangedDelegate(NotifyAltitudeUpdate));
		}
	}

	private void InitializeNewModelUI()
	{
		newModelWindow.Initialize();
		UXIconButton newModelButton = editorTools.newModelButton;
		newModelButton.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(newModelButton.OnClick, (UXBaseButton.OnClickDelegate)(() =>
		{
			ShowNewModelWindow();
		}));
	}

	private void InitializeToggleUI()
	{
		editorToggles.InitializeButtons();
		editorToggles.View.Show();
		editorToggles.drawplaneToggle.SetToggleState(toggle: false);
		UXToggleIconButton drawplaneToggle = editorToggles.drawplaneToggle;
		drawplaneToggle.OnToggle = (UXToggleIconButton.OnToggleDelegate)Delegate.Combine(drawplaneToggle.OnToggle, (UXToggleIconButton.OnToggleDelegate)((bool active) =>
		{
			ToggleDrawPlane();
		}));
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

	public override void Update()
	{
		base.Update();
		if ((Object)(object)WorldEditorDrawPlane != (Object)null && WorldEditorDrawPlane.Active)
		{
			WorldEditorDrawPlane.UpdateDrawPlane();
		}
	}

	public override void HandleInput()
	{
		base.HandleInput();
		EditorStateMachine.Update();
	}

	protected override void ResolveGUIElements()
	{
		base.ResolveGUIElements();
		GameObject editGUI = UXUtils.GetGUIHandler().editGUI;
		editorTools = editGUI.GetComponentInChildren<MVGUIEditorTools>();
		editorToggles = editGUI.GetComponentInChildren<MVGUIEditorToggles>();
		CubeTools = editGUI.GetComponentInChildren<MVGUICubeTools>();
		newModelWindow = editGUI.GetComponentInChildren<MVGUINewModelDialog>();
		aggregateInventory = ((Component)editorTools).GetComponent<MVGUIAggregateInventoryButton>().aggregateInventory;
		shopView = editGUI.GetComponentInChildren<MVGUIShopView>();
		materialSelectionWindow = editGUI.GetComponentInChildren<MVGUIMaterialSelectionWindow>();
		playButton = editGUI.GetComponentInChildren<MVGUIPlayButton>();
		workplaneArrows = editGUI.GetComponentInChildren<MVGUIDrawplaneControls>();
	}

	public override void ToggleShowUI()
	{
		base.ToggleShowUI();
		if (uiShown)
		{
			editorTools.View.Show();
			editorToggles.View.Show();
			CubeTools.View.Show();
			shopView.View.Show();
			MVGameController.Instance.WOCM.AvatarLocal.ShowHealth = PlayInEditor;
		}
		else
		{
			editorTools.View.Hide();
			editorToggles.View.Hide();
			CubeTools.View.Hide();
			shopView.View.Hide();
			MVGameController.Instance.WOCM.AvatarLocal.ShowHealth = false;
		}
	}

	public override void RemoveUI()
	{
		base.RemoveUI();
		editorTools.View.Hide();
		editorToggles.View.Hide();
		CubeTools.View.Hide();
		shopView.View.Hide();
		materialSelectionWindow.View.Hide();
		workplaneArrows.View.Hide();
		aggregateInventory.ResetCollectionViews();
		shopView.shopInventory.ResetCollectionViews();
		Object.Destroy((Object)(object)((Component)WorldEditorDrawPlane).gameObject);
		WorldEditorDrawPlane = null;
	}

	public virtual void HideEditorTools()
	{
		_isInEditMode = false;
		CubeTools.View.Hide();
		editorToggles.toggleGroup.Hide();
		editorTools.View.Hide();
		shopView.View.Hide();
		if (WorldEditorDrawPlane.Active)
		{
			ToggleDrawPlane();
		}
	}

	public virtual void ShowEditorTools(bool cubeEditMode = false)
	{
		_isInEditMode = cubeEditMode;
		CubeTools.View.Show();
		editorToggles.toggleGroup.Show();
		editorTools.View.Show();
		shopView.View.Show();
		if (cubeEditMode)
		{
			editorTools.newModelButton.SetVisible(visible: false);
			editorTools.aggregateInventoryButton.SetVisible(visible: false);
			editorToggles.gridSnapToggle.SetVisible(visible: false);
			editorToggles.logicRenderingToggle.SetVisible(visible: false);
			shopView.View.Hide();
		}
	}

	public virtual MVGUIMaterialSelectionWindow ShowMaterialChangeWindow()
	{
		ShowSingleWindow(materialSelectionWindow.View);
		return materialSelectionWindow;
	}

	public virtual void ShowNewModelWindow()
	{
		if (!_isInEditMode)
		{
			ShowSingleWindow(newModelWindow.View);
		}
	}

	public virtual void ShowInventory()
	{
		if (!_isInEditMode)
		{
			ShowSingleWindow(aggregateInventory.View);
		}
	}

	public virtual void ShowShopInventory()
	{
		if (!_isInEditMode)
		{
			ShowSingleWindow(shopView.shopInventory.View);
		}
	}

	public MVGUIAggregateInventory GetShopInventory()
	{
		return shopView.shopInventory;
	}

	private void InitializeSelectMaterialUI()
	{
		CubeModelingStateMachine cubeModelingStateMachine = EditorStateMachine.CubeModelingStateMachine;
		cubeModelingStateMachine.OnCurrentMaterialChange = (CubeModelingStateMachine.OnCurrentMaterialChangeDelegate)Delegate.Combine(cubeModelingStateMachine.OnCurrentMaterialChange, (CubeModelingStateMachine.OnCurrentMaterialChangeDelegate)((byte materialId, Material material) =>
		{
			editorTools.currentSelectedMaterialCube.CurrentMaterial = material;
			MVGameController.Instance.WOCM.AvatarLocal.LaserPointer.CurrentCubeMaterial = materialId;
		}));
		materialSelectionWindow.OnMaterialSelection = (byte materialId) =>
		{
			EditorStateMachine.CubeModelingStateMachine.CurrentMaterialId = materialId;
		};
		editorTools.currentSelectedMaterialCube.OnClick = () =>
		{
			ShowMaterialChangeWindow();
		};
		CubeModelingStateMachine cubeModelingStateMachine2 = EditorStateMachine.CubeModelingStateMachine;
		cubeModelingStateMachine2.OnCurrentMaterialChange = (CubeModelingStateMachine.OnCurrentMaterialChangeDelegate)Delegate.Combine(cubeModelingStateMachine2.OnCurrentMaterialChange, (CubeModelingStateMachine.OnCurrentMaterialChangeDelegate)((byte materialId, Material material) =>
		{
			materialSelectionWindow.SetSelectedMaterial(materialId);
		}));
		EditorStateMachine.CubeModelingStateMachine.CurrentMaterialId = 21;
	}

	public void ToggleTool(CubeModelingEvent tool)
	{
		EditorStateMachine.CubeModelingStateMachine.Event = tool;
	}

	private void SetCubeModelingTool(CubeModelingEvent toolState)
	{
		EditorStateMachine.CubeModelingStateMachine.Event = toolState;
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

	public static bool IsGridSnap()
	{
		return MVGUIEditorToggles.GridSnap;
	}

	public virtual void ToggleGridSnap()
	{
		editorToggles.gridSnapToggle.Toggle();
	}

	public bool IsLogicRendered()
	{
		return editorToggles.LogicRendered;
	}

	public virtual void ToggleLogicRendering()
	{
		editorToggles.ToggleLogicRendering();
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

	public void ReturnDrawPlaneToLandscape()
	{
		WorldEditorDrawPlane.TargetGameObject = MVGameController.Instance.WOCM.GetSingletonWorldObject<MVCubeModelPrototypeTerrain>().GameObject;
		WorldEditorDrawPlane.RestorePos();
	}

	public virtual void ToggleDrawPlane()
	{
		if (!((Object)(object)WorldEditorDrawPlane == (Object)null))
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
			editorToggles.drawplaneToggle.SetToggleState(WorldEditorDrawPlane.Active);
		}
	}

	private void InitializePlayUI()
	{
		UXToggleIconButton uXToggleIconButton = playButton.playButton;
		uXToggleIconButton.OnToggle = (UXToggleIconButton.OnToggleDelegate)Delegate.Combine(uXToggleIconButton.OnToggle, new UXToggleIconButton.OnToggleDelegate(SetPlayInEditorMode));
	}

	public void TogglePlayInEditor()
	{
		playButton.playButton.Toggle();
		ShowItemHUD();
	}

	protected virtual void SetPlayInEditorMode(bool playInEditor)
	{
		PlayInEditor = playInEditor;
		if (playInEditor)
		{
			MVGameController.Instance.WOCM.RootGroup.PlayModeInitialize();
			MVGameController.Instance.WOCM.AvatarLocal.AvatarMode = MVGameController.Instance.WOCM.AvatarLocal.AvatarModes.WalkMode;
			MVGameController.Instance.WOCM.MoveableController.ResetMoveables();
			EditorStateMachine.Event = EditorEvent.ESWalkMode;
			HideEditorTools();
			if ((Object)(object)currentView != (Object)null)
			{
				HideCurrentWindow();
			}
		}
		else
		{
			MVGameController.Instance.WOCM.AvatarLocal.AvatarMode = MVGameController.Instance.WOCM.AvatarLocal.AvatarModes.JetPackMode;
			MVGameController.Instance.WOCM.MoveableController.ResetMoveables();
			ShowEditorTools();
			MVGameController.Instance.WOCM.RootGroup.PlayModeInitialize();
		}
		playButton.playButton.ToggleState = playInEditor;
	}

	public void AddToInventory(MVWorldObjectClient wo)
	{
		logger.Log("Add to inventory");
		Action<byte[]> callback = (byte[] imageData) =>
		{
			MVGameController.Instance.Game.AddWorldObjectToInventory(wo.Id, imageData);
		};
		Coroutines.StartCoroutine(CreateTextureFromData(wo, callback));
	}

	public void LockSelected()
	{
		if (EditorStateMachine.SingleSelectedWO != null)
		{
			MVGameController.Instance.Game.LockHierarchy(EditorStateMachine.SingleSelectedWO.Id, lockToggle);
			lockToggle = !lockToggle;
		}
	}

	public void GroupSelected()
	{
		if (EditorStateMachine.CurEvent != EditorEvent.ESWaitForGroup)
		{
			EditorStateMachine.PushState(EditorEvent.ESWaitForGroup);
		}
	}

	public void UngroupSelected()
	{
		if (EditorStateMachine.CurEvent != EditorEvent.ESWaitForUngroup)
		{
			EditorStateMachine.PushState(EditorEvent.ESWaitForUngroup);
		}
	}

	public bool Delete(HashSet<MVWorldObjectClient> deleteSet)
	{
		List<MVWorldObjectClient> list = new List<MVWorldObjectClient>(deleteSet);
		EditorStateMachine.DeSelectAll();
		foreach (MVWorldObjectClient item in list)
		{
			TextSlotIndex errorTextIndex = TextSlotIndex.Empty;
			if (!item.Delete(MVGameController.Instance.WOCM, ref errorTextIndex))
			{
				DialogFactory.CreateDialog(errorTextIndex).Show();
				return false;
			}
		}
		return true;
	}

	public void DebugAddCubeToSelected(IntVector pos)
	{
		if (EditorStateMachine.SingleSelectedWO is MVCubeModelBase)
		{
			((MVCubeModelBase)EditorStateMachine.SingleSelectedWO).AddCube(pos, new Cube(CubeDataPacker.CornersToByteArray(CubeBase.IdentityCorners), Cube.CreateMaterialArray(0)));
			Debug.Log((object)("Added cubes to " + ((Object)EditorStateMachine.SingleSelectedWO.GameObject).name));
		}
	}

	public static IEnumerator CreateTextureFromData(MVWorldObjectClient wo, Action<byte[]> callback)
	{
		int textureSize = 512;
		Texture2D previewTexture = new Texture2D(textureSize, textureSize, (TextureFormat)3, false);
		TexturePreviewer.Instance.tex = (Texture)(object)previewTexture;
		GameObject previewRoot = new GameObject("Item Preview");
		MVComponent[] mvComponents = wo.GameObject.GetComponentsInChildren<MVComponent>();
		MVComponent[] array = mvComponents;
		foreach (MVComponent mvComponent in array)
		{
			mvComponent.findWorldObjectParent = false;
		}
		Object val = Object.Instantiate((Object)(object)wo.GameObject);
		GameObject itemCopy = (GameObject)(object)((val is GameObject) ? val : null);
		MVComponent[] array2 = mvComponents;
		foreach (MVComponent mvComponent2 in array2)
		{
			mvComponent2.findWorldObjectParent = true;
		}
		ObjectPreviewer objectPreviewer = ObjectPreviewer.Create(textureSize, (CameraClearFlags)1, wo.PreviewLayerMask, new Vector3(1.2f, 0.1f, 0.3f), previewRoot.transform, itemCopy.transform.position, "Model preview", wo, itemCopy);
		yield return 0;
		RenderTexture.active = objectPreviewer.PreviewTexture;
		previewTexture.ReadPixels(new Rect(0f, 0f, (float)textureSize, (float)textureSize), 0, 0);
		previewTexture.Apply();
		RenderTexture.active = null;
		Object.Destroy((Object)(object)previewRoot);
		Object.Destroy((Object)(object)itemCopy);
		objectPreviewer.Destroy();
		byte[] bytes = previewTexture.EncodeToPNG();
		callback((byte[])bytes.Clone());
	}
}
