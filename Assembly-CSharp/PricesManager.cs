using System;
using System.Collections.Generic;
using MV.Common;

public static class PricesManager
{
	private static Dictionary<object, object> prices;

	public static void Init(Dictionary<object, object> prices)
	{
		PricesManager.prices = prices;
	}

	public static void Reset()
	{
		prices = null;
	}

	public static Price GetPrice(string priceName)
	{
		if (!prices.ContainsKey(priceName))
		{
			throw new Exception("Unknown price name");
		}
		int[] price = (int[])prices[priceName];
		return new Price(price);
	}
}
