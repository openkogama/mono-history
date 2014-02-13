using System;
using MV.WorldObject;
using UnityEngine;

internal class ESSelectionMode : ESStateBase
{
	private enum GizmoAction
	{
		None,
		Rotate,
		TranslateXZ,
		TranslateY,
		Open
	}

	private bool canLeave;

	private bool canMove;

	private bool exit;

	private Vector3 mousePosInitPossibleMove = Vector3.zero;

	private Vector3 mousePosRightClick = Vector3.zero;

	private Vector3 selectObjectHitPositionWorld;

	private Collider selectObjectCollider;

	private LinkObjectScript rightClickedLink;

	private MVGUISelectionGizmo selectionGizmo;

	private GizmoAction gizmoAction;

	private int downWorldObjectID = -1;

	private bool deselectAfterTranslate;

	public ESSelectionMode()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		InitializeSelectionGizmo();
	}

	private void InitializeSelectionGizmo()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		Object val = Object.Instantiate(Resources.Load("Prefabs/GUI/Gizmos/SelectionGizmo"));
		GameObject val2 = (GameObject)(object)((val is GameObject) ? val : null);
		val2.transform.localPosition = Vector3.zero;
		selectionGizmo = val2.GetComponent<MVGUISelectionGizmo>();
		MVGUISelectionGizmo mVGUISelectionGizmo = selectionGizmo;
		mVGUISelectionGizmo.OnRotate = (MVGUIGizmoBase.GizmoClickDelegate)Delegate.Combine(mVGUISelectionGizmo.OnRotate, (MVGUIGizmoBase.GizmoClickDelegate)(() =>
		{
			gizmoAction = GizmoAction.Rotate;
		}));
		MVGUISelectionGizmo mVGUISelectionGizmo2 = selectionGizmo;
		mVGUISelectionGizmo2.OnXZtranslate = (MVGUIGizmoBase.GizmoClickDelegate)Delegate.Combine(mVGUISelectionGizmo2.OnXZtranslate, (MVGUIGizmoBase.GizmoClickDelegate)(() =>
		{
			gizmoAction = GizmoAction.TranslateXZ;
		}));
		MVGUISelectionGizmo mVGUISelectionGizmo3 = selectionGizmo;
		mVGUISelectionGizmo3.OnYtranslate = (MVGUIGizmoBase.GizmoClickDelegate)Delegate.Combine(mVGUISelectionGizmo3.OnYtranslate, (MVGUIGizmoBase.GizmoClickDelegate)(() =>
		{
			gizmoAction = GizmoAction.TranslateY;
		}));
	}

	public override void Enter(EditorStateMachine e)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		Debug.Log((object)"ESSelectionMode enter");
		if (deselectAfterTranslate)
		{
			deselectAfterTranslate = false;
			e.DeSelectAll();
			e.Event = EditorEvent.ESTerrainEdit;
		}
		else
		{
			canLeave = false;
			exit = false;
			mousePosInitPossibleMove = (mousePosRightClick = Input.mousePosition);
			canMove = true;
			DetectGizmoPosition(e, rightClickGizmo: true);
		}
	}

	public override void Execute(EditorStateMachine e)
	{
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		base.Execute(e);
		if (CheckAndHandleGizmoAction(e))
		{
			gizmoAction = GizmoAction.None;
			return;
		}
		if (MVInputWrapper.GetKeyUp((KeyCode)13))
		{
			EnterObject(e);
		}
		if (e.ParentGroupID == MVGameController.Instance.WOCM.RootGroup.Id)
		{
			e.CameraController.SecondaryCameraActive = false;
		}
		else
		{
			e.CameraController.SecondaryCameraActive = true;
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
		if (!HandleClick(e))
		{
			if (MVInputWrapper.GetKeyDown((KeyCode)324))
			{
				mousePosRightClick = Input.mousePosition;
			}
			if (!HandleSelect(e) && !HandleMove(e) && MVInputWrapper.GetKeyDown((KeyCode)127))
			{
				MVGameController.Instance.EditorController.Delete(e.SelectedWOs);
				HideGizmos();
			}
		}
	}

	public override void Exit(EditorStateMachine e)
	{
		Debug.Log((object)"Exitting SelectionMode");
		if (e.ParentGroupIsRoot)
		{
			((Behaviour)((Component)e.CameraController).GetComponent<GrayscaleEffect>()).enabled = false;
		}
		HideGizmos();
	}

	private bool CheckAndHandleGizmoAction(EditorStateMachine e)
	{
		switch (gizmoAction)
		{
		case GizmoAction.Open:
			EnterObject(e);
			return true;
		case GizmoAction.Rotate:
			if (e.SelectedIDs.Count == 1)
			{
				deselectAfterTranslate = true;
			}
			e.PushState(EditorEvent.Rotating);
			return true;
		case GizmoAction.TranslateY:
			if (e.SelectedIDs.Count == 1)
			{
				deselectAfterTranslate = true;
			}
			e.Data.Add("yOnlyTranslate", null);
			e.PushState(EditorEvent.ESTranslate);
			return true;
		case GizmoAction.TranslateXZ:
			if (e.SelectedIDs.Count == 1)
			{
				deselectAfterTranslate = true;
			}
			e.PushState(EditorEvent.ESTranslate);
			return true;
		default:
			return false;
		}
	}

	private bool HandleClick(EditorStateMachine e)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		if (MVInputWrapper.GetKeyUp((KeyCode)323))
		{
			goto IL_0044;
		}
		if (MVInputWrapper.GetKeyUp((KeyCode)324))
		{
			Vector3 val = mousePosRightClick - Input.mousePosition;
			if (val.sqrMagnitude < 20f)
			{
				goto IL_0044;
			}
		}
		goto IL_0159;
		IL_0044:
		Debug.Log((object)("Handling click, downWorldObjectID = " + downWorldObjectID));
		MVWorldObjectClient worldObjectClient = MVGameController.Instance.WOCM.GetWorldObjectClient(downWorldObjectID);
		if (worldObjectClient != null)
		{
			Debug.Log((object)$"Selected World Object Type: {worldObjectClient.GetType().Name}");
			if (worldObjectClient.OnClickHandler(e, selectObjectCollider))
			{
				return true;
			}
			MVGUIGizmoBase mVGUIGizmoBase = null;
			if (MVInputWrapper.GetKeyUp((KeyCode)323))
			{
				mVGUIGizmoBase = selectionGizmo;
			}
			else if ((!MVInputWrapper.GetKeyUp((KeyCode)324) || e.SelectedIDs.Count != 1) && MVInputWrapper.GetKeyUp((KeyCode)324) && e.SelectedIDs.Count <= 1)
			{
			}
			if ((Object)(object)mVGUIGizmoBase != (Object)null)
			{
				DetectGizmoPosition(e, MVInputWrapper.GetKeyUp((KeyCode)324));
				mVGUIGizmoBase.Visible = true;
			}
		}
		else
		{
			DetectGizmoPosition(e, rightClickGizmo: true);
			if (!((Object)(object)rightClickedLink != (Object)null))
			{
			}
		}
		goto IL_0159;
		IL_0159:
		return false;
	}

	private void DetectGizmoPosition(EditorStateMachine e, bool rightClickGizmo)
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		LinkObjectScript linkHit = GetLinkHit(e);
		VoxelHit hit = default;
		rightClickedLink = null;
		downWorldObjectID = -1;
		if ((Object)(object)linkHit != (Object)null)
		{
			rightClickedLink = linkHit;
			HideGizmos();
		}
		else if (MVGameController.Instance.WOCM.Pick(ref hit) && hit.woId != -1)
		{
			selectObjectHitPositionWorld = hit.point;
			downWorldObjectID = MVGroup.GetParentBelow(e.ParentGroupID, hit.woId);
			selectObjectCollider = hit.collider;
			selectionGizmo.WorldPosition = selectObjectHitPositionWorld;
			HideGizmos();
		}
	}

	private LinkObjectScript GetLinkHit(EditorStateMachine e)
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		if (MVGameController.Instance.EditController.IsLogicRendered())
		{
			VoxelHit hit = default;
			float num = float.PositiveInfinity;
			if (MVGameController.Instance.WOCM.Pick(ref hit))
			{
				num = hit.distance;
			}
			Ray val = ((Component)e.CameraController).camera.ScreenPointToRay(Input.mousePosition);
			int num2 = 1 << LayerMask.NameToLayer("Logic");
			RaycastHit val2 = default;
			Physics.Raycast(val, ref val2, float.PositiveInfinity, num2);
			if ((Object)(object)val2.collider != (Object)null && val2.distance < num)
			{
				selectObjectHitPositionWorld = val2.point;
				return ((Component)val2.collider).gameObject.GetComponentInChildren<LinkObjectScript>();
			}
		}
		return null;
	}

	private bool HandleSelect(EditorStateMachine e)
	{
		if (exit)
		{
			e.DeSelectAll();
			e.Event = EditorEvent.ESTerrainEdit;
			HideGizmos();
			return true;
		}
		if (MVInputWrapper.GetKeyUp((KeyCode)323) || MVInputWrapper.GetKeyUp((KeyCode)324) || exit)
		{
			MVWorldObjectClient worldObjectClient = MVGameController.Instance.WOCM.GetWorldObjectClient(downWorldObjectID);
			if (worldObjectClient != null && !worldObjectClient.OnClickHandler(e, selectObjectCollider) && CanDeselect(e))
			{
				e.DeSelectAll();
				if (e.ParentGroupID == MVGameController.Instance.WOCM.RootGroup.Id)
				{
					e.Event = EditorEvent.ESTerrainEdit;
				}
				HideGizmos();
				return true;
			}
		}
		if (MVInputWrapper.GetKeyDown((KeyCode)323) || MVInputWrapper.GetKeyDown((KeyCode)324))
		{
			LinkObjectScript linkHit = GetLinkHit(e);
			VoxelHit hit = default;
			if (MVInputWrapper.GetKeyDown((KeyCode)324) && (Object)(object)linkHit != (Object)null)
			{
				DetectGizmoPosition(e, rightClickGizmo: true);
				rightClickedLink = linkHit;
			}
			else if (MVGameController.Instance.WOCM.Pick(ref hit) && hit.woId != -1)
			{
				downWorldObjectID = hit.woId;
				bool addToSelection = ((MVInputWrapper.GetKey((KeyCode)304) || MVInputWrapper.GetKey((KeyCode)303)) && MVInputWrapper.GetKeyDown((KeyCode)323)) || e.SelectedIDs.Contains(hit.woId);
				MVWorldObjectClient mVWorldObjectClient = e.Select(addToSelection);
				if (mVWorldObjectClient == null && canLeave && MVInputWrapper.GetKeyDown((KeyCode)323))
				{
					e.DeSelectAll();
					HideGizmos();
					return true;
				}
			}
			else if (MVInputWrapper.GetKeyDown((KeyCode)323))
			{
				e.DeSelectAll();
				HideGizmos();
				e.Event = EditorEvent.ESTerrainEdit;
				return true;
			}
		}
		return false;
	}

	private bool CanDeselect(EditorStateMachine e)
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		VoxelHit hit = default;
		bool flag = !MVGameController.Instance.WOCM.Pick(ref hit) || hit.woId == -1 || MVGameController.Instance.WOCM.IsType(hit.woId, WorldObjectType.CubeModelPrototypeTerrain);
		int num;
		if (MVInputWrapper.GetKeyUp((KeyCode)324))
		{
			Vector3 val = mousePosRightClick - Input.mousePosition;
			num = ((val.sqrMagnitude < 20f) ? 1 : 0);
		}
		else
		{
			num = 0;
		}
		bool flag2 = (byte)num != 0;
		bool flag3 = (MVInputWrapper.GetKeyUp((KeyCode)323) || flag2) && flag;
		bool flag4 = MVInputWrapper.GetKeyUp((KeyCode)324) && !flag2 && (flag || hit.woId != downWorldObjectID);
		return (flag3 || flag4) && (Object)(object)GetLinkHit(e) == (Object)null;
	}

	private bool ContinueToSelect(EditorStateMachine e)
	{
		VoxelHit hit = default;
		if (MVInputWrapper.GetKey((KeyCode)323) && MVGameController.Instance.WOCM.Pick(ref hit) && !e.IsSelected(hit.woId))
		{
			return true;
		}
		return false;
	}

	private bool HandleMove(EditorStateMachine e)
	{
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
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
			foreach (int selectedID in e.SelectedIDs)
			{
				if (hit.woId == selectedID)
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
				if (e.SelectedIDs.Count == 1)
				{
					deselectAfterTranslate = true;
				}
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
				MVGameController.Instance.Game.CameraController.CurCamera.FocusOnObject(e.SingleSelectedWO);
				e.Event = EditorEvent.EditCubes;
				return true;
			}
			if (e.SingleSelectedWO is MVGroup)
			{
				if (!e.ParentGroupIsRoot)
				{
					SharedCubeFunctions.SetLayerRecursively(MVGameController.Instance.WOCM.GetWorldObjectClient(e.ParentGroupID).Transform, select: false);
				}
				e.Select(addToSelection: false);
				SharedCubeFunctions.SetLayerRecursively(MVGameController.Instance.WOCM.GetWorldObjectClient(e.ParentGroupID).Transform, select: true);
				((Behaviour)((Component)e.CameraController).GetComponent<GrayscaleEffect>()).enabled = true;
				e.Event = EditorEvent.ObjectSelected;
				return true;
			}
		}
		return false;
	}

	private bool HandleEscape(EditorStateMachine e)
	{
		if (MVInputWrapper.GetKeyDown((KeyCode)27))
		{
			e.DeSelectAll();
			int parentGroupID = e.ParentGroupID;
			if (e.ParentGroupIsRoot)
			{
				e.Event = EditorEvent.ESTerrainEdit;
				return true;
			}
			if (e.ParentGroupIsRoot)
			{
				((Behaviour)((Component)e.CameraController).GetComponent<GrayscaleEffect>()).enabled = false;
				SharedCubeFunctions.SetLayerRecursively(MVGameController.Instance.WOCM.GetWorldObjectClient(parentGroupID).Transform, select: false);
			}
			else
			{
				((Behaviour)((Component)e.CameraController).GetComponent<GrayscaleEffect>()).enabled = true;
				SharedCubeFunctions.SetLayerRecursively(MVGameController.Instance.WOCM.GetWorldObjectClient(e.ParentGroupID).Transform, select: true);
			}
			e.SelectWO(parentGroupID, addToSelection: false);
			return true;
		}
		return false;
	}

	private void HideGizmos()
	{
		selectionGizmo.Visible = false;
	}
}
