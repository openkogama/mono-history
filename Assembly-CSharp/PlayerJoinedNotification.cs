using System.Collections.Generic;

public class PlayerJoinedNotification : PlayerNotification
{
	public override void Initialize(Dictionary<object, object> data)
	{
		base.Initialize(data);
		MVPlayer mVPlayer = MVGameControllerBase.Game.Players[(int)data[(byte)9]];
		NameLabel.text = mVPlayer.Username + TM._(" joined!");
	}
}
