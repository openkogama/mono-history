using UnityEngine;
using UnityEngine.Events;

public class ItemPurchaseConfirmationPopup : MonoBehaviour
{
	private UnityAction<bool> resultCallback;

	public void Initialize(UnityAction<bool> resultCallback)
	{
		this.resultCallback = resultCallback;
	}

	public void AcceptPurchase()
	{
		resultCallback(arg0: true);
	}

	public void DeclinePurchase()
	{
		resultCallback(arg0: false);
	}
}
