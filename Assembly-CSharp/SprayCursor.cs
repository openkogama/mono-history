using MV.WorldObject;
using UnityEngine;

public class SprayCursor
{
	private CellCursor sprayCursor;

	private float addCubeTime;

	private float addCubeLaserOnTime = 0.2f;

	public SprayCursor()
	{
		sprayCursor = new CellCursor(1, 0.03f, "Materials/CellCursorMaterial", 1f);
	}

	public void UpdateCursor(CubePickingInfo selectedCube, MVCubeModelBase targetCubeModel, bool addCube)
	{
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		if (addCube)
		{
			MVGameController.Instance.WOCM.AvatarLocal.LaserPointer.ActivateLaserForDuration(addCubeLaserOnTime);
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
			MVGameController.Instance.WOCM.AvatarLocal.LaserPointer.UpdatePosition(selectedCube.point);
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
