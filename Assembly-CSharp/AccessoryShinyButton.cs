using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AccessoryShinyButton : MonoBehaviour
{
	[Serializable]
	private class ScrollingUVSprite
	{
		public Image image;

		public AnimationCurve alphaCurve;

		public Vector2 direction = Vector2.zero;

		public AnimationCurve movementX;
	}

	[SerializeField]
	private List<ScrollingUVSprite> scrollingSpriteElements;

	private void Update()
	{
		for (int i = 0; i < scrollingSpriteElements.Count; i++)
		{
			Color color = scrollingSpriteElements[i].image.color;
			color.a = scrollingSpriteElements[i].alphaCurve.Evaluate(Time.time);
			scrollingSpriteElements[i].image.color = color;
		}
	}
}
