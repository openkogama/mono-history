namespace MV.WorldObject.AndroidAdData;

public class AndroidAdDataFyber
{
	public int appId;

	public string clientSecurityToken;

	public AndroidAdDataFyber()
	{
	}

	public AndroidAdDataFyber(int appId, string clientSecurityToken)
	{
		this.appId = appId;
		this.clientSecurityToken = clientSecurityToken;
	}
}
