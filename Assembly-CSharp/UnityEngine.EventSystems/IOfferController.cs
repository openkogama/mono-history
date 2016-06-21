using UnityEngine.Events;

namespace UnityEngine.EventSystems;

public interface IOfferController : IEventSystemHandler
{
	void RequestShowOffer(UnityAction OnOfferClosed);
}
