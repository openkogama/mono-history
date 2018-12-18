namespace MV.WorldObject;

public class GoldRewardedForLevelData
{
	public int level;

	public int goldReward;

	public GoldRewardedForLevelData()
	{
	}

	public GoldRewardedForLevelData(int level, int goldReward)
	{
		this.level = level;
		this.goldReward = goldReward;
	}

	public override string ToString()
	{
		return $"Level {level}. GoldReward {goldReward}.";
	}
}
