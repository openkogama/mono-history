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
		PartialRemoveFromHashtable(target, source, acceptMissingValuesInTarget: false);
	}

	public static bool PruneEmptyDictionaries(Dictionary<object, object> target)
	{
		List<string> list = new List<string>();
		foreach (KeyValuePair<object, object> item in target)
		{
			if (item.Value is Dictionary<object, object>)
			{
				Dictionary<object, object> target2 = (Dictionary<object, object>)item.Value;
				if (PruneEmptyDictionaries(target2))
				{
					list.Add((string)item.Key);
				}
			}
		}
		foreach (string item2 in list)
		{
			target.Remove(item2);
		}
		if (target.Count == 0)
		{
			return true;
		}
		return false;
	}

	public static void PartialRemoveFromHashtable(Dictionary<object, object> target, Dictionary<object, object> source, bool acceptMissingValuesInTarget)
	{
		foreach (KeyValuePair<object, object> item in source)
		{
			if (item.Value == null)
			{
				target.Remove(item.Key);
			}
			else if (!(item.Value is Dictionary<object, object> source2))
			{
				target.Remove(item.Key);
			}
			else if (target.ContainsKey(item.Key))
			{
				Dictionary<object, object> target2 = target[item.Key] as Dictionary<object, object>;
				PartialRemoveFromHashtable(target2, source2, acceptMissingValuesInTarget);
			}
			else if (!acceptMissingValuesInTarget)
			{
				throw new ArgumentException(string.Concat("Target hashtable doesn't contain an inner hashtable for key [", item.Key, "]"));
			}
		}
	}
}
