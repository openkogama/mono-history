using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RegisteredPromotionPopup : MonoBehaviour
{
	[SerializeField]
	private Text promotionHeader;

	[SerializeField]
	private RectTransform promotionImageParent;

	[SerializeField]
	private Text redirectButtonURLText;

	[SerializeField]
	private TouristPromotionLooksData looksData;

	[SerializeField]
	private GameObject goToKogamaPopupPrefab;

	[SerializeField]
	private CanvasGroup canvasGroup;

	[SerializeField]
	private Button continueButton;

	private bool waitingForAd;

	private float startTime;

	private float timeoutDelay = 20f;

	public void KogamaRedirect()
	{
		if (MVGameControllerBase.GameSessionData.GetIsRedirectAllowed())
		{
			BrowserCommGotoRequests.GotoMainpage();
		}
		else
		{
			ShowGoToKogamaPopup();
		}
	}

	protected void Start()
	{
		looksData.RandomizePromotion();
		promotionHeader.text = looksData.GetPromotionText();
		Image promotionImage = looksData.GetPromotionImage();
		promotionImage.transform.SetParent(promotionImageParent, worldPositionStays: false);
		MVNetworkGame game = MVGameControllerBase.Game;
		game.OnWinningConditionFulfilled = (Action<IWinningCondition>)Delegate.Combine(game.OnWinningConditionFulfilled, new Action<IWinningCondition>(OnWinningConditionFulfilled));
		Uri uri = new Uri(MVGameControllerBase.Game.KogamaMainpageURL);
		redirectButtonURLText.text = uri.Host.Replace("www.", string.Empty).ToUpper();
	}

	protected void OnDestroy()
	{
		if (MVGameControllerBase.IsAlive)
		{
			MVNetworkGame game = MVGameControllerBase.Game;
			game.OnWinningConditionFulfilled = (Action<IWinningCondition>)Delegate.Remove(game.OnWinningConditionFulfilled, new Action<IWinningCondition>(OnWinningConditionFulfilled));
		}
	}

	public void OnContinueClicked()
	{
		StartCoroutine(FadeOutAndPopPromotion());
	}

	public void OnViewAdClicked()
	{
		Debug.Log("Showing Ad");
		waitingForAd = true;
		continueButton.interactable = false;
		startTime = Time.time;
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IRegisterPromotionAdController x, BaseEventData y) =>
		{
			x.ShowRegisteredPromotionAd();
		});
	}

	private IEnumerator FadeOutAndPopPromotion()
	{
		yield return StartCoroutine(pTween.To(0.5f, 1f, 0f, (float t) =>
		{
			canvasGroup.alpha = t;
			if (t == 0f)
			{
				gameObject.SetActive(value: false);
				ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
				{
					x.Pop();
				});
			}
		}));
	}

	private void OnWinningConditionFulfilled(IWinningCondition winningCondition)
	{
		MVGameControllerDesktop.LockCursorManager.CursorLock = false;
	}

	private void ShowGoToKogamaPopup()
	{
		GameObject popUp = UnityEngine.Object.Instantiate(goToKogamaPopupPrefab);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(popUp, UIPushOption.Blocking, null, UIGroupFlags.Popup);
		});
	}

	private void Update()
	{
		if (waitingForAd && Time.time - startTime >= timeoutDelay)
		{
			Debug.Log("Show Ad timeout, try again");
			continueButton.interactable = true;
			waitingForAd = false;
		}
	}
}
