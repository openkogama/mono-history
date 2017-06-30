using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CERoamUUI : ESStateBase
{
	private int downWorldObjectID = -1;

	private bool enterEditNextFrame;

	private readonly AccessoryMover accessoryMover = new AccessoryMover();

	private Vector3 centerPos;

	private bool didExit;

	public CERoamUUI(Vector3 centerPos)
	{
		this.centerPos = centerPos;
	}

	public override void Enter(EditorStateMachine esm)
	{
		didExit = false;
		accessoryMover.Activate();
		ExecuteEvents.ExecuteHierarchy(esm.GameObject, null, (IAvatarEditUIState x, BaseEventData y) =>
		{
			x.Set(ActiveEditStateUI.AvatarManagement);
		});
		esm.CubeModelingStateMachine.RemoveCursors();
		tintedWo = MVWorldObjectClientManager.GetWorldObjectClientRefNullRef();
		SharedCubeFunctions.SetLayerRecursively(esm.ParentGroup.Transform, select: true);
		esm.CameraController.BlueModeEnabled = true;
		if (esm.ParentGroup is MVBody)
		{
			MVAvatarLocal.JetPackMode jetPackMode = (MVAvatarLocal.JetPackMode)MVGameControllerBase.WOCM.AvatarLocal.CurrentMode;
			jetPackMode.SetMoveConstraint(centerPos, 10f);
			jetPackMode.ModifySpeed(0.8f, 0.25f);
			ExecuteEvents.ExecuteHierarchy(esm.GameObject, null, (IAvatarEditAnimationState x, BaseEventData y) =>
			{
				x.Set("Idle");
			});
			MVGameControllerBase.CameraController.SetCamera(CameraType.AvatarEditModeCamera);
			((AvatarEditModeCamera)MVGameControllerBase.CameraController.CurCamera).FocusOnPosition(esm.ParentGroup.Transform.position + Vector3.up);
		}
		else
		{
			Debug.LogError("No MVBody");
		}
		MVGameControllerBase.WOCM.AvatarLocal.LaserPointer.ChangeState(LaserPointerState.Idle);
	}

	public override void Execute(EditorStateMachine esm)
	{
		if (enterEditNextFrame)
		{
			enterEditNextFrame = false;
			esm.Event = EditorEvent.CEEditBodyUUI;
		}
		TintObjectsOnMouseOver(esm);
		if (HandleSelect(esm))
		{
			EnterObject(esm);
		}
	}

	public override void Exit(EditorStateMachine esm)
	{
		DeTintCurrent();
		((MVAvatarLocal.JetPackMode)MVGameControllerBase.WOCM.AvatarLocal.CurrentMode).ModifySpeed(1f, 1f);
		didExit = true;
		accessoryMover.Destroy();
	}

	private bool HandleSelect(EditorStateMachine esm)
	{
		if (didExit)
		{
			return false;
		}
		if (accessoryMover.MoveAccessory())
		{
			return false;
		}
		VoxelHit hit = default;
		int layerMask = -5 & ~(1 << LayerMask.NameToLayer("Hidden"));
		if (EditModeObjectPicker.Pick(ref hit, new HashSet<int>(), layerMask) && hit.woId != -1)
		{
			if (MVInputWrapper.GetBooleanControlUp(KogamaControls.PointerSelect) && downWorldObjectID == hit.woId)
			{
				downWorldObjectID = -1;
				return esm.Select(addToSelection: false, layerMask) != null;
			}
			if (MVInputWrapper.GetBooleanControlDown(KogamaControls.PointerSelect))
			{
				downWorldObjectID = hit.woId;
			}
		}
		return false;
	}

	private bool EnterObject(EditorStateMachine esm)
	{
		if (esm.SingleSelectedWO != null)
		{
			if (esm.SingleSelectedWO is MVCubeModelInstance && esm.SingleSelectedWO.GroupId == esm.ParentGroupID)
			{
				ExecuteEvents.ExecuteHierarchy(esm.GameObject, null, (IAvatarEditAnimationState x, BaseEventData y) =>
				{
					x.Set("TPose");
				});
				enterEditNextFrame = true;
				return true;
			}
			if (esm.SingleSelectedWO is MVGroup)
			{
				MVGroup mVGroup = (MVGroup)esm.SingleSelectedWO;
				esm.EnterGroup(mVGroup);
				SharedCubeFunctions.SetLayerRecursively(mVGroup.Transform, select: true);
				esm.CameraController.BlueModeEnabled = true;
				esm.Event = EditorEvent.CERoamUUI;
				return true;
			}
		}
		return false;
	}
}
