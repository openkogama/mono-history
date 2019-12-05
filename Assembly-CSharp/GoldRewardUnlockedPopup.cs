using UnityEngine;
using UnityEngine.UI;

public class GoldRewardUnlockedPopup : MonoBehaviour
{
	[SerializeField]
	private Text titleText;

	[SerializeField]
	private RectTransform imageContentTransform;

	[SerializeField]
	private float bounceEffectDuration;

	[SerializeField]
	private AnimationCurve bounceEffect;

	private float bounceEffectStartTime;

	private string titleString = "{0} GOLD CLAIMED!";

	private void Start()
	{
		StartEffect();
		titleText.text = string.Format(TM._(titleString), 2);
	}

	private void StartEffect()
	{
		bounceEffectStartTime = Time.time;
		float num = bounceEffect.Evaluate((Time.time - bounceEffectStartTime) / bounceEffectDuration);
	}

	private void Update()
	{
		float num = bounceEffect.Evaluate((Time.time - bounceEffectStartTime) / bounceEffectDuration);
		imageContentTransform.localScale = new Vector3(num, num, 1f);
	}
}
