using MV.WorldObject;
using UnityEngine;

internal class ESTerrainEdit : ESStateBase
{
	private MVCubeModelPrototypeTerrain terrain;

	public override void Enter(EditorStateMachine e)
	{
		MVMaterialRepository.AllowDestructibleMaterialSelection = true;
		if (e.SingleSelectedWO != null)
		{
			e.DeSelectAll();
		}
		terrain = MVGameControllerBase.WOCM.GetSingletonWorldObject<MVCubeModelPrototypeTerrain>();
		e.CubeModelingStateMachine.StartEdit(terrain);
		tintedWo = null;
	}

	public override void Execute(EditorStateMachine e)
	{
		base.Execute(e);
		VoxelHit hit = default;
		bool flag = EditModeObjectPicker.Pick(ref hit);
		if (flag && (hit.woId == terrain.Id || hit.woId == -1))
		{
			flag = false;
		}
		TintObjectsOnMouseOver(e, flag, hit);
		if ((MVInputWrapper.GetBooleanControlDown(KogamaControls.PointerSelect) || MVInputWrapper.GetBooleanControlDown(KogamaControls.PointerSelectAlt)) && e.Select(addToSelection: false) != null)
		{
			e.Event = EditorEvent.ObjectSelected;
		}
		else
		{
			if (ResettingTerrain(hit))
			{
				return;
			}
			e.CubeModelingStateMachine.Update();
			if (hit.woId != 0 && hit.woId != terrain.Id)
			{
				e.CubeModelingStateMachine.CursorVisible = false;
			}
			if (!MVGameControllerBase.CameraController.IsLogicRendered || !MVInputWrapper.GetBooleanControlDown(KogamaControls.PointerSelectAlt))
			{
				return;
			}
			VoxelHit hit2 = default;
			float num = float.PositiveInfinity;
			if (EditModeObjectPicker.Pick(ref hit2))
			{
				num = hit2.distance;
			}
			Ray ray = MVGameControllerBase.CameraController.MainCamera.ScreenPointToRay(MVInputWrapper.GetPointerPosition());
			int layerMask = 1 << LayerMask.NameToLayer("Logic");
			Physics.Raycast(ray, out var hitInfo, float.PositiveInfinity, layerMask);
			if (hitInfo.collider != null)
			{
				LinkObjectScript componentInChildren = hitInfo.collider.gameObject.GetComponentInChildren<LinkObjectScript>();
				if (componentInChildren != null && hitInfo.distance < num)
				{
					e.Event = EditorEvent.ObjectSelected;
				}
			}
		}
	}

	private bool ResettingTerrain(VoxelHit targetHit)
	{
		if ((MVInputWrapper.GetBooleanControlUp(KogamaControls.PointerSelect) || MVInputWrapper.GetBooleanControlDown(KogamaControls.PointerSelect)) && targetHit.woId != -1)
		{
			MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(targetHit.woId);
			if (worldObjectClient != null && (worldObjectClient.WorldObjectType == WorldObjectType.CubeModelTerrainFineGrained || worldObjectClient.WorldObjectType == WorldObjectType.CubeModelPrototypeTerrain))
			{
				MVCubeModelPrototypeTerrain singletonWorldObject = MVGameControllerBase.WOCM.GetSingletonWorldObject<MVCubeModelPrototypeTerrain>();
				MVCubeModelFineGrainedTerrain singletonWorldObject2 = MVGameControllerBase.WOCM.GetSingletonWorldObject<MVCubeModelFineGrainedTerrain>();
				if (singletonWorldObject.RequiresResetToEdit || singletonWorldObject2.RequiresResetToEdit)
				{
					MVGameControllerBase.Game.World.RuntimeEventManager.ResetTerrain();
					MVGameControllerBase.Game.RequestResetTerrain();
					return true;
				}
			}
		}
		return false;
	}

	public override void Exit(EditorStateMachine e)
	{
		MVMaterialRepository.AllowDestructibleMaterialSelection = false;
		MVGameControllerBase.WOCM.AvatarLocal.LaserPointer.ChangeState(LaserPointerState.Idle);
		if (e.SelectedIDs.Count == 0)
		{
			DeTintCurrent();
		}
		e.CubeModelingStateMachine.RemoveCursors();
		e.CubeModelingStateMachine.EndEdit();
		terrain = null;
	}
}
