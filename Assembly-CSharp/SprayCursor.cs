using MV.WorldObject;
using UnityEngine;

public class SprayCursor
{
	private CellCursor sprayCursor;

	private float addCubeTime;

	private float addCubeLaserOnTime = 0.2f;

	public SprayCursor(Vector3[] cubeCorners)
	{
		sprayCursor = new CellCursor(1, 0.03f, PrefabPool.Instance.CellCursorMaterial, 1f, cubeCorners);
	}

	public void UpdateCursor(CubePickingInfo selectedCube, MVCubeModelBase targetCubeModel, bool addCube)
	{
		if (addCube)
		{
			MVGameControllerBase.GameEventManager.AvatarCommandsBuildMode.LaserCommands.ActivateLaserForDuration(addCubeLaserOnTime);
			addCubeTime = Time.time;
		}
		if (selectedCube != null)
		{
			sprayCursor.Active = true;
			IntVector position = selectedCube.iLocalPos + FaceToOffset(selectedCube.pickedFace);
			sprayCursor.SetCursor(position, targetCubeModel.GameObject);
			if (Time.time - addCubeTime < addCubeLaserOnTime)
			{
			}
			MVGameControllerBase.GameEventManager.AvatarCommandsBuildMode.LaserCommands.UpdatePosition(selectedCube.point);
		}
		else
		{
			sprayCursor.Active = false;
		}
	}

	public void Remove()
	{
		sprayCursor.Remove();
	}

	private IntVector FaceToOffset(Face face)
	{
		return face switch
		{
			Face.Back => new IntVector(0, 0, 1), 
			Face.Front => new IntVector(0, 0, -1), 
			Face.Bottom => new IntVector(0, -1, 0), 
			Face.Top => new IntVector(0, 1, 0), 
			Face.Left => new IntVector(-1, 0, 0), 
			Face.Right => new IntVector(1, 0, 0), 
			_ => new IntVector(0, 0, 0), 
		};
	}
}
