using System.Collections.Generic;

namespace MV.WorldObject;

public static class SharedWorldObjectValuesRepository
{
	private static Dictionary<WorldObjectType, Dictionary<object, object>> values = new Dictionary<WorldObjectType, Dictionary<object, object>>
	{
		{
			WorldObjectType.AdvancedGhost,
			AdvancedGhostData()
		},
		{
			WorldObjectType.SentryGun,
			SentryGun()
		}
	};

	public static Dictionary<object, object> GetValues(WorldObjectType worldObjectType)
	{
		if (!values.ContainsKey(worldObjectType))
		{
			return new Dictionary<object, object>();
		}
		return HashtableFunctions.DeepCopyHashTable(values[worldObjectType]);
	}

	private static Dictionary<object, object> AdvancedGhostData()
	{
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary.Add("RespawnInterval", 15000);
		return dictionary;
	}

	private static Dictionary<object, object> SentryGun()
	{
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary.Add("RespawnInterval", 15000);
		return dictionary;
	}
}
