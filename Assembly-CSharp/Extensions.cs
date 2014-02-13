using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class Extensions
{
	public static string ToSerializeString(this Vector3 vec)
	{
		StringBuilder stringBuilder = new StringBuilder(vec.x.ToString());
		stringBuilder.Append(" ").Append(vec.y);
		stringBuilder.Append(" ").Append(vec.z);
		return stringBuilder.ToString();
	}

	public static Vector3 ToVector3FromSerializeString(this string text)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		string[] array = text.Split(new char[1] { ' ' });
		if (array.Length != 3)
		{
			throw new ArgumentException("The input string doesnt contain 3 floats: " + text);
		}
		return new Vector3(Convert.ToSingle(array[0]), Convert.ToSingle(array[1]), Convert.ToSingle(array[2]));
	}

	public static void Log<T>(this IEnumerable<T> collection, string prependInfo = null, bool eachEntryNewLine = true)
	{
		collection.Log((Action<string>)Debug.Log, prependInfo, eachEntryNewLine);
	}

	public static void LogWarning<T>(this IEnumerable<T> collection, string prependInfo = null, bool eachEntryNewLine = true)
	{
		collection.Log((Action<string>)Debug.LogWarning, prependInfo, eachEntryNewLine);
	}

	public static void LogError<T>(this IEnumerable<T> collection, string prependInfo = null, bool eachEntryNewLine = true)
	{
		collection.Log((Action<string>)Debug.LogError, prependInfo, eachEntryNewLine);
	}

	private static void Log<T>(this IEnumerable<T> collection, Action<string> logFunc, string prependInfo = null, bool eachEntryNewLine = true)
	{
		logFunc(collection.BuildString(prependInfo, eachEntryNewLine));
	}

	public static string BuildString<T>(this IEnumerable<T> collection)
	{
		return collection.BuildString(null, eachEntryNewLine: true);
	}

	public static string BuildString<T>(this IEnumerable<T> collection, bool eachEntryNewLine)
	{
		return collection.BuildString(null, eachEntryNewLine);
	}

	public static string BuildString<T>(this IEnumerable<T> collection, string prependInfo)
	{
		return collection.BuildString(prependInfo, eachEntryNewLine: true);
	}

	public static string BuildString<T>(this IEnumerable<T> collection, string prependInfo, bool eachEntryNewLine)
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (prependInfo != null)
		{
			stringBuilder.Append(prependInfo).Append(Environment.NewLine);
		}
		bool flag = true;
		foreach (T item in collection)
		{
			if (flag)
			{
				flag = false;
			}
			else
			{
				stringBuilder.Append(", ");
				if (eachEntryNewLine)
				{
					stringBuilder.Append(Environment.NewLine);
				}
			}
			stringBuilder.Append(item);
		}
		return stringBuilder.ToString();
	}

	public static void LogRecursive(this IEnumerable collection, string prependInfo = null, bool eachEntryNewLine = true)
	{
		collection.LogRecursive((Action<string>)Debug.Log, prependInfo, eachEntryNewLine);
	}

	public static void LogWarningRecursive(this IEnumerable collection, string prependInfo = null, bool eachEntryNewLine = true)
	{
		collection.LogRecursive((Action<string>)Debug.LogWarning, prependInfo, eachEntryNewLine);
	}

	public static void LogErrorRecursive(this IEnumerable collection, string prependInfo = null, bool eachEntryNewLine = true)
	{
		collection.LogRecursive((Action<string>)Debug.LogError, prependInfo, eachEntryNewLine);
	}

	private static void LogRecursive(this IEnumerable collection, Action<string> logFunc, string prependInfo = null, bool eachEntryNewLine = true)
	{
		logFunc(collection.BuildStringRecursive(prependInfo, eachEntryNewLine));
	}

	public static string BuildStringRecursive(this IEnumerable collection, string prependInfo = null, bool eachEntryNewLine = true)
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (prependInfo != null)
		{
			stringBuilder.Append(prependInfo);
		}
		stringBuilder.AppendRecursive(collection, 0, eachEntryNewLine);
		return stringBuilder.ToString();
	}

	public static void AppendRecursive(this StringBuilder sb, IEnumerable collection, int depth = 0, bool eachEntryNewLine = true)
	{
		bool flag = true;
		foreach (object item in collection)
		{
			if (flag || eachEntryNewLine)
			{
				flag = false;
				if (sb.Length != 0)
				{
					sb.Append(Environment.NewLine);
				}
				sb.Append(string.Empty.PadLeft(2 * depth));
			}
			else
			{
				sb.Append(", ");
			}
			if (item is DictionaryEntry dictionaryEntry)
			{
				sb.Append("[k: ").Append(dictionaryEntry.Key.ToString()).Append(" v: ");
				if (dictionaryEntry.Value is IEnumerable && !(dictionaryEntry.Value is string))
				{
					sb.AppendRecursive((IEnumerable)dictionaryEntry.Value, depth + 1, eachEntryNewLine);
					flag = true;
				}
				else
				{
					sb.Append((dictionaryEntry.Value != null) ? dictionaryEntry.Value.ToString() : "NULL");
					sb.Append(']');
				}
			}
			else
			{
				sb.Append(item);
			}
		}
	}

	public static void Log<TKey, TValue>(this Dictionary<TKey, TValue> collection, string prependInfo = "")
	{
		StringBuilder stringBuilder = new StringBuilder(prependInfo);
		bool flag = true;
		foreach (KeyValuePair<TKey, TValue> item in collection)
		{
			if (flag)
			{
				flag = false;
			}
			else
			{
				stringBuilder.Append(", ");
			}
			stringBuilder.Append("[k: ").Append(item.Key).Append(" v: ")
				.Append(item.Value)
				.Append("]");
		}
		Debug.Log((object)stringBuilder.ToString());
	}

	public static void ScaleBounds(this GameObject gameObject, float targetSize)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		Bounds? axisAlignedBoundsRecursively = SharedCubeFunctions.GetAxisAlignedBoundsRecursively(gameObject.transform);
		Bounds val = new Bounds(Vector3.zero, Vector3.one);
		if (axisAlignedBoundsRecursively.HasValue)
		{
			val = axisAlignedBoundsRecursively.Value;
		}
		else
		{
			Debug.Log((object)"Failed to find bounds!");
		}
		float num = Mathf.Max(val.size.x, Mathf.Max(val.size.y, val.size.z));
		float num2 = targetSize / num;
		gameObject.transform.localScale = new Vector3(num2, num2, num2);
	}

	public static Transform FindChildRecursively(this Transform transform, string child)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected Obj, but got Unknown
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Expected Obj, but got Unknown
		Transform val = null;
		foreach (Transform item in transform)
		{
			Transform val2 = item;
			if (((Object)val2).name == child)
			{
				val = val2;
				break;
			}
		}
		if ((Object)(object)val == (Object)null)
		{
			foreach (Transform item2 in transform)
			{
				Transform transform2 = item2;
				val = transform2.FindChildRecursively(child);
				if ((Object)(object)val != (Object)null)
				{
					return val;
				}
			}
		}
		return val;
	}
}
