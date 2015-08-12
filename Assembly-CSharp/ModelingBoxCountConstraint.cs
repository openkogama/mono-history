using MV.WorldObject;
using UnityEngine;

public class ModelingBoxCountConstraint : ModelingBoxConstraint
{
	private MVCubeModelBase cubeModel;

	private int minCubesCount;

	public ModelingBoxCountConstraint(MVCubeModelBase cubeModel, IntVector minCorner, IntVector maxCorner, int minCubeCount)
		: base(minCorner, maxCorner)
	{
		this.cubeModel = cubeModel;
		minCubesCount = minCubeCount;
	}

	public override bool CanAddCubeAt(IntVector pos)
	{
		return base.CanAddCubeAt(pos);
	}

	public override bool CanRemoveCubeAt(IntVector pos)
	{
		bool flag = minCubesCount < cubeModel.CubeCount;
		if (!flag)
		{
			Debug.Log("Constraint violited! min: " + minCubesCount + " current " + cubeModel.CubeCount);
		}
		return flag;
	}

	public override bool CanEditCubeAt(IntVector pos)
	{
		return true;
	}

	private void CubeModel_Changed(object sender, CubeModelChangedEventArgs e)
	{
		switch (e.Action)
		{
		case CubeAction.Added:
			break;
		case CubeAction.Deleted:
			break;
		case CubeAction.CornersChanged:
			break;
		}
	}
}
