using System;
using UnityEngine;

public static class LSEnumGenerator
{
	private const string mapAdd = "            map.Add((int){0}.{1}, TM._(\"{2}\"));\n";

	private const string basicString = "\r\n    public static string _({0} enumVal)\r\n    {{\r\n        return {0}LS.Get(enumVal);\r\n    }}\r\n    private static class {0}LS\r\n            {{\r\n                private static EnumLocalizeBookkeeping enumLocalizeBookkeeping = new EnumLocalizeBookkeeping(Init);\r\n                public static string Get({0} enumVal)\r\n                {{\r\n                    return enumLocalizeBookkeeping.GetLocalizedString((int)enumVal);\r\n                }}\r\n                \r\n                private static void Init(Dictionary<int, string> map)\r\n                {{\r\n                    {1}\r\n                }}\r\n            \r\n        }}";

	public static string Generate(Type type, Func<string, string> customStringCallback)
	{
		string arg = GenerateEnumCode(type, customStringCallback);
		return string.Format("\r\n    public static string _({0} enumVal)\r\n    {{\r\n        return {0}LS.Get(enumVal);\r\n    }}\r\n    private static class {0}LS\r\n            {{\r\n                private static EnumLocalizeBookkeeping enumLocalizeBookkeeping = new EnumLocalizeBookkeeping(Init);\r\n                public static string Get({0} enumVal)\r\n                {{\r\n                    return enumLocalizeBookkeeping.GetLocalizedString((int)enumVal);\r\n                }}\r\n                \r\n                private static void Init(Dictionary<int, string> map)\r\n                {{\r\n                    {1}\r\n                }}\r\n            \r\n        }}", type.Name, arg);
	}

	private static string GenerateEnumCode(Type type, Func<string, string> customStringCallback)
	{
		string text = string.Empty;
		string name = type.Name;
		foreach (object value in Enum.GetValues(type))
		{
			string name2 = Enum.GetName(type, value);
			string text2 = name2;
			if (customStringCallback != null)
			{
				try
				{
					text2 = customStringCallback(text2);
				}
				catch (Exception message)
				{
					Debug.LogWarning(message);
				}
			}
			text += $"            map.Add((int){name}.{name2}, TM._(\"{text2}\"));\n";
		}
		return text;
	}
}
