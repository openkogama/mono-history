using UnityEngine;
using UnityEngine.UI;

public class PlayerCurrentGoldAmountTracker : MonoBehaviour
{
	[SerializeField]
	private Text goldAmount;

	private void Start()
	{
		goldAmount.text = MVGameControllerBase.Game.LocalPlayer.GoldAmount.ToString("N0");
	}

	public void RefreshGoldAmount()
	{
		goldAmount.text = MVGameControllerBase.Game.LocalPlayer.GoldAmount.ToString("N0");
	}
}
