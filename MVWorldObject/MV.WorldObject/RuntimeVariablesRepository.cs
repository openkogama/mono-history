using System.Collections;
using System.Collections.Generic;

namespace MV.WorldObject;

public static class RuntimeVariablesRepository
{
	private static Dictionary<WorldObjectType, Hashtable> runtimeVariables = new Dictionary<WorldObjectType, Hashtable>
	{
		{
			WorldObjectType.Avatar,
			AvatarRuntimeData()
		},
		{
			WorldObjectType.HoverCraft,
			HoverCraftRuntimeData()
		},
		{
			WorldObjectType.MonoPlane,
			MonoPlaneRuntimeData()
		},
		{
			WorldObjectType.JetPack,
			JetPackRuntimeData()
		},
		{
			WorldObjectType.AdvancedGhost,
			AdvancedGhostRuntimeData()
		},
		{
			WorldObjectType.SentryGun,
			SentryGun()
		}
	};

	public static Hashtable GetRuntimeVariables(WorldObjectType worldObjectType)
	{
		if (!runtimeVariables.ContainsKey(worldObjectType))
		{
			return new Hashtable();
		}
		return HashtableFunctions.DeepCopyHashTable(runtimeVariables[worldObjectType]);
	}

	private static Hashtable AvatarRuntimeData()
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add("health", 100f);
		hashtable.Add("isFiring", false);
		hashtable.Add("modifiers", new Hashtable());
		hashtable.Add("currentItem", new Hashtable { { "type", 5 } });
		hashtable.Add("lineOfFire", new Hashtable());
		hashtable.Add("invulnerable", false);
		hashtable.Add("seat", -1);
		hashtable.Add("collectibleCount", 0);
		Hashtable hashtable2 = new Hashtable();
		hashtable2.Add("state", "Idle");
		hashtable2.Add("timeStamp", 0);
		Hashtable value = hashtable2;
		hashtable.Add("animation", value);
		return hashtable;
	}

	private static Hashtable HoverCraftRuntimeData()
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add("health", 150f);
		hashtable.Add("isFiring", false);
		hashtable.Add("modifiers", new Hashtable());
		hashtable.Add("currentItem", new Hashtable());
		hashtable.Add("isDead", false);
		return hashtable;
	}

	private static Hashtable MonoPlaneRuntimeData()
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add("health", 80f);
		hashtable.Add("isFiring", false);
		hashtable.Add("modifiers", new Hashtable());
		hashtable.Add("currentItem", new Hashtable());
		hashtable.Add("isDead", false);
		return hashtable;
	}

	private static Hashtable JetPackRuntimeData()
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add("health", 20f);
		hashtable.Add("modifiers", new Hashtable());
		hashtable.Add("isDead", false);
		hashtable.Add("jetMode", (byte)0);
		return hashtable;
	}

	private static Hashtable AdvancedGhostRuntimeData()
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add("health", 80f);
		hashtable.Add("modifiers", new Hashtable());
		hashtable.Add("deathTime", 0);
		return hashtable;
	}

	private static Hashtable SentryGun()
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add("health", 300f);
		return hashtable;
	}
}
