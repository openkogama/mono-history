using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ClipBox
{
	private delegate bool ClippingHandler(float p, float q);

	private Vector2 clipMin = Vector2.zero;

	private Vector2 clipMax = Vector2.zero;

	public Vector2 ClipMin
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return clipMin;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			clipMin = value;
		}
	}

	public Vector2 ClipMax
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return clipMax;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			clipMax = value;
		}
	}

	public ClipBox()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
	}

	public ClipLine Clip(ClipLine line)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = line.End - line.Start;
		float tMinimum = 0f;
		float tMaximum = 1f;
		ClippingHandler clippingHandler = (float directedProjection, float directedDistance) =>
		{
			if (directedProjection == 0f)
			{
				if (directedDistance < 0f)
				{
					return false;
				}
			}
			else
			{
				float num = directedDistance / directedProjection;
				if (directedProjection < 0f)
				{
					if (num > tMaximum)
					{
						return false;
					}
					if (num > tMinimum)
					{
						tMinimum = num;
					}
				}
				else
				{
					if (num < tMinimum)
					{
						return false;
					}
					if (num < tMaximum)
					{
						tMaximum = num;
					}
				}
			}
			return true;
		};
		if (clippingHandler(0f - val.x, line.Start.x - clipMin.x) && clippingHandler(val.x, clipMax.x - line.Start.x) && clippingHandler(0f - val.y, line.Start.y - clipMin.y) && clippingHandler(val.y, clipMax.y - line.Start.y))
		{
			Vector2 val2 = line.Start;
			Vector2 end = line.End;
			if (tMaximum < 1f)
			{
				end = line.Start + tMaximum * val;
			}
			if (tMinimum > 0f)
			{
				val2 += tMinimum * val;
			}
			return new ClipLine(val2, end);
		}
		return null;
	}

	public IEnumerable<ClipLine> Clip(IEnumerable<ClipLine> clipLines)
	{
		return from clipLine in clipLines
			select Clip(clipLine) into clipLine
			where clipLine != null
			select clipLine;
	}
}
