using MV.WorldObject.Accessories;

public class AccessoryDataClient : AccessoryData
{
	public bool ShowItem => isAvailable || IsWithinTimeLimit || owns;

	private bool IsWithinTimeLimit => true;
}
