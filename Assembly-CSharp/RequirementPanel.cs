using UnityEngine;
using UnityEngine.UI;

public class RequirementPanel : MonoBehaviour
{
	[SerializeField]
	private Image canUseSprite;

	[SerializeField]
	private Text requirementCost;

	public void SetRequirementCostText(int cost)
	{
		requirementCost.text = cost.ToString();
	}

	public void SetCanAfford(Sprite canAffordSprite)
	{
		if (canUseSprite.sprite != canAffordSprite)
		{
			canUseSprite.sprite = canAffordSprite;
		}
	}
}
