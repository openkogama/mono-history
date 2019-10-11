using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class DesktopEditModeController : ModeControllerBase, IEditStateCommands, IEditModeUI, IGridSnapHandler, IEditModeController, IEventSystemHandler
{
	private bool enterPlayModeOnceGuard;

	private bool enterBuildModeOnceGuard = true;

	private bool isInPlayInEditMode;

	private Action<EditModeChangeArgs> editModeChange;

	private bool gridSnap;

	private DesktopPlayModeController desktopPlayModeController;

	[SerializeField]
	private EditorWorldObjectCreation editorWorldObjectCreation;

	[SerializeField]
	private UIStack uiStack;

	[SerializeField]
	private GameObject stackBottom;

	[SerializeField]
	private ChatControllerUGUI chatController;

	[SerializeField]
	private DrawPlaneControllerUUI drawPlaneController;

	[SerializeField]
	private MaterialsControllerEditMode materialsController;

	[SerializeField]
	private ContextMenuController contextMenuController;

	[SerializeField]
	private GizmoController gizmoController;

	[SerializeField]
	private EditModeRepositoryController repositoryController;

	[SerializeField]
	private EditModeClientShopController clientShopController;

	[SerializeField]
	private PlayerInventoryController playerInventoryController;

	[SerializeField]
	private CreateCubeModelController createCubeModelController;

	[SerializeField]
	private RectTransform notificationsManager;

	[SerializeField]
	private ChatBubbleController chatBubbleController;

	[SerializeField]
	private FirstTimeSetupTerrainEditTutorial firstTimeSetupTerrainEditTutorial;

	[SerializeField]
	private SetupCubeModelTutorialUI setupCubeModelTutorialUI;

	[SerializeField]
	private GoldPurchasedTracker goldPurchasedTracker;

	private float focusTime;

	private bool focusSuppressInput = true;

	private const float focusTimeInputSupressTimeOut = 5f;

	public EditorStateMachine EditModeStateMachine { get; set; }

	public bool IsInPlayInEditMode => isInPlayInEditMode;

	public ClientShopRepository ClientShopRepository { get; set; }

	public PlayerInventoryRepository PlayerInventoryRepository { get; set; }

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

	private void Awake()
	{
		chatController = UnityEngine.Object.Instantiate(chatController);
		chatController.transform.SetParent(stackBottom.transform, worldPositionStays: false);
		chatController.SubscribeToMessages();
		uiStack.Push(stackBottom, UIPushOption.None, null, UIGroupFlags.StackBottom);
		MVGameControllerDesktop.RegisterEditModeController(this);
		if (MVGameControllerBase.Game.LocalPlayer.IsReady)
		{
			SetUIReady();
		}
		else
		{
			MVGameControllerBase.OnFirstFrameUpdateActorReady = (Action)Delegate.Combine(MVGameControllerBase.OnFirstFrameUpdateActorReady, new Action(SetUIReady));
		}
	}

	protected void OnDestroy()
	{
		MVGameControllerDesktop.UnregisterEditModeController();
	}

	private void Start()
	{
		RegisterShortcuts();
	}

	private void HandleFocusInputSupress()
	{
		if (focusSuppressInput)
		{
			bool flag = Input.GetKeyUp(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Mouse1);
			bool flag2 = Time.realtimeSinceStartup - focusTime > 5f;
			if (flag || flag2)
			{
				focusSuppressInput = false;
			}
			else
			{
				MVInputWrapper.SuppressAllInput();
			}
		}
	}

	private void Update()
	{
		HandleFpsShortcut();
		HandleFocusInputSupress();
		if (EditModeStateMachine != null)
		{
			EditModeStateMachine.Update();
		}
		if (MVInputWrapper.GetBooleanControlDown(KogamaControls.ToggleHD) && uiStack.IsStackEmpty())
		{
			ToggleHD();
		}
	}

	private void OnApplicationFocus(bool focus)
	{
		if (focus)
		{
			focusSuppressInput = true;
			focusTime = Time.realtimeSinceStartup;
		}
	}

	private void ToggleHD()
	{
		int currentLevel = ((MVQualitySettings.CurrentLevel == 0) ? 1 : 0);
		MVQualitySettings.CurrentLevel = currentLevel;
	}

	public void RegisterPlayModeController(DesktopPlayModeController desktopPlayModeController)
	{
		this.desktopPlayModeController = desktopPlayModeController;
		desktopPlayModeController.gameObject.SetActive(value: false);
		desktopPlayModeController.OnLeaveEditPlayMode = (UnityAction)Delegate.Combine(desktopPlayModeController.OnLeaveEditPlayMode, new UnityAction(LeaveEditPlayMode));
	}

	public override void Initialize()
	{
		base.Initialize();
		MVGameControllerBase.MainCameraManager.IsLogicRendered = true;
		drawPlaneController.Initialize();
		DrawPlane.Initialize(drawPlaneController);
		chatBubbleController = UnityEngine.Object.Instantiate(chatBubbleController);
		chatBubbleController.transform.SetParent(transform, worldPositionStays: false);
		EditModeStateMachine = new EditorStateMachine(gameObject, contextMenuController, gizmoController);
		editorWorldObjectCreation.Initialize(EditModeStateMachine);
		materialsController.Initialize(EditModeStateMachine.CubeModelingStateMachine);
		uiStack.Push(materialsController.SetActive().gameObject, UIPushOption.None, null, UIGroupFlags.MainUI);
		EditModeStateMachine.CubeModelingStateMachine.CurrentMaterialId = 21;
		chatController.Initialize();
		contextMenuController.Initialize(EditModeStateMachine);
		gizmoController.Initialize(EditModeStateMachine);
		MVInputWrapper.SetInputMap(new DesktopPlayMode());
		EditModeStateMachine.Event = EditorEvent.ESTerrainEdit;
		clientShopController.Initialize(repositoryController);
		playerInventoryController.Initialize();
		desktopPlayModeController.Initialize();
		MVGameControllerBase.WOCM.RootGroup.PlayModeInitialize();
		goldPurchasedTracker.Initialize();
		notificationsManager = UnityEngine.Object.Instantiate(notificationsManager);
		notificationsManager.transform.SetParent(stackBottom.transform, worldPositionStays: false);
		firstTimeSetupTerrainEditTutorial.Initialize(EditModeStateMachine.CubeModelingStateMachine, materialsController);
		setupCubeModelTutorialUI.Initialize(EditModeStateMachine.CubeModelingStateMachine);
		ChatCommandManager.UpdateChatCommandCallback(ChatCommand.HideAllUI, (Action)Delegate.Combine(ChatCommandManager.GetChatCommandCallback(ChatCommand.HideAllUI), new Action(HideUI)));
	}

	private void HideUI()
	{
		gameObject.SetActive(value: false);
	}

	public void DisableEditMode()
	{
		isInPlayInEditMode = true;
		desktopPlayModeController.gameObject.SetActive(value: true);
		gameObject.SetActive(value: false);
		if (editModeChange != null)
		{
			editModeChange(new EditModeChangeArgs(state: true));
		}
		enterBuildModeOnceGuard = false;
	}

	public void EnterPlayMode()
	{
		if (!enterPlayModeOnceGuard)
		{
			ClearStateStack();
			SetState(EditorEvent.ESWaitForPlayModeAvatar);
			enterPlayModeOnceGuard = true;
		}
	}

	private void LeaveEditPlayMode()
	{
		if (!enterBuildModeOnceGuard)
		{
			desktopPlayModeController.gameObject.SetActive(value: false);
			gameObject.SetActive(value: true);
			EditModeStateMachine.Event = EditorEvent.ESWaitForBuildModeAvatar;
			enterBuildModeOnceGuard = true;
		}
	}

	public void EnterBuildMode()
	{
		isInPlayInEditMode = false;
		StartCoroutine(HandleCursorVisible());
		if (editModeChange != null)
		{
			editModeChange(new EditModeChangeArgs(state: false));
		}
		enterPlayModeOnceGuard = false;
	}

	public void SetState(EditorEvent editorEvent)
	{
		EditModeStateMachine.Event = editorEvent;
	}

	public void ClearStateStack()
	{
		EditModeStateMachine.ClearStateStack();
	}

	private IEnumerator HandleCursorVisible()
	{
		if (!Cursor.visible)
		{
			Cursor.visible = true;
			yield return null;
		}
	}

	public bool IsGridSnap()
	{
		return gridSnap;
	}

	public void Set(bool snap)
	{
		gridSnap = snap;
	}

	private void RegisterShortcuts()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IShortcutKeyRegister x, BaseEventData y) =>
		{
			x.RegisterShortcutKey(KogamaControls.Respawn, KeyState.Up, Respawn);
		});
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IShortcutKeyRegister x, BaseEventData y) =>
		{
			x.RegisterShortcutKey(KogamaControls.FocusOnSelectedModel, KeyState.Up, MoveToSelectedObject);
		});
	}

	public void Respawn()
	{
		MVGameControllerBase.GameEventManager.AvatarCommandsPlayMode.KillSelf();
	}

	public void MoveToSelectedObject()
	{
		MVWorldObjectClient singleSelectedWO = EditModeStateMachine.SingleSelectedWO;
		if (singleSelectedWO != null)
		{
			MVGameControllerBase.MainCameraManager.CurrentCamera.FocusOnObject(singleSelectedWO);
		}
	}

	public void DeleteWoid(int woid)
	{
		EditModeStateMachine.DeSelectAll();
		string errorText = string.Empty;
		if (!MVGameControllerBase.WOCM.GetWorldObjectClient(woid).Delete(MVGameControllerBase.WOCM, ref errorText))
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
			{
				x.CreateErrorNotificationPopup(errorText);
			});
		}
		else
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack handler, BaseEventData data) =>
			{
				handler.Pop();
			});
		}
	}

	public void SetUIReady()
	{
		uiStack.SetStackReady();
		MVGameControllerBase.OnFirstFrameUpdateActorReady = (Action)Delegate.Remove(MVGameControllerBase.OnFirstFrameUpdateActorReady, new Action(SetUIReady));
	}
}
