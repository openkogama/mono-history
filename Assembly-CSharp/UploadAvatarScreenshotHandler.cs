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
		PrepareScreenshot(currentBody, onUploadScreenshot, purchasedAvatar, out var successText);
		screenShooter.TakeScreenShot(OnScreenshotReady, avatarBody, ignoreAccessories: false, successText);
	}

	public void TakePurchasedScreenshot(MVBody currentBody, Action<Texture2D, string> onUploadScreenshot, bool purchasedAvatar = true)
	{
		PrepareScreenshot(currentBody, onUploadScreenshot, purchasedAvatar, out var successText);
		screenShooter.TakeScreenShot(OnScreenshotReadyUploadDirect, avatarBody, ignoreAccessories: false, successText);
	}

	private void PrepareScreenshot(MVBody currentBody, Action<Texture2D, string> onUploadScreenshot, bool purchasedAvatar, out string successText)
	{
		OnUploadScreenshot = onUploadScreenshot;
		if (purchasedAvatar)
		{
			successText = TM._("New avatar purchased!");
		}
		else
		{
			successText = TM._("Screenshot taken successfully");
		}
		avatarBody = currentBody;
		GameObject popup = UnityEngine.Object.Instantiate(invisibleBlocker);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(popup, UIPushOption.Blocking | UIPushOption.InvisibleBlocker, null, UIGroupFlags.Popup);
		});
	}

	private void OnScreenshotReadyUploadDirect(Texture2D texture, string text)
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
		OnUploadScreenshot(texture, text);
		UnityEngine.Object.Destroy(gameObject);
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
