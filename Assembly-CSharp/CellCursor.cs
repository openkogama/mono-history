using System.Collections.Generic;
using MV.WorldObject;
using UnityEngine;

public class CellCursor : ICursor
{
	private List<CellCursorCubeLineMesh> cursorCubes = new List<CellCursorCubeLineMesh>();

	public bool Active
	{
		set
		{
			foreach (CellCursorCubeLineMesh cursorCube in cursorCubes)
			{
				cursorCube.GameObject.SetActive(value);
			}
		}
	}

	public CellCursor(int cursorCubeCount, float diagonalWidth, string material, float fadeOutTime)
	{
		for (int i = 0; i < cursorCubeCount; i++)
		{
			cursorCubes.Add(new CellCursorCubeLineMesh(diagonalWidth, material, fadeOutTime));
		}
	}

	public void Remove()
	{
		foreach (CellCursorCubeLineMesh cursorCube in cursorCubes)
		{
			cursorCube.Destroy();
		}
		cursorCubes.Clear();
	}

	public CellCursorCubeLineMesh GetCellCursor(IntVector iLocalPos)
	{
		foreach (CellCursorCubeLineMesh cursorCube in cursorCubes)
		{
			if (cursorCube.LocalPos == iLocalPos)
			{
				return cursorCube;
			}
		}
		CellCursorCubeLineMesh cellCursorCubeLineMesh = cursorCubes[0];
		for (int i = 1; i < cursorCubes.Count; i++)
		{
			if (cursorCubes[i].PrevCursorSetTime < cellCursorCubeLineMesh.PrevCursorSetTime)
			{
				cellCursorCubeLineMesh = cursorCubes[i];
			}
		}
		return cellCursorCubeLineMesh;
	}

	public void SetCursor(CubePickingInfo info, GameObject cubeGameObject)
	{
		IntVector cubeCursorPos = GetCubeCursorPos(info);
		CellCursorCubeLineMesh cellCursor = GetCellCursor(cubeCursorPos);
		cellCursor.SetCursorCube(cubeCursorPos, cubeGameObject);
	}

	public void SetCursor(IntVector position, GameObject cubeGameObject)
	{
		CellCursorCubeLineMesh cellCursor = GetCellCursor(position);
		cellCursor.SetCursorCube(position, cubeGameObject);
	}

	public void UpdateCursor()
	{
		foreach (CellCursorCubeLineMesh cursorCube in cursorCubes)
		{
			cursorCube.Update();
		}
	}

	private IntVector GetCubeCursorPos(CubePickingInfo info)
	{
		if (!Cube.IsFaceBoxSideAligened(info.cube, info.pickedFace))
		{
			return info.iLocalPos;
		}
		IntVector intVector = new IntVector(0, 0, 0);
		switch (info.pickedFace)
		{
		case Face.Front:
			intVector.z = -1;
			break;
		case Face.Back:
			intVector.z = 1;
			break;
		case Face.Left:
			intVector.x = -1;
			break;
		case Face.Right:
			intVector.x = 1;
			break;
		case Face.Top:
			intVector.y = 1;
			break;
		case Face.Bottom:
			intVector.y = -1;
			break;
		}
		return info.iLocalPos + intVector;
	}
}
