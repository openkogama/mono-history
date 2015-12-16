using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class MVBlueprintBase : MVGroup
{
	protected Dictionary<object, object> blueprintData;

	protected Dictionary<object, object> childIdMap;

	protected Dictionary<object, object> idChildMap = new Dictionary<object, object>();

	public MVBlueprintBase(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, worldObjects)
	{
		MapDataToFields();
	}

	public MVBlueprintBase(Dictionary<object, object> data, GameObject prefabObject, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, prefabObject, worldObjects)
	{
		MapDataToFields();
	}

	public override void Initialize()
	{
		base.Initialize();
		foreach (MVWorldObjectClient child in Children)
		{
			if (child is MVCubeModelInstance && (interactionFlags & InteractionFlags.CanAddToInventory) != 0 && (child.InteractionFlags & InteractionFlags.CanAddToInventory) == 0)
			{
				interactionFlags &= ~InteractionFlags.CanAddToInventory;
			}
			child.InteractionFlags &= ~InteractionFlags.CanAddToInventory;
		}
	}

	public override void Compare(MVWorldObjectClient wo, bool visibleCubesOnly, ref int matchingCubeCount, ref int investigatedCubeCount)
	{
		if (wo.WorldObjectType != WorldObjectType)
		{
			Debug.LogError("Trying to compare different types of WorldObjects");
		}
		if (!(wo is MVBlueprintBase))
		{
			Debug.LogError("Not a blue print");
		}
		MVBlueprintBase mVBlueprintBase = (MVBlueprintBase)wo;
		Dictionary<object, object> dictionary = (Dictionary<object, object>)Data["BlueprintData"];
		Dictionary<object, object> dictionary2 = (Dictionary<object, object>)dictionary["ChildrenMap"];
		foreach (KeyValuePair<object, object> item in dictionary2)
		{
			MVWorldObjectClient child = GetChild((string)item.Key);
			MVWorldObjectClient child2 = mVBlueprintBase.GetChild((string)item.Key);
			if (child2 != null && child2.WorldObjectType == child.WorldObjectType)
			{
				child.Compare(child2, visibleCubesOnly, ref matchingCubeCount, ref investigatedCubeCount);
			}
		}
	}

	public override bool CompareWithKoGaMaPackage(MVWorldObjectClient wo, KoGaMaPackageClient koGaMaPackageClient, ref int insertedByProfileId)
	{
		List<string> list = new List<string>();
		foreach (string key in childIdMap.Keys)
		{
			list.Add(key);
		}
		return CompareWorldObjectsInChildrenMap(wo, koGaMaPackageClient, list, ref insertedByProfileId);
	}

	protected bool CompareWorldObjectsInChildrenMap(MVWorldObjectClient wo, KoGaMaPackageClient koGaMaPackageClient, List<string> compareChildren, ref int insertedByProfileId)
	{
		if (wo.GetType() != GetType())
		{
			Debug.LogError($"Types does not match this {GetType()} and other {wo.GetType()}");
			return false;
		}
		MVBlueprintBase mVBlueprintBase = (MVBlueprintBase)wo;
		foreach (string compareChild in compareChildren)
		{
			MVWorldObjectClient child = mVBlueprintBase.GetChild(compareChild);
			if (child == null)
			{
				Debug.LogError($"otherChild {compareChild} is null");
				return false;
			}
			MVWorldObjectClient child2 = GetChild(compareChild);
			if (!child2.CompareWithKoGaMaPackage(child, koGaMaPackageClient, ref insertedByProfileId))
			{
				return false;
			}
		}
		return true;
	}

	private void MapDataToFields()
	{
		if (!Data.ContainsKey("BlueprintData"))
		{
			Debug.LogError("No blueprint data");
			return;
		}
		blueprintData = (Dictionary<object, object>)Data["BlueprintData"];
		childIdMap = (Dictionary<object, object>)blueprintData["ChildrenMap"];
		foreach (KeyValuePair<object, object> item in childIdMap)
		{
			idChildMap[item.Value] = item.Key;
		}
	}

	public override MVWorldObjectClient Clone(int ownerActorNumber, int cloneGroupId, CloneBookkeeping cloneBookkeeping, Dictionary<int, MVWorldObjectClient> worldObjects, Dictionary<int, RuntimePrototypeCubeModel> prototypes)
	{
		MVBlueprintBase mVBlueprintBase = (MVBlueprintBase)base.Clone(ownerActorNumber, cloneGroupId, cloneBookkeeping, worldObjects, prototypes);
		if (mVBlueprintBase.blueprintData != null)
		{
			Dictionary<object, object> dictionary = mVBlueprintBase.blueprintData;
			if (dictionary.ContainsKey(BlueprintData.ChildrenMap.ToString()))
			{
				Dictionary<object, object> dictionary2 = (Dictionary<object, object>)dictionary[BlueprintData.ChildrenMap.ToString()];
				Dictionary<object, object> dictionary3 = new Dictionary<object, object>();
				List<object> list = new List<object>();
				foreach (object key in dictionary2.Keys)
				{
					list.Add(key);
				}
				foreach (object item in list)
				{
					int num = cloneBookkeeping.worldObjectIdsMaps[(int)dictionary2[item]];
					dictionary2[item] = num;
					dictionary3[num] = item;
				}
				mVBlueprintBase.idChildMap = dictionary3;
			}
			else
			{
				Debug.LogError("No children map");
			}
		}
		else
		{
			Debug.LogError("No blueprint data");
		}
		return mVBlueprintBase;
	}

	public MVWorldObjectClient GetChild(string child)
	{
		MVWorldObjectClient result = null;
		if (childIdMap.ContainsKey(child))
		{
			int woID = (int)childIdMap[child];
			result = GetChild(woID);
		}
		else
		{
			Debug.LogError($"Child with name {child} not found in {this}");
		}
		return result;
	}

	public override void OnDataUpdate()
	{
		base.OnDataUpdate();
		MapDataToFields();
	}
}
