using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class DeathPromotionController : MonoBehaviour, IDeathPromotionSelector, IEventSystemHandler
{
	[SerializeField]
	private TouristAdController touristAdController;

	[SerializeField]
	private RegisteredPromotionController registeredAdController;

	private IPromotionController adController;

	public bool ReadyForAd
	{
		get
		{
			if (adController == null)
			{
				return false;
			}
			return adController.ReadyForAd;
		}
	}

	public void Initialize()
	{
		if (MVGameControllerBase.IsTouristSession)
		{
			adController = touristAdController;
		}
		else
		{
			adController = registeredAdController;
		}
		if (adController != null)
		{
			adController.Initialize();
		}
	}

	public void TryShowPromotion(UnityAction<bool, bool> onPromotionPopped)
	{
		if (adController != null && adController.IsPromotionAvailable)
		{
			adController.ShowPromotion(onPromotionPopped);
		}
		else
		{
			onPromotionPopped?.Invoke(arg0: false, arg1: false);
		}
	}
}
