using System;
using UnityEngine;

namespace MV.WorldObject;

public class CubeBase
{
	protected byte unIndentedSides = 0;

	protected static byte[] identityByteCorners = new byte[8] { 20, 120, 124, 24, 4, 104, 100, 0 };

	protected byte[] byteCorners = (byte[])identityByteCorners.Clone();

	protected byte[] faceMaterials = new byte[6];

	private static readonly FaceFlags[] faceFlagsArray = (FaceFlags[])Enum.GetValues(typeof(FaceFlags));

	public byte[] ByteCorners => byteCorners;

	public byte[] FaceMaterials => faceMaterials;

	public static byte[] IdentityByteCorners => (byte[])identityByteCorners.Clone();

	public static Vector3[] IdentityCorners => CubeDataPacker.ByteArrayToCorners(identityByteCorners);

	public Vector3[] Corners
	{
		get
		{
			return CubeDataPacker.ByteArrayToCorners(byteCorners);
		}
		set
		{
			byteCorners = CubeDataPacker.CornersToByteArray(value);
		}
	}

	public static FaceFlags[] FaceFlagsArray => faceFlagsArray;

	public byte UnIndentedSides
	{
		get
		{
			return unIndentedSides;
		}
		set
		{
			unIndentedSides = value;
		}
	}

	public static void GetCorners(CubeBase cube, ref Vector3[] corners)
	{
		CubeDataPacker.ByteArrayToCorners(ref cube.byteCorners, ref corners);
	}

	public static byte GetMaterial(CubeBase cube, Face face)
	{
		if (cube == null)
		{
			return 0;
		}
		return cube.faceMaterials[(int)face];
	}

	public CubeBase(byte[] byteCorners, byte[] faceMaterials)
	{
		this.byteCorners = byteCorners;
		this.faceMaterials = faceMaterials;
		SetCubeFlags(this);
	}

	public CubeBase(BytePacker bp, byte byteFlags)
	{
		CubeDataPacker.ReadCompressedCube(byteFlags, bp, ref byteCorners, ref faceMaterials);
		SetCubeFlags(this);
	}

	public CubeBase(byte material)
		: this(identityByteCorners, new byte[6] { material, material, material, material, material, material })
	{
	}

	public static CubeBase Clone(CubeBase original)
	{
		if (original == null)
		{
			return null;
		}
		CubeBase cubeBase = new CubeBase((byte[])original.byteCorners.Clone(), (byte[])original.faceMaterials.Clone());
		SetCubeFlags(cubeBase);
		return cubeBase;
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (!(obj is CubeBase cube))
		{
			return false;
		}
		return Equals(cube);
	}

	public bool Equals(CubeBase cube)
	{
		if ((object)cube == null)
		{
			return false;
		}
		if (byteCorners.Length != cube.byteCorners.Length)
		{
			return false;
		}
		if (faceMaterials.Length != cube.faceMaterials.Length)
		{
			return false;
		}
		for (int i = 0; i < byteCorners.Length; i++)
		{
			if (byteCorners[i] != cube.byteCorners[i])
			{
				return false;
			}
		}
		for (int j = 0; j < faceMaterials.Length; j++)
		{
			if (faceMaterials[j] != cube.faceMaterials[j])
			{
				return false;
			}
		}
		return true;
	}

	public static bool operator ==(CubeBase a, CubeBase b)
	{
		if ((object)a == b)
		{
			return true;
		}
		if ((object)a == null || (object)b == null)
		{
			return false;
		}
		return a.Equals(b);
	}

	public static bool operator !=(CubeBase a, CubeBase b)
	{
		return !(a == b);
	}

	public override int GetHashCode()
	{
		int num = 0;
		byte[] array = byteCorners;
		foreach (byte b in array)
		{
			num += b;
		}
		byte[] array2 = faceMaterials;
		foreach (byte b2 in array2)
		{
			num += b2;
		}
		return num;
	}

	public static void SetCubeFlags(CubeBase cube)
	{
		cube.UnIndentedSides = 0;
		bool[] array = new bool[8];
		int num = 0;
		for (int i = 0; i < 8; i++)
		{
			array[i] = cube.byteCorners[i] != identityByteCorners[i];
			if (array[i])
			{
				num++;
			}
		}
		if (num == 0)
		{
			cube.UnIndentedSides = 63;
			return;
		}
		if (num > 4)
		{
			cube.UnIndentedSides = 0;
			return;
		}
		if (!array[0] && !array[1] && !array[2] && !array[3])
		{
			cube.UnIndentedSides |= 1;
		}
		if (!array[4] && !array[5] && !array[6] && !array[7])
		{
			cube.UnIndentedSides |= 2;
		}
		if (!array[2] && !array[3] && !array[4] && !array[5])
		{
			cube.UnIndentedSides |= 8;
		}
		if (!array[0] && !array[1] && !array[6] && !array[7])
		{
			cube.UnIndentedSides |= 4;
		}
		if (!array[0] && !array[3] && !array[4] && !array[7])
		{
			cube.UnIndentedSides |= 16;
		}
		if (!array[1] && !array[2] && !array[5] && !array[6])
		{
			cube.UnIndentedSides |= 32;
		}
	}

	public static Face FaceFlagToFace(FaceFlags faceFlag)
	{
		return faceFlag switch
		{
			FaceFlags.Top => Face.Top, 
			FaceFlags.Bottom => Face.Bottom, 
			FaceFlags.Front => Face.Front, 
			FaceFlags.Back => Face.Back, 
			FaceFlags.Left => Face.Left, 
			FaceFlags.Right => Face.Right, 
			_ => Face.Top, 
		};
	}

	public static FaceFlags FaceToFaceFlag(Face face)
	{
		return face switch
		{
			Face.Top => FaceFlags.Top, 
			Face.Bottom => FaceFlags.Bottom, 
			Face.Front => FaceFlags.Front, 
			Face.Back => FaceFlags.Back, 
			Face.Left => FaceFlags.Left, 
			Face.Right => FaceFlags.Right, 
			_ => (FaceFlags)0, 
		};
	}

	public static void GetFace(ref Vector3[] corners, ref Vector3[] faceVertices, Face face)
	{
		switch (face)
		{
		case Face.Top:
			faceVertices[0] = corners[0];
			faceVertices[1] = corners[1];
			faceVertices[2] = corners[2];
			faceVertices[3] = corners[3];
			break;
		case Face.Bottom:
			faceVertices[0] = corners[4];
			faceVertices[1] = corners[5];
			faceVertices[2] = corners[6];
			faceVertices[3] = corners[7];
			break;
		case Face.Back:
			faceVertices[0] = corners[5];
			faceVertices[1] = corners[4];
			faceVertices[2] = corners[3];
			faceVertices[3] = corners[2];
			break;
		case Face.Front:
			faceVertices[0] = corners[7];
			faceVertices[1] = corners[6];
			faceVertices[2] = corners[1];
			faceVertices[3] = corners[0];
			break;
		case Face.Left:
			faceVertices[0] = corners[4];
			faceVertices[1] = corners[7];
			faceVertices[2] = corners[0];
			faceVertices[3] = corners[3];
			break;
		case Face.Right:
			faceVertices[0] = corners[6];
			faceVertices[1] = corners[5];
			faceVertices[2] = corners[2];
			faceVertices[3] = corners[1];
			break;
		}
	}
}
