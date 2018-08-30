namespace MV.Common;

public struct Price
{
	public int gold;

	public Price(int gold)
	{
		this.gold = gold;
	}

	public Price(int[] price)
	{
		gold = price[0];
	}

	public int[] ToArray()
	{
		return new int[1] { gold };
	}
}
