namespace UnityEngine.EventSystems;

public interface IPurchaseSoundManager : IEventSystemHandler
{
	void SurpressSoundOnce();

	void PlayPurchaseSound();
}
