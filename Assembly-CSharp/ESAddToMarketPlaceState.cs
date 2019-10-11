using MV.Common;
using MV.WorldObject;
using UnityEngine;

internal class ESAddToMarketPlaceState : ESStateBase
{
	private enum AddToMarketPlaceInternalState
	{
		None,
		WaitingForMarketPlaceItem,
		CompareMarketPlaceItemWithInventoryItem,
		WaitingForMarketPlaceInfo
	}

	private AddToMarketPlaceInternalState internalState;

	private BytePacker inventoryItemData;

	private BytePacker marketPlaceItemData;

	public override void Enter(EditorStateMachine e)
	{
		Debug.Log("ESAddToMarketPlaceState");
		int num = (int)e.Data["ItemID"];
		if (!MVGameControllerBase.Game.PlayerRepository.PlayerInventory.TryGetValue(num, out var value))
		{
			Debug.LogError("Item not found");
			e.PopState();
			return;
		}
		if (value.authorProfileID == MVGameControllerBase.Game.LocalPlayer.ProfileID)
		{
			Debug.Log("Is already authorprofile. Skip to pricing, description and naming");
			e.PopState();
			return;
		}
		if (!value.resellable)
		{
			Debug.LogError("Is not resellable");
			e.PopState();
			return;
		}
		inventoryItemData = new BytePacker(value.data);
		internalState = AddToMarketPlaceInternalState.WaitingForMarketPlaceItem;
		MVGameControllerBase.Game.ReceivedItemFromQuery += WOCM_ReceivedItemFromQuery;
		MVGameControllerBase.OperationRequests.RequestMarketPlaceItem(num);
	}

	public override void Execute(EditorStateMachine e)
	{
		switch (internalState)
		{
		case AddToMarketPlaceInternalState.WaitingForMarketPlaceItem:
			if (marketPlaceItemData != null)
			{
				internalState = AddToMarketPlaceInternalState.CompareMarketPlaceItemWithInventoryItem;
			}
			break;
		case AddToMarketPlaceInternalState.CompareMarketPlaceItemWithInventoryItem:
		{
			KoGaMaPackageClient koGaMaPackageClient = new KoGaMaPackageClient(inventoryItemData, readRuntimeValues: false);
			koGaMaPackageClient.InventoryInitialize();
			KoGaMaPackageClient koGaMaPackageClient2 = new KoGaMaPackageClient(marketPlaceItemData, readRuntimeValues: false);
			koGaMaPackageClient2.InventoryInitialize();
			float num = KoGaMaPackageClient.Compare(koGaMaPackageClient2, koGaMaPackageClient);
			if (num <= CommonValues.CompareThreshold)
			{
				Debug.Log($"Compare val {num} <= threshold {CommonValues.CompareThreshold}. This item can be added to your shop");
				internalState = AddToMarketPlaceInternalState.WaitingForMarketPlaceInfo;
			}
			else
			{
				Debug.Log($"Compare val {num} > threshold {CommonValues.CompareThreshold}. This item can not be added to your shop");
				e.PopState();
			}
			koGaMaPackageClient.Destroy();
			koGaMaPackageClient2.Destroy();
			break;
		}
		case AddToMarketPlaceInternalState.WaitingForMarketPlaceInfo:
			Debug.Log(AddToMarketPlaceInternalState.WaitingForMarketPlaceInfo);
			break;
		}
	}

	private void WOCM_ReceivedItemFromQuery(object sender, ReceivedItemFromQueryEventArgs e)
	{
		MVGameControllerBase.Game.ReceivedItemFromQuery -= WOCM_ReceivedItemFromQuery;
		marketPlaceItemData = e.KoGaMaData;
	}

	public override void Exit(EditorStateMachine e)
	{
	}
}
