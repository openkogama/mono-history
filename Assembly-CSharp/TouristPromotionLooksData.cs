using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TouristPromotionLooksData : MonoBehaviour
{
	[Serializable]
	private struct PromotionLooksData
	{
		public Image PromotionImage;

		public string PromotionText;

		public bool ValidOnKogamaPortal;

		public bool ValidOnAnonymousExternalPortal;
	}

	[SerializeField]
	private List<PromotionLooksData> promotionData;

	private int promotionIndex;

	private bool initialized;

	private List<int> portalsIndices;

	private List<int> embeddedIndices;

	public void RandomizePromotion()
	{
		promotionIndex = UnityEngine.Random.Range(0, promotionData.Count);
	}

	public void RandomizePromotion(bool embedded)
	{
		if (!initialized)
		{
			portalsIndices = new List<int>();
			embeddedIndices = new List<int>();
			for (int i = 0; i < promotionData.Count; i++)
			{
				if (promotionData[i].ValidOnKogamaPortal)
				{
					portalsIndices.Add(i);
				}
				if (promotionData[i].ValidOnAnonymousExternalPortal)
				{
					embeddedIndices.Add(i);
				}
			}
			initialized = true;
		}
		if (embedded)
		{
			promotionIndex = embeddedIndices[UnityEngine.Random.Range(0, embeddedIndices.Count)];
		}
		else
		{
			promotionIndex = portalsIndices[UnityEngine.Random.Range(0, portalsIndices.Count)];
		}
	}

	public Image GetPromotionImage()
	{
		return UnityEngine.Object.Instantiate(promotionData[promotionIndex].PromotionImage);
	}

	public string GetPromotionText()
	{
		return TM._(promotionData[promotionIndex].PromotionText);
	}
}
