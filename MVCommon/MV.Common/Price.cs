namespace MV.Common;

public struct Price
{
	public int gold;

	public int silver;

	public Price(int gold, int silver)
	{
		this.gold = gold;
		this.silver = silver;
	}

	public Price(int[] price)
	{
		gold = price[0];
		silver = price[1];
	}

	public int[] ToArray()
	{
		return new int[2] { gold, silver };
	}
}
