using UnityEngine;

internal class ESWaitForUngroup : ESStateBase
{
	private enum WaitForGroupsState
	{
		WaitingForLock,
		WaitingForUngroup
	}

	private bool abort;

	private bool responseReceived;

	private int lockId = -1;

	private int parentGroupId = -1;

	private WaitForGroupsState state;

	public override void Enter(EditorStateMachine e)
	{
		e.LockState = true;
		if (e.SingleSelectedWO == null || (object)e.SingleSelectedWO.GetType() != typeof(MVGroup))
		{
			e.LockState = false;
			e.PopState();
			return;
		}
		lockId = e.SingleSelectedWO.Id;
		parentGroupId = e.SingleSelectedWO.GroupId;
		state = WaitForGroupsState.WaitingForLock;
		MVGameController.Instance.WOCM.OnHierarchyLockedResponse += WOCM_OnHierarchyLockedResponse;
		MVGameController.Instance.Game.LockHierarchy(lockId, lockHierarchy: true);
	}

	public override void Execute(EditorStateMachine e)
	{
		base.Execute(e);
		if (abort)
		{
			e.LockState = false;
			e.PopState();
			return;
		}
		switch (state)
		{
		case WaitForGroupsState.WaitingForLock:
			if (responseReceived)
			{
				responseReceived = false;
				e.DeSelect();
				MVGameController.Instance.WOCM.OnUngroupResponse += WOCM_OnUngroupResponse;
				MVGameController.Instance.Game.Ungroup(lockId);
				state = WaitForGroupsState.WaitingForUngroup;
			}
			break;
		case WaitForGroupsState.WaitingForUngroup:
			if (responseReceived)
			{
				if (MVGameController.Instance.WOCM.GetWorldObjectClient(parentGroupId).Id != MVGameController.Instance.WOCM.RootGroup.Id)
				{
					e.SelectWo(parentGroupId, addToSelection: false);
					e.LockState = false;
					e.PopState();
				}
				else
				{
					e.LockState = false;
					e.PopState();
					e.Event = EditorEvent.EditCubes;
				}
			}
			break;
		}
	}

	private void WOCM_OnUngroupResponse(object sender, OnUngroupResponseEventArgs e)
	{
		responseReceived = true;
		MVGameController.Instance.WOCM.OnUngroupResponse -= WOCM_OnUngroupResponse;
	}

	private void WOCM_OnHierarchyLockedResponse(object sender, OnHierarchyLockedEventArgs e)
	{
		Debug.Log((object)"WOCM_OnHierarchyLockedResponse");
		if (e.success)
		{
			responseReceived = true;
			MVGameController.Instance.WOCM.OnHierarchyLockedResponse -= WOCM_OnHierarchyLockedResponse;
		}
		else
		{
			MVGameController.Instance.Game.LockHierarchy(lockId, lockHierarchy: false);
			abort = true;
		}
	}

	public override void Exit(EditorStateMachine e)
	{
	}
}
