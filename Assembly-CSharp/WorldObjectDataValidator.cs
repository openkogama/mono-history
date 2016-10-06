using System;
using System.Collections.Generic;

public static class WorldObjectDataValidator
{
	private static Dictionary<object, object> lazyAddedData = new Dictionary<object, object>
	{
		{ "test", 2f },
		{ "levelAmount", 0 },
		{ "gameCoinAmount", 0 },
		{ "starAmount", 0 },
		{ "team", 0 }
	};

	public static void Validate(MVWorldObjectClient wo, string key, object value)
	{
		if (wo.Data.ContainsKey(key))
		{
			if (wo.Data[key].GetType() != value.GetType())
			{
				throw new Exception($"Types does not match wo data. key {key}, data {wo.Data[key].GetType()},  value {value.GetType()}");
			}
			return;
		}
		if (!lazyAddedData.ContainsKey(key))
		{
			throw new Exception($"Data not in wo or in lazyAddedData. key {key}, value {value}");
		}
		if (lazyAddedData[key].GetType() == value.GetType())
		{
			return;
		}
		throw new Exception($"Types does not match lazy added data. key {key}, data {lazyAddedData[key].GetType()},  value {value.GetType()}");
	}
}
