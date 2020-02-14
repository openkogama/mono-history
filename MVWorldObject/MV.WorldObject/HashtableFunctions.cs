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

	public static bool TryGetSubDictionary(out Dictionary<object, object> subDictionary, Dictionary<object, object> data, List<string> subDictionaryPath)
	{
		Dictionary<object, object> dictionary = data;
		for (int i = 0; i < subDictionaryPath.Count; i++)
		{
			if (dictionary.ContainsKey(subDictionaryPath[i]))
			{
				dictionary = (Dictionary<object, object>)dictionary[subDictionaryPath[i]];
				continue;
			}
			subDictionary = null;
			return false;
		}
		subDictionary = dictionary;
		return true;
	}

	public static bool ContainsSubDictionary(Dictionary<object, object> data, List<string> subDictionaryPath)
	{
		Dictionary<object, object> subDictionary;
		return TryGetSubDictionary(out subDictionary, data, subDictionaryPath);
	}

	public static Dictionary<object, object> CreateDictionaryUpdate(string key, object value, List<string> dictionaryBasePath)
	{
		Dictionary<object, object> dictionary = new Dictionary<object, object>();
		Dictionary<object, object> result = dictionary;
		for (int i = 0; i < dictionaryBasePath.Count; i++)
		{
			Dictionary<object, object> dictionary2 = new Dictionary<object, object>();
			dictionary.Add(dictionaryBasePath[i], dictionary2);
			dictionary = dictionary2;
			if (i == dictionaryBasePath.Count - 1)
			{
				dictionary[key] = value;
			}
		}
		return result;
	}

	public static Dictionary<object, object> GetSettingsSubDictionary(Dictionary<object, object> data, List<string> subDictionaryPath)
	{
		if (!TryGetSubDictionary(out var subDictionary, data, subDictionaryPath))
		{
			return new Dictionary<object, object>();
		}
		return subDictionary;
	}

	public static string PrettyString(Dictionary<object, object> dictionary)
	{
		return "\n" + PrettyString(dictionary, 0);
	}

	private static string PrettyString(Dictionary<object, object> dictionary, int padLeft)
	{
		string text = "";
		foreach (KeyValuePair<object, object> item in dictionary)
		{
			if (item.Value is Dictionary<object, object>)
			{
				text += string.Format("{0}+ {1}\n", "".PadLeft(padLeft), item.Key);
				text += PrettyString((Dictionary<object, object>)item.Value, padLeft + 1);
			}
			else
			{
				text += string.Format("{0}- [{1}, {2}]\n", "".PadLeft(padLeft), item.Key, item.Value);
			}
		}
		return text;
	}
}
