using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MV.WorldObject;
using UnityEngine;

internal class ESBlueprintCreator : ESStateBase
{
	private enum WaitForGroupsState
	{
		WaitingForLock,
		WaitingForGroup,
		WaitingForTransferWos
	}

	public static WorldObjectType worldObjectTypeToBeCreated = WorldObjectType.Blueprint;

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
		foreach (int value in hashtable2.Values)
		{
			if (MVGameController.Instance.WOCM.GetWorldObjectClient(value) == null)
			{
				Debug.Log((object)("Trying to add a blueprint with non-existant child-id " + value + ". Aborting"));
				e.LockState = false;
				e.PopState();
				return;
			}
		}
		lockList = new List<int>();
		state = WaitForGroupsState.WaitingForLock;
		MVWorldObjectClientManager wOCM = MVGameController.Instance.WOCM;
		wOCM.OnHierarchyLockedResponse = (EventHandler<OnHierarchyLockedEventArgs>)Delegate.Combine(wOCM.OnHierarchyLockedResponse, new EventHandler<OnHierarchyLockedEventArgs>(WOCM_OnHierarchyLockedResponse));
		foreach (int value2 in hashtable2.Values)
		{
			lockList.Add(value2);
			MVGameController.Instance.Game.LockHierarchy(value2, lockHierarchy: true);
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
			wOCM.OnTransferWosResponse = (EventHandler<OnTransferWosResponseEventArgs>)Delegate.Remove(wOCM.OnTransferWosResponse, new EventHandler<OnTransferWosResponseEventArgs>(WOCM_OnTransferWosResponse));
			MVWorldObjectClientManager wOCM2 = MVGameController.Instance.WOCM;
			wOCM2.OnTransferWosResponse = (EventHandler<OnTransferWosResponseEventArgs>)Delegate.Combine(wOCM2.OnTransferWosResponse, new EventHandler<OnTransferWosResponseEventArgs>(WOCM_OnTransferWosResponse));
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

	private void CreateGroup(EditorStateMachine e)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		List<MVWorldObjectClient> woList = lockList.Select((int woId) => MVGameController.Instance.WOCM.GetWorldObjectClient(woId)).ToList();
		Bounds val = MVGroup.ComputeBoundsForWOs(woList, BoundsContext.Default);
		float gridSize = ((!AEditController.IsGridSnap()) ? 0.0625f : 1f);
		Vector3 closestGridPoint = SharedCubeFunctions.GetClosestGridPoint(val.center, Quaternion.identity, gridSize, Vector3.one);
		World world = MVGameController.Instance.Game.World;
		world.InitializedGameQueryData = (EventHandler<InitializedGameQueryDataEventArgs>)Delegate.Remove(world.InitializedGameQueryData, new EventHandler<InitializedGameQueryDataEventArgs>(WOCM_InitializedGameQueryData));
		World world2 = MVGameController.Instance.Game.World;
		world2.InitializedGameQueryData = (EventHandler<InitializedGameQueryDataEventArgs>)Delegate.Combine(world2.InitializedGameQueryData, new EventHandler<InitializedGameQueryDataEventArgs>(WOCM_InitializedGameQueryData));
		MVGameController.Instance.Game.RegisterWorldObject(worldObjectTypeToBeCreated, e.ParentGroupID, woData, closestGridPoint, Quaternion.identity, Vector3.one, localOwner: true, transferOwnershipToServerOnLeave: true);
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
