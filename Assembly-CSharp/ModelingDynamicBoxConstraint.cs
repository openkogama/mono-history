using MV.WorldObject;
using UnityEngine;

public class ModelingDynamicBoxConstraint : ModelingBoxConstraint
{
	private MVCubeModelBase cubeModel;

	public IntVector Size { get; private set; }

	public ModelingDynamicBoxConstraint(MVCubeModelBase cubeModel, IntVector constraintSize)
		: base(constraintSize)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		this.cubeModel = cubeModel;
		this.cubeModel.Changed += CubeModel_Changed;
		Size = constraintSize;
		Center = CalcConstraintBoxCenter(cubeModel);
	}

	public void DetachFromCubeModel()
	{
		cubeModel.Changed -= CubeModel_Changed;
	}

	public override bool CanAddCubeAt(IntVector pos)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = MathFunctions.AbsVector(new Vector3((float)pos.x, (float)pos.y, (float)pos.z));
		if (val.x > (float)(Size.x - 1) || val.y > (float)(Size.y - 1) || val.z > (float)(Size.z - 1))
		{
			return false;
		}
		Bounds meshBounds = cubeModel.GetMeshBounds();
		Vector3 val2 = new Vector3((float)pos.x - 0.5f, (float)pos.y - 0.5f, (float)pos.z - 0.5f);
		Vector3 val3 = new Vector3((float)pos.x + 0.5f, (float)pos.y + 0.5f, (float)pos.z + 0.5f);
		for (int i = 0; i < 3; i++)
		{
			float num = val2[i];
			Vector3 min = meshBounds.min;
			if (num < min[i])
			{
				Vector3 min2 = meshBounds.min;
				min2[i] = val2[i];
				meshBounds.SetMinMax(min2, meshBounds.max);
			}
			float num2 = val3[i];
			Vector3 max = meshBounds.max;
			if (num2 > max[i])
			{
				Vector3 max2 = meshBounds.max;
				max2[i] = val3[i];
				meshBounds.SetMinMax(meshBounds.min, max2);
			}
		}
		IntVector min3 = default;
		IntVector max3 = default;
		SharedCollisionFunctions.GetVoxelBounds(ref min3, ref max3, meshBounds);
		IntVector intVector = max3 - min3 + new IntVector(1, 1, 1);
		if (intVector.x <= Size.x && intVector.y <= Size.y && intVector.z <= Size.z)
		{
			return true;
		}
		return false;
	}

	public override bool CanRemoveCubeAt(IntVector pos)
	{
		return true;
	}

	public override bool CanEditCubeAt(IntVector pos)
	{
		return true;
	}

	private void CubeModel_Changed(object sender, CubeModelChangedEventArgs e)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		Center = CalcConstraintBoxCenter(cubeModel);
	}

	private Vector3 CalcConstraintBoxCenter(MVCubeModelBase model)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		Bounds meshBounds = model.GetMeshBounds();
		Vector3 v = meshBounds.center;
		for (int i = 0; i < 3; i++)
		{
			Vector3 cubeConstraintVector = SharedCubeFunctions.CubeConstraintVector3;
			if (cubeConstraintVector[i] % 2f == 0f)
			{
				if (v[i] % 1f != 0f)
				{
					v[i] = Mathf.Ceil(v[i]) - 0.5f;
				}
				continue;
			}
			IntVector min = default;
			IntVector max = default;
			SharedCollisionFunctions.GetVoxelBounds(ref min, ref max, model.GetMeshBounds());
			short num = (max - min + new IntVector(1, 1, 1))[i];
			Vector3 cubeConstraintVector2 = SharedCubeFunctions.CubeConstraintVector3;
			if (num == (int)cubeConstraintVector2[i])
			{
				int num2 = min[i] + (max[i] - min[i]) / 2;
				v[i] = num2;
			}
		}
		Vector3 val = (SharedCubeFunctions.CubeConstraintVector3 - Vector3.one * 1f) / 2f;
		MathFunctions.ClampVector(ref v, -val, val);
		return model.Transform.TransformPoint(v);
	}
}
