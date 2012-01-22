using System.Collections.Generic;
using MV.WorldObject;

public static class StandardTerrainMethodNoCaves
{
	private static int terrainX = 250;

	private static int terrainY = 20;

	private static int terrainZ = 250;

	private static float noizeFactor = 100f;

	private static int cubeAddedCounter = 0;

	private static List<IntVector> generatedCubes = new List<IntVector>();

	public static void GenerateTerrain(int noiseBlockOffset)
	{
		for (int i = 0; i < terrainX; i++)
		{
			int blockX = i;
			for (int j = 0; j < terrainZ; j++)
			{
				int blockY = j;
				GenerateTerrain(blockX, blockY, terrainY);
			}
		}
	}

	private static void GenerateTerrain(int blockX, int blockY, int worldDepthInBlocks)
	{
		int num = worldDepthInBlocks / 4;
		int num2 = (int)((float)worldDepthInBlocks * 0.75f);
		float num3 = MathFunctions.PerlinSimplexNoise.noise((float)blockX * 0.0001f * noizeFactor, (float)blockY * 0.0001f * noizeFactor) * 0.5f;
		float num4 = MathFunctions.PerlinSimplexNoise.noise((float)blockX * 0.0005f * noizeFactor, (float)blockY * 0.0005f * noizeFactor) * 0.25f;
		float num5 = num3 + num4;
		num5 = num5 * (float)num2 + (float)num;
		for (int num6 = worldDepthInBlocks - 1; num6 >= 0; num6--)
		{
			if ((float)num6 <= num5)
			{
				generatedCubes.Add(new IntVector((short)blockX, (short)num6, (short)blockY));
			}
		}
	}

	public static void AddCube(RuntimePrototypeCubeModel runtimePrototypeCubeModel)
	{
	}
}
