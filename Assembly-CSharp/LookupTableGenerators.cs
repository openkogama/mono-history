using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class LookupTableGenerators
{
	private static int positionsOnPlaneAxis = 5;

	private static Dictionary<byte, Vector3> bytePositionLookUpTable = new Dictionary<byte, Vector3>();

	private static Dictionary<Vector3, byte> positionByteLookUpTable = new Dictionary<Vector3, byte>();

	public static void GenerateLoopUpTableForFaceCover()
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		List<List<Vector2>> permutations = new List<List<Vector2>>();
		GenerateLoopUpTableForFaceCoverRecur(permutations, new List<Vector2>());
		Debug.Log((object)permutations.Count);
		PrunePermutations(ref permutations);
		Debug.Log((object)permutations.Count);
		for (int i = 0; i < 10; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				Debug.Log((object)(permutations[i][j] + Vector2.one));
			}
			Debug.Log((object)"###############");
		}
	}

	private static void PrunePermutations(ref List<List<Vector2>> permutations)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		HashSet<Vector2> hashSet = new HashSet<Vector2>();
		HashSet<int> hashSet2 = new HashSet<int>();
		for (int i = 0; i < permutations.Count; i++)
		{
			if (permutations[i][0] == permutations[i][1] && permutations[i][0] == permutations[i][2] && permutations[i][0] == permutations[i][3])
			{
				hashSet2.Add(i);
			}
			hashSet.Clear();
			bool flag = false;
			for (int j = 0; j < 4; j++)
			{
				Vector2 v = permutations[i][(j + 1) % 4] - permutations[i][j % 4];
				Vector2 v2 = permutations[i][(j + 2) % 4] - permutations[i][(j + 1) % 4];
				float num = MathFunctions.SignedAngle(v, v2);
				if (num < (float)Math.PI / 3f || num > (float)Math.PI * 2f / 3f)
				{
					hashSet2.Add(i);
					flag = true;
					break;
				}
				if (IsRotated(permutations[i][j % 4], permutations[i][(j + 1) % 4], j))
				{
					hashSet2.Add(i);
					flag = true;
					break;
				}
				hashSet.Add(permutations[i][j]);
			}
			if (hashSet.Count < 4 && !flag)
			{
				hashSet2.Add(i);
			}
		}
		List<int> list = new List<int>(hashSet2);
		for (int num2 = list.Count - 1; num2 >= 0; num2--)
		{
			permutations.RemoveAt(list[num2]);
		}
	}

	private static bool IsRotated(Vector2 from, Vector2 to, int index)
	{
		switch (index)
		{
		case 0:
			if (to.x - from.x < 0f)
			{
				return true;
			}
			break;
		case 1:
			if (to.y - from.y < 0f)
			{
				return true;
			}
			break;
		case 2:
			if (to.x - from.x > 0f)
			{
				return true;
			}
			break;
		case 3:
			if (to.y - from.y > 0f)
			{
				return true;
			}
			break;
		}
		return false;
	}

	private static void GenerateLoopUpTableForFaceCoverRecur(List<List<Vector2>> permutations, List<Vector2> vectors)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < positionsOnPlaneAxis; i++)
		{
			for (int j = 0; j < positionsOnPlaneAxis; j++)
			{
				Vector2 item = new Vector2(((float)i - 2f) / 4f, ((float)j - 2f) / 4f);
				List<Vector2> list = new List<Vector2>(vectors);
				list.Add(item);
				if (list.Count == 4)
				{
					permutations.Add(list);
				}
				else
				{
					GenerateLoopUpTableForFaceCoverRecur(permutations, new List<Vector2>(list));
				}
			}
		}
	}

	public static void GenerateLookUpTables2()
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		string text = "static Vector3[] bytePositionLookUpTable = new Vector3[]{\n";
		string text2 = "static Dictionary<Vector3, byte> positionByteLookUpTable = new Dictionary<Vector3, byte>{\n";
		for (int i = 0; i < positionsOnPlaneAxis; i++)
		{
			for (int j = 0; j < positionsOnPlaneAxis; j++)
			{
				for (int k = 0; k < positionsOnPlaneAxis; k++)
				{
					Vector3 val = new Vector3(((float)i - 2f) / 4f, ((float)j - 2f) / 4f, ((float)k - 2f) / 4f);
					bytePositionLookUpTable.Add((byte)num, val);
					positionByteLookUpTable.Add(val, (byte)num);
					string text3 = "new Vector3(" + val.x + "f, " + val.y + "f, " + val.z + "f)";
					text = text + text3 + ",\n";
					string text4 = text2;
					text2 = text4 + "{" + text3 + ", " + num + "},\n";
					num++;
				}
			}
		}
		text += "\n};";
		TextWriter textWriter = new StreamWriter("bytePositionLookUpTable.cs");
		textWriter.Write(text);
		textWriter.Close();
		text2 += "\n};";
		TextWriter textWriter2 = new StreamWriter("positionByteLookUpTable.cs");
		textWriter2.Write(text2);
		textWriter2.Close();
	}

	public static void GenerateLookUpTables()
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		string text = "static Dictionary<byte, Vector3> bytePositionLookUpTable = new Dictionary<byte, Vector3>{\n";
		string text2 = "static Dictionary<Vector3, byte> positionByteLookUpTable = new Dictionary<Vector3, byte>{\n";
		for (int i = 0; i < positionsOnPlaneAxis; i++)
		{
			for (int j = 0; j < positionsOnPlaneAxis; j++)
			{
				for (int k = 0; k < positionsOnPlaneAxis; k++)
				{
					Vector3 val = new Vector3(((float)i - 2f) / 4f, ((float)j - 2f) / 4f, ((float)k - 2f) / 4f);
					bytePositionLookUpTable.Add((byte)num, val);
					positionByteLookUpTable.Add(val, (byte)num);
					string text3 = "new Vector3(" + val.x + "f, " + val.y + "f, " + val.z + "f)";
					string text4 = text;
					text = text4 + "{" + num + ", " + text3 + "},\n";
					text4 = text2;
					text2 = text4 + "{" + text3 + ", " + num + "},\n";
					num++;
				}
			}
		}
		text += "\n};";
		TextWriter textWriter = new StreamWriter("bytePositionLookUpTable.cs");
		textWriter.Write(text);
		textWriter.Close();
		text2 += "\n};";
		TextWriter textWriter2 = new StreamWriter("positionByteLookUpTable.cs");
		textWriter2.Write(text2);
		textWriter2.Close();
	}
}
