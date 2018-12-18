using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TouristPromotion : MonoBehaviour
{
	[SerializeField]
	private RawImage buttonRawImage;

	[SerializeField]
	private CanvasGroup canvasGroup;

	[SerializeField]
	private GameObject pleaseWaitOverlay;

	public void SetPromotionTexture(Texture tex)
	{
		buttonRawImage.texture = tex;
		pleaseWaitOverlay.SetActive(value: false);
	}

	public virtual void SkipCallback()
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
}
