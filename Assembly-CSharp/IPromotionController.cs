using UnityEngine.Events;

public interface IPromotionController
{
	bool IsPromotionAvailable { get; }

	void Initialize();

	void ShowPromotion(UnityAction<bool> onPop);
}
