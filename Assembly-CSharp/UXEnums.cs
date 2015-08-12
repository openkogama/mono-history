using System.Collections.Generic;
using UnityEngine;

public class UXEnums
{
	private static Dictionary<UXHorizontal, TextAlignment> toTextAlignment = new Dictionary<UXHorizontal, TextAlignment>
	{
		{
			UXHorizontal.Left,
			TextAlignment.Left
		},
		{
			UXHorizontal.Center,
			TextAlignment.Center
		},
		{
			UXHorizontal.Right,
			TextAlignment.Right
		}
	};

	private static Dictionary<Tuple<UXHorizontal, UXVertical>, TextAnchor> toTextAnchor = (toTextAnchor = new Dictionary<Tuple<UXHorizontal, UXVertical>, TextAnchor>
	{
		{
			new Tuple<UXHorizontal, UXVertical>(UXHorizontal.Left, UXVertical.Top),
			TextAnchor.UpperLeft
		},
		{
			new Tuple<UXHorizontal, UXVertical>(UXHorizontal.Center, UXVertical.Top),
			TextAnchor.UpperCenter
		},
		{
			new Tuple<UXHorizontal, UXVertical>(UXHorizontal.Right, UXVertical.Top),
			TextAnchor.UpperRight
		},
		{
			new Tuple<UXHorizontal, UXVertical>(UXHorizontal.Left, UXVertical.Middle),
			TextAnchor.MiddleLeft
		},
		{
			new Tuple<UXHorizontal, UXVertical>(UXHorizontal.Center, UXVertical.Middle),
			TextAnchor.MiddleCenter
		},
		{
			new Tuple<UXHorizontal, UXVertical>(UXHorizontal.Right, UXVertical.Middle),
			TextAnchor.MiddleRight
		},
		{
			new Tuple<UXHorizontal, UXVertical>(UXHorizontal.Left, UXVertical.Bottom),
			TextAnchor.LowerLeft
		},
		{
			new Tuple<UXHorizontal, UXVertical>(UXHorizontal.Center, UXVertical.Bottom),
			TextAnchor.LowerCenter
		},
		{
			new Tuple<UXHorizontal, UXVertical>(UXHorizontal.Right, UXVertical.Bottom),
			TextAnchor.LowerRight
		}
	});

	private static Dictionary<UXHorizontal, float> horizontalRatio = new Dictionary<UXHorizontal, float>
	{
		{
			UXHorizontal.Left,
			0f
		},
		{
			UXHorizontal.Center,
			0.5f
		},
		{
			UXHorizontal.Right,
			1f
		}
	};

	private static Dictionary<UXVertical, float> verticalRatio = new Dictionary<UXVertical, float>
	{
		{
			UXVertical.Top,
			1f
		},
		{
			UXVertical.Middle,
			0.5f
		},
		{
			UXVertical.Bottom,
			0f
		}
	};

	public static TextAlignment ToTextAlignment(UXHorizontal horizontal)
	{
		return toTextAlignment[horizontal];
	}

	public static TextAnchor ToTextAnchor(UXHorizontal horizontal, UXVertical vertical)
	{
		return toTextAnchor[new Tuple<UXHorizontal, UXVertical>(horizontal, vertical)];
	}

	public static float GetRatio(UXHorizontal horizontal)
	{
		return horizontalRatio[horizontal];
	}

	public static float GetRatio(UXVertical vertical)
	{
		return verticalRatio[vertical];
	}
}
