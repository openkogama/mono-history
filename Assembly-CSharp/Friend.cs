using MV.Common;

public class Friend
{
	public int profileID = -1;

	public FriendStatus status = FriendStatus.Invalid;

	public Friend(int id, FriendStatus status)
	{
		profileID = id;
		this.status = status;
	}
}
