namespace MV.WorldObject;

public class MVAvatar
{
	private readonly int woID = -1;

	private readonly MvAvatarMetaData avatarMetaData;

	public MvAvatarMetaData AvatarMetaData => avatarMetaData;

	public int WoID => woID;

	public MVAvatar(MvAvatarMetaData avatarMetaData, int woID)
	{
		this.avatarMetaData = avatarMetaData;
		this.woID = woID;
	}
}
