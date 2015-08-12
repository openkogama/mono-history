using System;
using System.Collections.Generic;

namespace MV.Common;

public static class CommonUtils
{
	public static void PartialUpdateHashtable(Dictionary<object, object> target, Dictionary<object, object> source)
	{
		foreach (KeyValuePair<object, object> item in source)
		{
			if (item.Value == null)
			{
				throw new ArgumentException(string.Concat("Update table contains NULL valye for key [", item.Key, "]"));
			}
			if (target.ContainsKey(item.Key))
			{
				object obj = target[item.Key];
				if (obj.GetType() != item.Value.GetType())
				{
					throw new ArgumentException(string.Concat("Incompatible types ", obj.GetType(), " and ", item.Value.GetType(), " for key [", item.Key, "]"));
				}
				if (!(obj is Dictionary<object, object> dictionary))
				{
					target[item.Key] = item.Value;
				}
				else if (dictionary != null)
				{
					Dictionary<object, object> source2 = item.Value as Dictionary<object, object>;
					PartialUpdateHashtable(dictionary, source2);
				}
			}
			else
			{
				target[item.Key] = item.Value;
			}
		}
	}

	public static void PartialRemoveFromHashtable(Dictionary<object, object> target, Dictionary<object, object> source)
	{
		foreach (KeyValuePair<object, object> item in source)
		{
			if (item.Value == null)
			{
				target.Remove(item.Key);
				continue;
			}
			if (!(item.Value is Dictionary<object, object> source2))
			{
				target.Remove(item.Key);
				continue;
			}
			if (target[item.Key] is Dictionary<object, object> target2)
			{
				PartialRemoveFromHashtable(target2, source2);
				continue;
			}
			throw new ArgumentException(string.Concat("Target hashtable doesn't contain an inner hashtable for key [", item.Key, "]"));
		}
	}
}
