using System.Collections.Generic;
using RewardGeneration;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RewardObject : MonoBehaviour
{
	[SerializeField]
	protected Text amountText;

	[SerializeField]
	private RectTransform cachedTransform;

	[SerializeField]
	private Image rarityDisplay;

	[SerializeField]
	private AnimationCurve claimRewardAnimationCurve;

	[SerializeField]
	private Button rewardClaimButton;

	[SerializeField]
	protected string textFormat;

	private UnityAction OnFinishedCallback;

	private UnityAction OnEarlyResetCallback;

	private bool rewardWon;

	private float timer;

	private Vector2 targetPos;

	private Vector2 targetSize;

	[SerializeField]
	protected List<Sprite> imagesOrderedByRarity = new List<Sprite>();

	public string Text => amountText.text;

	public RewardRarityGroupDef RewardRarity { get; private set; }

	public RectTransform CachedTransform
	{
		get
		{
			return cachedTransform;
		}
		set
		{
			cachedTransform = value;
		}
	}

	public virtual void Initialize(RewardRarityGroupDef rewardRarity)
	{
		RewardRarity = rewardRarity;
		amountText.text = FormatRewardText(rewardRarity);
		rarityDisplay.sprite = imagesOrderedByRarity[(int)rewardRarity.rarity];
	}

	protected virtual string FormatRewardText(RewardRarityGroupDef rewardGroup)
	{
		Debug.LogError("No FormatRewardText overridden for reward: " + rewardGroup.actorReward.RewardType);
		return string.Empty;
	}

	public void SelectReward(RectTransform targetRewardPosition, UnityAction OnFinishedPreviewingReward, UnityAction OnEarlyReset)
	{
		OnEarlyResetCallback = OnEarlyReset;
		OnFinishedCallback = OnFinishedPreviewingReward;
		rewardWon = true;
		targetPos = targetRewardPosition.anchoredPosition;
		targetSize = targetRewardPosition.sizeDelta;
		Vector3 position = cachedTransform.position;
		Vector2 vector = new Vector2(0.5f, 1f);
		cachedTransform.anchorMin = vector;
		cachedTransform.anchorMax = vector;
		cachedTransform.position = position;
	}

	private void Update()
	{
		if (rewardWon)
		{
			timer += Time.deltaTime;
			timer = Mathf.Clamp01(timer);
			float num = claimRewardAnimationCurve.Evaluate(timer);
			cachedTransform.anchoredPosition = Vector2.Lerp(cachedTransform.anchoredPosition, targetPos, num);
			cachedTransform.sizeDelta += (targetSize - cachedTransform.sizeDelta) * num;
			if (timer >= 1f)
			{
				OnEarlyResetCallback();
				cachedTransform.sizeDelta = targetSize;
				rewardWon = false;
				rewardClaimButton.gameObject.SetActive(value: true);
			}
		}
	}

	public void OnClose()
	{
		OnFinishedCallback();
	}

	public float GetPosX()
	{
		return cachedTransform.anchoredPosition.x;
	}
}
