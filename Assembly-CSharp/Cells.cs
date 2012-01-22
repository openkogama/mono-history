using MV.WorldObject;

public struct Cells(IntVector iVector, int chunkSize)
{
	private Cell[,,] cells = new Cell[chunkSize, chunkSize, chunkSize];

	private IntVector chunkPosCubes = chunkSize * iVector;

	private IntVector chunkPos = iVector;

	private int chunkSize = chunkSize;

	private IntVector chunkOffset = chunkSize / 2 * IntVector.One;

	public Cell[,,] CellsArray => cells;

	public int ChunkSize => chunkSize;

	public IntVector ChunkPos => chunkPos;

	public Cell this[IntVector key]
	{
		get
		{
			return cells[key.x, key.y, key.z];
		}
		set
		{
			cells[key.x, key.y, key.z] = value;
		}
	}

	public Cell this[int x, int y, int z]
	{
		get
		{
			return cells[x, y, z];
		}
		set
		{
			cells[x, y, z] = value;
		}
	}

	public Cells Clone()
	{
		Cells result = new Cells(chunkPos, chunkSize);
		Cell[,,] array = new Cell[chunkSize, chunkSize, chunkSize];
		for (int i = 0; i < chunkSize; i++)
		{
			for (int j = 0; j < chunkSize; j++)
			{
				for (int k = 0; k < chunkSize; k++)
				{
					array[i, j, k] = cells[i, j, k].Clone();
				}
			}
		}
		result.cells = array;
		return result;
	}

	public bool ContainsCube(IntVector worldPos)
	{
		if (this[GetArrayCoords(worldPos)].cube == null)
		{
			return false;
		}
		return true;
	}

	public void RemoveCube(IntVector worldPos)
	{
		this[GetArrayCoords(worldPos)] = default;
	}

	public void SetCube(IntVector worldPos, Cube cube)
	{
		Cell value = new Cell(cube, 0);
		this[GetArrayCoords(worldPos)] = value;
	}

	public IntVector GetArrayCoords(IntVector worldPos)
	{
		return worldPos - chunkPosCubes + chunkOffset;
	}

	public IntVector GetWorldCoords(IntVector localPos)
	{
		return localPos + chunkPosCubes - chunkOffset;
	}

	public Cell GetCell(IntVector worldPos)
	{
		return this[GetArrayCoords(worldPos)];
	}

	public bool IsWithinArrayCoordsRange(IntVector localPos)
	{
		if (localPos.x >= 0 && localPos.x < chunkSize && localPos.y >= 0 && localPos.y < chunkSize && localPos.z >= 0 && localPos.z < chunkSize)
		{
			return true;
		}
		return false;
	}
}
