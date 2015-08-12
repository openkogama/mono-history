using MV.WorldObject;
using UnityEngine;

public class CubePickingInfo
{
	public Cube cube;

	public Face pickedFace;

	public Edge pickedEdge;

	public bool pickedEdgeIndex0;

	public bool pickedEdgeIndex1;

	public Vector3 normal;

	public Vector3 point;

	public IntVector iLocalPos;

	public CubePickingInfo()
	{
	}

	public CubePickingInfo(CubePickingInfo cubePickingInfo)
	{
		cube = cubePickingInfo.cube;
		pickedFace = cubePickingInfo.pickedFace;
		pickedEdge = cubePickingInfo.pickedEdge;
		normal = cubePickingInfo.normal;
		point = cubePickingInfo.point;
		iLocalPos = cubePickingInfo.iLocalPos;
		pickedEdgeIndex0 = cubePickingInfo.pickedEdgeIndex0;
		pickedEdgeIndex1 = cubePickingInfo.pickedEdgeIndex1;
	}
}
