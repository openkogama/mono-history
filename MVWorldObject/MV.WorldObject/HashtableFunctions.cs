using System;
using System.Collections;

namespace MV.WorldObject;

public static class HashtableFunctions
{
	public static Hashtable DeepCopyHashTable(Hashtable from)
	{
		Hashtable hashtable = new Hashtable();
		foreach (DictionaryEntry item in from)
		{
			Type type = item.Value.GetType();
			object obj;
			if ((object)type == typeof(float[]))
			{
				int num = ((float[])item.Value).Length;
				obj = new float[num];
				Array.Copy((float[])item.Value, (float[])obj, num);
			}
			else if ((object)type == typeof(int[]))
			{
				int num2 = ((int[])item.Value).Length;
				obj = new int[num2];
				Array.Copy((int[])item.Value, (int[])obj, num2);
			}
			else if ((object)type == typeof(Hashtable))
			{
				obj = DeepCopyHashTable((Hashtable)item.Value);
			}
			else
			{
				if (!type.IsPrimitive && (object)type != typeof(string))
				{
					throw new ArgumentException("Type not handled in deepcopy hash table types " + item.Value.GetType());
				}
				obj = item.Value;
			}
			hashtable.Add(item.Key, obj);
		}
		return hashtable;
	}
}
