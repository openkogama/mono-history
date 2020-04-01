using UnityEngine.EventSystems;
using UnityEngine.Events;

internal interface IDeathPromotionSelector : IEventSystemHandler
{
	bool ReadyForAd { get; }

	void TryShowPromotion(UnityAction<bool, bool> onPromotionPopped);
}
