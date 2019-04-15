using System.Collections.Generic;

public static class GamePointAmountManager
{
	private static Dictionary<int, int> gamePointRewardWorldObjects = new Dictionary<int, int>();

	public static void UpdateRewardData(int woid, int gamePointRewardAmount)
	{
		if (!gamePointRewardWorldObjects.ContainsKey(woid))
		{
			gamePointRewardWorldObjects.Add(woid, gamePointRewardAmount);
		}
		gamePointRewardWorldObjects[woid] = gamePointRewardAmount;
	}

	public static int GetTotalGamePointAmount()
	{
		int num = 0;
		foreach (int value in gamePointRewardWorldObjects.Values)
		{
			num += value;
		}
		return num;
	}
}
