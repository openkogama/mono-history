using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace MV.WorldObject.Security;

public static class Encryption
{
	public static string GetMD5Hash(SortedDictionary<string, string> formArgs, string secretKey)
	{
		formArgs.Add("secret_key", secretKey);
		string text = "";
		foreach (KeyValuePair<string, string> formArg in formArgs)
		{
			if (!string.IsNullOrEmpty(text))
			{
				text += "&";
			}
			text += $"{formArg.Key}={formArg.Value}";
		}
		formArgs.Remove("secret_key");
		return GetMD5Hash(text);
	}

	private static string GetMD5Hash(string s)
	{
		using MD5 mD = MD5.Create();
		byte[] array = mD.ComputeHash(Encoding.UTF8.GetBytes(s));
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < array.Length; i++)
		{
			stringBuilder.Append(array[i].ToString("x2"));
		}
		return stringBuilder.ToString();
	}
}
