using System.Collections.Generic;
using MV.WorldObject;

public class MVNetworkSelector
{
	private readonly EditorStateMachine esm;

	private Queue<int> pendingRequestedOwnershipIds = new Queue<int>();

	public MVNetworkSelector(EditorStateMachine esm)
	{
		this.esm = esm;
		MVGameController.Instance.WOCM.OnWorldObjectTransferOwnershipResponse += Instance_OnWorldObjectTransferOwnershipResponse;
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

	public bool RequestOwnershipRecursive(int id)
	{
		if (pendingRequestedOwnershipIds.Count != 0)
		{
			return false;
		}
		if (!CanRequestOwnerShip(id))
		{
			return false;
		}
		RequestOwnershipRecursiveInternal(id);
		return true;
	}

	private void RequestOwnershipRecursiveInternal(int id)
	{
		RequestOwnership(id);
		if ((object)MVGameController.Instance.WOCM.GetWorldObjectClient(id).GetType() != typeof(MVGroup))
		{
			return;
		}
		foreach (MVWorldObjectClient child in ((MVGroup)MVGameController.Instance.WOCM.GetWorldObjectClient(id)).Children)
		{
			RequestOwnershipRecursiveInternal(child.Id);
		}
	}

	public bool CanRequestOwnerShip(int id)
	{
		if (!OwnershipTest(id))
		{
			return false;
		}
		if ((object)MVGameController.Instance.WOCM.GetWorldObjectClient(id).GetType() == typeof(MVGroup))
		{
			foreach (MVWorldObjectClient child in ((MVGroup)MVGameController.Instance.WOCM.GetWorldObjectClient(id)).Children)
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
		if (MVGameController.Instance.WOCM.GetWorldObjectClient(id).OwnerActorNr != 0 && MVGameController.Instance.WOCM.GetWorldObjectClient(id).OwnerActorNr != MVGameController.Instance.WOCM.LocalPlayer.ActorNr)
		{
			return false;
		}
		return true;
	}

	private void RequestOwnership(int id)
	{
		if (MVGameController.Instance.WOCM.GetWorldObjectClient(id).OwnerActorNr != MVGameController.Instance.WOCM.LocalPlayer.ActorNr && MVGameController.Instance.WOCM.GetWorldObjectClient(id).OwnerActorNr == 0)
		{
			pendingRequestedOwnershipIds.Enqueue(id);
			MVGameController.Instance.Game.TransferOwnership(id, MVGameController.Instance.WOCM.LocalPlayer.ActorNr);
		}
	}

	private static void RequestReleaseOwnership(int id)
	{
		MVGameController.Instance.Game.TransferOwnership(id, 0);
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
				esm.Event = EditorEvent.EditCubes;
			}
		}
	}
}
