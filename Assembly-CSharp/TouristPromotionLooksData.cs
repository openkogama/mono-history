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
	}

	[SerializeField]
	private List<PromotionLooksData> promotionData;

	private int promotionIndex;

	public void RandomizePromotion()
	{
		promotionIndex = UnityEngine.Random.Range(0, promotionData.Count);
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
