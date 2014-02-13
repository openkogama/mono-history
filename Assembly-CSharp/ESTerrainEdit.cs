using UnityEngine;

internal class ESTerrainEdit : ESStateBase
{
	private MVCubeModelPrototypeTerrain terrain;

	public override void Enter(EditorStateMachine e)
	{
		if (e.SingleSelectedWO != null)
		{
			e.DeSelectAll();
		}
		terrain = MVGameController.Instance.WOCM.GetSingletonWorldObject<MVCubeModelPrototypeTerrain>();
		e.CubeModelingStateMachine.StartEdit(terrain);
		tintedWo = null;
	}

	public override void Execute(EditorStateMachine e)
	{
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		base.Execute(e);
		VoxelHit hit = default;
		bool flag = MVGameController.Instance.WOCM.Pick(ref hit);
		if (flag && (hit.woId == terrain.Id || hit.woId == -1))
		{
			flag = false;
		}
		TintObjectsOnMouseOver(e, flag, hit);
		if ((MVInputWrapper.GetKeyDown((KeyCode)323) || MVInputWrapper.GetKeyDown((KeyCode)324)) && e.Select(addToSelection: false) != null)
		{
			e.Event = EditorEvent.ObjectSelected;
			return;
		}
		e.CubeModelingStateMachine.Update();
		if (hit.woId != terrain.Id)
		{
			e.CubeModelingStateMachine.CursorVisible = false;
		}
		if (!MVGameController.Instance.EditController.IsLogicRendered() || !MVInputWrapper.GetKeyDown((KeyCode)324))
		{
			return;
		}
		VoxelHit hit2 = default;
		float num = float.PositiveInfinity;
		if (MVGameController.Instance.WOCM.Pick(ref hit2))
		{
			num = hit2.distance;
		}
		Ray val = ((Component)e.CameraController).camera.ScreenPointToRay(Input.mousePosition);
		int num2 = 1 << LayerMask.NameToLayer("Logic");
		RaycastHit val2 = default;
		Physics.Raycast(val, ref val2, float.PositiveInfinity, num2);
		if ((Object)(object)val2.collider != (Object)null)
		{
			LinkObjectScript componentInChildren = ((Component)val2.collider).gameObject.GetComponentInChildren<LinkObjectScript>();
			if ((Object)(object)componentInChildren != (Object)null && val2.distance < num)
			{
				e.Event = EditorEvent.ObjectSelected;
			}
		}
	}

	public override void Exit(EditorStateMachine e)
	{
		MVGameController.Instance.WOCM.AvatarLocal.LaserPointer.ChangeState(LaserPointerState.Idle);
		if (e.SelectedIDs.Count == 0)
		{
			DeTintCurrent();
		}
		e.CubeModelingStateMachine.RemoveCursors();
		e.CubeModelingStateMachine.EndEdit();
		terrain = null;
	}
}
