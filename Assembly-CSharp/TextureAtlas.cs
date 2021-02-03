using System;
using UnityEngine;

public static class TextureAtlas
{
	public enum TextureEnums
	{
		scarletRed00,
		scarletRed01,
		scarletRed02,
		chocolate00,
		plum00,
		skyBlue00,
		skyBlue01,
		skyBlue02,
		chocolate01,
		plum01,
		chameleon00,
		chameleon01,
		chameleon02,
		chocolate02,
		plum02,
		orange00,
		orange01,
		orange02,
		butter00,
		butter01,
		aluminium00,
		aluminium01,
		aluminium02,
		aluminium03,
		butter02,
		funcIce00,
		funcLava00,
		funcBouncy00,
		funcLava01,
		funcParkour00,
		brickwall00,
		wood01,
		pavement00,
		concrete00,
		cloth01,
		pavement02,
		pavement03,
		brickwall01,
		brickwall02,
		aluminium04,
		metal00,
		metal01,
		funcBouncy01,
		funcIce01,
		pink00,
		grid00,
		grid01,
		circuit00,
		brickwall03,
		pattern01,
		metal02,
		funcSlime00,
		pattern02,
		wood02,
		funcBouncy02,
		cloud00,
		destructable02,
		destructable01,
		destructable00,
		destruction03,
		StripedCement,
		Machinery,
		EmbossedMetal,
		END_OF_TYPES
	}

	public const int tiles = 16;

	public const float singlePixelUV = 9.765625E-06f;

	public const float Tilewidth = 0.06248047f;

	public const float Tileheight = 0.06248047f;

	public static int[] GlowingMaterials = new int[3] { 26, 28, 55 };

	public static Vector2[] IndexMap = new Vector2[256];

	public static bool Initialized = false;

	public static Vector2 GetAtlasPoint(int MaterialID)
	{
		if (!Initialized)
		{
			InitializeIndexMap();
			Initialized = true;
		}
		return IndexMap[MaterialID];
	}

	public static void InitializeIndexMap()
	{
		for (int i = 0; i < 63; i++)
		{
			int num = 0;
			int num2 = Array.IndexOf(GlowingMaterials, i);
			if (num2 >= 0)
			{
				num = 63 - GlowingMaterials.Length + num2;
			}
			else if (i < GlowingMaterials[0])
			{
				num = i;
			}
			else if (i > GlowingMaterials[GlowingMaterials.Length - 1])
			{
				num = i - GlowingMaterials.Length;
			}
			else
			{
				int num3 = 0;
				for (int j = 0; j < GlowingMaterials.Length - 1 && GlowingMaterials[j] <= i; j++)
				{
					num3++;
				}
				num = i - num3;
			}
			int num4 = num % 16;
			int num5 = 16 - (num - num4) / 16 - 1;
			ref Vector2 reference = ref IndexMap[i];
			reference = new Vector2((float)num4 * 0.0625f + 9.765625E-06f, (float)num5 * 0.0625f + 9.765625E-06f);
		}
	}
}
