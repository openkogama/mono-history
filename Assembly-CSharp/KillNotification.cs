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

	public override void Initialize(Dictionary<object, object> data)
	{
		base.Initialize(data);
		MVPlayer mVPlayer = MVGameControllerBase.Game.Players[(int)data[(byte)6]];
		MVPlayer mVPlayer2 = MVGameControllerBase.Game.Players[(int)data[(byte)7]];
		Label.text = string.Format(GetText(data), mVPlayer2.Username, mVPlayer.Username);
	}

	private string GetText(Dictionary<object, object> data)
	{
		return (PlayerKilledByType)(byte)data[(byte)8] switch
		{
			PlayerKilledByType.AdvancedGhost => TM._("{0} was torn apart by an Oculus!"), 
			PlayerKilledByType.BazookaGun => TM._("{1} blew {0} up!"), 
			PlayerKilledByType.Crushed => TM._("{0} was crushed"), 
			PlayerKilledByType.Environmental => TM._("{0} was killed by the environment"), 
			PlayerKilledByType.Explosive => TM._("{0} blew up! Ouch!"), 
			PlayerKilledByType.FallOffWorld => TM._("{0} tried to fly"), 
			PlayerKilledByType.Fire => TM._("{0} burned alive"), 
			PlayerKilledByType.FlameThrower => TM._("{1} fried {0}"), 
			PlayerKilledByType.Ghost => TM._("{0} tried to hug a ghost"), 
			PlayerKilledByType.Impact => TM._("{0} hit the ground too hard"), 
			PlayerKilledByType.Mutant => TM._("{1} killed {0} using mutant!"), 
			PlayerKilledByType.None => TM._("None"), 
			PlayerKilledByType.RailGun => TM._("{1} noscoped {0}"), 
			PlayerKilledByType.Shotgun => TM._("{1} blew {0} to pieces!"), 
			PlayerKilledByType.Suicide => TM._("{0} grew tired of life :("), 
			_ => TM._("{1} murdered {0}"), 
		};
	}
}
