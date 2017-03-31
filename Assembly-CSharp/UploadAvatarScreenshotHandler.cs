using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UploadAvatarScreenshotHandler : MonoBehaviour
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

	private Action<Texture2D, string> OnUploadScreenshot;

	private MVBody avatarBody;

	public void TakeScreenshot(MVBody currentBody, Action<Texture2D, string> onUploadScreenshot, bool purchasedAvatar = false)
	{
		OnUploadScreenshot = onUploadScreenshot;
		avatarBody = currentBody;
		string successMessage = ((!purchasedAvatar) ? TM._("Screenshot taken successfully") : TM._("New avatar purchased!"));
		GameObject popup = UnityEngine.Object.Instantiate(invisibleBlocker);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(popup, UIPushOption.Blocking | UIPushOption.InvisibleBlocker, null, UIGroupFlags.Popup);
		});
		screenShooter.TakeScreenShot(OnScreenshotReady, avatarBody, ignoreAccessories: false, successMessage);
	}

	private void OnScreenshotReady(Texture2D texture, string text)
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
		toImage.color = new Color(1f, 1f, 1f, 1f);
		toImage.texture = texture;
	}

	public void OnUpdatePressed()
	{
		PleaseWaitPopup popup = UnityEngine.Object.Instantiate(pleaseWaitPopupPrefab);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(popup.gameObject, UIPushOption.Blocking, null, UIGroupFlags.Popup);
		});
		screenShooter.TakeScreenShot(UploadAndDestroy, avatarBody);
	}

	private void UploadAndDestroy(Texture2D texture, string successText)
	{
		OnUploadScreenshot(texture, successText);
	}
}
