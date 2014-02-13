namespace MV.Common;

public static class CommonValues
{
	private static Price respawnNowPrice = new Price(0, 10);

	public static Price RespawnNowPrice => respawnNowPrice;

	public static float CompareThreshold => 0.7f;
}
