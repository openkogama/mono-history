using System;
using UnityEngine;

namespace MV.WorldObject;

public struct IntVector
{
	public short x;

	public short y;

	public short z;

	public static readonly IntVector One = new IntVector(1, 1, 1);

	public short this[int key]
	{
		get
		{
			return key switch
			{
				0 => x, 
				1 => y, 
				2 => z, 
				_ => throw new IndexOutOfRangeException(), 
			};
		}
		set
		{
			switch (key)
			{
			case 0:
				x = value;
				break;
			case 1:
				y = value;
				break;
			case 2:
				z = value;
				break;
			default:
				throw new IndexOutOfRangeException();
			}
		}
	}

	public override bool Equals(object obj)
	{
		if (obj is IntVector)
		{
			return Equals((IntVector)obj);
		}
		return false;
	}

	public bool Equals(IntVector iV)
	{
		if (x == iV.x && y == iV.y)
		{
			return z == iV.z;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return x + y * 1000 + z * 1000000;
	}

	public static bool operator ==(IntVector a, IntVector b)
	{
		return a.Equals(b);
	}

	public static bool operator !=(IntVector a, IntVector b)
	{
		return !(a == b);
	}

	public IntVector(short x, short y, short z)
	{
		this.x = x;
		this.y = y;
		this.z = z;
	}

	public IntVector(int x, int y, int z)
	{
		this.x = (short)x;
		this.y = (short)y;
		this.z = (short)z;
	}

	public override string ToString()
	{
		return "x: " + x + " y: " + y + " z: " + z;
	}

	public static IntVector operator +(IntVector i1, IntVector i2)
	{
		return new IntVector((short)(i1.x + i2.x), (short)(i1.y + i2.y), (short)(i1.z + i2.z));
	}

	public static IntVector operator -(IntVector i1, IntVector i2)
	{
		return new IntVector((short)(i1.x - i2.x), (short)(i1.y - i2.y), (short)(i1.z - i2.z));
	}

	public static IntVector operator *(int i, IntVector iV)
	{
		return new IntVector((short)(i * iV.x), (short)(i * iV.y), (short)(i * iV.z));
	}

	public static IntVector operator *(IntVector iV, int i)
	{
		return new IntVector((short)(i * iV.x), (short)(i * iV.y), (short)(i * iV.z));
	}

	public static Vector3 operator *(IntVector iV, Vector3 vector3)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		return new Vector3(vector3.x * (float)iV.x, vector3.y * (float)iV.y, vector3.z * (float)iV.z);
	}

	public static IntVector operator /(IntVector iV, int i)
	{
		return new IntVector((short)(iV.x / i), (short)(iV.y / i), (short)(iV.z / i));
	}

	public static int IntVectorToIndex(IntVector intVector, int chunkSize)
	{
		return intVector.x + intVector.y * chunkSize + intVector.z * chunkSize * chunkSize;
	}

	public static IntVector IndexToIntVector(int index, int chunkSize)
	{
		int num = chunkSize * chunkSize;
		int num2 = index / num;
		int num3 = index % num * chunkSize;
		int num4 = num3 / num;
		int num5 = num3 % num * chunkSize / num;
		return new IntVector((short)num5, (short)num4, (short)num2);
	}
}
