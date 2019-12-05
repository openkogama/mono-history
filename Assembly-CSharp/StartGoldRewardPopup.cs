using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StartGoldRewardPopup : MonoBehaviour
{
	[SerializeField]
	private Text goldRewardAmountText;

	private void Start()
	{
		goldRewardAmountText.text = 2.ToString();
	}

	public void StartGoldRewardCountdown()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
	}
}
