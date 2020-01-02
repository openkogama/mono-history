using System.Collections;
using System.Globalization;
using System.Text;
using UnityEngine;

public static class JSBridge
{
	private static void DoKGMEval(string str)
	{
		Debug.Log(str);
		Debug.LogWarning("Attempting to use js eval in non webGL context");
	}

	public static void ExternalEval(string script)
	{
		if (script.Length > 0 && script[script.Length - 1] != ';')
		{
			script += (string)(object)';';
		}
		DoKGMEval(script);
	}

	public static void ExternalCall(string functionName, params object[] args)
	{
		DoKGMEval(BuildInvocationForArguments(functionName, args));
	}

	private static string BuildInvocationForArguments(string functionName, params object[] args)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(functionName);
		stringBuilder.Append('(');
		int num = args.Length;
		for (int i = 0; i < num; i++)
		{
			if (i != 0)
			{
				stringBuilder.Append(", ");
			}
			stringBuilder.Append(ObjectToJSString(args[i]));
		}
		stringBuilder.Append(')');
		stringBuilder.Append(';');
		return stringBuilder.ToString();
	}

	private static string ObjectToJSString(object o)
	{
		if (o == null)
		{
			return "null";
		}
		if (o is string)
		{
			return '"'.ToString() + o.ToString().Replace("\\", "\\\\").Replace("\"", "\\\"")
				.Replace("\n", "\\n")
				.Replace("\r", "\\r")
				.Replace("\0", string.Empty)
				.Replace("\u2028", string.Empty)
				.Replace("\u2029", string.Empty) + '"';
		}
		if (o is int || o is short || o is uint || o is ushort || o is byte)
		{
			return o.ToString();
		}
		if (o is float)
		{
			NumberFormatInfo numberFormat = CultureInfo.InvariantCulture.NumberFormat;
			return ((float)o).ToString(numberFormat);
		}
		if (o is double)
		{
			NumberFormatInfo numberFormat2 = CultureInfo.InvariantCulture.NumberFormat;
			return ((double)o).ToString(numberFormat2);
		}
		if (o is char)
		{
			if ((char)o == '"')
			{
				return "\"\\\"\"";
			}
			return '"'.ToString() + o.ToString() + '"';
		}
		if (!(o is IList))
		{
			return ObjectToJSString(o.ToString());
		}
		IList list = (IList)o;
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("new Array(");
		int count = list.Count;
		for (int i = 0; i < count; i++)
		{
			if (i != 0)
			{
				stringBuilder.Append(", ");
			}
			stringBuilder.Append(ObjectToJSString(list[i]));
		}
		stringBuilder.Append(")");
		return stringBuilder.ToString();
	}
}
