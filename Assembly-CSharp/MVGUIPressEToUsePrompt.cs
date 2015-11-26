using MV.Common;
using UnityEngine;

public class MVGUIPressEToUsePrompt : UXViewScript
{
	private const ShowUseOption useButtonInsufficientOptions = ShowUseOption.GameCoinsInsufficient | ShowUseOption.LevelInsufficient | ShowUseOption.StarsInsufficient;

	private const float posY = 13f;

	public float fadeTime = 1f;

	public UXPlane originalEBG;

	public Material originalEMatAvailable;

	public Material originalEMatNotAvailable;

	public UXPlane uxPlaneBGPressE;

	public Material checkmarkMat;

	public Material insufficientMat;

	public UXPlane uxPlaneEButton;

	public Material canUseMat;

	public Material cannotUseMat;

	public UXPlane uxPlaneBGLevels;

	public UXPlane uxPlaneLevel;

	public UXPlane uxPlaneLevelAvailability;

	private Renderer levelPlaneRenderer;

	public UXPlane uxPlaneBGGameCoins;

	public UXPlane uxPlaneGameCoins;

	public UXPlane uxPlaneGameCoinsAvailability;

	public UXPlane uxPlaneBGStars;

	public UXPlane uxPlaneStar;

	public UXPlane uxPlaneStarAvailability;

	public UXText uxTextPressEToUse;

	public UXText uxTextGameCoins;

	public UXText uxTextStars;

	private float currPosY;

	private int levelRequirementCurrent;

	public override void OnInitialize()
	{
		base.OnInitialize();
		levelPlaneRenderer = uxPlaneLevel.GetComponent<Renderer>();
		levelPlaneRenderer.material.mainTexture = null;
	}

	public void Show(ShowUseOption option, int woID = 0)
	{
		HideAllIcons();
		if (option != ShowUseOption.Normal && MVGameControllerBase.Game.GameType == MVGameType.Platformer)
		{
			currPosY = 13f;
			ShowPressERequirement();
			currPosY -= 3.5f;
			MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(woID);
			if ((option & ShowUseOption.UsingLevels) == ShowUseOption.UsingLevels)
			{
				int level = (int)worldObjectClient.Data["levelAmount"];
				ShowLevelRequirement(option, level);
				currPosY -= uxPlaneBGLevels.Height;
			}
			if ((option & ShowUseOption.UsingGameCoins) == ShowUseOption.UsingGameCoins)
			{
				int gameCoins = (int)worldObjectClient.Data["gameCoinAmount"];
				ShowGameCoinRequirement(option, gameCoins);
				currPosY -= uxPlaneBGGameCoins.Height;
			}
			if ((option & ShowUseOption.UsingStars) == ShowUseOption.UsingStars)
			{
				int stars = (int)worldObjectClient.Data["starAmount"];
				ShowStarRequirement(option, stars);
				currPosY -= uxPlaneBGStars.Height;
			}
		}
		else
		{
			if (((ShowUseOption.GameCoinsInsufficient | ShowUseOption.LevelInsufficient | ShowUseOption.StarsInsufficient) & option) == 0)
			{
				originalEBG.SetMaterial(originalEMatAvailable);
			}
			else
			{
				originalEBG.SetMaterial(originalEMatNotAvailable);
			}
			originalEBG.SetVisible(visible: true);
		}
		View.Show();
	}

	private void StreamingAssetCallback(WWW www)
	{
		if (levelPlaneRenderer != null)
		{
			if (levelPlaneRenderer.material.mainTexture != null)
			{
				Object.Destroy(levelPlaneRenderer.material.mainTexture);
			}
			levelPlaneRenderer.material.mainTexture = www.texture;
		}
	}

	private void ShowPressERequirement()
	{
		uxPlaneEButton.SetMaterial(canUseMat);
		uxPlaneEButton.SetVisible(visible: true);
		uxPlaneBGPressE.SetVisible(visible: true);
		uxTextPressEToUse.SetVisible(visible: true);
		Vector3 localPosition = uxPlaneBGPressE.gameObject.transform.localPosition;
		uxPlaneBGPressE.transform.localPosition = new Vector3(localPosition.x, currPosY, localPosition.z);
	}

	private void ShowLevelRequirement(ShowUseOption option, int level)
	{
		if (levelRequirementCurrent != level)
		{
			levelRequirementCurrent = level;
			BadgeManager.GetBadgeTexture(level, StreamingAssetCallback);
		}
		if ((option & ShowUseOption.LevelEnough) == ShowUseOption.LevelEnough)
		{
			uxPlaneLevelAvailability.SetMaterial(checkmarkMat);
		}
		else
		{
			uxPlaneLevelAvailability.SetMaterial(insufficientMat);
			uxPlaneEButton.SetMaterial(cannotUseMat);
		}
		uxPlaneLevel.SetVisible(visible: true);
		uxPlaneLevelAvailability.SetVisible(visible: true);
		uxPlaneBGLevels.SetVisible(visible: true);
		Vector3 localPosition = uxPlaneBGLevels.gameObject.transform.localPosition;
		uxPlaneBGLevels.transform.localPosition = new Vector3(localPosition.x, currPosY, localPosition.z);
	}

	private void ShowGameCoinRequirement(ShowUseOption option, int gameCoins)
	{
		if ((option & ShowUseOption.GameCoinsEnough) == ShowUseOption.GameCoinsEnough)
		{
			uxPlaneGameCoinsAvailability.SetMaterial(checkmarkMat);
		}
		else
		{
			uxPlaneGameCoinsAvailability.SetMaterial(insufficientMat);
			uxPlaneEButton.SetMaterial(cannotUseMat);
		}
		uxPlaneGameCoins.SetVisible(visible: true);
		uxPlaneGameCoinsAvailability.SetVisible(visible: true);
		uxPlaneBGGameCoins.SetVisible(visible: true);
		uxTextGameCoins.SetVisible(visible: true);
		uxTextGameCoins.Text = gameCoins.ToString();
		Vector3 localPosition = uxPlaneBGGameCoins.gameObject.transform.localPosition;
		uxPlaneBGGameCoins.transform.localPosition = new Vector3(localPosition.x, currPosY, localPosition.z);
	}

	private void ShowStarRequirement(ShowUseOption option, int stars)
	{
		if ((option & ShowUseOption.StarsEnough) == ShowUseOption.StarsEnough)
		{
			uxPlaneStarAvailability.SetMaterial(checkmarkMat);
		}
		else
		{
			uxPlaneStarAvailability.SetMaterial(insufficientMat);
			uxPlaneEButton.SetMaterial(cannotUseMat);
		}
		uxPlaneStar.SetVisible(visible: true);
		uxPlaneStarAvailability.SetVisible(visible: true);
		uxPlaneBGStars.SetVisible(visible: true);
		uxTextStars.SetVisible(visible: true);
		uxTextStars.Text = stars.ToString();
		Vector3 localPosition = uxPlaneBGStars.gameObject.transform.localPosition;
		uxPlaneBGStars.transform.localPosition = new Vector3(localPosition.x, currPosY, localPosition.z);
	}

	public void Hide()
	{
		View.Hide();
	}

	private void HideAllIcons()
	{
		uxPlaneEButton.SetVisible(visible: false);
		uxPlaneLevel.SetVisible(visible: false);
		uxPlaneLevelAvailability.SetVisible(visible: false);
		uxPlaneGameCoins.SetVisible(visible: false);
		uxPlaneGameCoinsAvailability.SetVisible(visible: false);
		uxPlaneStar.SetVisible(visible: false);
		uxPlaneStarAvailability.SetVisible(visible: false);
		originalEBG.SetVisible(visible: false);
		uxPlaneBGPressE.SetVisible(visible: false);
		uxPlaneBGStars.SetVisible(visible: false);
		uxPlaneBGLevels.SetVisible(visible: false);
		uxPlaneBGGameCoins.SetVisible(visible: false);
		uxTextPressEToUse.SetVisible(visible: false);
		uxTextGameCoins.SetVisible(visible: false);
		uxTextStars.SetVisible(visible: false);
	}

	public override void OnShow()
	{
		StopAllCoroutines();
		StartCoroutine(pTween.To(fadeTime, 0f, 1f, (float t) =>
		{
			uxPlaneEButton.SetAlpha(t, string.Empty);
			uxPlaneLevel.SetAlpha(t, string.Empty);
			uxPlaneLevelAvailability.SetAlpha(t, string.Empty);
			uxPlaneGameCoins.SetAlpha(t, string.Empty);
			uxPlaneGameCoinsAvailability.SetAlpha(t, string.Empty);
			uxPlaneStar.SetAlpha(t, string.Empty);
			uxPlaneStarAvailability.SetAlpha(t, string.Empty);
			uxPlaneBGPressE.SetAlpha(t, string.Empty);
			uxPlaneBGStars.SetAlpha(t, string.Empty);
			uxPlaneBGLevels.SetAlpha(t, string.Empty);
			uxPlaneBGGameCoins.SetAlpha(t, string.Empty);
			originalEBG.SetAlpha(t, string.Empty);
			uxTextPressEToUse.SetAlpha(t, string.Empty);
			uxTextGameCoins.SetAlpha(t, string.Empty);
			uxTextStars.SetAlpha(t, string.Empty);
		}));
	}

	public override void OnHide()
	{
		StopAllCoroutines();
		StartCoroutine(pTween.To(fadeTime, 1f, 0f, (float t) =>
		{
			uxPlaneEButton.SetAlpha(t, string.Empty);
			uxPlaneLevel.SetAlpha(t, string.Empty);
			uxPlaneLevelAvailability.SetAlpha(t, string.Empty);
			uxPlaneGameCoins.SetAlpha(t, string.Empty);
			uxPlaneGameCoinsAvailability.SetAlpha(t, string.Empty);
			uxPlaneStar.SetAlpha(t, string.Empty);
			uxPlaneStarAvailability.SetAlpha(t, string.Empty);
			uxPlaneBGPressE.SetAlpha(t, string.Empty);
			uxPlaneBGStars.SetAlpha(t, string.Empty);
			uxPlaneBGLevels.SetAlpha(t, string.Empty);
			uxPlaneBGGameCoins.SetAlpha(t, string.Empty);
			originalEBG.SetAlpha(t, string.Empty);
			uxTextPressEToUse.SetAlpha(t, string.Empty);
			uxTextGameCoins.SetAlpha(t, string.Empty);
			uxTextStars.SetAlpha(t, string.Empty);
			if (t == 0f)
			{
				HideAllIcons();
			}
		}));
	}
}
