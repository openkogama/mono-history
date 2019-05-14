using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TierPurchaseNotEnoughGoldErrorPopup : MonoBehaviour
{
	[SerializeField]
	private Text tierText;

	public void Initialize(GamePassTier tierToPurchase)
	{
		Text text = tierText;
		int num = (int)tierToPurchase;
		text.text = num.ToString();
	}

	public void GetGold()
	{
		BrowserCommGotoRequests.GotoPurchaseGold(newTab: false, modalPopup: true);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
	}
}
