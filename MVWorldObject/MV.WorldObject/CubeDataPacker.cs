using System.Collections.Generic;
using UnityEngine;

namespace MV.WorldObject;

public static class CubeDataPacker
{
	private static Vector3[] bytePositionLookUpTable = new Vector3[125]
	{
		new Vector3(-0.5f, -0.5f, -0.5f),
		new Vector3(-0.5f, -0.5f, -0.25f),
		new Vector3(-0.5f, -0.5f, 0f),
		new Vector3(-0.5f, -0.5f, 0.25f),
		new Vector3(-0.5f, -0.5f, 0.5f),
		new Vector3(-0.5f, -0.25f, -0.5f),
		new Vector3(-0.5f, -0.25f, -0.25f),
		new Vector3(-0.5f, -0.25f, 0f),
		new Vector3(-0.5f, -0.25f, 0.25f),
		new Vector3(-0.5f, -0.25f, 0.5f),
		new Vector3(-0.5f, 0f, -0.5f),
		new Vector3(-0.5f, 0f, -0.25f),
		new Vector3(-0.5f, 0f, 0f),
		new Vector3(-0.5f, 0f, 0.25f),
		new Vector3(-0.5f, 0f, 0.5f),
		new Vector3(-0.5f, 0.25f, -0.5f),
		new Vector3(-0.5f, 0.25f, -0.25f),
		new Vector3(-0.5f, 0.25f, 0f),
		new Vector3(-0.5f, 0.25f, 0.25f),
		new Vector3(-0.5f, 0.25f, 0.5f),
		new Vector3(-0.5f, 0.5f, -0.5f),
		new Vector3(-0.5f, 0.5f, -0.25f),
		new Vector3(-0.5f, 0.5f, 0f),
		new Vector3(-0.5f, 0.5f, 0.25f),
		new Vector3(-0.5f, 0.5f, 0.5f),
		new Vector3(-0.25f, -0.5f, -0.5f),
		new Vector3(-0.25f, -0.5f, -0.25f),
		new Vector3(-0.25f, -0.5f, 0f),
		new Vector3(-0.25f, -0.5f, 0.25f),
		new Vector3(-0.25f, -0.5f, 0.5f),
		new Vector3(-0.25f, -0.25f, -0.5f),
		new Vector3(-0.25f, -0.25f, -0.25f),
		new Vector3(-0.25f, -0.25f, 0f),
		new Vector3(-0.25f, -0.25f, 0.25f),
		new Vector3(-0.25f, -0.25f, 0.5f),
		new Vector3(-0.25f, 0f, -0.5f),
		new Vector3(-0.25f, 0f, -0.25f),
		new Vector3(-0.25f, 0f, 0f),
		new Vector3(-0.25f, 0f, 0.25f),
		new Vector3(-0.25f, 0f, 0.5f),
		new Vector3(-0.25f, 0.25f, -0.5f),
		new Vector3(-0.25f, 0.25f, -0.25f),
		new Vector3(-0.25f, 0.25f, 0f),
		new Vector3(-0.25f, 0.25f, 0.25f),
		new Vector3(-0.25f, 0.25f, 0.5f),
		new Vector3(-0.25f, 0.5f, -0.5f),
		new Vector3(-0.25f, 0.5f, -0.25f),
		new Vector3(-0.25f, 0.5f, 0f),
		new Vector3(-0.25f, 0.5f, 0.25f),
		new Vector3(-0.25f, 0.5f, 0.5f),
		new Vector3(0f, -0.5f, -0.5f),
		new Vector3(0f, -0.5f, -0.25f),
		new Vector3(0f, -0.5f, 0f),
		new Vector3(0f, -0.5f, 0.25f),
		new Vector3(0f, -0.5f, 0.5f),
		new Vector3(0f, -0.25f, -0.5f),
		new Vector3(0f, -0.25f, -0.25f),
		new Vector3(0f, -0.25f, 0f),
		new Vector3(0f, -0.25f, 0.25f),
		new Vector3(0f, -0.25f, 0.5f),
		new Vector3(0f, 0f, -0.5f),
		new Vector3(0f, 0f, -0.25f),
		new Vector3(0f, 0f, 0f),
		new Vector3(0f, 0f, 0.25f),
		new Vector3(0f, 0f, 0.5f),
		new Vector3(0f, 0.25f, -0.5f),
		new Vector3(0f, 0.25f, -0.25f),
		new Vector3(0f, 0.25f, 0f),
		new Vector3(0f, 0.25f, 0.25f),
		new Vector3(0f, 0.25f, 0.5f),
		new Vector3(0f, 0.5f, -0.5f),
		new Vector3(0f, 0.5f, -0.25f),
		new Vector3(0f, 0.5f, 0f),
		new Vector3(0f, 0.5f, 0.25f),
		new Vector3(0f, 0.5f, 0.5f),
		new Vector3(0.25f, -0.5f, -0.5f),
		new Vector3(0.25f, -0.5f, -0.25f),
		new Vector3(0.25f, -0.5f, 0f),
		new Vector3(0.25f, -0.5f, 0.25f),
		new Vector3(0.25f, -0.5f, 0.5f),
		new Vector3(0.25f, -0.25f, -0.5f),
		new Vector3(0.25f, -0.25f, -0.25f),
		new Vector3(0.25f, -0.25f, 0f),
		new Vector3(0.25f, -0.25f, 0.25f),
		new Vector3(0.25f, -0.25f, 0.5f),
		new Vector3(0.25f, 0f, -0.5f),
		new Vector3(0.25f, 0f, -0.25f),
		new Vector3(0.25f, 0f, 0f),
		new Vector3(0.25f, 0f, 0.25f),
		new Vector3(0.25f, 0f, 0.5f),
		new Vector3(0.25f, 0.25f, -0.5f),
		new Vector3(0.25f, 0.25f, -0.25f),
		new Vector3(0.25f, 0.25f, 0f),
		new Vector3(0.25f, 0.25f, 0.25f),
		new Vector3(0.25f, 0.25f, 0.5f),
		new Vector3(0.25f, 0.5f, -0.5f),
		new Vector3(0.25f, 0.5f, -0.25f),
		new Vector3(0.25f, 0.5f, 0f),
		new Vector3(0.25f, 0.5f, 0.25f),
		new Vector3(0.25f, 0.5f, 0.5f),
		new Vector3(0.5f, -0.5f, -0.5f),
		new Vector3(0.5f, -0.5f, -0.25f),
		new Vector3(0.5f, -0.5f, 0f),
		new Vector3(0.5f, -0.5f, 0.25f),
		new Vector3(0.5f, -0.5f, 0.5f),
		new Vector3(0.5f, -0.25f, -0.5f),
		new Vector3(0.5f, -0.25f, -0.25f),
		new Vector3(0.5f, -0.25f, 0f),
		new Vector3(0.5f, -0.25f, 0.25f),
		new Vector3(0.5f, -0.25f, 0.5f),
		new Vector3(0.5f, 0f, -0.5f),
		new Vector3(0.5f, 0f, -0.25f),
		new Vector3(0.5f, 0f, 0f),
		new Vector3(0.5f, 0f, 0.25f),
		new Vector3(0.5f, 0f, 0.5f),
		new Vector3(0.5f, 0.25f, -0.5f),
		new Vector3(0.5f, 0.25f, -0.25f),
		new Vector3(0.5f, 0.25f, 0f),
		new Vector3(0.5f, 0.25f, 0.25f),
		new Vector3(0.5f, 0.25f, 0.5f),
		new Vector3(0.5f, 0.5f, -0.5f),
		new Vector3(0.5f, 0.5f, -0.25f),
		new Vector3(0.5f, 0.5f, 0f),
		new Vector3(0.5f, 0.5f, 0.25f),
		new Vector3(0.5f, 0.5f, 0.5f)
	};

	private static Dictionary<Vector3, byte> positionByteLookUpTable = new Dictionary<Vector3, byte>
	{
		{
			new Vector3(-0.5f, -0.5f, -0.5f),
			0
		},
		{
			new Vector3(-0.5f, -0.5f, -0.25f),
			1
		},
		{
			new Vector3(-0.5f, -0.5f, 0f),
			2
		},
		{
			new Vector3(-0.5f, -0.5f, 0.25f),
			3
		},
		{
			new Vector3(-0.5f, -0.5f, 0.5f),
			4
		},
		{
			new Vector3(-0.5f, -0.25f, -0.5f),
			5
		},
		{
			new Vector3(-0.5f, -0.25f, -0.25f),
			6
		},
		{
			new Vector3(-0.5f, -0.25f, 0f),
			7
		},
		{
			new Vector3(-0.5f, -0.25f, 0.25f),
			8
		},
		{
			new Vector3(-0.5f, -0.25f, 0.5f),
			9
		},
		{
			new Vector3(-0.5f, 0f, -0.5f),
			10
		},
		{
			new Vector3(-0.5f, 0f, -0.25f),
			11
		},
		{
			new Vector3(-0.5f, 0f, 0f),
			12
		},
		{
			new Vector3(-0.5f, 0f, 0.25f),
			13
		},
		{
			new Vector3(-0.5f, 0f, 0.5f),
			14
		},
		{
			new Vector3(-0.5f, 0.25f, -0.5f),
			15
		},
		{
			new Vector3(-0.5f, 0.25f, -0.25f),
			16
		},
		{
			new Vector3(-0.5f, 0.25f, 0f),
			17
		},
		{
			new Vector3(-0.5f, 0.25f, 0.25f),
			18
		},
		{
			new Vector3(-0.5f, 0.25f, 0.5f),
			19
		},
		{
			new Vector3(-0.5f, 0.5f, -0.5f),
			20
		},
		{
			new Vector3(-0.5f, 0.5f, -0.25f),
			21
		},
		{
			new Vector3(-0.5f, 0.5f, 0f),
			22
		},
		{
			new Vector3(-0.5f, 0.5f, 0.25f),
			23
		},
		{
			new Vector3(-0.5f, 0.5f, 0.5f),
			24
		},
		{
			new Vector3(-0.25f, -0.5f, -0.5f),
			25
		},
		{
			new Vector3(-0.25f, -0.5f, -0.25f),
			26
		},
		{
			new Vector3(-0.25f, -0.5f, 0f),
			27
		},
		{
			new Vector3(-0.25f, -0.5f, 0.25f),
			28
		},
		{
			new Vector3(-0.25f, -0.5f, 0.5f),
			29
		},
		{
			new Vector3(-0.25f, -0.25f, -0.5f),
			30
		},
		{
			new Vector3(-0.25f, -0.25f, -0.25f),
			31
		},
		{
			new Vector3(-0.25f, -0.25f, 0f),
			32
		},
		{
			new Vector3(-0.25f, -0.25f, 0.25f),
			33
		},
		{
			new Vector3(-0.25f, -0.25f, 0.5f),
			34
		},
		{
			new Vector3(-0.25f, 0f, -0.5f),
			35
		},
		{
			new Vector3(-0.25f, 0f, -0.25f),
			36
		},
		{
			new Vector3(-0.25f, 0f, 0f),
			37
		},
		{
			new Vector3(-0.25f, 0f, 0.25f),
			38
		},
		{
			new Vector3(-0.25f, 0f, 0.5f),
			39
		},
		{
			new Vector3(-0.25f, 0.25f, -0.5f),
			40
		},
		{
			new Vector3(-0.25f, 0.25f, -0.25f),
			41
		},
		{
			new Vector3(-0.25f, 0.25f, 0f),
			42
		},
		{
			new Vector3(-0.25f, 0.25f, 0.25f),
			43
		},
		{
			new Vector3(-0.25f, 0.25f, 0.5f),
			44
		},
		{
			new Vector3(-0.25f, 0.5f, -0.5f),
			45
		},
		{
			new Vector3(-0.25f, 0.5f, -0.25f),
			46
		},
		{
			new Vector3(-0.25f, 0.5f, 0f),
			47
		},
		{
			new Vector3(-0.25f, 0.5f, 0.25f),
			48
		},
		{
			new Vector3(-0.25f, 0.5f, 0.5f),
			49
		},
		{
			new Vector3(0f, -0.5f, -0.5f),
			50
		},
		{
			new Vector3(0f, -0.5f, -0.25f),
			51
		},
		{
			new Vector3(0f, -0.5f, 0f),
			52
		},
		{
			new Vector3(0f, -0.5f, 0.25f),
			53
		},
		{
			new Vector3(0f, -0.5f, 0.5f),
			54
		},
		{
			new Vector3(0f, -0.25f, -0.5f),
			55
		},
		{
			new Vector3(0f, -0.25f, -0.25f),
			56
		},
		{
			new Vector3(0f, -0.25f, 0f),
			57
		},
		{
			new Vector3(0f, -0.25f, 0.25f),
			58
		},
		{
			new Vector3(0f, -0.25f, 0.5f),
			59
		},
		{
			new Vector3(0f, 0f, -0.5f),
			60
		},
		{
			new Vector3(0f, 0f, -0.25f),
			61
		},
		{
			new Vector3(0f, 0f, 0f),
			62
		},
		{
			new Vector3(0f, 0f, 0.25f),
			63
		},
		{
			new Vector3(0f, 0f, 0.5f),
			64
		},
		{
			new Vector3(0f, 0.25f, -0.5f),
			65
		},
		{
			new Vector3(0f, 0.25f, -0.25f),
			66
		},
		{
			new Vector3(0f, 0.25f, 0f),
			67
		},
		{
			new Vector3(0f, 0.25f, 0.25f),
			68
		},
		{
			new Vector3(0f, 0.25f, 0.5f),
			69
		},
		{
			new Vector3(0f, 0.5f, -0.5f),
			70
		},
		{
			new Vector3(0f, 0.5f, -0.25f),
			71
		},
		{
			new Vector3(0f, 0.5f, 0f),
			72
		},
		{
			new Vector3(0f, 0.5f, 0.25f),
			73
		},
		{
			new Vector3(0f, 0.5f, 0.5f),
			74
		},
		{
			new Vector3(0.25f, -0.5f, -0.5f),
			75
		},
		{
			new Vector3(0.25f, -0.5f, -0.25f),
			76
		},
		{
			new Vector3(0.25f, -0.5f, 0f),
			77
		},
		{
			new Vector3(0.25f, -0.5f, 0.25f),
			78
		},
		{
			new Vector3(0.25f, -0.5f, 0.5f),
			79
		},
		{
			new Vector3(0.25f, -0.25f, -0.5f),
			80
		},
		{
			new Vector3(0.25f, -0.25f, -0.25f),
			81
		},
		{
			new Vector3(0.25f, -0.25f, 0f),
			82
		},
		{
			new Vector3(0.25f, -0.25f, 0.25f),
			83
		},
		{
			new Vector3(0.25f, -0.25f, 0.5f),
			84
		},
		{
			new Vector3(0.25f, 0f, -0.5f),
			85
		},
		{
			new Vector3(0.25f, 0f, -0.25f),
			86
		},
		{
			new Vector3(0.25f, 0f, 0f),
			87
		},
		{
			new Vector3(0.25f, 0f, 0.25f),
			88
		},
		{
			new Vector3(0.25f, 0f, 0.5f),
			89
		},
		{
			new Vector3(0.25f, 0.25f, -0.5f),
			90
		},
		{
			new Vector3(0.25f, 0.25f, -0.25f),
			91
		},
		{
			new Vector3(0.25f, 0.25f, 0f),
			92
		},
		{
			new Vector3(0.25f, 0.25f, 0.25f),
			93
		},
		{
			new Vector3(0.25f, 0.25f, 0.5f),
			94
		},
		{
			new Vector3(0.25f, 0.5f, -0.5f),
			95
		},
		{
			new Vector3(0.25f, 0.5f, -0.25f),
			96
		},
		{
			new Vector3(0.25f, 0.5f, 0f),
			97
		},
		{
			new Vector3(0.25f, 0.5f, 0.25f),
			98
		},
		{
			new Vector3(0.25f, 0.5f, 0.5f),
			99
		},
		{
			new Vector3(0.5f, -0.5f, -0.5f),
			100
		},
		{
			new Vector3(0.5f, -0.5f, -0.25f),
			101
		},
		{
			new Vector3(0.5f, -0.5f, 0f),
			102
		},
		{
			new Vector3(0.5f, -0.5f, 0.25f),
			103
		},
		{
			new Vector3(0.5f, -0.5f, 0.5f),
			104
		},
		{
			new Vector3(0.5f, -0.25f, -0.5f),
			105
		},
		{
			new Vector3(0.5f, -0.25f, -0.25f),
			106
		},
		{
			new Vector3(0.5f, -0.25f, 0f),
			107
		},
		{
			new Vector3(0.5f, -0.25f, 0.25f),
			108
		},
		{
			new Vector3(0.5f, -0.25f, 0.5f),
			109
		},
		{
			new Vector3(0.5f, 0f, -0.5f),
			110
		},
		{
			new Vector3(0.5f, 0f, -0.25f),
			111
		},
		{
			new Vector3(0.5f, 0f, 0f),
			112
		},
		{
			new Vector3(0.5f, 0f, 0.25f),
			113
		},
		{
			new Vector3(0.5f, 0f, 0.5f),
			114
		},
		{
			new Vector3(0.5f, 0.25f, -0.5f),
			115
		},
		{
			new Vector3(0.5f, 0.25f, -0.25f),
			116
		},
		{
			new Vector3(0.5f, 0.25f, 0f),
			117
		},
		{
			new Vector3(0.5f, 0.25f, 0.25f),
			118
		},
		{
			new Vector3(0.5f, 0.25f, 0.5f),
			119
		},
		{
			new Vector3(0.5f, 0.5f, -0.5f),
			120
		},
		{
			new Vector3(0.5f, 0.5f, -0.25f),
			121
		},
		{
			new Vector3(0.5f, 0.5f, 0f),
			122
		},
		{
			new Vector3(0.5f, 0.5f, 0.25f),
			123
		},
		{
			new Vector3(0.5f, 0.5f, 0.5f),
			124
		}
	};

	private static readonly byte[] IdentityByteCorners = new byte[8] { 20, 120, 124, 24, 4, 104, 100, 0 };

	private static int rowMaxLength = 63;

	public static byte Vector3ToByte(Vector3 corner)
	{
		if (!positionByteLookUpTable.ContainsKey(corner))
		{
			for (int i = 0; i < 3; i++)
			{
				if (corner[i] == 0f)
				{
					corner[i] = 0f;
				}
			}
			return positionByteLookUpTable[corner];
		}
		return positionByteLookUpTable[corner];
	}

	public static byte[] CornersToByteArray(Vector3[] corners)
	{
		byte[] array = new byte[8];
		for (int i = 0; i < 8; i++)
		{
			array[i] = Vector3ToByte(corners[i]);
		}
		return array;
	}

	public static Vector3 ByteToVector3(byte key)
	{
		return bytePositionLookUpTable[key];
	}

	public static void ByteToVector3(ref byte key, ref Vector3 vector)
	{
		vector = bytePositionLookUpTable[key];
	}

	public static Vector3[] ByteArrayToCorners(byte[] byteArray)
	{
		Vector3[] array = new Vector3[8];
		for (int i = 0; i < 8; i++)
		{
			array[i] = ByteToVector3(byteArray[i]);
		}
		return array;
	}

	public static void ByteArrayToCorners(ref byte[] byteArray, ref Vector3[] corners)
	{
		for (int i = 0; i < 8; i++)
		{
			ByteToVector3(ref byteArray[i], ref corners[i]);
		}
	}

	public static void WriteCompressedCube(BytePacker bp, short x, short y, short z, byte[] byteCorners, byte[] materials)
	{
		bp.Write(x);
		bp.Write(y);
		bp.Write(z);
		WriteCompressedCubeData(bp, byteCorners, materials);
	}

	private static void GetCompressionFlags(ref byte compressionFlags, byte[] byteCorners, byte[] materials)
	{
		bool flag = true;
		for (int i = 0; i < byteCorners.Length; i++)
		{
			if (byteCorners[i] != IdentityByteCorners[i])
			{
				flag = false;
			}
		}
		bool flag2 = true;
		for (int j = 0; j < 6; j++)
		{
			if (j > 0 && materials[j - 1] != materials[j])
			{
				flag2 = false;
			}
		}
		if (flag)
		{
			compressionFlags |= 1;
		}
		if (flag2)
		{
			compressionFlags |= 2;
		}
	}

	public static void WriteCompressedCubeData(BytePacker bp, byte[] byteCorners, byte[] materials)
	{
		byte compressionFlags = 0;
		GetCompressionFlags(ref compressionFlags, byteCorners, materials);
		compressionFlags |= 4;
		bp.Write(compressionFlags);
		if ((compressionFlags & 1) == 0)
		{
			bp.Write(byteCorners);
		}
		if ((compressionFlags & 2) == 0)
		{
			bp.Write(materials);
		}
		else
		{
			bp.Write(materials[0]);
		}
	}

	public static int GetDataLength(BytePacker bp)
	{
		byte b = bp.ReadByte();
		bp.Position--;
		int num = 1;
		if ((b & 1) == 0)
		{
			num += 8;
		}
		if ((b & 2) == 0)
		{
			return num + 6;
		}
		return num + 1;
	}

	public static void ReadCompressedCube(byte cubeFlags, BytePacker bp, ref byte[] byteCorners, ref byte[] materials)
	{
		if ((cubeFlags & 1) == 0)
		{
			byteCorners = bp.ReadBytes(8);
		}
		else
		{
			byteCorners = IdentityByteCorners;
		}
		if ((cubeFlags & 2) == 0)
		{
			materials = bp.ReadBytes(6);
			return;
		}
		byte b = bp.ReadByte();
		for (int i = 0; i < 6; i++)
		{
			materials[i] = b;
		}
	}

	public static void AddCube(IntVector pos, byte[] cubeData, ref Dictionary<IntVector, byte[]> cubeDict)
	{
		IntVector intVector = new IntVector(0, 0, 0);
		if (cubeDict.ContainsKey(pos))
		{
			if (AreCubesEqual(cubeDict[pos], cubeData))
			{
				return;
			}
			int cubesInRow = GetCubesInRow(cubeDict[pos][0]);
			if (cubesInRow > 1)
			{
				intVector = pos;
				intVector.x++;
				SetCubesInRow(ref cubeDict[intVector][0], cubesInRow - 1);
			}
		}
		cubeDict[pos] = cubeData;
		SetCubesInRow(ref cubeDict[pos][0], 1);
		bool cubeOriginal = GetCubeOriginal(pos, left: true, ref cubeDict, out var targetCube);
		bool flag = false;
		int num = 0;
		int num2 = 0;
		if (cubeOriginal)
		{
			flag = AreCubesEqual(cubeData, cubeDict[targetCube]);
			num = GetCubesInRow(cubeDict[targetCube][0]);
			num2 = pos.x - targetCube.x;
		}
		if (cubeOriginal && !flag && num2 < num)
		{
			int num3 = num2;
			int num4 = num - num3 - 1;
			SetCubesInRow(ref cubeDict[targetCube][0], num3);
			CombineRows(targetCube, ref cubeDict);
			if (num4 > 0)
			{
				intVector = pos;
				intVector.x++;
				SetCubesInRow(ref cubeDict[intVector][0], num4);
				CombineRows(intVector, ref cubeDict);
			}
			else
			{
				CombineRows(pos, ref cubeDict);
			}
		}
		else
		{
			CombineRows(pos, ref cubeDict);
		}
	}

	public static void RemoveCube(IntVector pos, ref Dictionary<IntVector, byte[]> cubeDict)
	{
		IntVector intVector = new IntVector(0, 0, 0);
		if (!cubeDict.ContainsKey(pos))
		{
			return;
		}
		int cubesInRow = GetCubesInRow(cubeDict[pos][0]);
		if (cubesInRow > 1)
		{
			intVector = pos;
			intVector.x++;
			SetCubesInRow(ref cubeDict[intVector][0], cubesInRow - 1);
			cubeDict.Remove(pos);
			CombineRows(intVector, ref cubeDict);
			return;
		}
		cubeDict.Remove(pos);
		bool cubeOriginal = GetCubeOriginal(pos, left: true, ref cubeDict, out var targetCube);
		int num = 0;
		int num2 = 0;
		if (cubeOriginal)
		{
			num = GetCubesInRow(cubeDict[targetCube][0]);
			num2 = pos.x - targetCube.x;
		}
		if (cubeOriginal && num2 < num)
		{
			int num3 = num2;
			int num4 = num - num3 - 1;
			SetCubesInRow(ref cubeDict[targetCube][0], num3);
			CombineRows(targetCube, ref cubeDict);
			if (num4 > 0)
			{
				intVector = pos;
				intVector.x++;
				SetCubesInRow(ref cubeDict[intVector][0], num4);
				CombineRows(intVector, ref cubeDict);
			}
		}
	}

	private static void CombineRows(IntVector pos, ref Dictionary<IntVector, byte[]> cubeDict)
	{
		byte[] array = cubeDict[pos];
		int cubesInRow = GetCubesInRow(array[0]);
		bool cubeOriginal = GetCubeOriginal(pos, left: true, ref cubeDict, out var targetCube);
		bool cubeOriginal2 = GetCubeOriginal(pos, left: false, ref cubeDict, out var targetCube2);
		bool flag = false;
		if (cubeOriginal && AreCubesEqual(array, cubeDict[targetCube]))
		{
			int cubesInRow2 = GetCubesInRow(cubeDict[targetCube][0]);
			if (cubesInRow2 + cubesInRow <= rowMaxLength)
			{
				SetCubesInRow(ref cubeDict[targetCube][0], cubesInRow2 + cubesInRow);
				SetCubesInRow(ref cubeDict[pos][0], 0);
				flag = true;
			}
		}
		if (!cubeOriginal2 || !AreCubesEqual(array, cubeDict[targetCube2]))
		{
			return;
		}
		if (flag)
		{
			int cubesInRow3 = GetCubesInRow(cubeDict[targetCube][0]);
			int cubesInRow4 = GetCubesInRow(cubeDict[targetCube2][0]);
			if (cubesInRow3 + cubesInRow4 <= rowMaxLength)
			{
				SetCubesInRow(ref cubeDict[targetCube][0], cubesInRow3 + cubesInRow4);
				SetCubesInRow(ref cubeDict[targetCube2][0], 0);
			}
		}
		else
		{
			int cubesInRow5 = GetCubesInRow(cubeDict[targetCube2][0]);
			if (cubesInRow + cubesInRow5 <= rowMaxLength)
			{
				SetCubesInRow(ref cubeDict[pos][0], cubesInRow + cubesInRow5);
				SetCubesInRow(ref cubeDict[targetCube2][0], 0);
			}
		}
	}

	public static byte[] GetCubeByteData(IntVector pos, ref Dictionary<IntVector, byte[]> cubeDict)
	{
		while (true)
		{
			if (!cubeDict.ContainsKey(pos))
			{
				return null;
			}
			if (GetCubesInRow(cubeDict[pos][0]) != 0)
			{
				break;
			}
			pos.x--;
		}
		return cubeDict[pos];
	}

	private static bool GetCubeOriginal(IntVector pos, bool left, ref Dictionary<IntVector, byte[]> cubeDict, out IntVector targetCube)
	{
		short num = 1;
		if (left)
		{
			num = -1;
		}
		targetCube = new IntVector((short)(pos.x + num), pos.y, pos.z);
		while (true)
		{
			if (!cubeDict.ContainsKey(targetCube))
			{
				return false;
			}
			if (GetCubesInRow(cubeDict[targetCube][0]) != 0)
			{
				break;
			}
			targetCube.x += num;
		}
		return true;
	}

	private static void SetCubesInRow(ref byte cubeFlags, int cubesInRow)
	{
		cubeFlags &= 3;
		cubesInRow <<= 2;
		cubeFlags |= (byte)cubesInRow;
	}

	public static int GetCubesInRow(byte cubeFlags)
	{
		return cubeFlags >> 2;
	}

	private static bool AreCubesEqual(byte[] cube0, byte[] cube1)
	{
		if (cube0.Length != cube1.Length)
		{
			return false;
		}
		for (int i = 1; i < cube0.Length; i++)
		{
			if (cube0[i] != cube1[i])
			{
				return false;
			}
		}
		return true;
	}
}
