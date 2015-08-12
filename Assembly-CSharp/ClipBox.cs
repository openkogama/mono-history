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
			return clipMin;
		}
		set
		{
			clipMin = value;
		}
	}

	public Vector2 ClipMax
	{
		get
		{
			return clipMax;
		}
		set
		{
			clipMax = value;
		}
	}

	public ClipLine Clip(ClipLine line)
	{
		Vector2 vector = line.End - line.Start;
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
		if (clippingHandler(0f - vector.x, line.Start.x - clipMin.x) && clippingHandler(vector.x, clipMax.x - line.Start.x) && clippingHandler(0f - vector.y, line.Start.y - clipMin.y) && clippingHandler(vector.y, clipMax.y - line.Start.y))
		{
			Vector2 start = line.Start;
			Vector2 end = line.End;
			if (tMaximum < 1f)
			{
				end = line.Start + tMaximum * vector;
			}
			if (tMinimum > 0f)
			{
				start += tMinimum * vector;
			}
			return new ClipLine(start, end);
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
