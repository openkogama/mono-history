using System.Collections.Generic;
using MV.Common;
using UnityEngine;
using UnityEngine.UI;

public class KillNotification : Notification
{
	[SerializeField]
	private Text Label;

	[SerializeField]
	private Image Background;

	protected override NotificationLifetime Lifetime => NotificationLifetime.Low;

	public override void Initialize(Dictionary<object, object> data)
	{
		base.Initialize(data);
		MVPlayer mVPlayer = MVGameControllerBase.Game.MVPlayerContainer[(int)data[(byte)6]];
		MVPlayer mVPlayer2 = MVGameControllerBase.Game.MVPlayerContainer[(int)data[(byte)7]];
		bool shotSelf = false;
		if (mVPlayer2.UserProfileData.UserName == mVPlayer.UserProfileData.UserName)
		{
			shotSelf = true;
		}
		Color color;
		Color color2;
		if (MVGameControllerBase.Game.TeamManager.GetTeamList().Count > 1)
		{
			color = Styles.GetTeamColor(mVPlayer2.Team);
			color2 = Styles.GetTeamColor(mVPlayer.Team);
		}
		else
		{
			color = Styles.GetColor(ColorStyle.TeamNone);
			color2 = Styles.GetColor(ColorStyle.TeamNone);
		}
		Label.text = string.Format(GetKillText(data, shotSelf), Styles.ColorToHex(color), mVPlayer2.UserProfileData.UserName, Styles.ColorToHex(color2), mVPlayer.UserProfileData.UserName);
	}

	public static string GetKillText(Dictionary<object, object> data, bool shotSelf)
	{
		PlayerKilledByType type = (PlayerKilledByType)data[(byte)8];
		return GetKillText(type, shotSelf);
	}

	public static string GetKillText(PlayerKilledByType type, bool shotSelf)
	{
		switch (type)
		{
		case PlayerKilledByType.AdvancedGhost:
			return TM._("<color=#{0}>{1}</color> was eliminated by an Oculus.");
		case PlayerKilledByType.BazookaGun:
			if (shotSelf)
			{
				return TM._("<color=#{0}>{1}</color> shot himself with a bazooka.");
			}
			return TM._("<color=#{2}>{3}</color> eliminated <color=#{0}>{1}</color> with a bazooka.");
		case PlayerKilledByType.Crushed:
			return TM._("<color=#{0}>{1}</color> was crushed.");
		case PlayerKilledByType.Environmental:
			return TM._("<color=#{0}>{1}</color> was killed by the environment.");
		case PlayerKilledByType.Explosive:
			return TM._("<color=#{0}>{1}</color> blew up.");
		case PlayerKilledByType.FallOffWorld:
			return TM._("<color=#{0}>{1}</color> fell off the world.");
		case PlayerKilledByType.Fire:
			return TM._("<color=#{0}>{1}</color> was burned.");
		case PlayerKilledByType.FlameThrower:
			return TM._("<color=#{2}>{3}</color> incinerated <color=#{0}>{1}</color> with a flamethrower.");
		case PlayerKilledByType.Ghost:
			return TM._("<color=#{0}>{1}</color> got caught by a ghost.");
		case PlayerKilledByType.Impact:
			return TM._("<color=#{0}>{1}</color> hit the ground too hard.");
		case PlayerKilledByType.Mutant:
			return TM._("<color=#{2}>{3}</color> eliminated <color=#{0}>{1}</color> using mutant.");
		case PlayerKilledByType.None:
			return TM._("None");
		case PlayerKilledByType.Sword:
			return TM._("<color=#{2}>{3}</color> eliminated <color=#{0}>{1}</color> with a sword.");
		case PlayerKilledByType.RailGun:
			return TM._("<color=#{2}>{3}</color> sniped <color=#{0}>{1}</color> with a railgun.");
		case PlayerKilledByType.Shotgun:
			return TM._("<color=#{2}>{3}</color> eliminated <color=#{0}>{1}</color> with a shotgun.");
		case PlayerKilledByType.Suicide:
			return TM._("<color=#{0}>{1}</color> respawned.");
		case PlayerKilledByType.GodzillaLaser:
			return TM._("<color=#{0}>{1}</color> was incinerated by Colossus.");
		case PlayerKilledByType.KillZone:
			return TM._("<color=#{0}>{1}</color> was crushed by Colossus.");
		default:
			return TM._("<color=#{2}>{3}</color> eliminated <color=#{0}>{1}</color>.");
		}
	}
}
