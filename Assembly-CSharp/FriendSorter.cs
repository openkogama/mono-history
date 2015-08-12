using System.Collections.Generic;
using MV.Common;

public class FriendSorter : IComparer<PlayerData>
{
	public int Compare(PlayerData data1, PlayerData data2)
	{
		return PlayerValue(data2) - PlayerValue(data1);
	}

	private int PlayerValue(PlayerData data)
	{
		if (data.player == MVGameController.Game.LocalPlayer)
		{
			return 2;
		}
		if (data.player.IsAnonymous)
		{
			return -3;
		}
		if (data.friend != null)
		{
			return data.friend.status switch
			{
				FriendStatus.Accepted => 1, 
				FriendStatus.Rejected => -2, 
				_ => 0, 
			};
		}
		return -1;
	}
}
