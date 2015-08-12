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
		InitializeSelectionGizmo();
	}

	private void InitializeSelectionGizmo()
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load("Prefabs/GUI/Gizmos/SelectionGizmo")) as GameObject;
		gameObject.transform.localPosition = Vector3.zero;
		selectionGizmo = gameObject.GetComponent<MVGUISelectionGizmo>();
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
		Debug.Log("ESSelectionMode enter");
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
			mousePosInitPossibleMove = (mousePosRightClick = MVInputWrapper.GetPointerPosition());
			canMove = true;
			DetectGizmoPosition(e, rightClickGizmo: true);
		}
	}

	public override void Execute(EditorStateMachine e)
	{
		base.Execute(e);
		if (CheckAndHandleGizmoAction(e))
		{
			gizmoAction = GizmoAction.None;
			return;
		}
		if (MVInputWrapper.GetBooleanControlUp(KogamaControls.EnterObject))
		{
			EnterObject(e);
		}
		if (e.ParentGroupID == MVGameController.WOCM.RootGroup.Id)
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
		if (MVInputWrapper.GetBooleanControlUp(KogamaControls.PointerSelect))
		{
			canLeave = true;
		}
		if (!HandleClick(e))
		{
			if (MVInputWrapper.GetBooleanControlDown(KogamaControls.PointerSelectAlt))
			{
				mousePosRightClick = MVInputWrapper.GetPointerPosition();
			}
			if (!HandleSelect(e) && !HandleMove(e) && MVInputWrapper.GetBooleanControlDown(KogamaControls.DeleteObject))
			{
				MVGameController.EditorController.Delete(e.SelectedWOs);
				HideGizmos();
			}
		}
	}

	public override void Exit(EditorStateMachine e)
	{
		Debug.Log("Exitting SelectionMode");
		if (e.ParentGroupIsRoot)
		{
			e.CameraController.GetComponent<GrayscaleEffect>().enabled = false;
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
		if (MVInputWrapper.GetBooleanControlUp(KogamaControls.PointerSelect) || (MVInputWrapper.GetBooleanControlUp(KogamaControls.PointerSelectAlt) && (mousePosRightClick - MVInputWrapper.GetPointerPosition()).sqrMagnitude < 20f))
		{
			Debug.Log("Handling click, downWorldObjectID = " + downWorldObjectID);
			MVWorldObjectClient worldObjectClient = MVGameController.WOCM.GetWorldObjectClient(downWorldObjectID);
			if (worldObjectClient != null)
			{
				Debug.Log($"Selected World Object Type: {worldObjectClient.GetType().Name}");
				if (worldObjectClient.OnClickHandler(e, selectObjectCollider))
				{
					return true;
				}
				MVGUIGizmoBase mVGUIGizmoBase = null;
				if (MVInputWrapper.GetBooleanControlUp(KogamaControls.PointerSelect))
				{
					mVGUIGizmoBase = selectionGizmo;
				}
				else if ((!MVInputWrapper.GetBooleanControlUp(KogamaControls.PointerSelectAlt) || e.SelectedIDs.Count != 1) && MVInputWrapper.GetBooleanControlUp(KogamaControls.PointerSelectAlt) && e.SelectedIDs.Count <= 1)
				{
				}
				if (mVGUIGizmoBase != null)
				{
					DetectGizmoPosition(e, MVInputWrapper.GetBooleanControlUp(KogamaControls.PointerSelectAlt));
					mVGUIGizmoBase.Visible = true;
				}
			}
			else
			{
				DetectGizmoPosition(e, rightClickGizmo: true);
				if (!(rightClickedLink != null))
				{
				}
			}
		}
		return false;
	}

	private void DetectGizmoPosition(EditorStateMachine e, bool rightClickGizmo)
	{
		LinkObjectScript linkHit = GetLinkHit(e);
		VoxelHit hit = default;
		rightClickedLink = null;
		downWorldObjectID = -1;
		if (linkHit != null)
		{
			rightClickedLink = linkHit;
			HideGizmos();
		}
		else if (MVGameController.WOCM.Pick(ref hit) && hit.woId != -1)
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
		if (MVGameController.EditorController.IsLogicRendered())
		{
			VoxelHit hit = default;
			float num = float.PositiveInfinity;
			if (MVGameController.WOCM.Pick(ref hit))
			{
				num = hit.distance;
			}
			Ray ray = e.CameraController.GetComponent<Camera>().ScreenPointToRay(MVInputWrapper.GetPointerPosition());
			int layerMask = 1 << LayerMask.NameToLayer("Logic");
			Physics.Raycast(ray, out var hitInfo, float.PositiveInfinity, layerMask);
			if (hitInfo.collider != null && hitInfo.distance < num)
			{
				selectObjectHitPositionWorld = hitInfo.point;
				return hitInfo.collider.gameObject.GetComponentInChildren<LinkObjectScript>();
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
		if (MVInputWrapper.GetBooleanControlUp(KogamaControls.PointerSelect) || MVInputWrapper.GetBooleanControlUp(KogamaControls.PointerSelectAlt) || exit)
		{
			MVWorldObjectClient worldObjectClient = MVGameController.WOCM.GetWorldObjectClient(downWorldObjectID);
			if (worldObjectClient != null && !worldObjectClient.OnClickHandler(e, selectObjectCollider) && CanDeselect(e))
			{
				e.DeSelectAll();
				if (e.ParentGroupID == MVGameController.WOCM.RootGroup.Id)
				{
					e.Event = EditorEvent.ESTerrainEdit;
				}
				HideGizmos();
				return true;
			}
		}
		if (MVInputWrapper.GetBooleanControlDown(KogamaControls.PointerSelect) || MVInputWrapper.GetBooleanControlDown(KogamaControls.PointerSelectAlt))
		{
			LinkObjectScript linkHit = GetLinkHit(e);
			VoxelHit hit = default;
			if (MVInputWrapper.GetBooleanControlDown(KogamaControls.PointerSelectAlt) && linkHit != null)
			{
				DetectGizmoPosition(e, rightClickGizmo: true);
				rightClickedLink = linkHit;
			}
			else if (MVGameController.WOCM.Pick(ref hit) && hit.woId != -1)
			{
				downWorldObjectID = hit.woId;
				bool addToSelection = (MVInputWrapper.GetBooleanControlDown(KogamaControls.AddToSelection) && MVInputWrapper.GetBooleanControlDown(KogamaControls.PointerSelect)) || e.SelectedIDs.Contains(hit.woId);
				MVWorldObjectClient mVWorldObjectClient = e.Select(addToSelection);
				if (mVWorldObjectClient == null && canLeave && MVInputWrapper.GetBooleanControlDown(KogamaControls.PointerSelect))
				{
					e.DeSelectAll();
					HideGizmos();
					return true;
				}
			}
			else if (MVInputWrapper.GetBooleanControlDown(KogamaControls.PointerSelect))
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
		VoxelHit hit = default;
		bool flag = !MVGameController.WOCM.Pick(ref hit) || hit.woId == -1 || MVGameController.WOCM.IsType(hit.woId, WorldObjectType.CubeModelPrototypeTerrain);
		bool flag2 = MVInputWrapper.GetBooleanControlUp(KogamaControls.PointerSelectAlt) && (mousePosRightClick - MVInputWrapper.GetPointerPosition()).sqrMagnitude < 20f;
		bool flag3 = (MVInputWrapper.GetBooleanControlUp(KogamaControls.PointerSelect) || flag2) && flag;
		bool flag4 = MVInputWrapper.GetBooleanControlUp(KogamaControls.PointerSelectAlt) && !flag2 && (flag || hit.woId != downWorldObjectID);
		return (flag3 || flag4) && GetLinkHit(e) == null;
	}

	private bool ContinueToSelect(EditorStateMachine e)
	{
		VoxelHit hit = default;
		if (MVInputWrapper.GetBooleanControl(KogamaControls.PointerSelect) && MVGameController.WOCM.Pick(ref hit) && !e.IsSelected(hit.woId))
		{
			return true;
		}
		return false;
	}

	private bool HandleMove(EditorStateMachine e)
	{
		if (MVInputWrapper.GetBooleanControlUp(KogamaControls.PointerSelect))
		{
			canMove = false;
			return false;
		}
		if (MVInputWrapper.GetBooleanControlDown(KogamaControls.PointerSelect))
		{
			canMove = false;
			VoxelHit hit = default;
			if (!MVGameController.WOCM.Pick(ref hit))
			{
				canMove = false;
				return false;
			}
			foreach (int selectedID in e.SelectedIDs)
			{
				if (hit.woId == selectedID)
				{
					canMove = true;
					mousePosInitPossibleMove = MVInputWrapper.GetPointerPosition();
				}
			}
		}
		if (MVInputWrapper.GetBooleanControl(KogamaControls.PointerSelect) && canMove && ((mousePosInitPossibleMove - MVInputWrapper.GetPointerPosition()).magnitude > 0.5f || MVInputWrapper.GetAxisRaw("Mouse ScrollWheel") != 0f))
		{
			if (e.SelectedIDs.Count == 1)
			{
				deselectAfterTranslate = true;
			}
			e.PushState(EditorEvent.ESTranslate);
			return true;
		}
		return false;
	}

	private bool EnterObject(EditorStateMachine e)
	{
		if (e.SingleSelectedWO != null)
		{
			if (e.SingleSelectedWO.GetType() == typeof(MVCubeModelInstance))
			{
				MVGameController.Game.CameraController.CurCamera.FocusOnObject(e.SingleSelectedWO);
				e.Event = EditorEvent.EditCubes;
				return true;
			}
			if (e.SingleSelectedWO is MVGroup)
			{
				if (!e.ParentGroupIsRoot)
				{
					SharedCubeFunctions.SetLayerRecursively(MVGameController.WOCM.GetWorldObjectClient(e.ParentGroupID).Transform, select: false);
				}
				e.Select(addToSelection: false);
				SharedCubeFunctions.SetLayerRecursively(MVGameController.WOCM.GetWorldObjectClient(e.ParentGroupID).Transform, select: true);
				e.CameraController.GetComponent<GrayscaleEffect>().enabled = true;
				e.Event = EditorEvent.ObjectSelected;
				return true;
			}
		}
		return false;
	}

	private bool HandleEscape(EditorStateMachine e)
	{
		if (MVInputWrapper.GetBooleanControlDown(KogamaControls.LeaveObject))
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
				e.CameraController.GetComponent<GrayscaleEffect>().enabled = false;
				SharedCubeFunctions.SetLayerRecursively(MVGameController.WOCM.GetWorldObjectClient(parentGroupID).Transform, select: false);
			}
			else
			{
				e.CameraController.GetComponent<GrayscaleEffect>().enabled = true;
				SharedCubeFunctions.SetLayerRecursively(MVGameController.WOCM.GetWorldObjectClient(e.ParentGroupID).Transform, select: true);
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
