using System;

namespace Assets.CodeStage.AntiCheatToolkit.Scripts.ObscuredTypes;

public static class CryptoKeyGenerator
{
	private static Random rand = new Random();

	public static int GenerateKey(int from, int to)
	{
		return rand.Next(from, to);
	}
}
