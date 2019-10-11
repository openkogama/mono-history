using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine.Events;

public abstract class ARepository
{
	public delegate void OnRepositoryChangeDelegate(ARepository repository);

	public delegate void OnWorldObjectTypeExtracted(WorldObjectType worldObjectType, Dictionary<object, object> woData);

	public OnRepositoryChangeDelegate OnRepositoryChange;

	public Dictionary<int, string> PlanetOwnershipTypes;

	public Dictionary<object, object> itemIDToInventorySlotIndex;

	public ARepository()
	{
		PlanetOwnershipTypes = new Dictionary<int, string>();
		itemIDToInventorySlotIndex = new Dictionary<object, object>();
	}

	public virtual void RemoveItem(int itemId)
	{
		itemIDToInventorySlotIndex.Remove(itemId);
		NotifyRepositoryChange();
	}

	public void SwapItems(int itemId1, int itemId2)
	{
		int num = (int)itemIDToInventorySlotIndex[itemId1];
		itemIDToInventorySlotIndex[itemId1] = (int)itemIDToInventorySlotIndex[itemId2];
		itemIDToInventorySlotIndex[itemId2] = num;
		NotifyRepositoryChange();
	}

	public void MoveItem(int itemId, int slotIndex)
	{
		itemIDToInventorySlotIndex[itemId] = slotIndex;
		NotifyRepositoryChange();
	}

	public void NotifyRepositoryChange()
	{
		if (OnRepositoryChange != null)
		{
			OnRepositoryChange(this);
		}
	}

	public static KoGaMaPackageClient GetKoGaMaPackageFromItem(MVItem item)
	{
		byte[] data = item.data;
		BytePacker koGaMaData = new BytePacker(data);
		KoGaMaPackageClient koGaMaPackageClient = new KoGaMaPackageClient(koGaMaData, readRuntimeValues: false);
		koGaMaPackageClient.InventoryInitialize();
		return koGaMaPackageClient;
	}

	public static void GetWorldObjectTypeFromMVItemData(byte[] data, OnWorldObjectTypeExtracted onWorldObjectExtracted)
	{
		UnityAction<Dictionary<object, object>, KogamaDataType> callBack = (Dictionary<object, object> returnData, KogamaDataType dataType) =>
		{
			if (dataType == KogamaDataType.WorldObjects && returnData.ContainsKey(WorldObjectDataParameters.WorldObjectType))
			{
				WorldObjectType worldObjectType = (WorldObjectType)(int)returnData[WorldObjectDataParameters.WorldObjectType];
				if (onWorldObjectExtracted != null)
				{
					onWorldObjectExtracted(worldObjectType, (Dictionary<object, object>)returnData[WorldObjectDataParameters.Data]);
				}
			}
		};
		BytePacker bp = new BytePacker(data);
		KoGaMaDataHandler.GetKoGaMaData(bp, callBack, readRuntimeData: false);
	}
}
