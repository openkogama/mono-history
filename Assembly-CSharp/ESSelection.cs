using System.Collections.Generic;
using MV.Common;
using MV.WorldObject;
using UnityEngine;
using UnityEngine.EventSystems;

internal class ESSelection : ESStateBase
{
	private class PickResult<T>
	{
		public readonly Vector3 mousePosition;

		public readonly VoxelHit hit;

		public readonly T data;

		public PickResult(Vector3 mousePosition, VoxelHit hit, T data)
		{
			this.mousePosition = mousePosition;
			this.hit = hit;
			this.data = data;
		}
	}

	private EditorStateMachine editorStateMachine;

	private LinkObjectBase selectedLinkObject;

	private WorldObjectClientRef selectedWorldObject = MVWorldObjectClientManager.GetWorldObjectClientRefNullRef();

	private PickResult<WorldObjectClientRef> pickedTarget;

	private PickResult<LinkObjectBase> pickedLink;

	private ContextMenuController contextMenuController;

	private GizmoController gizmoController;

	public ESSelection(ContextMenuController contextMenuController, GizmoController gizmoController)
	{
		this.contextMenuController = contextMenuController;
		this.gizmoController = gizmoController;
	}

	private void ShowContextMenuGizmo()
	{
		if (selectedWorldObject.WorldObjectClient != null)
		{
			ContextMenuController contextMenuController = this.contextMenuController;
			int id = selectedWorldObject.WorldObjectClient.Id;
			VoxelHit hit = pickedTarget.hit;
			contextMenuController.ShowContextMenu(id, hit.point);
		}
	}

	private void ShowLinkMenuGizmo()
	{
		ContextMenuController contextMenuController = this.contextMenuController;
		int linkID = selectedLinkObject.linkID;
		bool isObjectLink = selectedLinkObject.isObjectLink;
		VoxelHit hit = pickedLink.hit;
		contextMenuController.ShowContextMenuLink(linkID, isObjectLink, hit.point);
	}

	private void PopMenuGizmos()
	{
		contextMenuController.PopGizmos();
	}

	public override void Enter(EditorStateMachine e)
	{
		bool flag = e.SingleSelectedWO != null && (e.SingleSelectedWO.InteractionFlags & InteractionFlags.DirectlySelectable) != 0;
		if (e.Data.ContainsKey("FromTranslateState"))
		{
			if (e.ParentGroupIsRoot)
			{
				e.DeSelectAll();
				e.Event = EditorEvent.ESTerrainEdit;
				return;
			}
			if (flag)
			{
				SharedCubeFunctions.SetLayerRecursively(e.ParentGroup.GameObject.transform, select: false);
				e.ExitGroup();
				if (e.ParentGroupIsRoot)
				{
					e.Event = EditorEvent.ESTerrainEdit;
				}
			}
		}
		editorStateMachine = e;
		editorStateMachine.SelectionController.SelectedWorldObjectDeleted += SelectionController_SelectedWorldObjectDeletedHandler;
		if (!e.ParentGroupIsRoot && !flag)
		{
			e.CameraController.BlueModeEnabled = true;
		}
		VoxelHit hit = default;
		if (EditModeObjectPicker.Pick(ref hit))
		{
			pickedTarget = new PickResult<WorldObjectClientRef>(MVInputWrapper.GetPointerPosition(), hit, MVGameControllerBase.WOCM.GetWorldObjectClientRef(hit.woId));
			if (e.SingleSelectedWO == null)
			{
				selectedWorldObject = MVWorldObjectClientManager.GetWorldObjectClientRefNullRef();
			}
			else
			{
				selectedWorldObject = MVGameControllerBase.WOCM.GetWorldObjectClientRef(e.SingleSelectedWO.Id);
			}
		}
		LinkObjectBase linkHit = GetLinkHit(e, ref hit);
		pickedLink = ((!(linkHit != null)) ? null : new PickResult<LinkObjectBase>(MVInputWrapper.GetPointerPosition(), hit, linkHit));
	}

	public override void Execute(EditorStateMachine e)
	{
		base.Execute(e);
		if (contextMenuController.MouseDown)
		{
			return;
		}
		if (MVInputWrapper.GetBooleanControlDown(KogamaControls.DeleteObject))
		{
			if (e.SingleSelectedWO == null)
			{
				return;
			}
			MVWorldObjectClient singleSelectedWO = e.SingleSelectedWO;
			editorStateMachine.DeSelectAll();
			string errorText = string.Empty;
			if (!singleSelectedWO.Delete(MVGameControllerBase.WOCM, ref errorText))
			{
				ExecuteEvents.ExecuteHierarchy(e.GameObject, null, (IModalPopupCreator handler, BaseEventData data) =>
				{
					handler.CreateErrorNotificationPopup(errorText);
				});
			}
			ExecuteEvents.ExecuteHierarchy(e.GameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.PopGroups(UIGroupFlags.GameObjectUI | UIGroupFlags.GameObjectUISubMenu);
			});
			return;
		}
		if (MVInputWrapper.GetBooleanControlDown(KogamaControls.LeaveObject))
		{
			HandleEscape(e);
			return;
		}
		VoxelHit hit = default;
		bool flag = EditModeObjectPicker.Pick(ref hit);
		if (flag && (MVGameControllerBase.WOCM.IsType(hit.woId, WorldObjectType.CubeModelPrototypeTerrain) || hit.woId == -1))
		{
			flag = false;
		}
		if (flag)
		{
			TintObjectsOnMouseOver(e, flag, hit);
		}
		if (pickedTarget != null && pickedTarget.data.WorldObjectClient == null)
		{
			pickedTarget = null;
		}
		if (MVInputWrapper.GetBooleanControlDown(KogamaControls.PointerSelect))
		{
			if (flag)
			{
				pickedTarget = new PickResult<WorldObjectClientRef>(MVInputWrapper.GetPointerPosition(), hit, MVGameControllerBase.WOCM.GetWorldObjectClientRef(hit.woId));
				HashSet<int> selectedIDs = e.SelectedIDs;
				VoxelHit hit2 = pickedTarget.hit;
				bool flag2 = selectedIDs.Contains(hit2.woId);
				if ((pickedTarget.data.WorldObjectClient.InteractionFlags & InteractionFlags.NotUserTransformable) != InteractionFlags.NotUserTransformable)
				{
					bool addToSelection = flag2;
					selectedWorldObject = e.Select(pickedTarget.hit, addToSelection);
				}
			}
		}
		else if (MVInputWrapper.GetBooleanControl(KogamaControls.PointerSelect))
		{
			if (flag && pickedTarget != null && ((pickedTarget.mousePosition - MVInputWrapper.GetPointerPosition()).magnitude > 0.5f || MVInputWrapper.GetAxisRaw("Mouse ScrollWheel") != 0f))
			{
				if (MVGameControllerBase.Game.GameType == MVGameType.Platformer)
				{
					e.Data.Add("translateMode", TranslateMode.XY);
					e.Data.Add("moveWithAvatar", false);
				}
				else
				{
					e.Data.Add("translateMode", TranslateMode.XZ);
					e.Data.Add("moveWithAvatar", true);
				}
				e.PushState(EditorEvent.ESTranslate);
			}
		}
		else if (MVInputWrapper.GetBooleanControlUp(KogamaControls.PointerSelect))
		{
			if (selectedWorldObject.WorldObjectClient != null && pickedTarget != null && pickedTarget.data.WorldObjectClient != null && IsMouseUpValid(pickedTarget.mousePosition))
			{
				bool flag3 = CheckAndExecuteOnClickHandler(e, pickedTarget);
				bool flag4 = !e.SelectedIDs.Contains(selectedWorldObject.WorldObjectClient.Id);
				if (!flag3 && !flag4)
				{
					GizmoController gizmoController = this.gizmoController;
					int id = selectedWorldObject.WorldObjectClient.Id;
					VoxelHit hit3 = pickedTarget.hit;
					gizmoController.Show(id, hit3.point, e);
				}
			}
			else
			{
				e.DeSelectAll();
				if (e.ParentGroup == MVGameControllerBase.WOCM.RootGroup)
				{
					e.Event = EditorEvent.ESTerrainEdit;
				}
			}
			pickedLink = null;
			pickedTarget = null;
		}
		else if (MVInputWrapper.GetBooleanControlDown(KogamaControls.PointerSelectAlt))
		{
			VoxelHit hit4 = default;
			LinkObjectBase linkHit = GetLinkHit(e, ref hit4);
			if (linkHit != null)
			{
				pickedLink = new PickResult<LinkObjectBase>(MVInputWrapper.GetPointerPosition(), hit4, linkHit);
			}
			else if (flag)
			{
				WorldObjectClientRef worldObjectClientRef = MVGameControllerBase.WOCM.GetWorldObjectClientRef(hit.woId);
				pickedTarget = new PickResult<WorldObjectClientRef>(MVInputWrapper.GetPointerPosition(), hit, worldObjectClientRef);
				if (!e.SelectedWOs.Contains(pickedTarget.data.WorldObjectClient))
				{
					selectedWorldObject = e.Select(pickedTarget.hit, addToSelection: false);
				}
			}
		}
		else
		{
			if (!MVInputWrapper.GetBooleanControlUp(KogamaControls.PointerSelectAlt))
			{
				return;
			}
			PopMenuGizmos();
			if (pickedLink != null && IsMouseUpValid(pickedLink.mousePosition))
			{
				selectedLinkObject = pickedLink.data;
				ShowLinkMenuGizmo();
			}
			else if (pickedTarget != null && IsMouseUpValid(pickedTarget.mousePosition) && !CheckAndExecuteOnClickHandler(e, pickedTarget))
			{
				if (e.SelectedIDs.Count == 1)
				{
					ShowContextMenuGizmo();
				}
				else
				{
					Debug.LogWarning("There should be selected objects at this point");
				}
			}
			pickedLink = null;
			pickedTarget = null;
		}
	}

	public override void Exit(EditorStateMachine e)
	{
		if (e.ParentGroupIsRoot || (e.SingleSelectedWO != null && e.SingleSelectedWO.HasInteractionFlag(InteractionFlags.DirectlySelectable)))
		{
			e.CameraController.BlueModeEnabled = false;
		}
		editorStateMachine.SelectionController.SelectedWorldObjectDeleted -= SelectionController_SelectedWorldObjectDeletedHandler;
	}

	private bool IsMouseUpValid(Vector3 mousePosition)
	{
		return (mousePosition - MVInputWrapper.GetPointerPosition()).sqrMagnitude < 20f;
	}

	private bool CheckAndExecuteOnClickHandler(EditorStateMachine e, PickResult<WorldObjectClientRef> pick)
	{
		if (pick.data.WorldObjectClient == null)
		{
			return false;
		}
		MVWorldObjectClient worldObjectClient = pick.data.WorldObjectClient;
		VoxelHit hit = pick.hit;
		return worldObjectClient.OnClickHandler(e, hit.collider);
	}

	private void SelectionController_SelectedWorldObjectDeletedHandler(object sender, WorldObjectDestroyedEventArgs e)
	{
		if (selectedWorldObject.WorldObjectClient != null && selectedWorldObject.WorldObjectClient.Id == e.WordObjectID)
		{
			editorStateMachine.PopState();
		}
	}

	private LinkObjectBase GetLinkHit(EditorStateMachine e, ref VoxelHit hit)
	{
		if (MVGameControllerBase.CameraController.IsLogicRendered)
		{
			float num = float.PositiveInfinity;
			if (EditModeObjectPicker.Pick(ref hit))
			{
				num = hit.distance;
			}
			Ray ray = MVGameControllerBase.CameraController.MainCamera.ScreenPointToRay(MVInputWrapper.GetPointerPosition());
			int layerMask = 1 << LayerMask.NameToLayer("Logic");
			Physics.Raycast(ray, out var hitInfo, float.PositiveInfinity, layerMask);
			if (hitInfo.collider != null && hitInfo.distance < num)
			{
				Debug.Log(hitInfo.collider.gameObject.name);
				hit.point = hitInfo.point;
				return hitInfo.collider.gameObject.GetComponentInChildren<LinkObjectBase>();
			}
		}
		return null;
	}

	private bool EnterObject(EditorStateMachine e, MVWorldObjectClient selectedWo)
	{
		Debug.Log("EnterObject");
		return selectedWo.OnEnterObject(e);
	}

	private void HandleEscape(EditorStateMachine e)
	{
		if (e.ParentGroupIsRoot)
		{
			e.CameraController.BlueModeEnabled = false;
			e.DeSelectAll();
			e.Event = EditorEvent.ESTerrainEdit;
			return;
		}
		SharedCubeFunctions.SetLayerRecursively(e.ParentGroup.Transform, select: false);
		e.ExitGroup();
		if (e.ParentGroupIsRoot)
		{
			e.CameraController.BlueModeEnabled = false;
			e.DeSelectAll();
			e.Event = EditorEvent.ESTerrainEdit;
		}
		else
		{
			EnterObject(e, e.ParentGroup);
		}
	}
}
