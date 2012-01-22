using System.Collections;
using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

internal class ESWaitForGroup : ESStateBase
{
	private enum WaitForGroupsState
	{
		WaitingForLock,
		WaitingForGroup
	}

	private bool abort;

	private bool responseReceived;

	private List<int> lockList = new List<int>();

	private int lockCount;

	private WaitForGroupsState state;

	public override void Enter(EditorStateMachine e)
	{
		e.LockState = true;
		if (e.Selected.Count == 0 || e.Selected.Count == 1)
		{
			e.LockState = false;
			e.PopState();
			return;
		}
		lockList = new List<int>();
		state = WaitForGroupsState.WaitingForLock;
		MVGameController.Instance.WOCM.OnHierarchyLockedResponse += WOCM_OnHierarchyLockedResponse;
		foreach (int item in e.Selected)
		{
			lockList.Add(item);
			MVGameController.Instance.Game.LockHierarchy(item, lockHierarchy: true);
		}
		lockCount = lockList.Count;
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
			if (!responseReceived)
			{
				break;
			}
			GroupSelected(e);
			foreach (int @lock in lockList)
			{
				MVGameController.Instance.Game.LockHierarchy(@lock, lockHierarchy: false);
			}
			state = WaitForGroupsState.WaitingForGroup;
			responseReceived = false;
			e.DeSelect();
			break;
		case WaitForGroupsState.WaitingForGroup:
			if (responseReceived)
			{
				e.LockState = false;
				e.PopState();
			}
			break;
		}
	}

	public void GroupSelected(EditorStateMachine e)
	{
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		Hashtable hashtable = new Hashtable();
		Hashtable hashtable2 = new Hashtable();
		HashSet<MVWorldObjectClient> selectedWOs = e.SelectedWOs;
		foreach (int @lock in lockList)
		{
			hashtable2.Add(@lock, (byte)0);
		}
		hashtable.Add("children", hashtable2);
		Vector3 worldCenter = SharedCubeFunctions.GetWorldCenter(selectedWOs);
		MVGameController.Instance.WOCM.OnWorldObjectRegisterResponse += WOCM_OnWorldObjectRegisterResponse;
		MVGameController.Instance.Game.RegisterWorldObject(WorldObjectType.Group, e.ParentGroup, hashtable, new Hashtable(), worldCenter, Quaternion.identity, Vector3.one, localOwner: true, transferOwnershipToServerOnLeave: true);
	}

	private void WOCM_OnWorldObjectRegisterResponse(object sender, OnWorldObjectRegisterResponseEventArgs e)
	{
		responseReceived = true;
		MVGameController.Instance.Game.TransferOwnership(e.worldObjectID, 0);
		MVGameController.Instance.WOCM.OnWorldObjectRegisterResponse -= WOCM_OnWorldObjectRegisterResponse;
	}

	private void WOCM_OnHierarchyLockedResponse(object sender, OnHierarchyLockedEventArgs e)
	{
		Debug.Log((object)"WOCM_OnHierarchyLockedResponse");
		if (e.success)
		{
			lockCount--;
			if (lockCount == 0)
			{
				responseReceived = true;
				MVGameController.Instance.WOCM.OnHierarchyLockedResponse -= WOCM_OnHierarchyLockedResponse;
			}
			return;
		}
		foreach (int @lock in lockList)
		{
			MVGameController.Instance.Game.LockHierarchy(@lock, lockHierarchy: false);
		}
		abort = true;
		MVGameController.Instance.WOCM.OnHierarchyLockedResponse -= WOCM_OnHierarchyLockedResponse;
	}

	public override void Exit(EditorStateMachine e)
	{
	}
}
