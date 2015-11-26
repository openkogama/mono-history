using System;
using System.Collections.Generic;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public abstract class EditorController : AIngameController, IEditModeUI, IPlayModeUI, ICubeModelingEditMode
{
	protected CubeModelingController cubeModelingController;

	protected PlayControllerEdit playController = new PlayControllerEdit();

	protected bool _isInEditMode;

	protected MVGUIPublishButton publish;

	protected MVGUIPlayButton playButton;

	protected MVGUIAggregateInventory aggregateInventory;

	protected MVGUIShopView shopView;

	protected MVGUINewModelDialog newModelWindow;

	protected MVGUIEditorTools editorTools;

	protected MVGUIEditorToggles editorToggles;

	protected MVGUIMenu menu;

	protected MVGUIGameInfo gameInfo;

	protected MVGUIDrawplane drawplaneToggle;

	protected MVGUIFullscreenToggle fullscreenToggle;

	protected MVGUIMuteToggle muteToggle;

	protected MVGUIEditModeScreenShot screenShot;

	protected MVGUILevel level;

	private Action<EditModeChangeArgs> editModeChange;

	public Action<EditModeChangeArgs> EditModeChange
	{
		get
		{
			return editModeChange;
		}
		set
		{
			editModeChange = value;
		}
	}

	public bool PlayInEditor { get; private set; }

	public PlayControllerBase PlayController => playController;

	public override bool WindowShown => (currentView != null && currentView.isVisible) || menu.View.isVisible;

	public EditorWorldObjectCreation EditorWorldObjectCreation { get; private set; }

	public CubeModelingController CubeModelingController => cubeModelingController;

	public EditorStateMachine EditorStateMachine { get; protected set; }

	public abstract DrawPlaneController DrawPlaneController { get; }

	public bool InLobbyState
	{
		get
		{
			return playController.InLobbyState && PlayInEditor;
		}
		set
		{
			playController.InLobbyState = value;
		}
	}

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
		EditorStateMachine = new EditorStateMachine();
		MVNetworkGame game = MVGameControllerBase.Game;
		game.OnItemAddedToWorld = (Action<bool>)Delegate.Combine(game.OnItemAddedToWorld, new Action<bool>(OnItemAddedToWorld));
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
		DrawPlaneController.Update();
	}

	public override void HandleInput()
	{
		if (!PlayInEditor)
		{
			base.HandleInput();
			EditorStateMachine.Update();
		}
		if (MVInputWrapper.GetBooleanControlDown(KogamaControls.Respawn))
		{
			if (_isInEditMode)
			{
				return;
			}
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
		if (MVInputWrapper.GetBooleanControlDown(KogamaControls.ToggleDrawPlane))
		{
			ToggleDrawPlane();
		}
		if (MVInputWrapper.GetBooleanControlDown(KogamaControls.FocusOnSelectedModel))
		{
			MVWorldObjectClient singleSelectedWO = EditorStateMachine.SingleSelectedWO;
			if (singleSelectedWO != null)
			{
				MVGameControllerBase.CameraController.CurCamera.FocusOnObject(singleSelectedWO);
			}
		}
		if (MVInputWrapper.GetBooleanControlUp(KogamaControls.ShowPlayerWindow))
		{
			ShowPlayersWindow(show: false);
		}
		else if (MVInputWrapper.GetBooleanControlDown(KogamaControls.ShowPlayerWindow))
		{
			ShowPlayersWindow(show: true);
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
			MVGameControllerBase.Game.AddWorldObjectToInventory(wo.Id, imageData);
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
			if (!item.Delete(MVGameControllerBase.WOCM, ref errorText))
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

	public virtual void EnterCubeModelEdit(float scale)
	{
		((MVAvatarLocal.JetPackMode)MVGameControllerBase.WOCM.AvatarLocal.CurrentMode).ModifySpeed(Mathf.Min(1f, 2f * scale), Mathf.Min(1f, 2f * scale));
		Hide();
		drawplaneToggle.View.Show();
		cubeModelingController.ShowEditorTools();
		cubeModelingController.ShowCurrentSelectedMaterial();
		_isInEditMode = true;
	}

	public abstract void LeaveCubeModelEdit();

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
		return MVGameControllerBase.WOCM.GetWorldObjectClient(woID);
	}

	public MVWorldObjectClient GetSettingsDialogSelectionWO()
	{
		int settingsDialogSelectionWOID = GetSettingsDialogSelectionWOID();
		if (settingsDialogSelectionWOID == -1)
		{
			return null;
		}
		return MVGameControllerBase.WOCM.GetWorldObjectClient(settingsDialogSelectionWOID);
	}

	private void OnItemAddedToWorld(bool success)
	{
		if (!success)
		{
			Debug.LogWarning("Add item to world returned -1. This means that a singleton wo is already present and thus the request was rejected. We need a way to handle server operations in consistent manner.");
			EditorStateMachine.Event = EditorEvent.ESTerrainEdit;
		}
	}

	private void SetPlayInEditorMode(bool playInEditor)
	{
		PlayInEditor = playInEditor;
		if (playInEditor)
		{
			MVGameControllerBase.WOCM.RootGroup.PlayModeInitialize();
			EditorStateMachine.Event = EditorEvent.ESWalkMode;
			MVGameControllerBase.WOCM.AvatarLocal.SetMode(AvatarRuntimeState.Playing);
			MVGameControllerBase.WOCM.MoveableController.ResetMoveables();
			Hide();
			UXUtils.FindGUIObjectOfType<MVGUIChatWindow>().AddLine(TM._("Leveling is disabled in edit play mode"), Color.red);
			MVTeam team = MVGameControllerBase.Game.LocalPlayer.Team;
			MVTeamManager teamManager = MVGameControllerBase.Game.TeamManager;
			if (!teamManager.IsTeamActive(team))
			{
				List<MVTeam> teamList = teamManager.GetTeamList();
				MVGameControllerBase.Game.SetTeam(teamList[0]);
			}
		}
		else
		{
			if (MVGameControllerBase.Game.GameType == MVGameType.Classic)
			{
				MVGameControllerBase.WOCM.AvatarLocal.SetMode(AvatarRuntimeState.Edit);
			}
			else if (MVGameControllerBase.Game.GameType == MVGameType.Platformer)
			{
				MVGameControllerBase.WOCM.AvatarLocal.SetMode(AvatarRuntimeState.Edit2D);
				((MVAvatarLocal.EditorAvatarMode2D)MVGameControllerBase.WOCM.AvatarLocal.CurrentMode).ResetToZPos();
				MVGameControllerBase.CameraController.StartTransitionCam(0.5f);
			}
			SetupEditMode();
			Show();
		}
		playButton.playButton.ToggleState = playInEditor;
		menu.playersWindow.UpdateTeamLists();
		playController.PlayFromEdit(playInEditor);
		if (EditModeChange != null)
		{
			EditModeChange(new EditModeChangeArgs(playInEditor));
		}
	}

	private void TogglePlayInEditor()
	{
		playButton.playButton.Toggle();
	}

	private void SetupEditMode()
	{
		MVGameControllerBase.WOCM.MoveableController.ResetMoveables();
		MVGameControllerBase.WOCM.RootGroup.PlayModeInitialize();
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

	public void HideEditorTools()
	{
		cubeModelingController.HideCubeTools();
		DrawPlaneController.HideDrawPlane();
	}

	private void ToggleGridSnap()
	{
		editorToggles.gridSnapToggle.Toggle();
	}

	public abstract void ToggleDrawPlane();

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

	protected abstract void Show();

	private void Hide()
	{
		if (currentView != null)
		{
			HideCurrentWindow();
		}
		drawplaneToggle.View.Hide();
		cubeModelingController.HideCurrentSelectedMaterial();
		cubeModelingController.HideCubeTools();
		DrawPlaneController.HideDrawPlane();
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

	public void ShowEUseIcon(ShowUseOption option, int woID = 0)
	{
		playController.ShowEUseIcon(option, woID);
	}

	public void HideEUseIcon()
	{
		playController.HideEUseIcon();
	}

	public IGUICrossHair GetCrossHair()
	{
		return playController.GetCrossHair();
	}
}
