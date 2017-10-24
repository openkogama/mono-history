using UnityEngine;

public class ShowUse3D : ShowUse
{
	[SerializeField]
	private GameObject canAfford;

	[SerializeField]
	private GameObject canNotAfford;

	private const ShowUseOption useButtonInsufficientOptions = ShowUseOption.GameCoinsInsufficient | ShowUseOption.LevelInsufficient | ShowUseOption.StarsInsufficient | ShowUseOption.TeamRestricted;

	public override void CalculateUseGraphics(ShowUseOption useOption, int woID = 0)
	{
		if (((ShowUseOption.GameCoinsInsufficient | ShowUseOption.LevelInsufficient | ShowUseOption.StarsInsufficient | ShowUseOption.TeamRestricted) & useOption) == 0)
		{
			canAfford.SetActive(value: true);
			canNotAfford.SetActive(value: false);
		}
		else
		{
			canAfford.SetActive(value: false);
			canNotAfford.SetActive(value: true);
		}
	}

	public override void Hide()
	{
		canAfford.SetActive(value: false);
		canNotAfford.SetActive(value: false);
	}

	public override void Show()
	{
		canAfford.SetActive(value: true);
		canNotAfford.SetActive(value: true);
	}
}
