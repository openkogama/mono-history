using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TouristPromotion : MonoBehaviour
{
	[SerializeField]
	private RectTransform promotionImageParent;

	[SerializeField]
	private CanvasGroup canvasGroup;

	[SerializeField]
	private Text promotionHeader;

	[SerializeField]
	private TouristPromotionLooksData looksData;

	[SerializeField]
	private GameObject adIcon;

	protected bool promotionShowsAd;

	protected virtual void Start()
	{
		StatHatWrapper.Count("TouristPromotion.Kogama", 1);
		looksData.RandomizePromotion();
		promotionHeader.text = looksData.GetPromotionText();
		Image promotionImage = looksData.GetPromotionImage();
		promotionImage.transform.SetParent(promotionImageParent, worldPositionStays: false);
	}

	public void Initialize(bool withAd)
	{
		promotionShowsAd = withAd;
		adIcon.SetActive(promotionShowsAd);
	}

	public void OnRegisterClicked()
	{
		BrowserCommGotoRequests.GotoSignup();
	}

	public void OnLoginClicked()
	{
		BrowserCommGotoRequests.GotoLogin();
	}

	public virtual void OnContinueClicked()
	{
		if (promotionShowsAd)
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (ITouristAdController x, BaseEventData y) =>
			{
				x.ShowAd();
			});
		}
		else
		{
			StartCoroutine(FadeOutAndPopPromotion());
		}
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
}
