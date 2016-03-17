using MV.Common;
using UnityEngine.Events;

namespace UnityEngine.EventSystems;

public interface IModalPopupCreator : IEventSystemHandler
{
	void Create(MVPurchaseReturnCode returnCode, int priceGold, int priceSilver);

	void CreateErrorNotificationPopup(string error);

	PleaseWaitPopup Create();

	ConfirmationPopup Create(string text, UnityAction<bool, ConfirmationPopup> resultCallback, string header = "");

	NotificationPopup Create(string text, string header = "");
}
