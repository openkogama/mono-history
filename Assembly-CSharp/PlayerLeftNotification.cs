using System.Collections.Generic;

public class PlayerLeftNotification : PlayerNotification
{
	public override void Initialize(Dictionary<object, object> data)
	{
		base.Initialize(data);
		MVPlayer mVPlayer = MVGameControllerBase.Game.MVPlayerContainer[(int)data[(byte)9]];
		NameLabel.text = mVPlayer.UserProfileData.UserName + TM._(" left!");
	}
}
