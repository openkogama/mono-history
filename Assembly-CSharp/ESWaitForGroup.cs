using System;
using System.Collections;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;

internal class ESWaitForGroup : ESStateBase
{
	private enum WaitForGroupsState
	{
		WaitingForLock,
		WaitingForGroup,
		WaitingForTransferWos
	}

	private bool abort;

	private bool responseReceived;

	private List<int> lockList = new List<int>();

	private int lockCount;

	private WaitForGroupsState state;

	private int createGroupId = -1;

	public override void Enter(EditorStateMachine e)
	{
		e.LockState = true;
		if (e.SelectedIDs.Count == 0 || e.SelectedIDs.Count == 1)
		{
			e.LockState = false;
			e.PopState();
			return;
		}
		lockList = new List<int>();
		state = WaitForGroupsState.WaitingForLock;
		MVWorldObjectClientManager wOCM = MVGameController.Instance.WOCM;
		wOCM.OnHierarchyLockedResponse = (EventHandler<OnHierarchyLockedEventArgs>)Delegate.Combine(wOCM.OnHierarchyLockedResponse, new EventHandler<OnHierarchyLockedEventArgs>(WOCM_OnHierarchyLockedResponse));
		foreach (int selectedID in e.SelectedIDs)
		{
			lockList.Add(selectedID);
			MVGameController.Instance.Game.LockHierarchy(selectedID, lockHierarchy: true);
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
			if (responseReceived)
			{
				CreateGroup(e);
				state = WaitForGroupsState.WaitingForGroup;
				responseReceived = false;
			}
			break;
		case WaitForGroupsState.WaitingForGroup:
		{
			if (!responseReceived)
			{
				break;
			}
			MVWorldObjectClientManager wOCM = MVGameController.Instance.WOCM;
			wOCM.OnTransferWosResponse = (EventHandler<OnTransferWosResponseEventArgs>)Delegate.Combine(wOCM.OnTransferWosResponse, new EventHandler<OnTransferWosResponseEventArgs>(WOCM_OnTransferWosResponse));
			MVGameController.Instance.Game.TransferWorldObjectsToGroup(createGroupId, lockList.ToArray());
			foreach (int @lock in lockList)
			{
				MVGameController.Instance.Game.LockHierarchy(@lock, lockHierarchy: false);
			}
			MVGameController.Instance.Game.TransferOwnership(createGroupId, 0, null);
			state = WaitForGroupsState.WaitingForTransferWos;
			responseReceived = false;
			break;
		}
		case WaitForGroupsState.WaitingForTransferWos:
			if (responseReceived)
			{
				e.SelectWO(createGroupId, addToSelection: false);
				e.LockState = false;
				e.PopState();
			}
			break;
		}
	}

	private void WOCM_OnTransferWosResponse(object sender, OnTransferWosResponseEventArgs e)
	{
		responseReceived = true;
		MVWorldObjectClientManager wOCM = MVGameController.Instance.WOCM;
		wOCM.OnTransferWosResponse = (EventHandler<OnTransferWosResponseEventArgs>)Delegate.Remove(wOCM.OnTransferWosResponse, new EventHandler<OnTransferWosResponseEventArgs>(WOCM_OnTransferWosResponse));
	}

	public void CreateGroup(EditorStateMachine e)
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		List<Transform> list = new List<Transform>();
		foreach (MVWorldObjectClient selectedWO in e.SelectedWOs)
		{
			list.Add(selectedWO.Transform);
		}
		Vector3 worldCenter = SharedCubeFunctions.GetWorldCenter(list);
		World world = MVGameController.Instance.Game.World;
		world.InitializedGameQueryData = (EventHandler<InitializedGameQueryDataEventArgs>)Delegate.Combine(world.InitializedGameQueryData, new EventHandler<InitializedGameQueryDataEventArgs>(WOCM_InitializedGameQueryData));
		MVGameController.Instance.Game.RequestBuiltInItem(BuiltInItem.Group, e.ParentGroupID, new Hashtable(), worldCenter, Quaternion.identity, Vector3.one, localOwner: true, transferOwnershipToServerOnLeave: true);
	}

	private void WOCM_InitializedGameQueryData(object sender, InitializedGameQueryDataEventArgs e)
	{
		if (MVGameController.Instance.Game.LocalPlayerActorNumber == e.InstigatorActorNumber)
		{
			responseReceived = true;
			createGroupId = e.RootWO.Id;
			World world = MVGameController.Instance.Game.World;
			world.InitializedGameQueryData = (EventHandler<InitializedGameQueryDataEventArgs>)Delegate.Remove(world.InitializedGameQueryData, new EventHandler<InitializedGameQueryDataEventArgs>(WOCM_InitializedGameQueryData));
			Debug.Log((object)"Received group");
		}
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
				MVWorldObjectClientManager wOCM = MVGameController.Instance.WOCM;
				wOCM.OnHierarchyLockedResponse = (EventHandler<OnHierarchyLockedEventArgs>)Delegate.Remove(wOCM.OnHierarchyLockedResponse, new EventHandler<OnHierarchyLockedEventArgs>(WOCM_OnHierarchyLockedResponse));
			}
			return;
		}
		foreach (int @lock in lockList)
		{
			MVGameController.Instance.Game.LockHierarchy(@lock, lockHierarchy: false);
		}
		abort = true;
		MVWorldObjectClientManager wOCM2 = MVGameController.Instance.WOCM;
		wOCM2.OnHierarchyLockedResponse = (EventHandler<OnHierarchyLockedEventArgs>)Delegate.Remove(wOCM2.OnHierarchyLockedResponse, new EventHandler<OnHierarchyLockedEventArgs>(WOCM_OnHierarchyLockedResponse));
	}

	public override void Exit(EditorStateMachine e)
	{
	}
}
