using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoostImageController : MonoBehaviour
{
	[Serializable]
	private struct BoosterImageDef
	{
		public BoostType type;

		public Image image;
	}

	[SerializeField]
	private List<BoosterImageDef> boosterImages;

	public Image GetBoostVisualization(BoostType type)
	{
		for (int i = 0; i < boosterImages.Count; i++)
		{
			if (boosterImages[i].type == type)
			{
				return boosterImages[i].image;
			}
		}
		Debug.LogWarning("Boost type: " + type);
		Debug.LogError("No image found for boost type.");
		return null;
	}
}
