using System.Collections.Generic;
using UnityEngine;

public class BodyData : MonoBehaviour
{
	public string[] PartNames = new string[8] { "Head", "Torso", "RArm", "LArm", "RUpLeg", "RLowLeg", "LUpLeg", "LLowLeg" };

	public Transform[] PartBones;

	public Vector3[] PartBoneSpacePosition = new Vector3[8]
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

	public Vector3[] PartConstraintsBoxMin = new Vector3[8]
	{
		new Vector3(-4f, 7f, -2f),
		new Vector3(-3f, 2f, -2f),
		new Vector3(-3f, 1f, -5f),
		new Vector3(-3f, 1f, -5f),
		new Vector3(-3f, -3f, -3f),
		new Vector3(-3f, -1f, -3f),
		new Vector3(-3f, -3f, -3f),
		new Vector3(-3f, -1f, -3f)
	};

	public Vector3[] PartConstraintsBoxMax = new Vector3[8]
	{
		new Vector3(3f, 15f, 5f),
		new Vector3(3f, 8f, 5f),
		new Vector3(3f, 7f, 1f),
		new Vector3(3f, 7f, 1f),
		new Vector3(3f, 2f, 3f),
		new Vector3(3f, 3f, 3f),
		new Vector3(3f, 2f, 3f),
		new Vector3(3f, 3f, 3f)
	};

	public int[] PartConstraintsMinCubes = new int[8] { 100, 50, 10, 10, 19, 5, 19, 5 };

	private Dictionary<string, int> partMap = new Dictionary<string, int>();

	public BodyData()
	{
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		//IL_0346: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		return PartBoneSpacePosition[partMap[part]];
	}

	public Vector3 GetPartConstraintMin(string part)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		return PartConstraintsBoxMin[partMap[part]];
	}

	public Vector3 GetPartConstraintMax(string part)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		return PartConstraintsBoxMax[partMap[part]];
	}

	public Vector3 GetPartConstraintCenter(string part)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = PartConstraintsBoxMin[partMap[part]];
		Vector3 val2 = PartConstraintsBoxMax[partMap[part]];
		return (val + val2) / 2f;
	}

	public int GetPartConstraintMinCount(string part)
	{
		return PartConstraintsMinCubes[partMap[part]];
	}
}
