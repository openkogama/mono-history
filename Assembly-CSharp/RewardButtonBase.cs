using UnityEngine;
using UnityEngine.UI;

public class RewardButtonBase : MonoBehaviour
{
	[SerializeField]
	private Button buttonToDisable;

	[SerializeField]
	private Image RadialFill;

	[SerializeField]
	private ImageAnimator OutlineAnimator;

	private void Start()
	{
		OutlineAnimator.enabled = false;
	}

	protected virtual void EnableEffects()
	{
		OutlineAnimator.enabled = true;
		RadialFill.fillAmount = 1f;
		buttonToDisable.image.color = buttonToDisable.colors.normalColor;
	}

	protected virtual void DisableEffects()
	{
		OutlineAnimator.enabled = false;
		buttonToDisable.image.color = buttonToDisable.colors.disabledColor;
	}

	protected virtual void UpdateOutline(float progress)
	{
		RadialFill.fillAmount = progress;
	}
}
