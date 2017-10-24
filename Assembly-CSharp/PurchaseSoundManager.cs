using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PurchaseSoundManager : MonoBehaviour, IPurchaseSoundManager, IEventSystemHandler
{
	[SerializeField]
	private AudioSource purchaseSound;

	private bool surpressSound;

	private void Start()
	{
		MVNetworkGame game = MVGameControllerBase.Game;
		game.PurchaseProductResponseHandler = (Action<int, Dictionary<object, object>>)Delegate.Combine(game.PurchaseProductResponseHandler, new Action<int, Dictionary<object, object>>(ProductPurchaseResponseHandler));
	}

	private void ProductPurchaseResponseHandler(int returnCode, Dictionary<object, object> purchaseResponseData)
	{
		if (surpressSound)
		{
			surpressSound = false;
		}
		else if (returnCode == 0 && !surpressSound)
		{
			purchaseSound.Play();
		}
	}

	public void SurpressSoundOnce()
	{
		surpressSound = true;
	}

	public void PlayPurchaseSound()
	{
		purchaseSound.Play();
	}
}
