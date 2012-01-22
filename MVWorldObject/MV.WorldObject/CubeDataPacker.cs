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
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		byte[] array = new byte[8];
		for (int i = 0; i < 8; i++)
		{
			array[i] = Vector3ToByte(corners[i]);
		}
		return array;
	}

	public static Vector3 ByteToVector3(byte key)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return bytePositionLookUpTable[key];
	}

	public static void ByteToVector3(ref byte key, ref Vector3 vector)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		vector = bytePositionLookUpTable[key];
	}

	public static Vector3[] ByteArrayToCorners(byte[] byteArray)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		Vector3[] array = new Vector3[8];
		for (int i = 0; i < 8; i++)
		{
			ref Vector3 reference = ref array[i];
			reference = ByteToVector3(byteArray[i]);
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

	static CubeDataPacker()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0354: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_0391: Unknown result type (might be due to invalid IL or missing references)
		//IL_0396: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0415: Unknown result type (might be due to invalid IL or missing references)
		//IL_041a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0436: Unknown result type (might be due to invalid IL or missing references)
		//IL_043b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0457: Unknown result type (might be due to invalid IL or missing references)
		//IL_045c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0478: Unknown result type (might be due to invalid IL or missing references)
		//IL_047d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0499: Unknown result type (might be due to invalid IL or missing references)
		//IL_049e: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_04db: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0501: Unknown result type (might be due to invalid IL or missing references)
		//IL_051d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0522: Unknown result type (might be due to invalid IL or missing references)
		//IL_053e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0543: Unknown result type (might be due to invalid IL or missing references)
		//IL_055f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0564: Unknown result type (might be due to invalid IL or missing references)
		//IL_0580: Unknown result type (might be due to invalid IL or missing references)
		//IL_0585: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0604: Unknown result type (might be due to invalid IL or missing references)
		//IL_0609: Unknown result type (might be due to invalid IL or missing references)
		//IL_0625: Unknown result type (might be due to invalid IL or missing references)
		//IL_062a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0646: Unknown result type (might be due to invalid IL or missing references)
		//IL_064b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0667: Unknown result type (might be due to invalid IL or missing references)
		//IL_066c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0688: Unknown result type (might be due to invalid IL or missing references)
		//IL_068d: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_06eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_070c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0711: Unknown result type (might be due to invalid IL or missing references)
		//IL_072d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0732: Unknown result type (might be due to invalid IL or missing references)
		//IL_074e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0753: Unknown result type (might be due to invalid IL or missing references)
		//IL_076f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0774: Unknown result type (might be due to invalid IL or missing references)
		//IL_0790: Unknown result type (might be due to invalid IL or missing references)
		//IL_0795: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0814: Unknown result type (might be due to invalid IL or missing references)
		//IL_0819: Unknown result type (might be due to invalid IL or missing references)
		//IL_0835: Unknown result type (might be due to invalid IL or missing references)
		//IL_083a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0856: Unknown result type (might be due to invalid IL or missing references)
		//IL_085b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0877: Unknown result type (might be due to invalid IL or missing references)
		//IL_087c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0898: Unknown result type (might be due to invalid IL or missing references)
		//IL_089d: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_08be: Unknown result type (might be due to invalid IL or missing references)
		//IL_08da: Unknown result type (might be due to invalid IL or missing references)
		//IL_08df: Unknown result type (might be due to invalid IL or missing references)
		//IL_08fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0900: Unknown result type (might be due to invalid IL or missing references)
		//IL_091c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0921: Unknown result type (might be due to invalid IL or missing references)
		//IL_093d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0942: Unknown result type (might be due to invalid IL or missing references)
		//IL_095e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0963: Unknown result type (might be due to invalid IL or missing references)
		//IL_097f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0984: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_09e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_09e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a03: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a08: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a24: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a29: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a45: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a4a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a66: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a6b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a87: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a8c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aa8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ac9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ace: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b0b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b10: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b2c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b31: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b4d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b52: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b6e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b73: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b8f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b94: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bb0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bb5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bd1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bd6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bf2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bf7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c13: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c18: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c34: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c39: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c55: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c5a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c76: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c7b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c97: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c9c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cb8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cbd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cd9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cde: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cfa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d1b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d20: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d3c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d41: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d5d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d62: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d7e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d83: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d9f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0da4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dc0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dc5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0de1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0de6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e02: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e07: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e23: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e28: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e44: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e49: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e65: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e6a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e86: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e8b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ea7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0eac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ec8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ecd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ee9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0eee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f0a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f0f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f2b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f30: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f4c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f51: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f6d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f72: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f8e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f93: Unknown result type (might be due to invalid IL or missing references)
		//IL_0faf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fb4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fd0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fd5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ff1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ff6: Unknown result type (might be due to invalid IL or missing references)
		//IL_1012: Unknown result type (might be due to invalid IL or missing references)
		//IL_1017: Unknown result type (might be due to invalid IL or missing references)
		//IL_1038: Unknown result type (might be due to invalid IL or missing references)
		//IL_1053: Unknown result type (might be due to invalid IL or missing references)
		//IL_106e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1089: Unknown result type (might be due to invalid IL or missing references)
		//IL_10a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_10bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_10da: Unknown result type (might be due to invalid IL or missing references)
		//IL_10f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_1110: Unknown result type (might be due to invalid IL or missing references)
		//IL_112b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1147: Unknown result type (might be due to invalid IL or missing references)
		//IL_1163: Unknown result type (might be due to invalid IL or missing references)
		//IL_117f: Unknown result type (might be due to invalid IL or missing references)
		//IL_119b: Unknown result type (might be due to invalid IL or missing references)
		//IL_11b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_11d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_11ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_120b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1227: Unknown result type (might be due to invalid IL or missing references)
		//IL_1243: Unknown result type (might be due to invalid IL or missing references)
		//IL_125f: Unknown result type (might be due to invalid IL or missing references)
		//IL_127b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1297: Unknown result type (might be due to invalid IL or missing references)
		//IL_12b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_12cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_12eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_1307: Unknown result type (might be due to invalid IL or missing references)
		//IL_1323: Unknown result type (might be due to invalid IL or missing references)
		//IL_133f: Unknown result type (might be due to invalid IL or missing references)
		//IL_135b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1377: Unknown result type (might be due to invalid IL or missing references)
		//IL_1393: Unknown result type (might be due to invalid IL or missing references)
		//IL_13af: Unknown result type (might be due to invalid IL or missing references)
		//IL_13cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_13e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_1403: Unknown result type (might be due to invalid IL or missing references)
		//IL_141f: Unknown result type (might be due to invalid IL or missing references)
		//IL_143b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1457: Unknown result type (might be due to invalid IL or missing references)
		//IL_1473: Unknown result type (might be due to invalid IL or missing references)
		//IL_148f: Unknown result type (might be due to invalid IL or missing references)
		//IL_14ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_14c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_14e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_14ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_151b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1537: Unknown result type (might be due to invalid IL or missing references)
		//IL_1553: Unknown result type (might be due to invalid IL or missing references)
		//IL_156f: Unknown result type (might be due to invalid IL or missing references)
		//IL_158b: Unknown result type (might be due to invalid IL or missing references)
		//IL_15a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_15c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_15df: Unknown result type (might be due to invalid IL or missing references)
		//IL_15fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_1617: Unknown result type (might be due to invalid IL or missing references)
		//IL_1633: Unknown result type (might be due to invalid IL or missing references)
		//IL_164f: Unknown result type (might be due to invalid IL or missing references)
		//IL_166b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1687: Unknown result type (might be due to invalid IL or missing references)
		//IL_16a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_16bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_16db: Unknown result type (might be due to invalid IL or missing references)
		//IL_16f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_1713: Unknown result type (might be due to invalid IL or missing references)
		//IL_172f: Unknown result type (might be due to invalid IL or missing references)
		//IL_174b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1767: Unknown result type (might be due to invalid IL or missing references)
		//IL_1783: Unknown result type (might be due to invalid IL or missing references)
		//IL_179f: Unknown result type (might be due to invalid IL or missing references)
		//IL_17bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_17d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_17f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_180f: Unknown result type (might be due to invalid IL or missing references)
		//IL_182b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1847: Unknown result type (might be due to invalid IL or missing references)
		//IL_1863: Unknown result type (might be due to invalid IL or missing references)
		//IL_187f: Unknown result type (might be due to invalid IL or missing references)
		//IL_189b: Unknown result type (might be due to invalid IL or missing references)
		//IL_18b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_18d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_18ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_190b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1927: Unknown result type (might be due to invalid IL or missing references)
		//IL_1943: Unknown result type (might be due to invalid IL or missing references)
		//IL_195f: Unknown result type (might be due to invalid IL or missing references)
		//IL_197b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1997: Unknown result type (might be due to invalid IL or missing references)
		//IL_19b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_19cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_19eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a07: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a23: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a3f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a5b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a77: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a93: Unknown result type (might be due to invalid IL or missing references)
		//IL_1aaf: Unknown result type (might be due to invalid IL or missing references)
		//IL_1acb: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ae7: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b03: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b1f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b3b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b57: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b73: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b8f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bab: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bc7: Unknown result type (might be due to invalid IL or missing references)
		//IL_1be3: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bff: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c1b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c37: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c53: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c6f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c8b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ca7: Unknown result type (might be due to invalid IL or missing references)
		//IL_1cc3: Unknown result type (might be due to invalid IL or missing references)
		//IL_1cdf: Unknown result type (might be due to invalid IL or missing references)
		//IL_1cfb: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d17: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d33: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d4f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d6b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d87: Unknown result type (might be due to invalid IL or missing references)
		//IL_1da3: Unknown result type (might be due to invalid IL or missing references)
		//IL_1dbf: Unknown result type (might be due to invalid IL or missing references)
	}
}
