using System.Collections.Generic;
using UnityEngine;

public class AdvancedGhostBlinker : BlinkerBase
{
	public Color blinkHealingColor = new Color(251f, 0f, 0f);

	public Color blinkDamageColor = new Color(0f, 250f, 125f);

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
			}
		};
		Visible = true;
	}

	public void Init(MeshFilter[] meshFilters)
	{
		base.meshFilters = meshFilters;
	}
}
