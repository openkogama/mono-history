using System.Collections.Generic;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class TierUnlockAccessItemsPopup : MonoBehaviour
{
	private struct AccessItemData
	{
		public MVWorldObjectDocumentationType type;

		public List<MVWorldObjectClient> worldObjects;

		public MVTeam teamRequirement;
	}

	[SerializeField]
	private Transform itemElementContainer;

	[SerializeField]
	private TierUnlockedItemElement tierUnlockedItemElementPrefab;

	public const string teamKey = "team";

	public void Initialize(GamePassTier tier, Dictionary<MVWorldObjectDocumentationType, List<MVWorldObjectClient>> tierShopData)
	{
		List<AccessItemData> sortedData = GetSortedData(tierShopData);
		for (int i = 0; i < sortedData.Count; i++)
		{
			TierUnlockedItemElement tierUnlockedItemElement = Object.Instantiate(tierUnlockedItemElementPrefab);
			tierUnlockedItemElement.transform.SetParent(itemElementContainer, worldPositionStays: false);
			tierUnlockedItemElement.SetTeam(sortedData[i].teamRequirement);
			tierUnlockedItemElement.Initialize(sortedData[i].worldObjects, i);
		}
	}

	private List<AccessItemData> GetSortedData(Dictionary<MVWorldObjectDocumentationType, List<MVWorldObjectClient>> tierShopData)
	{
		List<AccessItemData> list = new List<AccessItemData>();
		foreach (KeyValuePair<MVWorldObjectDocumentationType, List<MVWorldObjectClient>> tierShopDatum in tierShopData)
		{
			List<MVWorldObjectClient> value = tierShopDatum.Value;
			for (int i = 0; i < value.Count; i++)
			{
				MVTeam mVTeam = MVTeam.None;
				if (value[i].Data.ContainsKey("team"))
				{
					mVTeam = (MVTeam)value[i].Data["team"];
				}
				if (!HasAccessItemDataTeamAndObjectType(mVTeam, tierShopDatum.Key, list))
				{
					AccessItemData item = CreateAccessItemData(mVTeam, tierShopDatum.Key);
					list.Add(item);
				}
				for (int j = 0; j < list.Count; j++)
				{
					if (list[j].teamRequirement == mVTeam && list[j].type == tierShopDatum.Key)
					{
						list[j].worldObjects.Add(value[i]);
					}
				}
			}
		}
		return list;
	}

	private bool HasAccessItemDataTeamAndObjectType(MVTeam teamRequirement, MVWorldObjectDocumentationType objectType, List<AccessItemData> accessItemsData)
	{
		for (int i = 0; i < accessItemsData.Count; i++)
		{
			if (accessItemsData[i].teamRequirement == teamRequirement && accessItemsData[i].type == objectType)
			{
				return true;
			}
		}
		return false;
	}

	private AccessItemData CreateAccessItemData(MVTeam teamRequirement, MVWorldObjectDocumentationType objectType)
	{
		return new AccessItemData
		{
			worldObjects = new List<MVWorldObjectClient>(),
			teamRequirement = teamRequirement,
			type = objectType
		};
	}
}
