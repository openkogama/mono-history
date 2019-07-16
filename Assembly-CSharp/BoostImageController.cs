using System;
using System.Collections.Generic;
using UnityEngine;

public class BoostImageController : MonoBehaviour
{
	[Serializable]
	private struct BoosterImageDef
	{
		public BoostType type;

		public BoostRadialUpdate radialUpdater;
	}

	[SerializeField]
	private List<BoosterImageDef> boosterImages;

	public BoostRadialUpdate GetBoostVisualization(BoostType type)
	{
		for (int i = 0; i < boosterImages.Count; i++)
		{
			if (boosterImages[i].type == type)
			{
				return boosterImages[i].radialUpdater;
			}
		}
		Debug.LogWarning("Boost type: " + type);
		Debug.LogError("No image found for boost type.");
		return null;
	}
}
