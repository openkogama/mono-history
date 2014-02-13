using System;
using System.Collections;

namespace MV.Common;

public static class CommonUtils
{
	public static void PartialUpdateHashtable(Hashtable target, Hashtable source)
	{
		foreach (DictionaryEntry item in source)
		{
			if (item.Value == null)
			{
				throw new ArgumentException(string.Concat("Update table contains NULL valye for key [", item.Key, "]"));
			}
			if (target.ContainsKey(item.Key))
			{
				object obj = target[item.Key];
				if ((object)obj.GetType() != item.Value.GetType())
				{
					throw new ArgumentException(string.Concat(new object[7]
					{
						"Incompatible types ",
						obj.GetType(),
						" and ",
						item.Value.GetType(),
						" for key [",
						item.Key,
						"]"
					}));
				}
				if (!(obj is Hashtable hashtable))
				{
					target[item.Key] = item.Value;
				}
				else if (hashtable != null)
				{
					Hashtable source2 = item.Value as Hashtable;
					PartialUpdateHashtable(hashtable, source2);
				}
			}
			else
			{
				target[item.Key] = item.Value;
			}
		}
	}

	public static void PartialRemoveFromHashtable(Hashtable target, Hashtable source)
	{
		foreach (DictionaryEntry item in source)
		{
			if (item.Value == null)
			{
				target.Remove(item.Key);
				continue;
			}
			if (!(item.Value is Hashtable source2))
			{
				target.Remove(item.Key);
				continue;
			}
			if (target[item.Key] is Hashtable target2)
			{
				PartialRemoveFromHashtable(target2, source2);
				continue;
			}
			throw new ArgumentException(string.Concat("Target hashtable doesn't contain an inner hashtable for key [", item.Key, "]"));
		}
	}
}
