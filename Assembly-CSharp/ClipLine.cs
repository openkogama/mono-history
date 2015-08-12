using UnityEngine;

public class ClipLine
{
	private Vector2 start;

	private Vector2 end;

	public Vector2 Start => start;

	public Vector2 End => end;

	public ClipLine(Vector2 start, Vector2 end)
	{
		this.start = start;
		this.end = end;
	}
}
