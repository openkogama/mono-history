using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TouristPromotionAndroid : MonoBehaviour
{
	[SerializeField]
	private Button promotionWallButton;

	[SerializeField]
	private RawImage buttonRawImage;

	[SerializeField]
	private CanvasGroup canvasGroup;

	public void SetPromotionTexture(Texture tex)
	{
		buttonRawImage.texture = tex;
	}

	public void SkipCallback()
	{
		StartCoroutine(FadeOutAndPopPromotion());
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

	public void SignupCallback()
	{
		MVGameControllerBase.ApplicationQuit(new TouristSignupQuit());
	}

	public void LoginCallback()
	{
		MVGameControllerBase.ApplicationQuit(new TouristLoginQuit());
	}
}
