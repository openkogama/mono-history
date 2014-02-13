using System;
using System.Collections.Generic;
using MV.WorldObject;

public class MVNetworkSelector
{
	private readonly EditorStateMachine esm;

	private Queue<int> pendingRequestedOwnershipIds = new Queue<int>();

	private static MVWorldObjectClientManager WOCM => MVGameController.Instance.WOCM;

	public MVNetworkSelector(EditorStateMachine esm)
	{
		this.esm = esm;
		MVWorldObjectClientManager wOCM = WOCM;
		wOCM.OnWorldObjectTransferOwnershipResponse = (EventHandler<OnTransferOwnershipResponseEventArgs>)Delegate.Combine(wOCM.OnWorldObjectTransferOwnershipResponse, new EventHandler<OnTransferOwnershipResponseEventArgs>(Instance_OnWorldObjectTransferOwnershipResponse));
	}

	public bool RequestOwnership(HashSet<int> selectionSet)
	{
		if (!CanRequestOwnership(selectionSet))
		{
			return false;
		}
		foreach (int item in selectionSet)
		{
			RequestOwnership(item);
		}
		return true;
	}

	public bool CanRequestOwnerShip(int id)
	{
		if (!OwnershipTest(id))
		{
			return false;
		}
		if (WOCM.GetWorldObjectClient(id) is MVGroup mVGroup)
		{
			foreach (MVWorldObjectClient child in mVGroup.Children)
			{
				if (!CanRequestOwnerShip(child.Id))
				{
					return false;
				}
			}
		}
		return true;
	}

	public void RequestReleaseOwnership(HashSet<int> selectionSet)
	{
		foreach (int item in selectionSet)
		{
			RequestReleaseOwnership(item);
		}
	}

	private static bool CanRequestOwnership(HashSet<int> selectionSet)
	{
		foreach (int item in selectionSet)
		{
			if (!OwnershipTest(item))
			{
				return false;
			}
		}
		return true;
	}

	private static bool OwnershipTest(int id)
	{
		MVWorldObjectClient worldObjectClient = WOCM.GetWorldObjectClient(id);
		if (worldObjectClient != null && worldObjectClient.OwnerActorNr != 0 && worldObjectClient.OwnerActorNr != MVGameController.Instance.Game.LocalPlayer.ActorNr)
		{
			return false;
		}
		return true;
	}

	private void RequestOwnership(int id)
	{
		MVWorldObjectClient worldObjectClient = WOCM.GetWorldObjectClient(id);
		if ((worldObjectClient == null || worldObjectClient.OwnerActorNr != MVGameController.Instance.Game.LocalPlayer.ActorNr) && WOCM.GetWorldObjectClient(id).OwnerActorNr == 0)
		{
			pendingRequestedOwnershipIds.Enqueue(id);
			MVGameController.Instance.Game.TransferOwnership(id, MVGameController.Instance.Game.LocalPlayer.ActorNr, null);
		}
	}

	private static void RequestReleaseOwnership(int id)
	{
		MVWorldObjectClient worldObjectClient = WOCM.GetWorldObjectClient(id);
		MVGameController.Instance.Game.TransferOwnership(id, 0, worldObjectClient.Transform);
	}

	private void Instance_OnWorldObjectTransferOwnershipResponse(object sender, OnTransferOwnershipResponseEventArgs e)
	{
		if (e.ownerActorNr != 0 && e.success)
		{
			int id = pendingRequestedOwnershipIds.Dequeue();
			MVWorldObjectClient worldObjectClient = MVGameController.Instance.WOCM.GetWorldObjectClient(id);
			worldObjectClient.State = MVWorldObjectState.Dirty;
		}
		else if (e.ownerActorNr != 0 && !e.success)
		{
			int num = pendingRequestedOwnershipIds.Dequeue();
			if (pendingRequestedOwnershipIds.Count == 0)
			{
				esm.Event = EditorEvent.ESTerrainEdit;
			}
		}
	}
}
