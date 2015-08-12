using System;
using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public class EditorController : AEditController
{
	private PlayControllerEdit playController = new PlayControllerEdit();

	private bool _isInEditMode;

	private MVGUIPublishButton publish;

	private MVGUIPlayButton playButton;

	private MVGUIAggregateInventory aggregateInventory;

	private MVGUIShopView shopView;

	private MVGUINewModelDialog newModelWindow;

	private MVGUIEditorTools editorTools;

	private MVGUIEditorToggles editorToggles;

	private MVGUIMenu menu;

	private MVGUIGameInfo gameInfo;

	private MVGUIDrawplane drawplaneToggle;

	private MVGUIFullscreenToggle fullscreenToggle;

	private MVGUIMuteToggle muteToggle;

	private MVGUIEditModeScreenShot screenShot;

	private MVGUILevel level;

	public bool PlayInEditor { get; private set; }

	public PlayControllerBase PlayController => playController;

	public override bool WindowShown => (currentView != null && currentView.isVisible) || menu.View.isVisible;

	public EditorWorldObjectCreation EditorWorldObjectCreation { get; private set; }

	protected override void ResolveGUIElements()
	{
		base.ResolveGUIElements();
		GameObject gameObject = UXUtils.FindGUIObjectOfType<MVGUIEditor>().gameObject;
		cubeModelingController = new CubeModelingController(this, EditorStateMachine.CubeModelingStateMachine);
		cubeModelingController.ResolveCubeTools(gameObject);
		editorTools = AIngameController.FindGUIObjectOfType<MVGUIEditorTools>(gameObject);
		editorToggles = AIngameController.FindGUIObjectOfType<MVGUIEditorToggles>(gameObject);
		newModelWindow = AIngameController.FindGUIObjectOfType<MVGUINewModelDialog>(gameObject);
		aggregateInventory = AIngameController.FindGUIObjectOfType<MVGUIAggregateInventoryButton>(gameObject).aggregateInventory;
		shopView = AIngameController.FindGUIObjectOfType<MVGUIShopView>(gameObject);
		menu = AIngameController.FindGUIObjectOfType<MVGUIMenu>(gameObject);
		publish = AIngameController.FindGUIObjectOfType<MVGUIPublishButton>(gameObject);
		playButton = AIngameController.FindGUIObjectOfType<MVGUIPlayButton>(gameObject);
		fullscreenToggle = AIngameController.FindGUIObjectOfType<MVGUIFullscreenToggle>(gameObject);
		muteToggle = AIngameController.FindGUIObjectOfType<MVGUIMuteToggle>(gameObject);
		screenShot = AIngameController.FindGUIObjectOfType<MVGUIEditModeScreenShot>(gameObject);
		drawplaneToggle = AIngameController.FindGUIObjectOfType<MVGUIDrawplane>(gameObject);
		gameInfo = AIngameController.FindGUIObjectOfType<MVGUIGameInfo>(gameObject);
		level = AIngameController.FindGUIObjectOfType<MVGUILevel>(gameObject);
	}

	public override void Initialize()
	{
		base.Initialize();
		EditorWorldObjectCreation = new EditorWorldObjectCreation(EditorStateMachine);
		EditorStateMachine.Event = EditorEvent.ESTerrainEdit;
		InitializePlayUI();
		publish.Initialize();
		SetupEditMode();
		InitializeToggleUI();
		InitializeNewModelUI();
		playController.Initialize(playButton);
		playController.PlayFromEdit(playInEditor: false);
		Show();
	}

	public override void Update()
	{
		base.Update();
		cubeModelingController.Update();
	}

	public override void HandleInput()
	{
		if (!PlayInEditor)
		{
			base.HandleInput();
		}
		if (MVInputWrapper.GetBooleanControlDown(KogamaControls.Respawn))
		{
			RespawnAvatar();
		}
		if (MVInputWrapper.GetBooleanControlDown(KogamaControls.ToggleLogicRendering))
		{
			ToggleLogicRendering();
		}
		if (MVInputWrapper.GetBooleanControlDown(KogamaControls.TogglePlayInEditor))
		{
			Debug.Log("Toggle in play mode");
			TogglePlayInEditor();
		}
		if (PlayInEditor)
		{
			playController.HandleInput();
			return;
		}
		if (MVInputWrapper.GetBooleanControlUp(KogamaControls.ShowChat))
		{
			chatController.ShowChat(takeControl: true, retainControlAfterMessageSend: false);
		}
		cubeModelingController.HandleInput();
		if (MVInputWrapper.GetBooleanControlDown(KogamaControls.ToggleGripdSnapSize))
		{
			ToggleGridSnap();
		}
		if (MVInputWrapper.GetBooleanControlUp(KogamaControls.OpenInventory))
		{
			ShowInventory();
		}
		if (MVInputWrapper.GetBooleanControlUp(KogamaControls.CreateNewModel))
		{
			ShowNewModelWindow();
		}
		if (MVInputWrapper.GetBooleanControlDown(KogamaControls.FocusOnSelectedModel))
		{
			MVWorldObjectClient singleSelectedWO = MVGameController.EditController.EditorStateMachine.SingleSelectedWO;
			if (singleSelectedWO != null)
			{
				MVGameController.Game.CameraController.CurCamera.FocusOnObject(singleSelectedWO);
			}
		}
		if (MVInputWrapper.GetBooleanControlUp(KogamaControls.ShowPlayerWindow))
		{
			MVGameController.EditorController.ShowPlayersWindow(show: false);
		}
		else if (MVInputWrapper.GetBooleanControlDown(KogamaControls.ShowPlayerWindow))
		{
			MVGameController.EditorController.ShowPlayersWindow(show: true);
		}
	}

	public bool IsGridSnap()
	{
		return MVGUIEditorToggles.GridSnap;
	}

	public void AddToInventory(MVWorldObjectClient wo)
	{
		Action<byte[]> callback = (byte[] imageData) =>
		{
			MVGameController.Game.AddWorldObjectToInventory(wo.Id, imageData);
		};
		Coroutines.StartCoroutine(ImageGenerator.CreateTextureFromData(wo, callback));
	}

	public bool Delete(HashSet<MVWorldObjectClient> deleteSet)
	{
		List<MVWorldObjectClient> list = new List<MVWorldObjectClient>(deleteSet);
		EditorStateMachine.DeSelectAll();
		foreach (MVWorldObjectClient item in list)
		{
			string errorText = string.Empty;
			if (!item.Delete(MVGameController.WOCM, ref errorText))
			{
				UXUtils.UXDialogFactory.CreateDialog(errorText, string.Empty).Show();
				return false;
			}
		}
		return true;
	}

	public MVGUIMaterialSelectionWindow ShowMaterialChangeWindow()
	{
		return cubeModelingController.ShowMaterialChangeWindow();
	}

	public void ShowPlayersWindow(bool show)
	{
		menu.ShowOnShortcut(show);
	}

	public void EnterCubeModelEdit()
	{
		Hide();
		drawplaneToggle.View.Show();
		cubeModelingController.ShowEditorTools();
		cubeModelingController.ShowCurrentSelectedMaterial();
		_isInEditMode = true;
	}

	public void LeaveCubeModelEdit()
	{
		Show();
		_isInEditMode = false;
	}

	public void ShowInventory()
	{
		if (!_isInEditMode)
		{
			ShowSingleWindow(aggregateInventory.View);
		}
	}

	public MVGUIAggregateInventory GetShopInventory()
	{
		return shopView.shopInventory;
	}

	public bool IsLogicRendered()
	{
		return editorToggles.LogicRendered;
	}

	public void ToggleLogicRendering()
	{
		editorToggles.ToggleLogicRendering();
	}

	public int GetSettingsDialogSelectionWOID()
	{
		if (EditorStateMachine.SingleSelectedWO != null)
		{
			return EditorStateMachine.SingleSelectedWO.Id;
		}
		return -1;
	}

	public bool IsMenuShown()
	{
		return menu.View.isVisible;
	}

	public MVWorldObjectClient GetSettingsDialogWOById(int woID)
	{
		if (woID == -1)
		{
			return null;
		}
		return MVGameController.WOCM.GetWorldObjectClient(woID);
	}

	public MVWorldObjectClient GetSettingsDialogSelectionWO()
	{
		int settingsDialogSelectionWOID = GetSettingsDialogSelectionWOID();
		if (settingsDialogSelectionWOID == -1)
		{
			return null;
		}
		return MVGameController.WOCM.GetWorldObjectClient(settingsDialogSelectionWOID);
	}

	private void SetPlayInEditorMode(bool playInEditor)
	{
		PlayInEditor = playInEditor;
		if (playInEditor)
		{
			MVGameController.WOCM.RootGroup.PlayModeInitialize();
			MVGameController.WOCM.AvatarLocal.AvatarMode = MVGameController.WOCM.AvatarLocal.AvatarModes.WalkMode;
			MVGameController.WOCM.MoveableController.ResetMoveables();
			EditorStateMachine.Event = EditorEvent.ESWalkMode;
			Hide();
			if (!LevelingManager.silentMode)
			{
				UXUtils.FindGUIObjectOfType<MVGUIChatWindow>().AddLine(TM._("Leveling is disabled in edit play mode"), Color.red);
			}
			MVTeam team = MVGameController.Game.LocalPlayer.Team;
			MVTeamManager teamManager = MVGameController.Game.TeamManager;
			if (!teamManager.IsTeamActive(team))
			{
				List<MVTeam> teamList = teamManager.GetTeamList();
				MVGameController.Game.SetTeam(teamList[0]);
			}
		}
		else
		{
			SetupEditMode();
			Show();
		}
		playButton.playButton.ToggleState = playInEditor;
		menu.playersWindow.UpdateTeamLists();
		playController.PlayFromEdit(playInEditor);
	}

	private void TogglePlayInEditor()
	{
		playButton.playButton.Toggle();
	}

	private void SetupEditMode()
	{
		MVGameController.WOCM.AvatarLocal.AvatarMode = MVGameController.WOCM.AvatarLocal.AvatarModes.JetPackMode;
		MVGameController.WOCM.MoveableController.ResetMoveables();
		MVGameController.WOCM.RootGroup.PlayModeInitialize();
	}

	private void ShowNewModelWindow()
	{
		if (!_isInEditMode)
		{
			ShowSingleWindow(newModelWindow.View);
		}
	}

	public void ShowShopInventory()
	{
		if (!_isInEditMode)
		{
			ShowSingleWindow(shopView.shopInventory.View);
		}
	}

	private void ToggleGridSnap()
	{
		editorToggles.gridSnapToggle.Toggle();
	}

	private void ToggleDrawPlane()
	{
		cubeModelingController.ToggleDrawPlane();
		drawplaneToggle.drawplaneToggle.SetToggleState(cubeModelingController.WorldEditorDrawPlane.Active);
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
		drawplaneToggle.drawplaneToggle.SetToggleState(toggle: false);
		UXToggleIconButton uXToggleIconButton = drawplaneToggle.drawplaneToggle;
		uXToggleIconButton.OnToggle = (UXToggleIconButton.OnToggleDelegate)Delegate.Combine(uXToggleIconButton.OnToggle, (UXToggleIconButton.OnToggleDelegate)((bool active) =>
		{
			ToggleDrawPlane();
		}));
	}

	private void InitializePlayUI()
	{
		UXToggleIconButton uXToggleIconButton = playButton.playButton;
		uXToggleIconButton.OnToggle = (UXToggleIconButton.OnToggleDelegate)Delegate.Combine(uXToggleIconButton.OnToggle, new UXToggleIconButton.OnToggleDelegate(SetPlayInEditorMode));
	}

	private void Show()
	{
		drawplaneToggle.View.Show();
		cubeModelingController.ShowCurrentSelectedMaterial();
		cubeModelingController.ShowEditorTools();
		editorToggles.View.Show();
		editorTools.View.Show();
		shopView.View.Show();
		publish.View.Show();
		playButton.View.Show();
		fullscreenToggle.View.Show();
		muteToggle.View.Show();
		if (MVGameController.Game.LocalPlayer.PlanetOwnershipTypeID == 2)
		{
			screenShot.View.Show();
		}
		gameInfo.View.Show();
		level.View.Show();
	}

	private void Hide()
	{
		if (currentView != null)
		{
			HideCurrentWindow();
		}
		drawplaneToggle.View.Hide();
		cubeModelingController.HideCurrentSelectedMaterial();
		cubeModelingController.HideEditorTools();
		editorTools.View.Hide();
		editorToggles.View.Hide();
		shopView.View.Hide();
		publish.View.Hide();
		playButton.View.Hide();
		fullscreenToggle.View.Hide();
		muteToggle.View.Hide();
		gameInfo.View.Hide();
		level.View.Hide();
		screenShot.View.Hide();
	}
}
