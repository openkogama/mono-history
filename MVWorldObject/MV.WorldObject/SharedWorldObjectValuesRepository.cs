using System.Collections;
using System.Collections.Generic;

namespace MV.WorldObject;

public static class SharedWorldObjectValuesRepository
{
	private static Dictionary<WorldObjectType, Hashtable> values = new Dictionary<WorldObjectType, Hashtable>
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

	public static Hashtable GetValues(WorldObjectType worldObjectType)
	{
		if (!values.ContainsKey(worldObjectType))
		{
			return new Hashtable();
		}
		return HashtableFunctions.DeepCopyHashTable(values[worldObjectType]);
	}

	private static Hashtable AdvancedGhostData()
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add("RespawnInterval", 15000);
		return hashtable;
	}

	private static Hashtable SentryGun()
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add("RespawnInterval", 15000);
		return hashtable;
	}
}
