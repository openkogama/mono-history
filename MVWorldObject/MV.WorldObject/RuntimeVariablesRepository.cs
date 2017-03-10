using System.Collections.Generic;

namespace MV.WorldObject;

public static class RuntimeVariablesRepository
{
	private static Dictionary<WorldObjectType, Dictionary<object, object>> runtimeVariables = new Dictionary<WorldObjectType, Dictionary<object, object>>
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
		},
		{
			WorldObjectType.HamsterWheel,
			HamsterWheelRuntimeData()
		},
		{
			WorldObjectType.GodzillaTrigger,
			GodzillaTriggerRuntimeData()
		}
	};

	public static Dictionary<object, object> GetRuntimeVariables(WorldObjectType worldObjectType)
	{
		if (!runtimeVariables.ContainsKey(worldObjectType))
		{
			return new Dictionary<object, object>();
		}
		return HashtableFunctions.DeepCopyHashTable(runtimeVariables[worldObjectType]);
	}

	private static Dictionary<object, object> AvatarRuntimeData()
	{
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary.Add("health", 100f);
		dictionary.Add("isFiring", false);
		dictionary.Add("modifiers", new Dictionary<object, object>());
		dictionary.Add("currentItem", new Dictionary<object, object> { { "type", 5 } });
		dictionary.Add("lineOfFire", new Dictionary<object, object>());
		dictionary.Add("invulnerable", false);
		dictionary.Add("seat", -1);
		dictionary.Add("avatarModeTypes", 4);
		Dictionary<object, object> dictionary2 = new Dictionary<object, object>();
		dictionary2.Add("state", "Idle");
		dictionary2.Add("timeStamp", 0);
		Dictionary<object, object> value = dictionary2;
		dictionary.Add("animation", value);
		return dictionary;
	}

	private static Dictionary<object, object> HoverCraftRuntimeData()
	{
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary.Add("health", 150f);
		dictionary.Add("isFiring", false);
		dictionary.Add("modifiers", new Dictionary<object, object>());
		dictionary.Add("currentItem", new Dictionary<object, object>());
		dictionary.Add("isDead", false);
		return dictionary;
	}

	private static Dictionary<object, object> HamsterWheelRuntimeData()
	{
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary.Add("health", 150f);
		dictionary.Add("isFiring", false);
		dictionary.Add("modifiers", new Dictionary<object, object>());
		dictionary.Add("currentItem", new Dictionary<object, object>());
		dictionary.Add("isDead", false);
		dictionary.Add("isMovingForward", false);
		dictionary.Add("isMovingBackwards", false);
		dictionary.Add("isGrounded", false);
		return dictionary;
	}

	private static Dictionary<object, object> MonoPlaneRuntimeData()
	{
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary.Add("health", 80f);
		dictionary.Add("isFiring", false);
		dictionary.Add("modifiers", new Dictionary<object, object>());
		dictionary.Add("currentItem", new Dictionary<object, object>());
		dictionary.Add("isDead", false);
		return dictionary;
	}

	private static Dictionary<object, object> JetPackRuntimeData()
	{
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary.Add("health", 20f);
		dictionary.Add("modifiers", new Dictionary<object, object>());
		dictionary.Add("isDead", false);
		dictionary.Add("jetMode", (byte)0);
		return dictionary;
	}

	private static Dictionary<object, object> AdvancedGhostRuntimeData()
	{
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary.Add("health", 80f);
		dictionary.Add("modifiers", new Dictionary<object, object>());
		dictionary.Add("deathTime", 0);
		return dictionary;
	}

	private static Dictionary<object, object> SentryGun()
	{
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary.Add("health", 300f);
		return dictionary;
	}

	private static Dictionary<object, object> GodzillaTriggerRuntimeData()
	{
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary.Add("occupantWOID", -1);
		return dictionary;
	}
}
