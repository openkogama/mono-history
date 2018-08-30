using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCurrentGoldAmountTracker : MonoBehaviour
{
	[SerializeField]
	private Text goldAmount;

	private void Start()
	{
		goldAmount.text = MVGameControllerBase.Game.LocalPlayer.GoldAmount.ToString("N0").Replace(",", " ");
		MVLocalPlayer localPlayer = MVGameControllerBase.Game.LocalPlayer;
		localPlayer.OnGoldAmountChange = (Action)Delegate.Combine(localPlayer.OnGoldAmountChange, new Action(RefreshGoldAmount));
	}

	private void RefreshGoldAmount()
	{
		goldAmount.text = MVGameControllerBase.Game.LocalPlayer.GoldAmount.ToString("N0").Replace(",", " ");
	}

	private void OnDestroy()
	{
		if (MVGameControllerBase.Game != null)
		{
			MVLocalPlayer localPlayer = MVGameControllerBase.Game.LocalPlayer;
			localPlayer.OnGoldAmountChange = (Action)Delegate.Remove(localPlayer.OnGoldAmountChange, new Action(RefreshGoldAmount));
		}
	}
}
