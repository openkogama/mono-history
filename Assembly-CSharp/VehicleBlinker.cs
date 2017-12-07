using System.Collections.Generic;
using UnityEngine;

public class VehicleBlinker : BlinkerBase
{
	[SerializeField]
	private Color blinkDamageColor = new Color(251f, 0f, 0f);

	[SerializeField]
	private Color blinkHealingColor = new Color(0f, 250f, 125f);

	[SerializeField]
	private Color blinkAboutToExpireColor = new Color(29f, 108f, 219f);

	private void Awake()
	{
		blinkers = new Dictionary<BlinkType, Blinker>
		{
			{
				BlinkType.Damage,
				new Blinker(4f, blinkMaterial, blinkDamageColor)
			},
			{
				BlinkType.Healing,
				new Blinker(4f, blinkMaterial, blinkHealingColor)
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
