using MV.Common;

public class Friend
{
	public int profileID = -1;

	public int friendID = -1;

	public FriendStatus status = FriendStatus.Invalid;

	public Friend(int friendID, int id, FriendStatus status)
	{
		this.friendID = friendID;
		profileID = id;
		this.status = status;
	}
}
