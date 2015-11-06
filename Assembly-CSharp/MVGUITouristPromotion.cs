using System;
using System.Collections.Generic;
using MV.Common;
using MV.WorldObject.Security;
using UnityEngine;

public class MVGUITouristPromotion : UXViewScript
{
	private class ShowPromotionBookkeeping
	{
		private const int deathShowFrequence = 3;

		private bool devMode;

		private bool isDead;

		private bool deathConditionTriggered = true;

		private float deathTime;

		private float deathTimeDelay = 1f;

		private bool gameEntered = true;

		private bool continuedClicked;

		private bool showTouristPromotion;

		private int deaths;

		public bool Show => ShowPromotion();

		public ShowPromotionBookkeeping(bool devMode)
		{
			this.devMode = devMode;
		}

		public void Continue()
		{
			continuedClicked = true;
		}

		private bool ShowPromotion()
		{
			showTouristPromotion |= DeadShowCondition();
			showTouristPromotion |= showTouristPromotion;
			showTouristPromotion |= devMode;
			bool flag = ContinuedClicked();
			showTouristPromotion = showTouristPromotion && !flag;
			return showTouristPromotion;
		}

		private bool ContinuedClicked()
		{
			if (continuedClicked)
			{
				Debug.Log("continuedClicked " + continuedClicked);
				continuedClicked = false;
				return true;
			}
			return false;
		}

		private bool GameEnteredCondition()
		{
			if (TouristPromotionAllowed && gameEntered)
			{
				gameEntered = false;
				return true;
			}
			return false;
		}

		private bool DeadShowCondition()
		{
			if (TouristPromotionAllowed)
			{
				if (!isDead && MVGameControllerBase.WOCM.AvatarLocal.IsDead)
				{
					isDead = true;
					deaths++;
					if (deaths % 3 == 0)
					{
						deathTime = Time.time;
						deathConditionTriggered = false;
					}
				}
				else if (isDead && !MVGameControllerBase.WOCM.AvatarLocal.IsDead)
				{
					isDead = false;
				}
				if (!deathConditionTriggered && Time.time - deathTime > deathTimeDelay)
				{
					deathConditionTriggered = true;
					Debug.Log("Returning true " + deaths);
					return true;
				}
			}
			return false;
		}
	}

	private class PromotionDataManager
	{
		private string baseAssetString = "Promotion/Promotion_{0}.png";

		private List<Texture> promotionDatas = new List<Texture>();

		private int currentSlideIndex;

		public Texture NextPromotionData
		{
			get
			{
				if (promotionDatas.Count == 0)
				{
					Debug.LogWarning("PromotionDatas.Count == 0");
					return null;
				}
				Texture result = promotionDatas[currentSlideIndex % promotionDatas.Count];
				currentSlideIndex++;
				return result;
			}
		}

		public PromotionDataManager()
		{
			DownloadSlide();
		}

		private void DownloadSlide()
		{
			try
			{
				AsyncWWWManager.WWWRequest(new CachedGetRequest(Urls.StreamingAssets + GetPath(promotionDatas.Count + 1), StreamingAssetCallback));
			}
			catch (Exception ex)
			{
				Debug.LogError("ex: " + ex.Message);
			}
		}

		private string GetPath(int i)
		{
			return string.Format(baseAssetString, i.ToString("D2"));
		}

		private void StreamingAssetCallback(WWW www)
		{
			if (www.error != null)
			{
				Debug.LogWarning("www.error != null");
			}
			else if (www.texture != null)
			{
				promotionDatas.Add(www.texture);
				DownloadSlide();
			}
			else
			{
				Debug.Log("Failed " + www.url);
			}
		}
	}

	private PromotionDataManager promotionDataManager;

	private ShowPromotionBookkeeping showPromotionBookkeeping;

	private MVGUIChatWindow guiChatWindow;

	[SerializeField]
	private bool devMode;

	[SerializeField]
	private UXBaseButton buttonPromotionSlide;

	[SerializeField]
	private UXText promotionText;

	[SerializeField]
	private MVGUIClickWall clickWall;

	[SerializeField]
	private UXBaseButton buttonContinue;

	[SerializeField]
	private UXBaseButton buttonLogin;

	[SerializeField]
	private UXBaseButton buttonSignup;

	[SerializeField]
	private FadeTransition fadeTransition;

	private static bool TouristPromotionAllowed => MVGameControllerBase.IsTouristSession && MVGameControllerBase.Game.IsPlaying && MVGameControllerBase.JoinState == MVJoinState.Playing;

	public void Start()
	{
		guiChatWindow = UXUtils.FindGUIObjectOfType<MVGUIChatWindow>();
		showPromotionBookkeeping = new ShowPromotionBookkeeping(devMode);
		if (devMode || (MVGameControllerBase.IsTouristSession && MVClientSettings.ShowTouristPromotion))
		{
			promotionDataManager = new PromotionDataManager();
			ResolveGUIElements();
		}
		else
		{
			gameObject.SetActive(value: false);
		}
	}

	private void Update()
	{
		if (showPromotionBookkeeping.Show)
		{
			if (!View.isVisible)
			{
				View.Show();
			}
		}
		else if (View.isVisible && fadeTransition.GuiFadeState != GuiFadeState.FadeOut)
		{
			fadeTransition.FadeOut(View.Hide);
		}
	}

	public override void OnShow()
	{
		base.OnShow();
		fadeTransition.FadeIn(null);
	}

	public override void OnHide()
	{
		base.OnHide();
		SetToPromotionData();
	}

	private void ResolveGUIElements()
	{
		ResolveClickwall();
		ResolveButtons();
	}

	private void ResolveButtons()
	{
		UXBaseButton uXBaseButton = buttonContinue;
		uXBaseButton.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXBaseButton.OnClick, (UXBaseButton.OnClickDelegate)(() =>
		{
			Debug.Log("Continue");
			showPromotionBookkeeping.Continue();
			guiChatWindow.CreateHelpTxt();
			LockCursorManager.LockCursor = true;
			MVGameControllerLegacyUI.PlayController.ShowBriefing();
		}));
		UXBaseButton uXBaseButton2 = buttonLogin;
		uXBaseButton2.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXBaseButton2.OnClick, (UXBaseButton.OnClickDelegate)(() =>
		{
			BrowserComm.ToJavaScript.ExternalCall("gotoLogin");
			BrowserComm.ExecuteBrowserRequest(MVGameControllerBase.GameSessionData.loginURL);
		}));
		UXBaseButton uXBaseButton3 = buttonSignup;
		uXBaseButton3.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXBaseButton3.OnClick, new UXBaseButton.OnClickDelegate(GotoSignup));
		UXBaseButton uXBaseButton4 = buttonPromotionSlide;
		uXBaseButton4.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(uXBaseButton4.OnClick, new UXBaseButton.OnClickDelegate(GotoSignup));
	}

	private void GotoSignup()
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

	private void ResolveClickwall()
	{
		clickWall.Init();
		UXMouseClickObject uXMouseClickObject = clickWall.gameObject.AddComponent<UXMouseClickObject>();
		uXMouseClickObject.OnMouseDown = (UXMouseClickObject clickObject, Vector3 mousePositionWorld) =>
		{
			Debug.Log("OnMouseDown");
			return true;
		};
		uXMouseClickObject.OnMouseUp = (UXMouseClickObject clickObject, Vector3 mousePositionWorld) =>
		{
			Debug.Log("OnMouseUp");
		};
	}

	private void SetToPromotionData()
	{
		if (promotionDataManager != null)
		{
			Texture nextPromotionData = promotionDataManager.NextPromotionData;
			if (nextPromotionData == null)
			{
				Debug.LogWarning("PromotionData is null");
			}
			else
			{
				buttonPromotionSlide.GetComponent<Renderer>().material.mainTexture = nextPromotionData;
			}
		}
	}
}
