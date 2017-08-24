using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FirstTimeEventPopup : MonoBehaviour
{
	[SerializeField]
	private Button skipButton;

	[SerializeField]
	private FirstTimeFadeHandler fader;

	public void FadeIn()
	{
		fader.StartFadeIn();
	}

	public void StartFade(Action<GameObject> finishedAction)
	{
		fader.StartFadeOut(finishedAction, gameObject);
	}

	public void StartFadeWithSelfPop()
	{
		fader.StartFadeOut(PopSelf, gameObject);
	}

	private void PopSelf(GameObject popupGameObject)
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			if (x.PopToStackElement(popupGameObject))
			{
				x.Pop();
			}
		});
	}

	public void SetSkippable(bool skipAllowed)
	{
		if (skipButton != null)
		{
			skipButton.gameObject.SetActive(skipAllowed);
		}
	}
}
