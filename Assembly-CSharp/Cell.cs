public struct Cell(Cube cube, byte lightValue)
{
	public Cube cube = cube;

	public byte lightValue = lightValue;

	public Cell Clone()
	{
		return new Cell(Cube.Clone(cube), lightValue);
	}
}
