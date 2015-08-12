using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace MV.WorldObject.Security;

public static class SecurityHelper
{
	private const string ENCRYPTION_KEY = "P63oUa9unCY";

	private static readonly byte[] SALT;

	private static readonly byte[] key;

	private static readonly byte[] iv;

	private static readonly Rfc2898DeriveBytes keyGenerator;

	static SecurityHelper()
	{
		SALT = Encoding.ASCII.GetBytes("P63oUa9unCY");
		keyGenerator = new Rfc2898DeriveBytes("P63oUa9unCY", SALT);
		key = keyGenerator.GetBytes(32);
		iv = keyGenerator.GetBytes(16);
	}

	public static string Encrypt(string inputText)
	{
		RijndaelManaged rijndaelManaged = new RijndaelManaged();
		rijndaelManaged.Key = key;
		rijndaelManaged.IV = iv;
		RijndaelManaged rijndaelManaged2 = rijndaelManaged;
		byte[] bytes = Encoding.Unicode.GetBytes(inputText);
		using ICryptoTransform transform = rijndaelManaged2.CreateEncryptor();
		using MemoryStream memoryStream = new MemoryStream();
		using CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write);
		cryptoStream.Write(bytes, 0, bytes.Length);
		cryptoStream.FlushFinalBlock();
		return Convert.ToBase64String(memoryStream.ToArray());
	}

	public static string Decrypt(string inputText)
	{
		RijndaelManaged rijndaelManaged = new RijndaelManaged();
		byte[] array = Convert.FromBase64String(inputText);
		using ICryptoTransform transform = rijndaelManaged.CreateDecryptor(key, iv);
		using MemoryStream stream = new MemoryStream(array);
		using CryptoStream cryptoStream = new CryptoStream(stream, transform, CryptoStreamMode.Read);
		byte[] array2 = new byte[array.Length];
		int count = cryptoStream.Read(array2, 0, array2.Length);
		return Encoding.Unicode.GetString(array2, 0, count);
	}
}
