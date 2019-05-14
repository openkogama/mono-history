using MV.WorldObject.Accessories;

public class AccessoryDataClient : AccessoryData
{
	public bool ShowItem => iAvlb || IsWithinTimeLimit || owns;

	private bool IsWithinTimeLimit => true;
}
