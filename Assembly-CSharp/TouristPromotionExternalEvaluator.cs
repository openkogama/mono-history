using System.Collections.Generic;
using UnityEngine;

public class TouristPromotionExternalEvaluator : MonoBehaviour
{
	private class TouristPromotionExternalDef
	{
		public TouristPromotion Promotion { get; private set; }

		public int FrequencyPercent { get; private set; }

		public TouristPromotionExternalDef(TouristPromotion promotion, int frequencyPercent)
		{
			Promotion = promotion;
			FrequencyPercent = frequencyPercent;
		}
	}

	[SerializeField]
	private TouristPromotion creyGamesPrefab;

	private List<TouristPromotionExternalDef> availablePromotions = new List<TouristPromotionExternalDef>();

	public void Start()
	{
		availablePromotions.Add(new TouristPromotionExternalDef(creyGamesPrefab, MVGameControllerBase.Game.CreySettings.TouristPromotionCreyFrequencyPercent));
	}

	public bool TryGetExternalPromotion(out TouristPromotion externalPromotion)
	{
		externalPromotion = null;
		if (MVGameControllerBase.GameSessionData.embedded)
		{
			return false;
		}
		return externalPromotion != null;
	}
}
