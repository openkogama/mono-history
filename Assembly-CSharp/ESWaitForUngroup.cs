using System;
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
		if (e.SingleSelectedWO == null || e.SingleSelectedWO is MVBlueprintBase || e.SingleSelectedWO.GetType() != typeof(MVGroup))
		{
			e.LockState = false;
			e.PopState();
			return;
		}
		lockId = e.SingleSelectedWO.Id;
		parentGroupId = e.SingleSelectedWO.GroupId;
		state = WaitForGroupsState.WaitingForLock;
		MVWorldObjectClientManager wOCM = MVGameController.WOCM;
		wOCM.OnHierarchyLockedResponse = (EventHandler<OnHierarchyLockedEventArgs>)Delegate.Combine(wOCM.OnHierarchyLockedResponse, new EventHandler<OnHierarchyLockedEventArgs>(WOCM_OnHierarchyLockedResponse));
		MVGameController.Game.LockHierarchy(lockId, lockHierarchy: true);
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
				e.DeSelectAll();
				MVWorldObjectClientManager wOCM = MVGameController.WOCM;
				wOCM.OnUngroupResponse = (EventHandler<OnUngroupResponseEventArgs>)Delegate.Combine(wOCM.OnUngroupResponse, new EventHandler<OnUngroupResponseEventArgs>(WOCM_OnUngroupResponse));
				MVGameController.Game.Ungroup(lockId);
				state = WaitForGroupsState.WaitingForUngroup;
			}
			break;
		case WaitForGroupsState.WaitingForUngroup:
			if (responseReceived)
			{
				if (MVGameController.WOCM.GetWorldObjectClient(parentGroupId).Id != MVGameController.WOCM.RootGroup.Id)
				{
					e.SelectWO(parentGroupId, addToSelection: false);
					e.LockState = false;
					e.PopState();
				}
				else
				{
					e.LockState = false;
					e.PopState();
					e.Event = EditorEvent.ESTerrainEdit;
				}
			}
			break;
		}
	}

	private void WOCM_OnUngroupResponse(object sender, OnUngroupResponseEventArgs e)
	{
		responseReceived = true;
		MVWorldObjectClientManager wOCM = MVGameController.WOCM;
		wOCM.OnUngroupResponse = (EventHandler<OnUngroupResponseEventArgs>)Delegate.Remove(wOCM.OnUngroupResponse, new EventHandler<OnUngroupResponseEventArgs>(WOCM_OnUngroupResponse));
	}

	private void WOCM_OnHierarchyLockedResponse(object sender, OnHierarchyLockedEventArgs e)
	{
		Debug.Log("WOCM_OnHierarchyLockedResponse");
		if (e.success)
		{
			responseReceived = true;
			MVWorldObjectClientManager wOCM = MVGameController.WOCM;
			wOCM.OnHierarchyLockedResponse = (EventHandler<OnHierarchyLockedEventArgs>)Delegate.Remove(wOCM.OnHierarchyLockedResponse, new EventHandler<OnHierarchyLockedEventArgs>(WOCM_OnHierarchyLockedResponse));
		}
		else
		{
			MVGameController.Game.LockHierarchy(lockId, lockHierarchy: false);
			abort = true;
		}
	}

	public override void Exit(EditorStateMachine e)
	{
	}
}
