using System;
using System.Security.Cryptography;
using UnityEngine;

public static class TextureHash
{
	public static string CreateHashCode(Texture texture)
	{
		Color[] pixels = ((Texture2D)texture).GetPixels(0, 0, 10, 10);
		byte[] buffer = ColorsToByteArray(pixels, 10);
		using SHA1CryptoServiceProvider sHA1CryptoServiceProvider = new SHA1CryptoServiceProvider();
		return Convert.ToBase64String(sHA1CryptoServiceProvider.ComputeHash(buffer));
	}

	private static byte[] ColorsToByteArray(Color[] colors, int sampleSize)
	{
		byte[] array = new byte[sampleSize * sampleSize * 4];
		for (int i = 0; i < colors.Length; i++)
		{
			byte[] array2 = ColorToByteArray(colors[i]);
			int num = i * 4;
			for (int j = 0; j < 4; j++)
			{
				array[j + num] = array2[j];
			}
		}
		return array;
	}

	private static byte[] ColorToByteArray(Color color)
	{
		byte[] array = new byte[4];
		for (int i = 0; i < 4; i++)
		{
			array[i] = ColorFloatToByte(color[i]);
		}
		return array;
	}

	private static byte ColorFloatToByte(float colorFloat)
	{
		return (byte)(colorFloat * 255f);
	}
}
