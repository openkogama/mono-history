using System;
using UnityEngine;

internal class ESSelectionMode : ESStateBase
{
	private bool canLeave;

	private bool canMove;

	private Vector3 mousePosInitPossibleMove = Vector3.zero;

	private Vector3 selectObjectHitPositionWorld;

	private Collider selectObjectCollider;

	private LinkObjectScript rightClickedLink;

	private MVGUISelectionGizmo selectionGizmo;

	private bool gizmoRotate;

	private bool gizmoTranslateXZ;

	private bool gizmoTranslateY;

	private bool gizmoOpen;

	private int downWorldObjectID = -1;

	private EditorStateMachine editorStateMachine;

	private WorldObjectWithClone selectedWorldObjectWithClone;

	private WorldObjectWithAddToInventory selectedWorldObjectWithAddToInventory;

	private WorldObjectWithLogicReset selectedWorldObjectWithLogicReset;

	private ILogger logger = LoggerManager.Instance.GetLogger(typeof(ESSelectionMode));

	public ESSelectionMode()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		InitializeSelectionGizmo();
	}

	private void InitializeSelectionGizmo()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		Object val = Object.Instantiate(Resources.Load("Prefabs/GUI/SelectionGizmo"));
		GameObject val2 = (GameObject)(object)((val is GameObject) ? val : null);
		val2.transform.localPosition = Vector3.zero;
		selectionGizmo = val2.GetComponent<MVGUISelectionGizmo>();
		MVGUISelectionGizmo mVGUISelectionGizmo = selectionGizmo;
		mVGUISelectionGizmo.OnRotate = (MVGUISelectionGizmo.GizmoClickDelegate)Delegate.Combine(mVGUISelectionGizmo.OnRotate, (MVGUISelectionGizmo.GizmoClickDelegate)(() =>
		{
			gizmoRotate = true;
		}));
		MVGUISelectionGizmo mVGUISelectionGizmo2 = selectionGizmo;
		mVGUISelectionGizmo2.OnDelete = (MVGUISelectionGizmo.GizmoClickDelegate)Delegate.Combine(mVGUISelectionGizmo2.OnDelete, (MVGUISelectionGizmo.GizmoClickDelegate)(() =>
		{
			MVGameController.Instance.EditorController.Delete();
			selectionGizmo.Visible = false;
		}));
		MVGUISelectionGizmo mVGUISelectionGizmo3 = selectionGizmo;
		mVGUISelectionGizmo3.OnXZtranslate = (MVGUISelectionGizmo.GizmoClickDelegate)Delegate.Combine(mVGUISelectionGizmo3.OnXZtranslate, (MVGUISelectionGizmo.GizmoClickDelegate)(() =>
		{
			gizmoTranslateXZ = true;
		}));
		MVGUISelectionGizmo mVGUISelectionGizmo4 = selectionGizmo;
		mVGUISelectionGizmo4.OnYtranslate = (MVGUISelectionGizmo.GizmoClickDelegate)Delegate.Combine(mVGUISelectionGizmo4.OnYtranslate, (MVGUISelectionGizmo.GizmoClickDelegate)(() =>
		{
			gizmoTranslateY = true;
		}));
		MVGUISelectionGizmo mVGUISelectionGizmo5 = selectionGizmo;
		mVGUISelectionGizmo5.OnOpen = (MVGUISelectionGizmo.GizmoClickDelegate)Delegate.Combine(mVGUISelectionGizmo5.OnOpen, (MVGUISelectionGizmo.GizmoClickDelegate)(() =>
		{
			gizmoOpen = true;
		}));
		MVGUISelectionGizmo mVGUISelectionGizmo6 = selectionGizmo;
		mVGUISelectionGizmo6.OnLogicReset = (MVGUISelectionGizmo.GizmoClickDelegate)Delegate.Combine(mVGUISelectionGizmo6.OnLogicReset, (MVGUISelectionGizmo.GizmoClickDelegate)(() =>
		{
			if (selectedWorldObjectWithLogicReset != null)
			{
				selectedWorldObjectWithLogicReset.Reset();
				selectionGizmo.Visible = false;
			}
		}));
		MVGUISelectionGizmo mVGUISelectionGizmo7 = selectionGizmo;
		mVGUISelectionGizmo7.OnClone = (MVGUISelectionGizmo.GizmoClickDelegate)Delegate.Combine(mVGUISelectionGizmo7.OnClone, (MVGUISelectionGizmo.GizmoClickDelegate)(() =>
		{
			if (selectedWorldObjectWithClone != null)
			{
				selectedWorldObjectWithClone.Clone();
				selectionGizmo.Visible = false;
			}
		}));
		MVGUISelectionGizmo mVGUISelectionGizmo8 = selectionGizmo;
		mVGUISelectionGizmo8.OnAddToInventory = (MVGUISelectionGizmo.GizmoClickDelegate)Delegate.Combine(mVGUISelectionGizmo8.OnAddToInventory, (MVGUISelectionGizmo.GizmoClickDelegate)(() =>
		{
			if (selectedWorldObjectWithAddToInventory != null)
			{
				selectedWorldObjectWithAddToInventory.AddToInventory();
				selectionGizmo.Visible = false;
			}
		}));
		MVGUISelectionGizmo mVGUISelectionGizmo9 = selectionGizmo;
		mVGUISelectionGizmo9.OnEdit = (MVGUISelectionGizmo.GizmoClickDelegate)Delegate.Combine(mVGUISelectionGizmo9.OnEdit, (MVGUISelectionGizmo.GizmoClickDelegate)(() =>
		{
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			if (MVGameController.Instance.guiManager.ShowContextMenu(MVGameController.Instance.WOCM.WorldObjects[downWorldObjectID].WorldObjectType, Input.mousePosition))
			{
				editorStateMachine.SelectWo(downWorldObjectID, addToSelection: false);
				editorStateMachine.Event = EditorEvent.ESContextMenu;
			}
		}));
	}

	public override void Enter(EditorStateMachine e)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		logger.Log("Entered");
		canLeave = false;
		mousePosInitPossibleMove = Input.mousePosition;
		canMove = true;
		editorStateMachine = e;
		DetectGizmoPosition(e);
	}

	public override void Execute(EditorStateMachine e)
	{
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		base.Execute(e);
		if (gizmoTranslateXZ)
		{
			gizmoTranslateXZ = false;
			gizmoRotate = false;
			gizmoTranslateY = false;
			e.PushState(EditorEvent.ESTranslate);
			return;
		}
		if (gizmoRotate)
		{
			gizmoRotate = false;
			gizmoTranslateY = false;
			e.PushState(EditorEvent.Rotating);
			return;
		}
		if (gizmoTranslateY)
		{
			gizmoTranslateY = false;
			e.Data.Add("yOnlyTranslate", null);
			e.PushState(EditorEvent.ESTranslate);
			return;
		}
		if (gizmoOpen)
		{
			EnterObject(editorStateMachine);
			gizmoOpen = false;
			return;
		}
		if (e.ParentGroup == MVGameController.Instance.WOCM.RootGroup.Id)
		{
			e.WeCamera.SecondaryCameraActive = false;
		}
		else
		{
			e.WeCamera.SecondaryCameraActive = true;
		}
		if (HandleEscape(e))
		{
			return;
		}
		TintObjectsOnMouseOver(e);
		if (MVInputWrapper.GetKeyUp((KeyCode)323))
		{
			canLeave = true;
		}
		if (HandleSelect(e) || HandleMove(e))
		{
			return;
		}
		if (MVInputWrapper.GetKeyUp((KeyCode)323))
		{
			MVWorldObjectClient mVWorldObjectClient = MVGameController.Instance.WOCM.WorldObjects[downWorldObjectID];
			logger.Log($"Selected World Object Type: {mVWorldObjectClient.GetType().Name}");
			if (!mVWorldObjectClient.OnClickHandler(e, selectObjectCollider))
			{
				selectionGizmo.WorldPosition = selectObjectHitPositionWorld;
				if (!selectionGizmo.Visible)
				{
					selectionGizmo.ShowEditButton = mVWorldObjectClient is WorldObjectWithEdit;
					selectionGizmo.ShowSettingButton = mVWorldObjectClient is WorldObjectWithSettings;
					selectionGizmo.ShowLogicResetButton = mVWorldObjectClient is WorldObjectWithLogicReset;
					selectionGizmo.ShowAddToInventoryButton = mVWorldObjectClient is WorldObjectWithAddToInventory;
					selectionGizmo.ShowCloneButton = mVWorldObjectClient is WorldObjectWithClone;
					selectedWorldObjectWithClone = mVWorldObjectClient as WorldObjectWithClone;
					selectedWorldObjectWithAddToInventory = mVWorldObjectClient as WorldObjectWithAddToInventory;
					selectedWorldObjectWithLogicReset = mVWorldObjectClient as WorldObjectWithLogicReset;
					selectionGizmo.Visible = true;
				}
			}
		}
		if (MVInputWrapper.GetKeyDown((KeyCode)127))
		{
			MVGameController.Instance.EditorController.Delete();
			selectionGizmo.Visible = false;
		}
		else
		{
			if (!MVGameController.Instance.EditorController.IsLogicRendered())
			{
				return;
			}
			if (MVInputWrapper.GetKeyDown((KeyCode)324))
			{
				Ray val = ((Component)e.WeCamera).camera.ScreenPointToRay(Input.mousePosition);
				RaycastHit val2 = default;
				Physics.Raycast(val, ref val2, 100f);
				if ((Object)(object)val2.collider != (Object)null)
				{
					LinkObjectScript componentInChildren = ((Component)val2.collider).gameObject.GetComponentInChildren<LinkObjectScript>();
					if ((Object)(object)componentInChildren != (Object)null)
					{
						rightClickedLink = componentInChildren;
					}
				}
			}
			if (!MVInputWrapper.GetKeyUp((KeyCode)324) || !((Object)(object)rightClickedLink != (Object)null))
			{
				return;
			}
			Ray val3 = ((Component)e.WeCamera).camera.ScreenPointToRay(Input.mousePosition);
			RaycastHit val4 = default;
			Physics.Raycast(val3, ref val4, 100f);
			if ((Object)(object)val4.collider != (Object)null)
			{
				LinkObjectScript componentInChildren2 = ((Component)val4.collider).gameObject.GetComponentInChildren<LinkObjectScript>();
				if ((Object)(object)componentInChildren2 != (Object)null && (Object)(object)componentInChildren2 == (Object)(object)rightClickedLink)
				{
					MVGameController.Instance.Game.RemoveLink(MVGameController.Instance.WOCM.Links[componentInChildren2.linkID]);
				}
			}
			rightClickedLink = null;
		}
	}

	private void DetectGizmoPosition(EditorStateMachine e)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		VoxelHit hit = default;
		if (MVGameController.Instance.WOCM.Pick(ref hit) && hit.woId != -1)
		{
			downWorldObjectID = hit.woId;
			selectObjectHitPositionWorld = hit.point;
			selectObjectCollider = hit.collider;
			selectionGizmo.WorldPosition = selectObjectHitPositionWorld;
			selectionGizmo.Visible = false;
		}
	}

	public bool HandleSelect(EditorStateMachine e)
	{
		if (MVInputWrapper.GetKeyUp((KeyCode)323))
		{
			VoxelHit hit = default;
			if (!MVGameController.Instance.WOCM.Pick(ref hit) || hit.woId == -1 || MVGameController.Instance.WOCM.Terrain.Id == hit.woId)
			{
				e.DeSelect();
				if (e.ParentGroup == MVGameController.Instance.WOCM.RootGroup.Id)
				{
					e.Event = EditorEvent.EditCubes;
				}
				selectionGizmo.Visible = false;
				return true;
			}
		}
		if (MVInputWrapper.GetKeyDown((KeyCode)323))
		{
			VoxelHit hit2 = default;
			if (MVGameController.Instance.WOCM.Pick(ref hit2) && hit2.woId != -1)
			{
				bool flag = false;
				downWorldObjectID = hit2.woId;
				bool addToSelection = MVInputWrapper.GetKey((KeyCode)306) || MVInputWrapper.GetKey((KeyCode)305);
				flag = e.Select(addToSelection);
				if (flag)
				{
					DetectGizmoPosition(e);
				}
				if (!flag && canLeave)
				{
					e.DeSelect();
					selectionGizmo.Visible = false;
					return true;
				}
			}
		}
		return false;
	}

	public bool ContinueToSelect(EditorStateMachine e)
	{
		VoxelHit hit = default;
		if (MVInputWrapper.GetKey((KeyCode)323) && MVGameController.Instance.WOCM.Pick(ref hit) && !e.IsSelected(hit.woId))
		{
			return true;
		}
		return false;
	}

	public bool HandleMove(EditorStateMachine e)
	{
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		if (MVInputWrapper.GetKeyUp((KeyCode)323))
		{
			canMove = false;
			return false;
		}
		if (MVInputWrapper.GetKeyDown((KeyCode)323))
		{
			canMove = false;
			VoxelHit hit = default;
			if (!MVGameController.Instance.WOCM.Pick(ref hit))
			{
				canMove = false;
				return false;
			}
			foreach (int item in e.Selected)
			{
				if (hit.woId == item)
				{
					canMove = true;
					mousePosInitPossibleMove = Input.mousePosition;
				}
			}
		}
		if (MVInputWrapper.GetKey((KeyCode)323) && canMove)
		{
			Vector3 val = mousePosInitPossibleMove - Input.mousePosition;
			if (val.magnitude > 0.5f || MVInputWrapper.GetAxisRaw("Mouse ScrollWheel") != 0f)
			{
				e.PushState(EditorEvent.ESTranslate);
				return true;
			}
		}
		return false;
	}

	private bool EnterObject(EditorStateMachine e)
	{
		if (e.SingleSelectedWO != null)
		{
			if ((object)e.SingleSelectedWO.GetType() == typeof(MVCubeModelInstance))
			{
				e.Event = EditorEvent.EditCubes;
				return true;
			}
			if ((object)e.SingleSelectedWO.GetType() == typeof(MVGroup))
			{
				if (!e.ParentGroupIsRoot)
				{
					SharedCubeFunctions.SetLayerRecursively(MVGameController.Instance.WOCM.GetWorldObjectClient(e.ParentGroup).GameObject.transform, select: false);
				}
				e.PushParent(e.SingleSelectedWO.Id);
				e.Select(addToSelection: false);
				SharedCubeFunctions.SetLayerRecursively(MVGameController.Instance.WOCM.GetWorldObjectClient(e.ParentGroup).GameObject.transform, select: true);
				((Behaviour)((Component)e.WeCamera).GetComponent<GrayscaleEffect>()).enabled = true;
				e.Event = EditorEvent.ObjectSelected;
				return true;
			}
		}
		return false;
	}

	public bool HandleEscape(EditorStateMachine e)
	{
		if (MVInputWrapper.GetKeyDown((KeyCode)27))
		{
			e.DeSelect();
			int parentGroup = e.ParentGroup;
			if (e.ParentGroupIsRoot)
			{
				e.Event = EditorEvent.EditCubes;
				return true;
			}
			e.PopParent();
			if (e.ParentGroupIsRoot)
			{
				((Behaviour)((Component)e.WeCamera).GetComponent<GrayscaleEffect>()).enabled = false;
				SharedCubeFunctions.SetLayerRecursively(MVGameController.Instance.WOCM.GetWorldObjectClient(parentGroup).GameObject.transform, select: false);
			}
			else
			{
				((Behaviour)((Component)e.WeCamera).GetComponent<GrayscaleEffect>()).enabled = true;
				SharedCubeFunctions.SetLayerRecursively(MVGameController.Instance.WOCM.GetWorldObjectClient(e.ParentGroup).GameObject.transform, select: true);
			}
			e.SelectWo(parentGroup, addToSelection: false);
			return true;
		}
		return false;
	}

	public override void Exit(EditorStateMachine e)
	{
		if (e.ParentGroupIsRoot)
		{
			((Behaviour)((Component)e.WeCamera).GetComponent<GrayscaleEffect>()).enabled = false;
		}
		selectionGizmo.Visible = false;
	}
}
