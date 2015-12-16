using System;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

public class BodyData : MonoBehaviour
{
	public string[] PartNames = new string[8] { "Head", "Torso", "RArm", "LArm", "RUpLeg", "RLowLeg", "LUpLeg", "LLowLeg" };

	public Transform[] PartBones;

	[SerializeField]
	private Vector3[] PartBoneSpacePosition = new Vector3[8]
	{
		new Vector3(0f, -7.7f, -1.5f),
		new Vector3(0f, -2.8f, -1.5f),
		new Vector3(0.5f, -7.5f, 2f),
		new Vector3(0.5f, -7.5f, 2f),
		new Vector3(-1f, -1.5f, 0f),
		new Vector3(-1f, -1.5f, 0f),
		new Vector3(-1f, -1.5f, 0f),
		new Vector3(-1f, -1.5f, 0f)
	};

	private ObscuredFloat[][] PartConstraintsBoxMin;

	private ObscuredFloat[][] PartConstraintsBoxMax;

	private ObscuredInt[] PartConstraintsMinCubes;

	private Dictionary<string, int> partMap = new Dictionary<string, int>();

	private void InitVariables()
	{
		PartConstraintsBoxMin = new ObscuredFloat[8][]
		{
			new ObscuredFloat[3] { -4f, 7f, -2f },
			new ObscuredFloat[3] { -4f, 2f, -2f },
			new ObscuredFloat[3] { -2f, 1f, -3f },
			new ObscuredFloat[3] { -2f, 1f, -3f },
			new ObscuredFloat[3] { -1f, 0f, -1f },
			new ObscuredFloat[3] { -1f, 0f, -1f },
			new ObscuredFloat[3] { -1f, 0f, -1f },
			new ObscuredFloat[3] { -1f, 0f, -2f }
		};
		PartConstraintsBoxMax = new ObscuredFloat[8][]
		{
			new ObscuredFloat[3] { 3f, 14f, 5f },
			new ObscuredFloat[3] { 3f, 8f, 5f },
			new ObscuredFloat[3] { 1f, 7f, -2f },
			new ObscuredFloat[3] { 1f, 7f, -2f },
			new ObscuredFloat[3] { 3f, 2f, 2f },
			new ObscuredFloat[3] { 2f, 1f, 2f },
			new ObscuredFloat[3] { 3f, 2f, 2f },
			new ObscuredFloat[3] { 2f, 1f, 1f }
		};
		PartConstraintsMinCubes = new ObscuredInt[8] { 20, 20, 3, 3, 3, 3, 3, 3 };
	}

	private void Awake()
	{
		for (int i = 0; i < PartNames.Length; i++)
		{
			partMap.Add(PartNames[i], i);
		}
	}

	public int GetPartIndex(string part)
	{
		return partMap[part];
	}

	public Transform GetPartBone(string part)
	{
		return PartBones[partMap[part]];
	}

	public Vector3 GetPartBoneSpacePosition(string part)
	{
		return PartBoneSpacePosition[partMap[part]];
	}

	public Vector3 GetPartConstraintMin(string part)
	{
		return GetVectorFromObscuredFloatArray(PartConstraintsBoxMin[partMap[part]]);
	}

	public Vector3 GetPartConstraintMax(string part)
	{
		return GetVectorFromObscuredFloatArray(PartConstraintsBoxMax[partMap[part]]);
	}

	public Vector3 GetPartConstraintCenter(string part)
	{
		Vector3 vectorFromObscuredFloatArray = GetVectorFromObscuredFloatArray(PartConstraintsBoxMin[partMap[part]]);
		Vector3 vectorFromObscuredFloatArray2 = GetVectorFromObscuredFloatArray(PartConstraintsBoxMax[partMap[part]]);
		return (vectorFromObscuredFloatArray + vectorFromObscuredFloatArray2) / 2f;
	}

	public int GetPartConstraintMinCount(string part)
	{
		return PartConstraintsMinCubes[partMap[part]];
	}

	private static Vector3 GetVectorFromObscuredFloatArray(ObscuredFloat[] values)
	{
		if (values.Length != 3)
		{
			throw new Exception("Invalid length of obscured float array for Vector3 " + values.Length);
		}
		return new Vector3(values[0], values[1], values[2]);
	}
}
