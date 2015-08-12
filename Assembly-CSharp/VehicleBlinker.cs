using System.Collections.Generic;
using UnityEngine;

public class VehicleBlinker : BlinkerBase
{
	public Color blinkDamageColor = new Color(251f, 0f, 0f);

	public Color blinkAboutToExpireColor = new Color(29f, 108f, 219f);

	private void Awake()
	{
		blinkers = new Dictionary<BlinkType, Blinker>
		{
			{
				BlinkType.Damage,
				new Blinker(4f, blinkMaterial, blinkDamageColor)
			},
			{
				BlinkType.AboutToExpire,
				new Blinker(4f, blinkMaterial, blinkAboutToExpireColor)
			}
		};
	}

	public void Init(MeshFilter[] meshFilters)
	{
		base.meshFilters = meshFilters;
	}
}
