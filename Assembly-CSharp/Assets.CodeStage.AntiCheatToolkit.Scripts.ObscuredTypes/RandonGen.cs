using System;

namespace Assets.CodeStage.AntiCheatToolkit.Scripts.ObscuredTypes;

public static class RandonGen
{
	private static Random rnd = new Random();

	public static int RandomInt(int from, int to)
	{
		return rnd.Next(from, to);
	}
}
