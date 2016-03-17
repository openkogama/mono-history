using UnityEngine;
using UnityEngine.UI;

public class ShowUse2D : ShowUse
{
	private const float fadeTime = 1f;

	private const ShowUseOption useButtonInsufficientOptions = ShowUseOption.GameCoinsInsufficient | ShowUseOption.LevelInsufficient | ShowUseOption.StarsInsufficient;

	[SerializeField]
	private RequirementPanel starRequirement;

	[SerializeField]
	private RequirementPanel levelRequirement;

	private LevelRequirementPanel levelCallbackScript;

	[SerializeField]
	private RequirementPanel gameCoinRequirement;

	[SerializeField]
	private CanvasGroup panelGroup;

	[SerializeField]
	private Sprite checkMark;

	[SerializeField]
	private Sprite crossMark;

	[SerializeField]
	private Image eButton;

	[SerializeField]
	private Sprite canUse;

	[SerializeField]
	private Sprite canNotUse;

	private bool fading;

	private bool prevFadingState;

	private bool updateFade;

	private void Awake()
	{
		starRequirement = Object.Instantiate(starRequirement);
		starRequirement.transform.SetParent(panelGroup.transform, worldPositionStays: false);
		levelRequirement = Object.Instantiate(levelRequirement);
		levelRequirement.transform.SetParent(panelGroup.transform, worldPositionStays: false);
		levelCallbackScript = levelRequirement.GetComponent<LevelRequirementPanel>();
		gameCoinRequirement = Object.Instantiate(gameCoinRequirement);
		gameCoinRequirement.transform.SetParent(panelGroup.transform, worldPositionStays: false);
	}

	public override void Hide()
	{
		fading = true;
		updateFade = !prevFadingState;
		prevFadingState = true;
	}

	public override void Show()
	{
		fading = false;
		updateFade = prevFadingState;
		prevFadingState = true;
	}

	private void Update()
	{
		if (!updateFade)
		{
			return;
		}
		if (fading)
		{
			panelGroup.alpha -= Time.deltaTime / 1f;
			if (panelGroup.alpha <= 0f)
			{
				updateFade = false;
				Mathf.Clamp01(panelGroup.alpha);
			}
		}
		else
		{
			panelGroup.alpha += Time.deltaTime / 1f;
			if (panelGroup.alpha >= 1f)
			{
				updateFade = false;
				Mathf.Clamp01(panelGroup.alpha);
			}
		}
	}

	public override void CalculateUseGraphics(ShowUseOption useOption, int woID = 0)
	{
		if (useOption != ShowUseOption.Normal)
		{
			HideRequirements();
			MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(woID);
			if ((useOption & ShowUseOption.UsingLevels) == ShowUseOption.UsingLevels)
			{
				int level = (int)worldObjectClient.Data["levelAmount"];
				ShowLevelRequirement(useOption, level);
			}
			if ((useOption & ShowUseOption.UsingGameCoins) == ShowUseOption.UsingGameCoins)
			{
				int coins = (int)worldObjectClient.Data["gameCoinAmount"];
				ShowGameCoinRequirement(useOption, coins);
			}
			if ((useOption & ShowUseOption.UsingStars) == ShowUseOption.UsingStars)
			{
				int stars = (int)worldObjectClient.Data["starAmount"];
				ShowStarRequirement(useOption, stars);
			}
			if (((ShowUseOption.GameCoinsInsufficient | ShowUseOption.LevelInsufficient | ShowUseOption.StarsInsufficient) & useOption) == 0)
			{
				eButton.sprite = canUse;
			}
			else
			{
				eButton.sprite = canNotUse;
			}
		}
		else
		{
			eButton.sprite = canUse;
		}
	}

	private void HideRequirements()
	{
		levelRequirement.gameObject.SetActive(value: false);
		starRequirement.gameObject.SetActive(value: false);
		gameCoinRequirement.gameObject.SetActive(value: false);
	}

	private void ShowLevelRequirement(ShowUseOption useOption, int level)
	{
		levelRequirement.gameObject.SetActive(value: true);
		levelCallbackScript.SetLevelSpriteFromCallback(level);
		levelRequirement.SetRequirementCostText(level);
		Sprite canAfford = crossMark;
		if ((useOption & ShowUseOption.LevelEnough) == ShowUseOption.LevelEnough)
		{
			canAfford = checkMark;
		}
		levelRequirement.SetCanAfford(canAfford);
	}

	private void ShowGameCoinRequirement(ShowUseOption useOption, int coins)
	{
		gameCoinRequirement.gameObject.SetActive(value: true);
		gameCoinRequirement.SetRequirementCostText(coins);
		Sprite canAfford = crossMark;
		if ((useOption & ShowUseOption.GameCoinsEnough) == ShowUseOption.GameCoinsEnough)
		{
			canAfford = checkMark;
		}
		gameCoinRequirement.SetCanAfford(canAfford);
	}

	private void ShowStarRequirement(ShowUseOption useOption, int stars)
	{
		starRequirement.gameObject.SetActive(value: true);
		starRequirement.SetRequirementCostText(stars);
		Sprite canAfford = crossMark;
		if ((useOption & ShowUseOption.StarsEnough) == ShowUseOption.StarsEnough)
		{
			canAfford = checkMark;
		}
		starRequirement.SetCanAfford(canAfford);
	}
}
