using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

public static class Extensions
{
	private static ObscuredString obscuredString = string.Empty;

	public static string ToSerializeString(this Vector3 vec)
	{
		StringBuilder stringBuilder = new StringBuilder(vec.x.ToString());
		stringBuilder.Append(" ").Append(vec.y);
		stringBuilder.Append(" ").Append(vec.z);
		return stringBuilder.ToString();
	}

	public static Vector3 ToVector3FromSerializeString(this string text)
	{
		string[] array = text.Split(' ');
		if (array.Length != 3)
		{
			throw new ArgumentException("The input string doesnt contain 3 floats: " + text);
		}
		return new Vector3(Convert.ToSingle(array[0]), Convert.ToSingle(array[1]), Convert.ToSingle(array[2]));
	}

	public static void Log<T>(this IEnumerable<T> collection, string prependInfo = null, bool eachEntryNewLine = true)
	{
		collection.Log(Debug.Log, prependInfo, eachEntryNewLine);
	}

	public static void LogWarning<T>(this IEnumerable<T> collection, string prependInfo = null, bool eachEntryNewLine = true)
	{
		collection.Log(Debug.LogWarning, prependInfo, eachEntryNewLine);
	}

	public static void LogError<T>(this IEnumerable<T> collection, string prependInfo = null, bool eachEntryNewLine = true)
	{
		collection.Log(Debug.LogError, prependInfo, eachEntryNewLine);
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
		collection.LogRecursive(Debug.Log, prependInfo, eachEntryNewLine);
	}

	public static void LogWarningRecursive(this IEnumerable collection, string prependInfo = null, bool eachEntryNewLine = true)
	{
		collection.LogRecursive(Debug.LogWarning, prependInfo, eachEntryNewLine);
	}

	public static void LogErrorRecursive(this IEnumerable collection, string prependInfo = null, bool eachEntryNewLine = true)
	{
		collection.LogRecursive(Debug.LogError, prependInfo, eachEntryNewLine);
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
			if (item is KeyValuePair<object, object> keyValuePair)
			{
				sb.Append("[k: ").Append(keyValuePair.Key.ToString()).Append(" v: ");
				if (keyValuePair.Value is IEnumerable && !(keyValuePair.Value is string))
				{
					sb.AppendRecursive((IEnumerable)keyValuePair.Value, depth + 1, eachEntryNewLine);
					flag = true;
				}
				else
				{
					sb.Append((keyValuePair.Value != null) ? keyValuePair.Value.ToString() : "NULL");
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
		Debug.Log(stringBuilder.ToString());
	}

	public static object GetObscuredType(this Dictionary<object, object> hashtable, string key)
	{
		obscuredString = key;
		return hashtable[obscuredString];
	}

	public static void SetObscuredType<T>(this Dictionary<object, object> hashtable, string key, T value)
	{
		obscuredString = key;
		hashtable[obscuredString] = value;
	}

	public static bool ContainsObscuredKey(this Dictionary<object, object> hashtable, string key)
	{
		obscuredString = key;
		return hashtable.ContainsKey(obscuredString);
	}

	public static void ScaleBounds(this GameObject gameObject, float targetSize)
	{
		Bounds? axisAlignedBoundsRecursively = SharedCubeFunctions.GetAxisAlignedBoundsRecursively(gameObject.transform);
		Bounds bounds = new Bounds(Vector3.zero, Vector3.one);
		if (axisAlignedBoundsRecursively.HasValue)
		{
			bounds = axisAlignedBoundsRecursively.Value;
		}
		else
		{
			Debug.Log("Failed to find bounds!");
		}
		float num = Mathf.Max(bounds.size.x, Mathf.Max(bounds.size.y, bounds.size.z));
		float num2 = targetSize / num;
		gameObject.transform.localScale = new Vector3(num2, num2, num2);
	}

	public static Transform FindChildRecursively(this Transform transform, string child)
	{
		Transform transform2 = null;
		foreach (Transform item in transform)
		{
			if (item.name == child)
			{
				transform2 = item;
				break;
			}
		}
		if (transform2 == null)
		{
			foreach (Transform item2 in transform)
			{
				transform2 = item2.FindChildRecursively(child);
				if (transform2 != null)
				{
					return transform2;
				}
			}
		}
		return transform2;
	}
}
