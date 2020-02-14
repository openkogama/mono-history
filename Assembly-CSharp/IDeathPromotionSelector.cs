using UnityEngine.EventSystems;
using UnityEngine.Events;

internal interface IDeathPromotionSelector : IEventSystemHandler
{
	void TryShowPromotion(UnityAction<bool> onPromotionPopped);
}
