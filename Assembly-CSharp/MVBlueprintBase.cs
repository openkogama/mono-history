using System.Collections;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class MVBlueprintBase : MVGroup
{
	protected Hashtable blueprintData;

	protected Hashtable childIdMap;

	protected Hashtable idChildMap = new Hashtable();

	public MVBlueprintBase(Hashtable data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, worldObjects)
	{
		MapDataToFields();
	}

	public MVBlueprintBase(Hashtable data, string prefabPath, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, prefabPath, worldObjects)
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
			Debug.LogError((object)"Trying to compare different types of WorldObjects");
		}
		if (!(wo is MVBlueprintBase))
		{
			Debug.LogError((object)"Not a blue print");
		}
		MVBlueprintBase mVBlueprintBase = (MVBlueprintBase)wo;
		Hashtable hashtable = (Hashtable)Data["BlueprintData"];
		Hashtable hashtable2 = (Hashtable)hashtable["ChildrenMap"];
		foreach (DictionaryEntry item in hashtable2)
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
		if ((object)wo.GetType() != GetType())
		{
			Debug.LogError((object)$"Types does not match this {GetType()} and other {wo.GetType()}");
			return false;
		}
		MVBlueprintBase mVBlueprintBase = (MVBlueprintBase)wo;
		foreach (string compareChild in compareChildren)
		{
			MVWorldObjectClient child = mVBlueprintBase.GetChild(compareChild);
			if (child == null)
			{
				Debug.LogError((object)$"otherChild {compareChild} is null");
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
		if (!Data.Contains("BlueprintData"))
		{
			Debug.LogError((object)"No blueprint data");
			return;
		}
		blueprintData = (Hashtable)Data["BlueprintData"];
		childIdMap = (Hashtable)blueprintData["ChildrenMap"];
		foreach (DictionaryEntry item in childIdMap)
		{
			idChildMap[item.Value] = item.Key;
		}
	}

	public override MVWorldObjectClient Clone(int ownerActorNumber, int cloneGroupId, CloneBookkeeping cloneBookkeeping, Dictionary<int, MVWorldObjectClient> worldObjects, Dictionary<int, RuntimePrototypeCubeModel> prototypes)
	{
		MVBlueprintBase mVBlueprintBase = (MVBlueprintBase)base.Clone(ownerActorNumber, cloneGroupId, cloneBookkeeping, worldObjects, prototypes);
		if (mVBlueprintBase.blueprintData != null)
		{
			Hashtable hashtable = mVBlueprintBase.blueprintData;
			if (hashtable.Contains(BlueprintData.ChildrenMap.ToString()))
			{
				Hashtable hashtable2 = (Hashtable)hashtable[BlueprintData.ChildrenMap.ToString()];
				Hashtable hashtable3 = new Hashtable();
				List<object> list = new List<object>();
				foreach (object key in hashtable2.Keys)
				{
					list.Add(key);
				}
				foreach (object item in list)
				{
					int num = cloneBookkeeping.worldObjectIdsMaps[(int)hashtable2[item]];
					hashtable2[item] = num;
					hashtable3[num] = item;
				}
				mVBlueprintBase.idChildMap = hashtable3;
			}
			else
			{
				Debug.LogError((object)"No children map");
			}
		}
		else
		{
			Debug.LogError((object)"No blueprint data");
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
			Debug.LogError((object)$"Child with name {child} not found in {this}");
		}
		return result;
	}

	public override void OnDataUpdate()
	{
		base.OnDataUpdate();
		MapDataToFields();
	}
}
