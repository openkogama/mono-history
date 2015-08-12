using System;
using CodeStage.AntiCheat.ObscuredTypes;
using MV.WorldObject;
using UnityEngine;

public struct ObscuredIntVector
{
	public ObscuredShort x;

	public ObscuredShort y;

	public ObscuredShort z;

	public static readonly ObscuredIntVector One = new ObscuredIntVector(1, 1, 1);

	public short this[int key]
	{
		get
		{
			return key switch
			{
				0 => (short)x, 
				1 => (short)y, 
				2 => (short)z, 
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

	public ObscuredIntVector(short x, short y, short z)
	{
		this.x = x;
		this.y = y;
		this.z = z;
	}

	public ObscuredIntVector(int x, int y, int z)
	{
		this.x = (short)x;
		this.y = (short)y;
		this.z = (short)z;
	}

	public ObscuredIntVector(IntVector intVector)
	{
		x = intVector.x;
		y = intVector.y;
		z = intVector.z;
	}

	public ObscuredIntVector(float x, float y, float z)
	{
		this.x = (short)x;
		this.y = (short)y;
		this.z = (short)z;
	}

	public override bool Equals(object obj)
	{
		return obj is ObscuredIntVector && Equals((ObscuredIntVector)obj);
	}

	public bool Equals(ObscuredIntVector iV)
	{
		return (short)x == (short)iV.x && (short)y == (short)iV.y && (short)z == (short)iV.z;
	}

	public override int GetHashCode()
	{
		return (short)x + (short)y * 1000 + (short)z * 1000000;
	}

	public Vector3 ToVector3()
	{
		return new Vector3((short)x, (short)y, (short)z);
	}

	public override string ToString()
	{
		return string.Concat("x: ", x, " y: ", y, " z: ", z);
	}

	public int SquareMagnitude()
	{
		return (short)x * (short)x + (short)y * (short)y + (short)z * (short)z;
	}

	public static int ObscuredIntVectorToIndex(ObscuredIntVector ObscuredIntVector, int chunkSize)
	{
		return (short)ObscuredIntVector.x + (short)ObscuredIntVector.y * chunkSize + (short)ObscuredIntVector.z * chunkSize * chunkSize;
	}

	public static ObscuredIntVector IndexToObscuredIntVector(int index, int chunkSize)
	{
		int num = chunkSize * chunkSize;
		int num2 = index / num;
		int num3 = index % num * chunkSize;
		int num4 = num3 / num;
		int num5 = num3 % num * chunkSize / num;
		return new ObscuredIntVector((short)num5, (short)num4, (short)num2);
	}

	public static bool operator ==(ObscuredIntVector a, ObscuredIntVector b)
	{
		return a.Equals(b);
	}

	public static bool operator !=(ObscuredIntVector a, ObscuredIntVector b)
	{
		return !(a == b);
	}

	public static ObscuredIntVector operator +(ObscuredIntVector i1)
	{
		return i1;
	}

	public static ObscuredIntVector operator -(ObscuredIntVector i1)
	{
		return new ObscuredIntVector(-(short)i1.x, -(short)i1.y, -(short)i1.z);
	}

	public static ObscuredIntVector operator +(ObscuredIntVector i1, ObscuredIntVector i2)
	{
		return new ObscuredIntVector((short)((short)i1.x + (short)i2.x), (short)((short)i1.y + (short)i2.y), (short)((short)i1.z + (short)i2.z));
	}

	public static ObscuredIntVector operator -(ObscuredIntVector i1, ObscuredIntVector i2)
	{
		return new ObscuredIntVector((short)((short)i1.x - (short)i2.x), (short)((short)i1.y - (short)i2.y), (short)((short)i1.z - (short)i2.z));
	}

	public static ObscuredIntVector operator *(int i, ObscuredIntVector iV)
	{
		return new ObscuredIntVector((short)(i * (short)iV.x), (short)(i * (short)iV.y), (short)(i * (short)iV.z));
	}

	public static ObscuredIntVector operator *(ObscuredIntVector iV, int i)
	{
		return new ObscuredIntVector((short)(i * (short)iV.x), (short)(i * (short)iV.y), (short)(i * (short)iV.z));
	}

	public static Vector3 operator *(ObscuredIntVector iV, Vector3 vector3)
	{
		return new Vector3(vector3.x * (float)(short)iV.x, vector3.y * (float)(short)iV.y, vector3.z * (float)(short)iV.z);
	}

	public static ObscuredIntVector operator /(ObscuredIntVector iV, int i)
	{
		return new ObscuredIntVector((short)((short)iV.x / i), (short)((short)iV.y / i), (short)((short)iV.z / i));
	}
}
