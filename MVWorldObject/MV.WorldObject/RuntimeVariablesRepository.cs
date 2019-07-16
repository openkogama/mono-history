using System.Collections.Generic;
using MV.Common;

namespace MV.WorldObject;

public static class RuntimeVariablesRepository
{
	private static Dictionary<WorldObjectType, Dictionary<object, object>> runtimeVariables = new Dictionary<WorldObjectType, Dictionary<object, object>>
	{
		{
			WorldObjectType.PlayModeAvatar,
			AvatarRuntimeData()
		},
		{
			WorldObjectType.BuildModeAvatar,
			BuildModeAvatarRuntimeData()
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
			WorldObjectType.CollectibleItem,
			CollectibleRuntimeData()
		}
	};

	public static void SetupRuntimeVariable(WorldObjectType worldObjectType, Dictionary<object, object> targetRuntimeVariables)
	{
		CommonUtils.PartialUpdateHashtable(targetRuntimeVariables, GetRuntimeVariables(worldObjectType));
	}

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
		dictionary.Add("maxHealth", 100);
		dictionary.Add("shield", 0f);
		dictionary.Add("isFiring", false);
		dictionary.Add("modifiers", new Dictionary<object, object>());
		dictionary.Add("currentItem", new Dictionary<object, object> { { "type", 5 } });
		dictionary.Add("lineOfFire", new Dictionary<object, object>());
		dictionary.Add("invulnerable", false);
		dictionary.Add("seat", -1);
		dictionary.Add("spawnRoleModeType", 4);
		dictionary.Add("headRotationYaw", 0f);
		dictionary.Add("headRotationPitch", 0f);
		dictionary.Add("pointRotationYaw", 0f);
		dictionary.Add("pointRotationPitch", 0f);
		dictionary.Add("emote", 0);
		Dictionary<object, object> dictionary2 = new Dictionary<object, object>();
		dictionary2.Add("state", "Idle");
		dictionary2.Add("timeStamp", 0);
		Dictionary<object, object> value = dictionary2;
		dictionary.Add("animation", value);
		return dictionary;
	}

	private static Dictionary<object, object> BuildModeAvatarRuntimeData()
	{
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary.Add("currentItem", new Dictionary<object, object> { { "type", 5 } });
		dictionary.Add("headRotationYaw", 0f);
		dictionary.Add("headRotationPitch", 0f);
		dictionary.Add("pointRotationYaw", 0f);
		dictionary.Add("pointRotationPitch", 0f);
		dictionary.Add("emote", 0);
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
		dictionary.Add("shield", 0f);
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
		dictionary.Add("shield", 0f);
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
		dictionary.Add("shield", 0f);
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
		dictionary.Add("shield", 0f);
		dictionary.Add("modifiers", new Dictionary<object, object>());
		dictionary.Add("isDead", false);
		dictionary.Add("jetMode", (byte)0);
		return dictionary;
	}

	private static Dictionary<object, object> AdvancedGhostRuntimeData()
	{
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary.Add("health", 80f);
		dictionary.Add("shield", 0f);
		dictionary.Add("modifiers", new Dictionary<object, object>());
		dictionary.Add("deathTime", 0);
		return dictionary;
	}

	private static Dictionary<object, object> SentryGun()
	{
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary.Add("health", 300f);
		dictionary.Add("shield", 0f);
		return dictionary;
	}

	private static Dictionary<object, object> CollectibleRuntimeData()
	{
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		dictionary.Add("takenByList", new Dictionary<object, object>());
		return dictionary;
	}
}
