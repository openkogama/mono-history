using System;
using UnityEngine;

namespace MV.WorldObject;

public class CubeBase
{
	protected byte unIndentedSides;

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
		if (object.ReferenceEquals(a, b))
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
		{
			ref Vector3 reference21 = ref faceVertices[0];
			reference21 = corners[0];
			ref Vector3 reference22 = ref faceVertices[1];
			reference22 = corners[1];
			ref Vector3 reference23 = ref faceVertices[2];
			reference23 = corners[2];
			ref Vector3 reference24 = ref faceVertices[3];
			reference24 = corners[3];
			break;
		}
		case Face.Bottom:
		{
			ref Vector3 reference17 = ref faceVertices[0];
			reference17 = corners[4];
			ref Vector3 reference18 = ref faceVertices[1];
			reference18 = corners[5];
			ref Vector3 reference19 = ref faceVertices[2];
			reference19 = corners[6];
			ref Vector3 reference20 = ref faceVertices[3];
			reference20 = corners[7];
			break;
		}
		case Face.Back:
		{
			ref Vector3 reference13 = ref faceVertices[0];
			reference13 = corners[5];
			ref Vector3 reference14 = ref faceVertices[1];
			reference14 = corners[4];
			ref Vector3 reference15 = ref faceVertices[2];
			reference15 = corners[3];
			ref Vector3 reference16 = ref faceVertices[3];
			reference16 = corners[2];
			break;
		}
		case Face.Front:
		{
			ref Vector3 reference9 = ref faceVertices[0];
			reference9 = corners[7];
			ref Vector3 reference10 = ref faceVertices[1];
			reference10 = corners[6];
			ref Vector3 reference11 = ref faceVertices[2];
			reference11 = corners[1];
			ref Vector3 reference12 = ref faceVertices[3];
			reference12 = corners[0];
			break;
		}
		case Face.Left:
		{
			ref Vector3 reference5 = ref faceVertices[0];
			reference5 = corners[4];
			ref Vector3 reference6 = ref faceVertices[1];
			reference6 = corners[7];
			ref Vector3 reference7 = ref faceVertices[2];
			reference7 = corners[0];
			ref Vector3 reference8 = ref faceVertices[3];
			reference8 = corners[3];
			break;
		}
		case Face.Right:
		{
			ref Vector3 reference = ref faceVertices[0];
			reference = corners[6];
			ref Vector3 reference2 = ref faceVertices[1];
			reference2 = corners[5];
			ref Vector3 reference3 = ref faceVertices[2];
			reference3 = corners[2];
			ref Vector3 reference4 = ref faceVertices[3];
			reference4 = corners[1];
			break;
		}
		}
	}
}
