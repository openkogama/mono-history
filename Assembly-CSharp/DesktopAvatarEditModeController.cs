using System;
using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;

public class DesktopAvatarEditModeController : ModeControllerBase, ISetEditState, IEventSystemHandler, IAvatarEditUIState, IAvatarSetBodyGroup, IGetCurrentBody, IActivateUIElement
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

	private int firstTimeActiveAvatar = -1;

	private void Awake()
	{
		MVNetworkGame game = MVGameControllerBase.Game;
		game.OnActiveAvatar = (Action<int>)Delegate.Combine(game.OnActiveAvatar, new Action<int>(FirstTimeSetActiveAvatar));
		uiStack.Push(stackBottom);
		chatController = UnityEngine.Object.Instantiate(chatController);
		chatController.transform.SetParent(stackBottom.transform, worldPositionStays: false);
		chatController.SubscribeToMessages();
		chatController.Initialize();
		stackBottom.SetActive(value: true);
		MVGameControllerDesktop.RegisterAvaterEditModeController(this);
	}

	private void FirstTimeSetActiveAvatar(int activeAvatarId)
	{
		Debug.Log("FirstTimeSetActiveAvatar " + activeAvatarId);
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
		MVInputWrapper.SetInputMap(new DesktopPlayMode());
		MVGameControllerBase.CameraController.IsLogicRendered = false;
		InitializeLocalAvatar();
		drawPlaneController.Initialize();
		DrawPlane.Initialize(drawPlaneController);
		accessoryShopController.Initialize();
		avatarSelectionController = UnityEngine.Object.Instantiate(avatarSelectionController);
		avatarSelectionController.gameObject.transform.SetParent(stackBottom.transform, worldPositionStays: false);
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
		avatarSelectionController.Initialize(avatarEditModeBodyController);
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
		case ActivateUIElement.Materials:
			break;
		}
	}

	public void Set(ActiveEditStateUI activeUIElements)
	{
		if ((activeUIElements & ActiveEditStateUI.CubeModelTools) > ActiveEditStateUI.None)
		{
			Debug.Log("Activate cube model gui");
			materialsController.SetActive();
			materialsController.Push(UIPushOption.HideAll, OnPopCubeModelingController);
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

	public void SetBodyGroup(MVBody bodyGroup)
	{
		editorStateMachine.EnterGroup(bodyGroup);
	}

	public void GetCurrentBody(Action<MVBody> callback)
	{
		callback(avatarEditModeBodyController.CurrentBody);
	}
}
