using System;
using System.Collections.Generic;

namespace MV.WorldObject;

public static class HashtableFunctions
{
	public static Dictionary<object, object> DeepCopyHashTable(Dictionary<object, object> from, Dictionary<object, object> to)
	{
		foreach (KeyValuePair<object, object> item in from)
		{
			Type type = item.Value.GetType();
			object obj;
			if (type == typeof(float[]))
			{
				int num = ((float[])item.Value).Length;
				obj = new float[num];
				Array.Copy((float[])item.Value, (float[])obj, num);
			}
			else if (type == typeof(int[]))
			{
				int num2 = ((int[])item.Value).Length;
				obj = new int[num2];
				Array.Copy((int[])item.Value, (int[])obj, num2);
			}
			else if (type == typeof(byte[]))
			{
				int num3 = ((byte[])item.Value).Length;
				obj = new byte[num3];
				Array.Copy((byte[])item.Value, (byte[])obj, num3);
			}
			else if (type == typeof(Dictionary<object, object>))
			{
				obj = DeepCopyHashTable((Dictionary<object, object>)item.Value);
			}
			else
			{
				if (!type.IsPrimitive && type != typeof(string))
				{
					throw new ArgumentException("Type not handled in deepcopy hash table types " + item.Value.GetType());
				}
				obj = item.Value;
			}
			to.Add(item.Key, obj);
		}
		return to;
	}

	public static Dictionary<object, object> DeepCopyHashTable(Dictionary<object, object> from)
	{
		Dictionary<object, object> to = new Dictionary<object, object>();
		return DeepCopyHashTable(from, to);
	}
}
