using System.Collections;
using System.Collections.Generic;
using MV.WorldObject.Security;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TouristPromotion : MonoBehaviour
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
		if (!LevelingManager.IsInitialized)
		{
			BrowserComm.ToJavaScript.ExternalCall("gotoSignup");
			BrowserComm.ExecuteBrowserRequest(MVGameControllerBase.GameSessionData.signupURL);
			return;
		}
		SortedDictionary<string, string> sortedDictionary = new SortedDictionary<string, string>();
		int xP = MVGameControllerBase.Game.LocalPlayer.XPProgressData.XP;
		sortedDictionary.Add("xp", xP.ToString());
		string mD5Hash = Encryption.GetMD5Hash(sortedDictionary, MVGameControllerBase.Game.XpKey);
		BrowserComm.ToJavaScript.ExternalCall("gotoSignupWithXP", xP, mD5Hash);
		BrowserComm.ExecuteBrowserRequest(MVGameControllerBase.GameSessionData.signupURL);
	}

	public void LoginCallback()
	{
		BrowserComm.ToJavaScript.ExternalCall("gotoLogin");
		BrowserComm.ExecuteBrowserRequest(MVGameControllerBase.GameSessionData.loginURL);
	}
}
