using System.Collections.Generic;
using UnityEngine;

public class AdvancedGhostBlinker : BlinkerBase
{
	public Color blinkDamageColor = new Color(251f, 0f, 0f);

	public AdvancedGhostBlinker()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
	}

	private void Awake()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
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
