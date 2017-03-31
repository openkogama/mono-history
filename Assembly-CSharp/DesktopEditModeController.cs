using System;
using System.Collections;
using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class DesktopEditModeController : ModeControllerBase, IEditModeUI, ISetEditState, IEventSystemHandler, IGridSnapHandler, IEditModeController
{
	private const float focusTimeInputSupressTimeOut = 5f;

	private bool isInPlayInEditMode;

	private Action<EditModeChangeArgs> editModeChange;

	private bool gridSnap;

	private DesktopPlayModeController desktopPlayModeController;

	private EditorStateMachine editorStateMachine;

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
	private DrawPlaneController2DUUI drawPlaneController2D;

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

	private float focusTime;

	private bool focusSuppressInput = true;

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
		uiStack.Push(stackBottom);
		MVGameControllerDesktop.RegisterEditModeController(this);
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
				MVInputWrapper.IsInputSuppressed = true;
			}
		}
	}

	private void Update()
	{
		HandleFpsShortcut();
		HandleFocusInputSupress();
		if (editorStateMachine != null)
		{
			editorStateMachine.Update();
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
		MVGameControllerBase.CameraController.IsLogicRendered = true;
		if (MVGameControllerBase.Game.GameType == MVGameType.Platformer)
		{
			drawPlaneController2D.Initialize();
			DrawPlane.Initialize(drawPlaneController2D);
			DrawPlane.SetToTerrain(active: false);
		}
		else
		{
			drawPlaneController.Initialize();
			DrawPlane.Initialize(drawPlaneController);
		}
		editorStateMachine = new EditorStateMachine(gameObject, contextMenuController, gizmoController);
		editorWorldObjectCreation.Initialize(editorStateMachine);
		materialsController.Initialize(editorStateMachine.CubeModelingStateMachine);
		materialsController.SetActive().SetParent(stackBottom.transform, worldPositionStays: false);
		editorStateMachine.CubeModelingStateMachine.CurrentMaterialId = 21;
		chatController.Initialize();
		contextMenuController.Initialize(editorStateMachine);
		gizmoController.Initialize(editorStateMachine);
		MVInputWrapper.SetInputMap(new DesktopPlayMode());
		editorStateMachine.Event = EditorEvent.ESTerrainEdit;
		clientShopController.Initialize(repositoryController);
		playerInventoryController.Initialize();
		desktopPlayModeController.Initialize();
		MVGameControllerBase.WOCM.AvatarLocal.Body.AccessoryMoveOverride = false;
		MVGameControllerBase.WOCM.RootGroup.PlayModeInitialize();
		notificationsManager = UnityEngine.Object.Instantiate(notificationsManager);
		notificationsManager.transform.SetParent(stackBottom.transform, worldPositionStays: false);
	}

	public void SetState(EditorEvent editorEvent)
	{
		if (editorEvent == EditorEvent.ESWalkMode)
		{
			editorStateMachine.ClearStateStack();
			isInPlayInEditMode = true;
			editorStateMachine.Event = EditorEvent.ESWalkMode;
			desktopPlayModeController.gameObject.SetActive(value: true);
			gameObject.SetActive(value: false);
			if (editModeChange != null)
			{
				editModeChange(new EditModeChangeArgs(state: true));
			}
		}
		else
		{
			editorStateMachine.Event = editorEvent;
		}
	}

	private IEnumerator HandleCursorVisible()
	{
		if (!Cursor.visible)
		{
			Cursor.visible = true;
			yield return null;
		}
	}

	private void LeaveEditPlayMode()
	{
		isInPlayInEditMode = false;
		editorStateMachine.Event = EditorEvent.ESTerrainEdit;
		desktopPlayModeController.gameObject.SetActive(value: false);
		gameObject.SetActive(value: true);
		StartCoroutine(HandleCursorVisible());
		if (editModeChange != null)
		{
			editModeChange(new EditModeChangeArgs(state: false));
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
		if (MVGameControllerBase.WOCM.AvatarLocal != null)
		{
			MVGameControllerBase.WOCM.AvatarLocal.Respawn();
		}
	}

	public void MoveToSelectedObject()
	{
		MVWorldObjectClient singleSelectedWO = editorStateMachine.SingleSelectedWO;
		if (singleSelectedWO != null)
		{
			MVGameControllerBase.CameraController.CurCamera.FocusOnObject(singleSelectedWO);
		}
	}

	public void DeleteWoid(int woid)
	{
		editorStateMachine.DeSelectAll();
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
}
