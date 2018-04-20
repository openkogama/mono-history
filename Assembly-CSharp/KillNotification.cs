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
		if (mVPlayer2.Username == mVPlayer.Username)
		{
			shotSelf = true;
		}
		Label.text = string.Format(GetKillText(data, shotSelf), mVPlayer2.Username, mVPlayer.Username);
	}

	public static string GetKillText(Dictionary<object, object> data, bool shotSelf)
	{
		PlayerKilledByType type = (PlayerKilledByType)(byte)data[(byte)8];
		return GetKillText(type, shotSelf);
	}

	public static string GetKillText(PlayerKilledByType type, bool shotSelf)
	{
		switch (type)
		{
		case PlayerKilledByType.AdvancedGhost:
			return TM._("{0} was eliminated by an Oculus.");
		case PlayerKilledByType.BazookaGun:
			if (shotSelf)
			{
				return TM._("{0} shot himself with a bazooka.");
			}
			return TM._("{1} eliminated {0} with a bazooka.");
		case PlayerKilledByType.Crushed:
			return TM._("{0} was crushed.");
		case PlayerKilledByType.Environmental:
			return TM._("{0} was killed by the environment.");
		case PlayerKilledByType.Explosive:
			return TM._("{0} blew up.");
		case PlayerKilledByType.FallOffWorld:
			return TM._("{0} fell off the world.");
		case PlayerKilledByType.Fire:
			return TM._("{0} was burned.");
		case PlayerKilledByType.FlameThrower:
			return TM._("{1} incinerated {0} with a flamethrower.");
		case PlayerKilledByType.Ghost:
			return TM._("{0} got caught by a ghost.");
		case PlayerKilledByType.Impact:
			return TM._("{0} hit the ground too hard.");
		case PlayerKilledByType.Mutant:
			return TM._("{1} eliminated {0} using mutant.");
		case PlayerKilledByType.None:
			return TM._("None");
		case PlayerKilledByType.RailGun:
			return TM._("{1} sniped {0} with a railgun.");
		case PlayerKilledByType.Shotgun:
			return TM._("{1} eliminated {0} with a shotgun.");
		case PlayerKilledByType.Suicide:
			return TM._("{0} respawned.");
		case PlayerKilledByType.GodzillaLaser:
			return TM._("{0} was incinerated by Colossus.");
		case PlayerKilledByType.KillZone:
			return TM._("{0} was crushed by Colossus.");
		default:
			return TM._("{1} eliminated {0}.");
		}
	}
}
