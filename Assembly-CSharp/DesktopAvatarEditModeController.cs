using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class DesktopAvatarEditModeController : ModeControllerBase, IActivateUIElement, IAvatarEditUIState, ISetEditState, IAvatarSetBodyGroup, IGetCurrentBody, IEventSystemHandler
{
	private EditorStateMachine editorStateMachine;

	[SerializeField]
	private AvatarEditModeBodyController avatarEditModeBodyController;

	[SerializeField]
	private AvatarSelectionController avatarSelectionController;

	[SerializeField]
	private UIStack uiStack;

	[SerializeField]
	private GameObject stackBottom;

	[SerializeField]
	private ChatControllerUGUI chatController;

	[SerializeField]
	private AccessoryShopController accessoryShopController;

	[SerializeField]
	private DrawPlaneControllerUUI drawPlaneController;

	[SerializeField]
	private MaterialsController materialsController;

	[SerializeField]
	private AvatarShopController avatarShopController;

	[SerializeField]
	private NotificationsManager notificationsManager;

	[SerializeField]
	private SetupCubeModelTutorialUI setupCubeModelTutorialUI;

	private int firstTimeActiveAvatar = -1;

	private void Awake()
	{
		MVInputWrapper.SetInputMap(new DesktopPlayMode());
		MVNetworkGame game = MVGameControllerBase.Game;
		game.OnActiveAvatar = (Action<int>)Delegate.Combine(game.OnActiveAvatar, new Action<int>(FirstTimeSetActiveAvatar));
		uiStack.Push(stackBottom, UIPushOption.None, null, UIGroupFlags.StackBottom);
		chatController = UnityEngine.Object.Instantiate(chatController);
		chatController.transform.SetParent(stackBottom.transform, worldPositionStays: false);
		chatController.SubscribeToMessages();
		chatController.Initialize();
		stackBottom.SetActive(value: true);
		avatarSelectionController = UnityEngine.Object.Instantiate(avatarSelectionController);
		uiStack.Push(avatarSelectionController.gameObject, UIPushOption.None, null, UIGroupFlags.MainUI);
		MVGameControllerDesktop.RegisterAvaterEditModeController(this);
		if (MVGameControllerBase.Game.LocalPlayer.IsReady)
		{
			SetUIReady();
		}
		else
		{
			MVGameControllerBase.OnFirstFrameUpdateActorReady = (Action)Delegate.Combine(MVGameControllerBase.OnFirstFrameUpdateActorReady, new Action(SetUIReady));
		}
	}

	private void FirstTimeSetActiveAvatar(int activeAvatarId)
	{
		firstTimeActiveAvatar = activeAvatarId;
	}

	private void Update()
	{
		HandleFpsShortcut();
		if (editorStateMachine != null)
		{
			editorStateMachine.Update();
		}
	}

	public override void Initialize()
	{
		base.Initialize();
		avatarShopController.Initialize(avatarEditModeBodyController);
		MVGameControllerBase.CameraController.IsLogicRendered = false;
		InitializeLocalAvatar();
		drawPlaneController.Initialize();
		DrawPlane.Initialize(drawPlaneController);
		accessoryShopController.Initialize();
		notificationsManager = UnityEngine.Object.Instantiate(notificationsManager);
		notificationsManager.transform.SetParent(stackBottom.transform, worldPositionStays: false);
		avatarEditModeBodyController.Initialize();
		editorStateMachine = new EditorStateMachine(gameObject, avatarEditModeBodyController.DisplayPos);
		materialsController.Initialize(editorStateMachine.CubeModelingStateMachine);
		if (firstTimeActiveAvatar != -1)
		{
			avatarEditModeBodyController.SetCurrentBodyByWoId(firstTimeActiveAvatar);
		}
		editorStateMachine.EnterGroup(avatarEditModeBodyController.CurrentBody);
		editorStateMachine.Event = EditorEvent.CERoamUUI;
		editorStateMachine.CubeModelingStateMachine.CurrentMaterialId = 21;
		avatarSelectionController.Initialize(avatarEditModeBodyController, editorStateMachine);
		setupCubeModelTutorialUI.Initialize(editorStateMachine.CubeModelingStateMachine);
	}

	public void Activate(ActivateUIElement element)
	{
		switch (element)
		{
		case ActivateUIElement.AvatarAccessoryShop:
			accessoryShopController.Activate(UIPushOption.HideAll);
			break;
		case ActivateUIElement.AvatarShop:
			avatarShopController.Activate(UIPushOption.Blocking);
			break;
		}
	}

	public void Set(ActiveEditStateUI activeUIElements)
	{
		if ((activeUIElements & ActiveEditStateUI.CubeModelTools) > ActiveEditStateUI.None)
		{
			materialsController.SetActive();
			materialsController.Push(UIPushOption.HideAllExceptStackBottom, OnPopCubeModelingController);
		}
	}

	private void OnPopCubeModelingController()
	{
		SetState(EditorEvent.CERoamUUI);
	}

	public void SetState(EditorEvent editorEvent)
	{
		editorStateMachine.Event = editorEvent;
	}

	private void InitializeLocalAvatar()
	{
		MVSpawnPointBlue mVSpawnPointBlue = (MVSpawnPointBlue)MVGameControllerBase.WOCM.GetWorldObjectClientWhere((MVWorldObjectClient wo) => wo is MVSpawnPointBlue);
		MVAvatarLocal avatarLocal = MVGameControllerBase.WOCM.AvatarLocal;
		avatarLocal.WorldPosition = mVSpawnPointBlue.WorldPosition - Vector3.up;
		avatarLocal.WorldRotation = mVSpawnPointBlue.WorldRotation;
		avatarLocal.SetMode(AvatarRuntimeState.Edit);
	}

	public void SelectEditorStateMachineToBodyGroup()
	{
		avatarSelectionController.SetStateToRoam();
	}

	public void SetBodyGroup(MVBody bodyGroup)
	{
		editorStateMachine.EnterGroup(bodyGroup);
	}

	public void GetCurrentBody(Action<MVBody> callback)
	{
		callback(avatarEditModeBodyController.CurrentBody);
	}

	public void SetUIReady()
	{
		uiStack.SetStackReady();
		MVGameControllerBase.OnFirstFrameUpdateActorReady = (Action)Delegate.Remove(MVGameControllerBase.OnFirstFrameUpdateActorReady, new Action(SetUIReady));
	}
}
