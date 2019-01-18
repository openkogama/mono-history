namespace MV.WorldObject.MetaData;

public class UserProfileData
{
	public bool IsAdmin;

	public string UserName;

	public int Gold;

	public SubscriptionData SubscriptionData;

	public static UserProfileData GetTouristProfileData(string userName)
	{
		UserProfileData userProfileData = new UserProfileData();
		userProfileData.IsAdmin = false;
		userProfileData.UserName = userName;
		userProfileData.Gold = 0;
		userProfileData.SubscriptionData = SubscriptionData.GetTouristProfileData();
		return userProfileData;
	}
}
