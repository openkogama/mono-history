using System;
using MV.WorldObject;
using UnityEngine;

public class ModelingDynamicBoxConstraint : ModelingBoxConstraint
{
	private MVCubeModelBase cubeModel;

	public ObscuredIntVector Size { get; private set; }

	public ModelingDynamicBoxConstraint(MVCubeModelBase cubeModel, IntVector constraintSize)
		: base(constraintSize)
	{
		this.cubeModel = cubeModel;
		MVCubeModelBase mVCubeModelBase = this.cubeModel;
		mVCubeModelBase.Changed = (Action<CubeModelChangedEventArgs>)Delegate.Combine(mVCubeModelBase.Changed, new Action<CubeModelChangedEventArgs>(CubeModel_Changed));
		Size = new ObscuredIntVector(constraintSize);
		Center = CalcConstraintBoxCenter(cubeModel);
	}

	public void DetachFromCubeModel()
	{
		MVCubeModelBase mVCubeModelBase = cubeModel;
		mVCubeModelBase.Changed = (Action<CubeModelChangedEventArgs>)Delegate.Remove(mVCubeModelBase.Changed, new Action<CubeModelChangedEventArgs>(CubeModel_Changed));
	}

	public override bool CanAddCubeAt(IntVector pos)
	{
		Vector3 vector = MathFunctions.AbsVector(new Vector3(pos.x, pos.y, pos.z));
		if (vector.x > (float)((short)Size.x - 1) || vector.y > (float)((short)Size.y - 1) || vector.z > (float)((short)Size.z - 1))
		{
			return false;
		}
		Bounds bounds = cubeModel.GetBounds();
		Vector3 vector2 = new Vector3((float)pos.x - 0.5f, (float)pos.y - 0.5f, (float)pos.z - 0.5f);
		Vector3 vector3 = new Vector3((float)pos.x + 0.5f, (float)pos.y + 0.5f, (float)pos.z + 0.5f);
		for (int i = 0; i < 3; i++)
		{
			if (vector2[i] < bounds.min[i])
			{
				Vector3 min = bounds.min;
				min[i] = vector2[i];
				bounds.SetMinMax(min, bounds.max);
			}
			if (vector3[i] > bounds.max[i])
			{
				Vector3 max = bounds.max;
				max[i] = vector3[i];
				bounds.SetMinMax(bounds.min, max);
			}
		}
		IntVector min2 = default;
		IntVector max2 = default;
		SharedCollisionFunctions.GetVoxelBounds(ref min2, ref max2, bounds);
		IntVector intVector = max2 - min2 + new IntVector(1, 1, 1);
		if (intVector.x <= (short)Size.x && intVector.y <= (short)Size.y && intVector.z <= (short)Size.z)
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

	private void CubeModel_Changed(CubeModelChangedEventArgs e)
	{
		Center = CalcConstraintBoxCenter(cubeModel);
	}

	private Vector3 CalcConstraintBoxCenter(MVCubeModelBase model)
	{
		Vector3 v = model.GetBounds().center;
		for (int i = 0; i < 3; i++)
		{
			if (SharedCubeFunctions.CubeConstraintVector3[i] % 2f == 0f)
			{
				if (v[i] % 1f != 0f)
				{
					v[i] = Mathf.Ceil(v[i]) - 0.5f;
				}
				continue;
			}
			IntVector min = default;
			IntVector max = default;
			SharedCollisionFunctions.GetVoxelBounds(ref min, ref max, model.GetBounds());
			if ((max - min + new IntVector(1, 1, 1))[i] == (int)SharedCubeFunctions.CubeConstraintVector3[i])
			{
				int num = min[i] + (max[i] - min[i]) / 2;
				v[i] = num;
			}
		}
		Vector3 vector = (SharedCubeFunctions.CubeConstraintVector3 - Vector3.one * 1f) / 2f;
		MathFunctions.ClampVector(ref v, -vector, vector);
		return model.Transform.TransformPoint(v);
	}
}
