using UnityEngine;
using UnityEngine.UI;

public class GamePassesElitePercentTextFormatter : MonoBehaviour
{
	[SerializeField]
	private Text textElement;

	private void Start()
	{
		textElement.text = string.Format(textElement.text, (1f - SubscriberRewardDataManager.VatValues.regularUserVat) * 100f, (1f - SubscriberRewardDataManager.VatValues.subscribedUserVat) * 100f);
	}
}
