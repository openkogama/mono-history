using System;
using System.Collections;
using System.Collections.Generic;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class EditorController : IIngameController
{
	public delegate void OnToggleLogicRenderingDelegate(bool active);

	public delegate void OnGridSnapChangedDelegate(bool gridSnap);

	private ILogger logger = LoggerManager.Instance.GetLogger(typeof(EditorController));

	private EditorStateMachine esm;

	private Stack<WorldEditorDrawPlane> worldEditorDrawPlaneStack = new Stack<WorldEditorDrawPlane>();

	private bool playInEditor;

	private Chat chat;

	private MVGUIEditorTools editorTools;

	private MVGUIEditorToggles editorToggles;

	private MVGUIGameHUD gameHud;

	private MVGUIGameState gameState;

	private MVGUIMainMenu mainMenu;

	private MVGUIMaterialSelectionWindow materialSelectionWindow;

	private MVGUIToolboxWindow toolboxWindow;

	private MVGUIPickupWindow pickupWindow;

	private MVGUIInventoryButton inventoryButton;

	private MVGUIRespawnButton respawnButton;

	private MVGUISocialWindowToggle socialWindowToggle;

	private MVGUISocialWindow socialWindow;

	private bool gridSnap;

	public OnToggleLogicRenderingDelegate OnToggleLogicRendering;

	public OnGridSnapChangedDelegate OnGridSnapChanged;

	private bool activeMenu;

	private bool lockToggle = true;

	private float previewCamAdditionalHeight = 2.5f;

	private float previewCamDist = 2.5f;

	private Vector3 cameraOffset = new Vector3(1.2f, 0f, 0.5f);

	private Texture2D previewTexture;

	public MVNetworkGameStateListener GameStateListener => MVGameController.Instance.Game.NetworkGameStateListener;

	public bool PlayInEditor => playInEditor;

	public WorldEditorDrawPlane WorldEditorDrawPlane => worldEditorDrawPlaneStack.Peek();

	public bool GridSnap
	{
		get
		{
			return gridSnap;
		}
		set
		{
			bool flag = gridSnap != value;
			gridSnap = value;
			if (flag && OnGridSnapChanged != null)
			{
				OnGridSnapChanged(gridSnap);
			}
		}
	}

	public EditorController()
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		esm = new EditorStateMachine();
		GameObject gameObject = ((Component)(MVCameraController)(object)Object.FindObjectOfType(typeof(MVCameraController))).gameObject;
		MVGameController.Instance.WOCM.WeCamera = gameObject.GetComponent<MVCameraController>();
		MVGameController.Instance.WOCM.WeCamera.Init();
		worldEditorDrawPlaneStack.Push(new WorldEditorDrawPlane(MVGameController.Instance.WOCM.Terrain.GameObject));
		worldEditorDrawPlaneStack.Peek().Active = false;
		MVGameController.Instance.WOCM.WeCamera.SetCamera(CameraType.JetPackCamera);
		esm.Event = EditorEvent.EditCubes;
		ResolveGUIElements();
		GameStateListener.OnGameStateChanged += GameStateListener_OnGameStateChanged;
	}

	private void GameStateListener_OnGameStateChanged(object sender, GameStateChangeEventArgs e)
	{
		switch (GameStateListener.CurrentGameState)
		{
		case MVGameStateType.PrepareRound:
			break;
		case MVGameStateType.Round:
			MVGameController.Instance.WOCM.LocalPlayer.Avatar.AvatarController.Respawn();
			break;
		case MVGameStateType.RoundEnded:
			break;
		}
	}

	public void PushDrawPlane(GameObject gameObject)
	{
		bool active = worldEditorDrawPlaneStack.Peek().Active;
		worldEditorDrawPlaneStack.Peek().Active = false;
		worldEditorDrawPlaneStack.Push(new WorldEditorDrawPlane(gameObject));
		worldEditorDrawPlaneStack.Peek().Active = active;
	}

	public void PopDrawPlane()
	{
		if (worldEditorDrawPlaneStack.Peek() != null)
		{
			bool active = worldEditorDrawPlaneStack.Peek().Active;
			worldEditorDrawPlaneStack.Peek().Destroy();
			worldEditorDrawPlaneStack.Pop();
			worldEditorDrawPlaneStack.Peek().Active = active;
		}
	}

	private void ResolveGUIElements()
	{
		editorTools = Object.FindObjectOfType(typeof(MVGUIEditorTools)) as MVGUIEditorTools;
		gameHud = Object.FindObjectOfType(typeof(MVGUIGameHUD)) as MVGUIGameHUD;
		gameState = Object.FindObjectOfType(typeof(MVGUIGameState)) as MVGUIGameState;
		materialSelectionWindow = MVGUIMaterialSelectionWindow.New();
		mainMenu = Object.FindObjectOfType(typeof(MVGUIMainMenu)) as MVGUIMainMenu;
		editorToggles = Object.FindObjectOfType(typeof(MVGUIEditorToggles)) as MVGUIEditorToggles;
		toolboxWindow = Object.FindObjectOfType(typeof(MVGUIToolboxWindow)) as MVGUIToolboxWindow;
		pickupWindow = Object.FindObjectOfType(typeof(MVGUIPickupWindow)) as MVGUIPickupWindow;
		inventoryButton = Object.FindObjectOfType(typeof(MVGUIInventoryButton)) as MVGUIInventoryButton;
		respawnButton = Object.FindObjectOfType(typeof(MVGUIRespawnButton)) as MVGUIRespawnButton;
		socialWindow = Object.FindObjectOfType(typeof(MVGUISocialWindow)) as MVGUISocialWindow;
		socialWindowToggle = Object.FindObjectOfType(typeof(MVGUISocialWindowToggle)) as MVGUISocialWindowToggle;
	}

	public override void Initialize()
	{
		logger.Log("Initialize");
		InitializeToggleUI();
		InitializeSelectMaterialUI();
		InitializeToolboxUI();
		InitializePickupUI();
		InitializePlayUI();
		InitializeNewModelUI();
		InitializeSocialUI();
		editorTools.View.Show();
		InitializeChat();
		gameHud.View.Show();
		gameHud.planetNameText.Text = MVGameController.Instance.planetName;
		gameState.View.Show();
		gameState.gameMsgs.Text = string.Empty;
		mainMenu.publishButton.OnClick = () =>
		{
			MVGUIPublishDialog.New();
		};
		mainMenu.View.Show();
		SetPlayInEditorMode(playInEditor: false);
	}

	private void InitializeToggleUI()
	{
		editorToggles.View.Show();
		UXToggleIconButton workplaneToggle = editorToggles.workplaneToggle;
		workplaneToggle.OnToggle = (UXToggleIconButton.OnToggleDelegate)Delegate.Combine(workplaneToggle.OnToggle, (UXToggleIconButton.OnToggleDelegate)((bool active) =>
		{
			if (IsDrawPlaneActive() != active)
			{
				ToggleWorkPlane();
			}
		}));
		WorldEditorDrawPlane.OnToggleWorkPlane = (WorldEditorDrawPlane.OnToggleDrawPlaneDelegate)Delegate.Combine(WorldEditorDrawPlane.OnToggleWorkPlane, (WorldEditorDrawPlane.OnToggleDrawPlaneDelegate)((bool active) =>
		{
			editorToggles.workplaneToggle.ToggleState = IsDrawPlaneActive();
		}));
		UXToggleIconButton logicRenderingToggle = editorToggles.logicRenderingToggle;
		logicRenderingToggle.OnToggle = (UXToggleIconButton.OnToggleDelegate)Delegate.Combine(logicRenderingToggle.OnToggle, (UXToggleIconButton.OnToggleDelegate)((bool active) =>
		{
			if (IsLogicRendered() != active)
			{
				ToggleLogicRendering();
			}
		}));
		OnToggleLogicRendering = (OnToggleLogicRenderingDelegate)Delegate.Combine(OnToggleLogicRendering, (OnToggleLogicRenderingDelegate)((bool active) =>
		{
			editorToggles.logicRenderingToggle.ToggleState = IsLogicRendered();
		}));
		editorToggles.workplaneToggle.ToggleState = IsDrawPlaneActive();
		editorToggles.logicRenderingToggle.ToggleState = IsLogicRendered();
		editorToggles.gridSnapToggle.ToggleState = GridSnap;
		UXToggleIconButton gridSnapToggle = editorToggles.gridSnapToggle;
		gridSnapToggle.OnToggle = (UXToggleIconButton.OnToggleDelegate)Delegate.Combine(gridSnapToggle.OnToggle, (UXToggleIconButton.OnToggleDelegate)((bool active) =>
		{
			GridSnap = active;
		}));
		inventoryButton.View.Show();
	}

	private void InitializeChat()
	{
		chat = new Chat();
		chat.AllowActivate = () => !activeMenu;
		chat.OnDeactivate = () =>
		{
			activeMenu = false;
		};
		chat.Show();
	}

	private void InitializeSocialUI()
	{
		socialWindowToggle.View.Show();
		socialWindow.InitializeListeners();
	}

	public void ShowChat()
	{
		chat.Activate();
	}

	private void InitializeSelectMaterialUI()
	{
		EditorStateMachine editorStateMachine = esm;
		editorStateMachine.OnCurrentMaterialChange = (EditorStateMachine.OnCurrentMaterialChangeDelegate)Delegate.Combine(editorStateMachine.OnCurrentMaterialChange, (EditorStateMachine.OnCurrentMaterialChangeDelegate)((byte materialId, Material material) =>
		{
			editorTools.currentSelectedMaterialCube.CurrentMaterial = material;
		}));
		materialSelectionWindow.OnMaterialSelection = (int materialId) =>
		{
			esm.CurrentMaterialId = (byte)materialId;
		};
		editorTools.currentSelectedMaterialCube.OnClick = () =>
		{
			ShowMaterialSelectionWindow();
		};
		UXView view = materialSelectionWindow.View;
		view.OnHide = (UXView.OnHideDelegate)Delegate.Combine(view.OnHide, new UXView.OnHideDelegate(HandleOnHideMaterialSelectionWindow));
		EditorStateMachine editorStateMachine2 = esm;
		editorStateMachine2.OnCurrentMaterialChange = (EditorStateMachine.OnCurrentMaterialChangeDelegate)Delegate.Combine(editorStateMachine2.OnCurrentMaterialChange, (EditorStateMachine.OnCurrentMaterialChangeDelegate)((byte materialId, Material material) =>
		{
			materialSelectionWindow.SetSelectedMaterialCube(materialId);
			activeMenu = false;
		}));
		esm.CurrentMaterialId = 21;
	}

	private void InitializeToolboxUI()
	{
		UXMouseClickObject toolboxIcon = editorTools.toolboxIcon;
		toolboxIcon.OnClick = (UXMouseClickObject.OnClickDelegate)Delegate.Combine(toolboxIcon.OnClick, (UXMouseClickObject.OnClickDelegate)((UXMouseClickObject clickObject, Vector3 mousePositionWorld) =>
		{
			ShowToolboxWindow();
		}));
		UXView view = toolboxWindow.View;
		view.OnHide = (UXView.OnHideDelegate)Delegate.Combine(view.OnHide, new UXView.OnHideDelegate(HandleOnHideToolboxWindow));
		UXIconButton lightButton = toolboxWindow.lightButton;
		lightButton.OnClick = (UXIconButton.OnClickDelegate)Delegate.Combine(lightButton.OnClick, (UXIconButton.OnClickDelegate)(() =>
		{
			AddDefaultPointLight();
		}));
		UXIconButton spawnPointButton = toolboxWindow.spawnPointButton;
		spawnPointButton.OnClick = (UXIconButton.OnClickDelegate)Delegate.Combine(spawnPointButton.OnClick, (UXIconButton.OnClickDelegate)(() =>
		{
			AddLogicObject(WorldObjectType.SpawnPoint, new Hashtable());
		}));
		UXIconButton triggerBoxButton = toolboxWindow.triggerBoxButton;
		triggerBoxButton.OnClick = (UXIconButton.OnClickDelegate)Delegate.Combine(triggerBoxButton.OnClick, (UXIconButton.OnClickDelegate)(() =>
		{
			AddTriggerBox();
		}));
		if (Object.op_Implicit((Object)(object)toolboxWindow.flagButton))
		{
			UXIconButton flagButton = toolboxWindow.flagButton;
			flagButton.OnClick = (UXIconButton.OnClickDelegate)Delegate.Combine(flagButton.OnClick, (UXIconButton.OnClickDelegate)(() =>
			{
				AddLogicObject(WorldObjectType.Flag, new Hashtable());
			}));
		}
		UXIconButton batteryButton = toolboxWindow.batteryButton;
		batteryButton.OnClick = (UXIconButton.OnClickDelegate)Delegate.Combine(batteryButton.OnClick, (UXIconButton.OnClickDelegate)(() =>
		{
			AddLogicObject(WorldObjectType.Battery, new Hashtable());
		}));
		UXIconButton toggleBoxButton = toolboxWindow.toggleBoxButton;
		toggleBoxButton.OnClick = (UXIconButton.OnClickDelegate)Delegate.Combine(toggleBoxButton.OnClick, (UXIconButton.OnClickDelegate)(() =>
		{
			AddToggleBox();
		}));
		UXIconButton negateButton = toolboxWindow.negateButton;
		negateButton.OnClick = (UXIconButton.OnClickDelegate)Delegate.Combine(negateButton.OnClick, (UXIconButton.OnClickDelegate)(() =>
		{
			AddLogicObject(WorldObjectType.Negate, new Hashtable());
		}));
		UXIconButton andButton = toolboxWindow.andButton;
		andButton.OnClick = (UXIconButton.OnClickDelegate)Delegate.Combine(andButton.OnClick, (UXIconButton.OnClickDelegate)(() =>
		{
			AddLogicObject(WorldObjectType.And, new Hashtable());
		}));
		if (Object.op_Implicit((Object)(object)toolboxWindow.explositionButton))
		{
			UXIconButton explositionButton = toolboxWindow.explositionButton;
			explositionButton.OnClick = (UXIconButton.OnClickDelegate)Delegate.Combine(explositionButton.OnClick, (UXIconButton.OnClickDelegate)(() =>
			{
				AddLogicObject(WorldObjectType.Explosives, new Hashtable());
			}));
		}
		if (Object.op_Implicit((Object)(object)toolboxWindow.textMsgButton))
		{
			UXIconButton textMsgButton = toolboxWindow.textMsgButton;
			textMsgButton.OnClick = (UXIconButton.OnClickDelegate)Delegate.Combine(textMsgButton.OnClick, (UXIconButton.OnClickDelegate)(() =>
			{
				AddTextMessage();
			}));
		}
		if (Object.op_Implicit((Object)(object)toolboxWindow.timeTriggerButton))
		{
			UXIconButton timeTriggerButton = toolboxWindow.timeTriggerButton;
			timeTriggerButton.OnClick = (UXIconButton.OnClickDelegate)Delegate.Combine(timeTriggerButton.OnClick, (UXIconButton.OnClickDelegate)(() =>
			{
				AddTimeTrigger();
			}));
		}
		if (Object.op_Implicit((Object)(object)toolboxWindow.fireButton))
		{
			UXIconButton fireButton = toolboxWindow.fireButton;
			fireButton.OnClick = (UXIconButton.OnClickDelegate)Delegate.Combine(fireButton.OnClick, (UXIconButton.OnClickDelegate)(() =>
			{
				AddLogicObject(WorldObjectType.Fire, new Hashtable());
			}));
		}
		if (Object.op_Implicit((Object)(object)toolboxWindow.smokeButton))
		{
			UXIconButton smokeButton = toolboxWindow.smokeButton;
			smokeButton.OnClick = (UXIconButton.OnClickDelegate)Delegate.Combine(smokeButton.OnClick, (UXIconButton.OnClickDelegate)(() =>
			{
				AddLogicObject(WorldObjectType.Smoke, new Hashtable());
			}));
		}
		if (Object.op_Implicit((Object)(object)toolboxWindow.teleporterButton))
		{
			UXIconButton teleporterButton = toolboxWindow.teleporterButton;
			teleporterButton.OnClick = (UXIconButton.OnClickDelegate)Delegate.Combine(teleporterButton.OnClick, (UXIconButton.OnClickDelegate)(() =>
			{
				AddLogicObject(WorldObjectType.Teleporter, new Hashtable());
			}));
		}
		if (Object.op_Implicit((Object)(object)toolboxWindow.goalButton))
		{
			UXIconButton goalButton = toolboxWindow.goalButton;
			goalButton.OnClick = (UXIconButton.OnClickDelegate)Delegate.Combine(goalButton.OnClick, (UXIconButton.OnClickDelegate)(() =>
			{
				AddLogicObject(WorldObjectType.Goal, new Hashtable());
			}));
		}
		if (Object.op_Implicit((Object)(object)toolboxWindow.pressurePlateButton))
		{
			UXIconButton pressurePlateButton = toolboxWindow.pressurePlateButton;
			pressurePlateButton.OnClick = (UXIconButton.OnClickDelegate)Delegate.Combine(pressurePlateButton.OnClick, (UXIconButton.OnClickDelegate)(() =>
			{
				AddLogicObject(WorldObjectType.PressurePlate, new Hashtable());
			}));
		}
	}

	private void InitializePickupUI()
	{
		UXMouseClickObject pickupIcon = editorTools.pickupIcon;
		pickupIcon.OnClick = (UXMouseClickObject.OnClickDelegate)Delegate.Combine(pickupIcon.OnClick, (UXMouseClickObject.OnClickDelegate)((UXMouseClickObject clickObject, Vector3 mousePositionWorld) =>
		{
			ShowPickupWindow();
		}));
		UXView view = pickupWindow.View;
		view.OnHide = (UXView.OnHideDelegate)Delegate.Combine(view.OnHide, new UXView.OnHideDelegate(HandleOnHidePickupWindow));
		if (Object.op_Implicit((Object)(object)pickupWindow.healthPickupButton))
		{
			UXIconButton healthPickupButton = pickupWindow.healthPickupButton;
			healthPickupButton.OnClick = (UXIconButton.OnClickDelegate)Delegate.Combine(healthPickupButton.OnClick, (UXIconButton.OnClickDelegate)(() =>
			{
				AddLogicObject(WorldObjectType.PickupItemHealthPack, new Hashtable());
			}));
		}
		if (Object.op_Implicit((Object)(object)pickupWindow.centerGunPickupButton))
		{
			UXIconButton centerGunPickupButton = pickupWindow.centerGunPickupButton;
			centerGunPickupButton.OnClick = (UXIconButton.OnClickDelegate)Delegate.Combine(centerGunPickupButton.OnClick, (UXIconButton.OnClickDelegate)(() =>
			{
				AddLogicObject(WorldObjectType.PickupItemCenterGun, new Hashtable());
			}));
		}
	}

	private void InitializePlayUI()
	{
		UXToggleIconButton play = editorTools.play;
		play.OnToggle = (UXToggleIconButton.OnToggleDelegate)Delegate.Combine(play.OnToggle, new UXToggleIconButton.OnToggleDelegate(SetPlayInEditorMode));
	}

	private void InitializeNewModelUI()
	{
		UXIconButton newModelButton = editorTools.newModelButton;
		newModelButton.OnClick = (UXIconButton.OnClickDelegate)Delegate.Combine(newModelButton.OnClick, (UXIconButton.OnClickDelegate)(() =>
		{
			MVGUINewModelDialog.New();
		}));
	}

	private void SetPlayInEditorMode(bool playInEditor)
	{
		this.playInEditor = playInEditor;
		if (playInEditor)
		{
			MVGameController.Instance.WOCM.WoAvatar.AvatarController.Mode = MVGameController.Instance.WOCM.WoAvatar.AvatarController.AvatarModes.WalkMode;
			MVGameController.Instance.WOCM.WoAvatar.Avatar.ShowHealth = true;
			esm.PushState(EditorEvent.ESWalkMode);
			respawnButton.View.Show();
			HideEditorButtons();
		}
		else
		{
			MVGameController.Instance.WOCM.WoAvatar.AvatarController.Mode = MVGameController.Instance.WOCM.WoAvatar.AvatarController.AvatarModes.JetPackMode;
			MVGameController.Instance.WOCM.WoAvatar.Avatar.ShowHealth = false;
			respawnButton.View.Hide();
			ShowEditorButtons();
		}
		editorTools.play.ToggleState = playInEditor;
	}

	private void HideEditorButtons()
	{
		editorTools.HideEditingTools();
		inventoryButton.View.Hide();
	}

	private void ShowEditorButtons()
	{
		editorTools.ShowEditingTools();
		inventoryButton.View.Show();
	}

	private void HideUI()
	{
		editorTools.View.Hide();
		editorToggles.View.Hide();
		toolboxWindow.View.Hide();
		gameHud.View.Hide();
		mainMenu.View.Hide();
		inventoryButton.View.Hide();
		respawnButton.View.Hide();
		socialWindowToggle.View.Hide();
		chat.Hide();
		activeMenu = false;
	}

	public void ShowMaterialSelectionWindow()
	{
		if (!activeMenu)
		{
			materialSelectionWindow.View.Show();
			activeMenu = true;
			UXFullscreenColliderBox.Instance.AddBlockingObject(materialSelectionWindow);
			UXFullscreenColliderBox.Instance.OnClick = () =>
			{
				materialSelectionWindow.View.Hide();
			};
		}
	}

	private void HandleOnHideMaterialSelectionWindow()
	{
		activeMenu = false;
		UXFullscreenColliderBox.Instance.RemoveBlockingObject(materialSelectionWindow);
	}

	private void ShowToolboxWindow()
	{
		if (!activeMenu)
		{
			toolboxWindow.View.Show();
			activeMenu = true;
			UXFullscreenColliderBox.Instance.AddBlockingObject(toolboxWindow);
			UXFullscreenColliderBox.Instance.OnClick = () =>
			{
				toolboxWindow.View.Hide();
			};
		}
	}

	private void ShowPickupWindow()
	{
		if (!activeMenu)
		{
			pickupWindow.View.Show();
			activeMenu = true;
			UXFullscreenColliderBox.Instance.AddBlockingObject(pickupWindow);
			UXFullscreenColliderBox.Instance.OnClick = () =>
			{
				pickupWindow.View.Hide();
			};
		}
	}

	private void HandleOnHideToolboxWindow()
	{
		activeMenu = false;
		UXFullscreenColliderBox.Instance.RemoveBlockingObject(toolboxWindow);
	}

	private void HandleOnHidePickupWindow()
	{
		activeMenu = false;
		UXFullscreenColliderBox.Instance.RemoveBlockingObject(pickupWindow);
	}

	private void Toggle(UXView view)
	{
		if (!activeMenu)
		{
			view.Show();
			activeMenu = true;
		}
		else if (view.isVisible)
		{
			view.Hide();
			activeMenu = false;
		}
	}

	public override void Update()
	{
		SetGameMsg();
		if (worldEditorDrawPlaneStack.Peek() != null && worldEditorDrawPlaneStack.Peek().Active)
		{
			worldEditorDrawPlaneStack.Peek().Update();
		}
	}

	public override void HandleInput()
	{
		esm.Update();
		MVGameController.Instance.WOCM.WeCamera.HandleInput();
	}

	public override void FixedUpdate()
	{
		MVGameController.Instance.WOCM.WeCamera.UpdateCamera();
	}

	public override void LateUpdate()
	{
		base.LateUpdate();
	}

	public override void Deinitialize()
	{
		logger.Log("Deinitialize");
		HideUI();
	}

	public void LeaveContextMenuState()
	{
		if (esm.CurEvent == EditorEvent.ESContextMenu)
		{
			esm.Event = EditorEvent.ObjectSelected;
		}
	}

	public int GetContextMenuSelectionWOID()
	{
		if (esm.SingleSelectedWO != null)
		{
			return esm.SingleSelectedWO.Id;
		}
		return -1;
	}

	public MVWorldObjectClient GetContextMenuSelectionWO()
	{
		int contextMenuSelectionWOID = GetContextMenuSelectionWOID();
		if (contextMenuSelectionWOID == -1)
		{
			return null;
		}
		return MVGameController.Instance.WOCM.WorldObjects[contextMenuSelectionWOID];
	}

	public RuntimePrototypeCubeModel GetContextMenuSelectionPrototype()
	{
		MVWorldObjectClient contextMenuSelectionWO = GetContextMenuSelectionWO();
		if (contextMenuSelectionWO == null)
		{
			return null;
		}
		MVCubeModelBase mVCubeModelBase = (MVCubeModelBase)contextMenuSelectionWO;
		return MVGameController.Instance.WOCM.WorldInventory.RuntimePrototypes[mVCubeModelBase.Pid];
	}

	public void AddLogicObject(WorldObjectType objectType, Hashtable data)
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		esm.Event = EditorEvent.ESWaitForSelect;
		MVGameController.Instance.WOCM.OnWorldObjectRegisterResponse += Instance_OnWorldObjectRegisterResponse;
		MVGameController.Instance.Game.RegisterWorldObject(objectType, MVGameController.Instance.WOCM.RootGroup.Id, data, new Hashtable(), Vector3.zero, Quaternion.identity, Vector3.one, localOwner: false, transferOwnershipToServerOnLeave: true);
		SetLogicRendering(active: true);
	}

	public void CloneLogicObject(WorldObjectType objectType, Hashtable data)
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		esm.Event = EditorEvent.ESWaitForClone;
		MVGameController.Instance.WOCM.OnWorldObjectRegisterResponse += Instance_OnWorldObjectRegisterResponse;
		MVGameController.Instance.Game.RegisterWorldObject(objectType, MVGameController.Instance.WOCM.RootGroup.Id, data, new Hashtable(), Vector3.zero, Quaternion.identity, Vector3.one, localOwner: false, transferOwnershipToServerOnLeave: true);
	}

	private void AddDefaultPointLight()
	{
		Hashtable hashtable = new Hashtable();
		float[] value = new float[3] { 1f, 1f, 1f };
		hashtable["color"] = value;
		hashtable["intensity"] = 1f;
		hashtable["range"] = 15f;
		AddLogicObject(WorldObjectType.PointLight, hashtable);
	}

	private void AddTriggerBox()
	{
		Hashtable hashtable = new Hashtable();
		hashtable["once"] = false;
		AddLogicObject(WorldObjectType.TriggerBox, hashtable);
	}

	private void AddTimeTrigger()
	{
		Hashtable hashtable = new Hashtable();
		hashtable["time"] = 2.5f;
		hashtable["currentTime"] = 2.5f;
		hashtable["once"] = false;
		AddLogicObject(WorldObjectType.TimeTrigger, hashtable);
	}

	private void AddToggleBox()
	{
		Hashtable hashtable = new Hashtable();
		hashtable["once"] = false;
		hashtable["state"] = false;
		AddLogicObject(WorldObjectType.ToggleBox, hashtable);
	}

	private void AddTextMessage()
	{
		Hashtable hashtable = new Hashtable();
		hashtable["text"] = "Hello, World!";
		AddLogicObject(WorldObjectType.TextMsg, hashtable);
	}

	public void AddSpawnPoint()
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		esm.Event = EditorEvent.ESWaitForSelect;
		Hashtable woData = new Hashtable();
		MVGameController.Instance.WOCM.OnWorldObjectRegisterResponse += Instance_OnWorldObjectRegisterResponse;
		MVGameController.Instance.Game.RegisterWorldObject(WorldObjectType.SpawnPoint, MVGameController.Instance.WOCM.RootGroup.Id, woData, new Hashtable(), Vector3.zero, Quaternion.identity, Vector3.one, localOwner: false, transferOwnershipToServerOnLeave: true);
	}

	public void AddFlag()
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		esm.Event = EditorEvent.ESWaitForSelect;
		Hashtable woData = new Hashtable();
		MVGameController.Instance.WOCM.OnWorldObjectRegisterResponse += Instance_OnWorldObjectRegisterResponse;
		MVGameController.Instance.Game.RegisterWorldObject(WorldObjectType.Flag, MVGameController.Instance.WOCM.RootGroup.Id, woData, new Hashtable(), Vector3.zero, Quaternion.identity, Vector3.one, localOwner: false, transferOwnershipToServerOnLeave: true);
	}

	public void AddTestLogicObject()
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		esm.Event = EditorEvent.ESWaitForSelect;
		Hashtable woData = new Hashtable();
		MVGameController.Instance.WOCM.OnWorldObjectRegisterResponse += Instance_OnWorldObjectRegisterResponse;
		MVGameController.Instance.Game.RegisterWorldObject(WorldObjectType.TestLogicCube, MVGameController.Instance.WOCM.RootGroup.Id, woData, new Hashtable(), Vector3.zero, Quaternion.identity, Vector3.one, localOwner: false, transferOwnershipToServerOnLeave: true);
	}

	public void OnAddNewPrototype(string name, float scale)
	{
		IntVector key = new IntVector(0, 0, 0);
		byte[] array = new byte[6];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = 21;
		}
		Cube value = new Cube(CubeBase.IdentityByteCorners, array);
		Dictionary<IntVector, Cube> dictionary = new Dictionary<IntVector, Cube>();
		dictionary.Add(key, value);
		BytePacker bytePackerFromCubeDict = RuntimePrototypeCubeModel.GetBytePackerFromCubeDict(dictionary, addCount: true);
		Debug.Log((object)bytePackerFromCubeDict.Length);
		int num = -1;
		foreach (KeyValuePair<int, string> itemType in MVGameController.Instance.WOCM.PlayerRepository.ItemTypes)
		{
			if (itemType.Value == "CubeModel")
			{
				num = itemType.Key;
			}
		}
		if (num != -1)
		{
			MVGameController.Instance.WOCM.OnRequestedPrototypeCreated += InstanceOnRequestedPrototypeCreated;
			MVGameController.Instance.Game.RegisterLocalPrototype(name, num, bytePackerFromCubeDict.ToArray(), scale);
		}
	}

	public void OnAddPrototypeFromInventory(MVItem item)
	{
		RuntimePrototypeCubeModel unchangedPrototypeByItem = MVGameController.Instance.WOCM.WorldInventory.GetUnchangedPrototypeByItem(item);
		if (unchangedPrototypeByItem != null)
		{
			RequestInstance(unchangedPrototypeByItem.PrototypeId);
			return;
		}
		MVGameController.Instance.WOCM.OnRequestedPrototypeCreated += InstanceOnRequestedPrototypeCreated;
		MVGameController.Instance.Game.RegisterPrototype(item);
	}

	public void SetGameMsg()
	{
		if (MVGameController.Instance.WOCM.LocalPlayer.Avatar.AvatarController.AvatarState == AvatarState.Editing)
		{
			return;
		}
		switch (GameStateListener.CurrentGameState)
		{
		case MVGameStateType.PrepareRound:
			gameState.gameMsgs.Text = "New Game starting in: " + Mathf.Ceil((float)(GameStateListener.TimeLeftMS / 1000));
			break;
		case MVGameStateType.Round:
			gameState.gameMsgs.Text = string.Empty;
			break;
		case MVGameStateType.RoundEnded:
		{
			string text = string.Empty;
			if (GameStateListener.LastReason == MVGameStateReason.Timeout)
			{
				text = "ROUND ENDS - NO WINNER!";
			}
			else if (GameStateListener.LastReason == MVGameStateReason.FlagCaptured)
			{
				if (GameStateListener.LastInstigatorActorNr == MVGameController.Instance.WOCM.LocalPlayerActorNumber)
				{
					text = "You WON the Game!";
				}
				else
				{
					text = ((!MVGameController.Instance.WOCM.Players.ContainsKey(GameStateListener.LastInstigatorActorNr)) ? "Game won by...unknown" : ("Game won by " + MVGameController.Instance.WOCM.Players[GameStateListener.LastInstigatorActorNr].Username));
				}
			}
			gameState.gameMsgs.Text = text;
			break;
		}
		}
	}

	public void RequestInstance(int prototypeId, GameObject originalInstance = null)
	{
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		Debug.Log((object)"RequestInstance");
		int parentGroup = esm.ParentGroup;
		Hashtable hashtable = new Hashtable();
		hashtable["protoTypeID"] = prototypeId;
		MVGameController.Instance.WOCM.OnWorldObjectRegisterResponse += Instance_OnWorldObjectRegisterResponse;
		float scale = MVGameController.Instance.WOCM.WorldInventory.RuntimePrototypes[prototypeId].Scale;
		if ((Object)(object)originalInstance == (Object)null)
		{
			Debug.Log((object)"Make unique?");
			Debug.LogWarning((object)"group register wo not implemented for request instance");
			esm.Event = EditorEvent.ESWaitForSelect;
			MVGameController.Instance.Game.RegisterWorldObject(WorldObjectType.CubeModel, parentGroup, hashtable, new Hashtable(), Vector3.zero, Quaternion.identity, Vector3.one * scale, localOwner: true, transferOwnershipToServerOnLeave: true);
		}
		else
		{
			Debug.Log((object)("Clone dataPrototypeId " + prototypeId));
			Debug.LogWarning((object)"group register wo not implemented for request instance");
			esm.Event = EditorEvent.ESWaitForClone;
			MVGameController.Instance.Game.RegisterWorldObject(WorldObjectType.CubeModel, parentGroup, hashtable, new Hashtable(), originalInstance.transform.localPosition, originalInstance.transform.localRotation, Vector3.one * scale, localOwner: false, transferOwnershipToServerOnLeave: true);
		}
	}

	public void LockSelected()
	{
		if (esm.SingleSelectedWO != null)
		{
			MVGameController.Instance.Game.LockHierarchy(esm.SingleSelectedWO.Id, lockToggle);
			lockToggle = !lockToggle;
		}
	}

	public void GroupSelected()
	{
		if (esm.CurEvent != EditorEvent.ESWaitForGroup)
		{
			esm.PushState(EditorEvent.ESWaitForGroup);
		}
	}

	public void UngroupSelected()
	{
		if (esm.CurEvent != EditorEvent.ESWaitForUngroup)
		{
			esm.PushState(EditorEvent.ESWaitForUngroup);
		}
	}

	public void ToggleIgnoreKeyInput()
	{
		MVInputWrapper.ignoreAllKeys = !MVInputWrapper.ignoreAllKeys;
		MVGameController.Instance.WOCM.WeCamera.IgnoreInput = !MVGameController.Instance.WOCM.WeCamera.IgnoreInput;
	}

	public bool IsDrawPlaneActive()
	{
		return worldEditorDrawPlaneStack.Count > 0 && worldEditorDrawPlaneStack.Peek().Active;
	}

	public void ToggleGrid()
	{
		esm.GridMode = !esm.GridMode;
	}

	public void ToggleWorkPlane()
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		if (worldEditorDrawPlaneStack.Count > 0)
		{
			if (worldEditorDrawPlaneStack.Count == 1)
			{
				worldEditorDrawPlaneStack.Peek().SetToGridAlignedPos(MVGameController.Instance.WOCM.WoAvatar.GameObject.transform.position - 2f * Vector3.up);
			}
			worldEditorDrawPlaneStack.Peek().Active = !worldEditorDrawPlaneStack.Peek().Active;
		}
	}

	public void ToggleLogicRendering()
	{
		Debug.Log((object)"ToggleLogicRendering");
		Camera camera = ((Component)MVGameController.Instance.WOCM.WeCamera).camera;
		if ((Object)(object)camera != (Object)null)
		{
			if (IsLogicRendered())
			{
				camera.cullingMask -= 1 << (LayerMask.NameToLayer("Logic") & 0x1F);
			}
			else
			{
				camera.cullingMask |= 1 << (LayerMask.NameToLayer("Logic") & 0x1F);
			}
			if (OnToggleLogicRendering != null)
			{
				OnToggleLogicRendering(IsLogicRendered());
			}
		}
	}

	public void SetLogicRendering(bool active)
	{
		if (active != IsLogicRendered())
		{
			ToggleLogicRendering();
		}
	}

	public bool IsLogicRendered()
	{
		Camera camera = ((Component)MVGameController.Instance.WOCM.WeCamera).camera;
		if ((Object)(object)camera != (Object)null)
		{
			return (camera.cullingMask & (1 << LayerMask.NameToLayer("Logic"))) != 0;
		}
		return false;
	}

	public void AddSoundEmitter()
	{
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		esm.Event = EditorEvent.ESWaitForSelect;
		Hashtable hashtable = new Hashtable();
		hashtable["active"] = true;
		hashtable["soundCue"] = 1;
		hashtable["range"] = 10f;
		hashtable["volume"] = 0.75f;
		hashtable["pitch"] = 1f;
		hashtable["once"] = false;
		hashtable["randomness"] = 0f;
		hashtable["doppler"] = 1f;
		MVGameController.Instance.WOCM.OnWorldObjectRegisterResponse += Instance_OnWorldObjectRegisterResponse;
		MVGameController.Instance.Game.RegisterWorldObject(WorldObjectType.SoundEmitter, esm.ParentGroup, hashtable, new Hashtable(), Vector3.zero, Quaternion.identity, Vector3.one, localOwner: false, transferOwnershipToServerOnLeave: true);
	}

	public bool Delete()
	{
		List<MVWorldObjectClient> list = new List<MVWorldObjectClient>(esm.SelectedWOs);
		int num = 0;
		foreach (MVWorldObjectClient selectedWO in esm.SelectedWOs)
		{
			num += MVGameController.Instance.WOCM.CountType(selectedWO.Id, typeof(MVSpawnPoint));
		}
		if (num >= MVGameController.Instance.WOCM.GetWorldObjectsByType(WorldObjectType.SpawnPoint).Count)
		{
			MVGameController.Instance.guiManager.ShowMessageBox("You cannot delete the last spawn-point.\nAll Planets must have at least one");
			return false;
		}
		foreach (MVWorldObjectClient item in list)
		{
			if (item.Delete())
			{
				MVGameController.Instance.Game.UnregisterWorldObject(item.Id);
			}
		}
		esm.DeSelect();
		return true;
	}

	public void DeleteFromContextMenu()
	{
		MVWorldObjectClient contextMenuSelectionWO = GetContextMenuSelectionWO();
		MVGameController.Instance.Game.UnregisterWorldObject(contextMenuSelectionWO.Id);
		if (contextMenuSelectionWO != null)
		{
			contextMenuSelectionWO.GameObject.SetActiveRecursively(false);
		}
		esm.DeSelect();
	}

	public void DebugAddCubeToSelected(IntVector pos)
	{
		if (esm.SingleSelectedWO is MVCubeModelBase)
		{
			((MVCubeModelBase)esm.SingleSelectedWO).AddCube(pos, new Cube(CubeDataPacker.CornersToByteArray(CubeBase.IdentityCorners), Cube.CreateMaterialArray(0)));
			Debug.Log((object)("Added cubes to " + ((Object)esm.SingleSelectedWO.GameObject).name));
		}
	}

	public void HandleDebugAddCubeToSelected()
	{
		if (esm.SingleSelectedWO is MVCubeModelBase)
		{
			((MVCubeModelBase)esm.SingleSelectedWO).HandleDelta();
			Debug.Log((object)("Added cubes to " + ((Object)esm.SingleSelectedWO.GameObject).name));
		}
	}

	private void Instance_OnWorldObjectRegisterResponse(object sender, OnWorldObjectRegisterResponseEventArgs e)
	{
		MVGameController.Instance.WOCM.OnWorldObjectRegisterResponse -= Instance_OnWorldObjectRegisterResponse;
		esm.SelectNewRegisteredObject(MVGameController.Instance.WOCM.GetWorldObjectClient(e.worldObjectID));
	}

	private void Instance_OnWorldObjectGroupRegisterResponse(object sender, OnWorldObjectRegisterResponseEventArgs e)
	{
		Debug.Log((object)("selected counter " + esm.Selected.Count));
		MVGameController.Instance.WOCM.OnWorldObjectRegisterResponse -= Instance_OnWorldObjectGroupRegisterResponse;
		esm.SelectWo(e.worldObjectID, addToSelection: true);
		Debug.Log((object)("selected counter " + esm.Selected.Count));
	}

	private void Instance_OnWorldObjectBlueprintRegisterResponse(object sender, OnWorldObjectRegisterResponseEventArgs e)
	{
		MVGameController.Instance.WOCM.OnWorldObjectRegisterResponse -= Instance_OnWorldObjectBlueprintRegisterResponse;
		esm.SelectNewRegisteredObject(MVGameController.Instance.WOCM.GetWorldObjectClient(e.worldObjectID));
		Debug.Log((object)("selected counter " + esm.Selected.Count));
	}

	private void RequestInstanceMakeUnique(int prototypeId)
	{
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		Hashtable hashtable = new Hashtable();
		hashtable["protoTypeID"] = prototypeId;
		float scale = MVGameController.Instance.WOCM.WorldInventory.Prototypes[prototypeId].Scale;
		MVGameController.Instance.WOCM.OnWorldObjectRegisterResponse += Instance_OnWorldObjectRegisterResponse;
		Debug.LogWarning((object)"Remember to implement group stuff for RequestInstanceMakeUnique");
		MVGameController.Instance.Game.RegisterWorldObject(WorldObjectType.CubeModel, MVGameController.Instance.WOCM.RootGroup.Id, hashtable, new Hashtable(), Vector3.zero, Quaternion.identity, Vector3.one * scale, localOwner: true, transferOwnershipToServerOnLeave: true);
	}

	private void InstanceOnRequestedPrototypeCreated(object sender, OnRequestedPrototypeCreatedEventArgs e)
	{
		MVGameController.Instance.WOCM.OnRequestedPrototypeCreated -= InstanceOnRequestedPrototypeCreated;
		RequestInstance(e.prototypeId);
	}

	private void InstanceOnRequestedMakeUniquePrototypeCreated(object sender, OnRequestedPrototypeCreatedEventArgs e)
	{
		MVGameController.Instance.WOCM.OnRequestedPrototypeCreated -= InstanceOnRequestedPrototypeCreated;
		RequestInstanceMakeUnique(e.prototypeId);
	}

	public void AddToInventory(MVWorldObjectClient wo)
	{
		if (wo.WorldObjectType != WorldObjectType.CubeModel)
		{
			throw new Exception($"Could not add object of type {wo.WorldObjectType} to inventory.");
		}
		RuntimePrototypeCubeModel runtimePrototypeCubeModel = MVGameController.Instance.WOCM.WorldInventory.RuntimePrototypes[(wo as MVCubeModelBase).Pid];
		logger.Log("Add to inventory");
		Coroutines.StartCoroutine(CreateTextureFromData(runtimePrototypeCubeModel.PrototypeId));
	}

	protected IEnumerator CreateTextureFromData(int prototypeId)
	{
		int width = 512;
		int height = 512;
		previewTexture = new Texture2D(width, height, (TextureFormat)3, false);
		GameObject previewCamObject = new GameObject();
		previewCamObject.layer = LayerMask.NameToLayer("Preview");
		Camera previewCam = previewCamObject.AddComponent<Camera>();
		previewCam.clearFlags = (CameraClearFlags)1;
		previewCam.backgroundColor = new Color(0f, 0f, 0f, 0f);
		previewCam.fieldOfView = 35f;
		previewCam.aspect = 1f;
		previewCam.cullingMask = 1 << LayerMask.NameToLayer("Preview");
		RenderTexture previewRenderTexture = (previewCam.targetTexture = new RenderTexture(width, height, 24));
		GameObject previewItem = MVGameController.Instance.WOCM.WorldInventory.RuntimePrototypes[prototypeId].GetMesh();
		HelperFunctions.SetLayerRecursively(previewItem.transform, "Preview");
		Bounds? bs = SharedCubeFunctions.GetAxisAlignedBoundsRecursively(previewItem.transform);
		Bounds b = new Bounds(Vector3.zero, Vector3.one);
		if (bs.HasValue)
		{
			b = bs.Value;
		}
		else
		{
			Debug.Log((object)"Failed to find bounds!");
		}
		float max = Mathf.Max(b.size.x, Mathf.Max(b.size.y, b.size.z));
		float scale = 1f / max;
		Debug.Log((object)("max: " + max + ", scale: " + scale));
		previewItem.transform.localScale = new Vector3(scale, scale, scale);
		Bounds? bsScaled = SharedCubeFunctions.GetAxisAlignedBoundsRecursively(previewItem.transform);
		Bounds bScaled = new Bounds(Vector3.zero, Vector3.one);
		if (bsScaled.HasValue)
		{
			bScaled = bsScaled.Value;
		}
		else
		{
			Debug.Log((object)"Failed to find bounds!");
		}
		float centerX = bScaled.center.x;
		float centerY = bScaled.center.y;
		float centerZ = bScaled.center.z;
		((Component)previewCam).transform.position = new Vector3(centerX + cameraOffset.x, centerY + previewCamAdditionalHeight * scale + cameraOffset.y, centerZ - previewCamDist + cameraOffset.z);
		Debug.Log((object)("PreviewCam pos: " + ((Component)previewCam).transform.position));
		Debug.Log((object)("PreviewItem pos: " + previewItem.transform.position));
		Vector3 lookAt = bScaled.center;
		((Component)previewCam).transform.LookAt(lookAt);
		yield return (object)new WaitForEndOfFrame();
		RenderTexture.active = previewRenderTexture;
		previewTexture.ReadPixels(new Rect(0f, 0f, (float)width, (float)height), 0, 0);
		previewTexture.Apply();
		Object.Destroy((Object)(object)previewCamObject);
		Object.Destroy((Object)(object)previewItem);
		OnPreviewTextureWritten();
	}

	private void OnPreviewTextureWritten()
	{
		int prototypeId = MVGameController.Instance.EditorController.GetContextMenuSelectionPrototype().PrototypeId;
		byte[] array = previewTexture.EncodeToPNG();
		MVGameController.Instance.Game.AddPrototypeToInventory(prototypeId, -1, (byte[])array.Clone());
	}
}
