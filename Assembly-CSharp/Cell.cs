public struct Cell(Cube cube)
{
	public Cube cube = cube;

	public byte lightValue = (byte)(((cube.UnIndentedSides & 0x3F) != 63) ? 255u : 0u);

	public Cell Clone()
	{
		return new Cell(Cube.Clone(cube));
	}
}
