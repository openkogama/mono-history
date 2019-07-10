using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CERoamUUI : ESStateBase
{
	private int downWorldObjectID = -1;

	private bool enterEditNextFrame;

	private Vector3 centerPos;

	private bool didExit;

	public CERoamUUI(Vector3 centerPos)
	{
		this.centerPos = centerPos;
	}

	public override void Enter(EditorStateMachine esm)
	{
		didExit = false;
		ExecuteEvents.ExecuteHierarchy(esm.GameObject, null, (IAvatarEditUIState x, BaseEventData y) =>
		{
			x.Set(ActiveEditStateUI.AvatarManagement);
		});
		esm.CubeModelingStateMachine.RemoveCursors();
		tintedWo = MVWorldObjectClientManager.GetWorldObjectClientRefNullRef();
		SharedCubeFunctions.SetLayerRecursively(esm.ParentGroup.Transform, select: true);
		ExecuteEvents.ExecuteHierarchy(esm.GameObject, null, (IAvatarEditAnimationState x, BaseEventData y) =>
		{
			x.Set("Idle");
		});
		MVGameControllerBase.GameEventManager.AvatarCommandsBuildMode.EnterBuildStateEvent(stateType, new MVBuildModeAvatarLocal.EditMode.CERoamUUISetupData(centerPos, esm.ParentGroup.Transform.position + Vector3.up));
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
		didExit = true;
		MVGameControllerBase.GameEventManager.AvatarCommandsBuildMode.ExitBuildStateEvent(stateType, null);
	}

	private bool HandleSelect(EditorStateMachine esm)
	{
		if (didExit)
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
				esm.MainCameraManager.BlueModeEnabled = true;
				esm.Event = EditorEvent.CERoamUUI;
				return true;
			}
		}
		return false;
	}
}
