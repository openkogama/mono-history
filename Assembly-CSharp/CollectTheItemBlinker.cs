using System.Collections.Generic;
using UnityEngine;

public class CollectTheItemBlinker : BlinkerBase
{
	[SerializeField]
	private Color dropOffCollectedItemColor;

	[SerializeField]
	private Color DespawnColor;

	private void Awake()
	{
		blinkers = new Dictionary<BlinkType, Blinker>
		{
			{
				BlinkType.DropOffCollectedItem,
				new Blinker(2f, blinkMaterial, dropOffCollectedItemColor)
			},
			{
				BlinkType.AboutToExpire,
				new Blinker(2f, blinkMaterial, DespawnColor)
			}
		};
	}

	public void OnBlinkingActivated(bool shouldBlink, BlinkType type)
	{
		if (shouldBlink)
		{
			StartBlinking(type, 2f);
		}
	}

	public void DeactivateBlinking()
	{
		foreach (BlinkType key in blinkers.Keys)
		{
			StopBlinking(key);
		}
	}
}
