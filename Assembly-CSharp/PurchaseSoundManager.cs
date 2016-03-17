using System;
using System.Collections.Generic;
using UnityEngine;

public class PurchaseSoundManager : MonoBehaviour
{
	[SerializeField]
	private AudioSource purchaseSound;

	private void Start()
	{
		MVNetworkGame game = MVGameControllerBase.Game;
		game.PurchaseProductResponseHandler = (Action<int, Dictionary<object, object>>)Delegate.Combine(game.PurchaseProductResponseHandler, new Action<int, Dictionary<object, object>>(ProductPurchaseResponseHandler));
	}

	private void ProductPurchaseResponseHandler(int returnCode, Dictionary<object, object> purchaseResponseData)
	{
		if (returnCode == 0)
		{
			purchaseSound.Play();
		}
	}
}
