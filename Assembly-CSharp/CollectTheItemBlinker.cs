using System.Collections.Generic;
using UnityEngine;

public class CollectTheItemBlinker : BlinkerBase
{
	[SerializeField]
	private Color dropOffCollectedItemColor;

	private void Awake()
	{
		blinkers = new Dictionary<BlinkType, Blinker> { 
		{
			BlinkType.DropOffCollectedItem,
			new Blinker(2f, blinkMaterial, dropOffCollectedItemColor)
		} };
	}

	public void OnBlinkingActivated()
	{
		StartBlinking(BlinkType.DropOffCollectedItem, 2f);
	}
}
