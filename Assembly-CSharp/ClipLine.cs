using UnityEngine;

public class ClipLine
{
	private Vector2 start;

	private Vector2 end;

	public Vector2 Start
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return start;
		}
	}

	public Vector2 End
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return end;
		}
	}

	public ClipLine(Vector2 start, Vector2 end)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		this.start = start;
		this.end = end;
	}
}
