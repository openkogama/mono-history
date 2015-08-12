using System.Collections.Generic;

namespace Localize;

public static class LocalizedStringEnumsXP
{
	private static class XPLS
	{
		private static StringLocalizeBookkeeping stringLocalizeBookkeeping = new StringLocalizeBookkeeping(Init);

		public static string Get(string stringVal)
		{
			return stringLocalizeBookkeeping.GetLocalizedString(stringVal);
		}

		private static void Init(Dictionary<string, string> map)
		{
			map.Add("COLLECTIBLE_PICKUP", TM._("XP: Collected star!"));
			map.Add("EditMode60Min", TM._("XP: You've been building for 60 min!"));
			map.Add("AllMaterialsUnlocked", TM._("XP: All materials unlocked!"));
			map.Add("WallJump5", TM._("XP: Parkour!"));
			map.Add("CubeGunCubeAdded100", TM._("XP: CubeGun boss! Added 100 cubes!"));
			map.Add("GameWon", TM._("XP: You won the game!"));
			map.Add("JumpOnOculusEyeOnce", TM._("XP: Oculus jump!"));
			map.Add("OculusKilledInCloseCombat", TM._("XP: Oculus killed in close combat!"));
			map.Add("KillOnOtherTeam", TM._("XP: Killed player on opposing team."));
			map.Add("360ParkourJump", TM._("XP: 360 parkour move."));
			map.Add("HoverCraft360", TM._("XP: HoverCraft 360 jump."));
			map.Add("1MinXpRewardPlayMode", TM._("XP: Playing reward."));
		}
	}

	public static string _(string stringVal)
	{
		return XPLS.Get(stringVal);
	}
}
