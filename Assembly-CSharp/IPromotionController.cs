using UnityEngine.Events;

public interface IPromotionController
{
	bool ReadyForAd { get; }

	bool IsPromotionAvailable { get; }

	void Initialize();

	void ShowPromotion(UnityAction<bool, bool> onPop);
}
