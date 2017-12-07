using System.Collections.Generic;
using UnityEngine;

public class AdvancedGhostBlinker : BlinkerBase
{
	public Color blinkDamageColor = new Color(251f, 0f, 0f);

	private void Awake()
	{
		blinkers = new Dictionary<BlinkType, Blinker> { 
		{
			BlinkType.Damage,
			new Blinker(4f, blinkMaterial, blinkDamageColor)
		} };
		Visible = true;
	}

	public void Init(MeshFilter[] meshFilters)
	{
		base.meshFilters = meshFilters;
	}
}
