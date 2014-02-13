using System;
using System.Collections;
using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

internal class ESBodyCreator : ESStateBase
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

	private Hashtable woData;

	public override void Enter(EditorStateMachine e)
	{
		woData = (Hashtable)e.Data["woData"];
		Hashtable hashtable = (Hashtable)woData["BlueprintData"];
		Hashtable hashtable2 = (Hashtable)hashtable["ChildrenMap"];
		e.LockState = true;
		if (hashtable2.Count == 0)
		{
			e.LockState = false;
			e.PopState();
			return;
		}
		lockList = new List<int>();
		state = WaitForGroupsState.WaitingForLock;
		MVWorldObjectClientManager wOCM = MVGameController.Instance.WOCM;
		wOCM.OnHierarchyLockedResponse = (EventHandler<OnHierarchyLockedEventArgs>)Delegate.Combine(wOCM.OnHierarchyLockedResponse, new EventHandler<OnHierarchyLockedEventArgs>(WOCM_OnHierarchyLockedResponse));
		foreach (int value in hashtable2.Values)
		{
			lockList.Add(value);
			MVGameController.Instance.Game.LockHierarchy(value, lockHierarchy: true);
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

	private void WOCM_OnTransferWosToAvatarResponse(object sender, OnTransferWosResponseEventArgs e)
	{
		responseReceived = true;
		MVWorldObjectClientManager wOCM = MVGameController.Instance.WOCM;
		wOCM.OnTransferWosResponse = (EventHandler<OnTransferWosResponseEventArgs>)Delegate.Remove(wOCM.OnTransferWosResponse, new EventHandler<OnTransferWosResponseEventArgs>(WOCM_OnTransferWosToAvatarResponse));
	}

	public void CreateGroup(EditorStateMachine e)
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		World world = MVGameController.Instance.Game.World;
		world.InitializedGameQueryData = (EventHandler<InitializedGameQueryDataEventArgs>)Delegate.Combine(world.InitializedGameQueryData, new EventHandler<InitializedGameQueryDataEventArgs>(WOCM_InitializedGameQueryData));
		int id = MVGameController.Instance.WOCM.RootGroup.Id;
		MVGameController.Instance.Game.RegisterWorldObject(WorldObjectType.Blueprint, id, woData, new Vector3(-6f, 12f, -4f), Quaternion.identity, Vector3.one, localOwner: true, transferOwnershipToServerOnLeave: true);
	}

	private void WOCM_InitializedGameQueryData(object sender, InitializedGameQueryDataEventArgs e)
	{
		if (MVGameController.Instance.Game.LocalPlayerActorNumber == e.InstigatorActorNumber)
		{
			Debug.Log((object)"Received register response");
			responseReceived = true;
			createGroupId = e.RootWO.Id;
			World world = MVGameController.Instance.Game.World;
			world.InitializedGameQueryData = (EventHandler<InitializedGameQueryDataEventArgs>)Delegate.Remove(world.InitializedGameQueryData, new EventHandler<InitializedGameQueryDataEventArgs>(WOCM_InitializedGameQueryData));
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
