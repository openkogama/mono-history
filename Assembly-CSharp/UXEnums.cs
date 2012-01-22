using System.Collections.Generic;
using UnityEngine;

public class UXEnums
{
	private static Dictionary<UXHorizontal, TextAlignment> toTextAlignment = new Dictionary<UXHorizontal, TextAlignment>
	{
		{
			UXHorizontal.Left,
			(TextAlignment)0
		},
		{
			UXHorizontal.Center,
			(TextAlignment)1
		},
		{
			UXHorizontal.Right,
			(TextAlignment)2
		}
	};

	private static Dictionary<Tuple<UXHorizontal, UXVertical>, TextAnchor> toTextAnchor = (toTextAnchor = new Dictionary<Tuple<UXHorizontal, UXVertical>, TextAnchor>
	{
		{
			new Tuple<UXHorizontal, UXVertical>(UXHorizontal.Left, UXVertical.Top),
			(TextAnchor)0
		},
		{
			new Tuple<UXHorizontal, UXVertical>(UXHorizontal.Center, UXVertical.Top),
			(TextAnchor)1
		},
		{
			new Tuple<UXHorizontal, UXVertical>(UXHorizontal.Right, UXVertical.Top),
			(TextAnchor)2
		},
		{
			new Tuple<UXHorizontal, UXVertical>(UXHorizontal.Left, UXVertical.Middle),
			(TextAnchor)3
		},
		{
			new Tuple<UXHorizontal, UXVertical>(UXHorizontal.Center, UXVertical.Middle),
			(TextAnchor)4
		},
		{
			new Tuple<UXHorizontal, UXVertical>(UXHorizontal.Right, UXVertical.Middle),
			(TextAnchor)5
		},
		{
			new Tuple<UXHorizontal, UXVertical>(UXHorizontal.Left, UXVertical.Bottom),
			(TextAnchor)6
		},
		{
			new Tuple<UXHorizontal, UXVertical>(UXHorizontal.Center, UXVertical.Bottom),
			(TextAnchor)7
		},
		{
			new Tuple<UXHorizontal, UXVertical>(UXHorizontal.Right, UXVertical.Bottom),
			(TextAnchor)8
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
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return toTextAlignment[horizontal];
	}

	public static TextAnchor ToTextAnchor(UXHorizontal horizontal, UXVertical vertical)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
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
