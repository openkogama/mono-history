using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ResetAvatarHandler : MonoBehaviour
{
	[SerializeField]
	private AvatarScreenShooter screenShooter;

	[SerializeField]
	private PleaseWaitPopup pleaseWaitPopupPrefab;

	[SerializeField]
	private GameObject invisibleBlocker;

	[SerializeField]
	private RawImage fromImage;

	[SerializeField]
	private RawImage toImage;

	private Action OnReset;

	private MVBody avatarBody;

	public void ResetAvatar(MVBody currentBody, Action onReset)
	{
		OnReset = onReset;
		avatarBody = currentBody;
		GameObject popup = UnityEngine.Object.Instantiate(invisibleBlocker);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(popup, UIPushOption.Blocking | UIPushOption.InvisibleBlocker, null, UIGroupFlags.Popup);
		});
		screenShooter.TakeScreenShot(OnScreenshotReady, avatarBody);
		RetrieveToScreenshot();
	}

	private void RetrieveToScreenshot()
	{
	}

	private void OnToScreenshotCallback(WWW www)
	{
		if (string.IsNullOrEmpty(www.error))
		{
			toImage.color = new Color(1f, 1f, 1f, 1f);
			toImage.texture = www.texture;
		}
		else
		{
			Debug.LogError("Failed to retrieve avatar image to reset to.");
		}
	}

	private void OnScreenshotReady(Texture2D texture, string text)
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
		fromImage.color = new Color(1f, 1f, 1f, 1f);
		fromImage.texture = texture;
	}

	public void OnAcceptReset()
	{
		PleaseWaitPopup popup = UnityEngine.Object.Instantiate(pleaseWaitPopupPrefab);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(popup.gameObject, UIPushOption.Blocking, null, UIGroupFlags.Popup);
		});
		OnReset();
	}
}
