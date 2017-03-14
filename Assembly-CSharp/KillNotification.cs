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
		MVPlayer mVPlayer = MVGameControllerBase.Game.Players[(int)data[(byte)6]];
		MVPlayer mVPlayer2 = MVGameControllerBase.Game.Players[(int)data[(byte)7]];
		Label.text = string.Format(GetKillText(data), mVPlayer2.Username, mVPlayer.Username);
	}

	public static string GetKillText(Dictionary<object, object> data)
	{
		PlayerKilledByType type = (PlayerKilledByType)(byte)data[(byte)8];
		return GetKillText(type);
	}

	public static string GetKillText(PlayerKilledByType type)
	{
		return type switch
		{
			PlayerKilledByType.AdvancedGhost => TM._("{0} was eliminated by an Oculus."), 
			PlayerKilledByType.BazookaGun => TM._("{0} eliminated {1} with a bazooka."), 
			PlayerKilledByType.Crushed => TM._("{0} was crushed."), 
			PlayerKilledByType.Environmental => TM._("{0} was killed by the environment."), 
			PlayerKilledByType.Explosive => TM._("{0} blew up."), 
			PlayerKilledByType.FallOffWorld => TM._("{0} fell off the world."), 
			PlayerKilledByType.Fire => TM._("{0} was burned."), 
			PlayerKilledByType.FlameThrower => TM._("{1} incinerated {0} with a flamethrower."), 
			PlayerKilledByType.Ghost => TM._("{0} got caught by a ghost."), 
			PlayerKilledByType.Impact => TM._("{0} hit the ground too hard."), 
			PlayerKilledByType.Mutant => TM._("{1} eliminated {0} using mutant."), 
			PlayerKilledByType.None => TM._("None"), 
			PlayerKilledByType.RailGun => TM._("{1} sniped {0} with a railgun."), 
			PlayerKilledByType.Shotgun => TM._("{1} eliminated {0} with a shotgun."), 
			PlayerKilledByType.Suicide => TM._("{0} respawned."), 
			PlayerKilledByType.GodzillaLaser => TM._("{0} was incinerated by Colossus."), 
			PlayerKilledByType.KillZone => TM._("{0} was crushed by Colossus."), 
			_ => TM._("{1} eliminated {0}."), 
		};
	}
}
