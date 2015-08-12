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
		terrain = MVGameController.WOCM.GetSingletonWorldObject<MVCubeModelPrototypeTerrain>();
		e.CubeModelingStateMachine.StartEdit(terrain);
		tintedWo = null;
	}

	public override void Execute(EditorStateMachine e)
	{
		base.Execute(e);
		VoxelHit hit = default;
		bool flag = MVGameController.WOCM.Pick(ref hit);
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
			if (hit.woId != terrain.Id)
			{
				e.CubeModelingStateMachine.CursorVisible = false;
			}
			if (!MVGameController.EditorController.IsLogicRendered() || !MVInputWrapper.GetBooleanControlDown(KogamaControls.PointerSelectAlt))
			{
				return;
			}
			VoxelHit hit2 = default;
			float num = float.PositiveInfinity;
			if (MVGameController.WOCM.Pick(ref hit2))
			{
				num = hit2.distance;
			}
			Ray ray = e.CameraController.GetComponent<Camera>().ScreenPointToRay(MVInputWrapper.GetPointerPosition());
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
			MVWorldObjectClient worldObjectClient = MVGameController.WOCM.GetWorldObjectClient(targetHit.woId);
			if (worldObjectClient != null && (worldObjectClient.WorldObjectType == WorldObjectType.CubeModelTerrainFineGrained || worldObjectClient.WorldObjectType == WorldObjectType.CubeModelPrototypeTerrain))
			{
				MVCubeModelPrototypeTerrain singletonWorldObject = MVGameController.WOCM.GetSingletonWorldObject<MVCubeModelPrototypeTerrain>();
				MVCubeModelFineGrainedTerrain singletonWorldObject2 = MVGameController.WOCM.GetSingletonWorldObject<MVCubeModelFineGrainedTerrain>();
				if (singletonWorldObject.RequiresResetToEdit || singletonWorldObject2.RequiresResetToEdit)
				{
					MVGameController.Game.World.RuntimeEventManager.ResetTerrain();
					MVGameController.Game.RequestResetTerrain();
					return true;
				}
			}
		}
		return false;
	}

	public override void Exit(EditorStateMachine e)
	{
		MVMaterialRepository.AllowDestructibleMaterialSelection = false;
		MVGameController.WOCM.AvatarLocal.LaserPointer.ChangeState(LaserPointerState.Idle);
		if (e.SelectedIDs.Count == 0)
		{
			DeTintCurrent();
		}
		e.CubeModelingStateMachine.RemoveCursors();
		e.CubeModelingStateMachine.EndEdit();
		terrain = null;
	}
}
