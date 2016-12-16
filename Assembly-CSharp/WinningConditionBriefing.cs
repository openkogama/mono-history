using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinningConditionBriefing : MonoBehaviour, IBriefing
{
	[Serializable]
	private struct WinningConditionDef
	{
		public WinningConditionType conditionType;

		public Sprite conditionSprite;
	}

	[SerializeField]
	private Transform offsetTransform;

	[SerializeField]
	private WinningConditionAndroid winningConditionPrefab;

	[SerializeField]
	private List<WinningConditionDef> winningConditionList;

	[SerializeField]
	private CanvasGroup group;

	private float fadeTime = 0.3f;

	private float stayTime = 3f;

	private float initialWaitTime = 1f;

	private List<WinningConditionAndroid> instantiatedConditions = new List<WinningConditionAndroid>();

	private bool winningConditionMet = true;

	private Dictionary<WinningConditionType, Sprite> currentWinningConditions = new Dictionary<WinningConditionType, Sprite>();

	private void Awake()
	{
		for (int i = 0; i < winningConditionList.Count; i++)
		{
			currentWinningConditions.Add(winningConditionList[i].conditionType, winningConditionList[i].conditionSprite);
		}
		group.alpha = 0f;
	}

	private void Start()
	{
		MVNetworkGame game = MVGameControllerBase.Game;
		game.OnWinningCondition = (Action<IWinningCondition>)Delegate.Combine(game.OnWinningCondition, new Action<IWinningCondition>(OnWinningConditionReceived));
	}

	private void OnWinningConditionReceived(IWinningCondition winningCondition)
	{
		winningConditionMet = true;
	}

	public void AddBriefing(WinningConditionType winType)
	{
		WinningConditionAndroid winningConditionAndroid = UnityEngine.Object.Instantiate(winningConditionPrefab);
		instantiatedConditions.Add(winningConditionAndroid);
		winningConditionAndroid.SetSprite(currentWinningConditions[winType]);
		string additionalInformation = string.Empty;
		switch (winType)
		{
		case WinningConditionType.Highest:
			additionalInformation = TM._("Reach highest altitude");
			break;
		case WinningConditionType.Lowest:
			additionalInformation = TM._("Reach lowest altitude");
			break;
		case WinningConditionType.Collectible:
			additionalInformation = TM._("Collect stars");
			break;
		case WinningConditionType.Flag:
			additionalInformation = TM._("Reach the flag");
			break;
		case WinningConditionType.Kill:
			additionalInformation = TM._("Eliminate other players");
			break;
		case WinningConditionType.Oculus:
			additionalInformation = TM._("Eliminate Oculi");
			break;
		case WinningConditionType.Time:
			additionalInformation = TM._("Wait for the timer to end");
			break;
		}
		winningConditionAndroid.SetAdditionalInformation(additionalInformation);
		winningConditionAndroid.transform.SetParent(offsetTransform, worldPositionStays: false);
	}

	public void AddBriefing(WinningConditionType winType, int limit)
	{
		WinningConditionAndroid winningConditionAndroid = UnityEngine.Object.Instantiate(winningConditionPrefab);
		instantiatedConditions.Add(winningConditionAndroid);
		winningConditionAndroid.SetSprite(currentWinningConditions[winType]);
		winningConditionAndroid.SetLimit(limit);
		string additionalInformation = string.Empty;
		switch (winType)
		{
		case WinningConditionType.Highest:
			additionalInformation = TM._("Reach highest altitude");
			break;
		case WinningConditionType.Lowest:
			additionalInformation = TM._("Reach lowest altitude");
			break;
		case WinningConditionType.Collectible:
			additionalInformation = TM._("Collect stars");
			break;
		case WinningConditionType.Flag:
			additionalInformation = TM._("Reach the flag");
			break;
		case WinningConditionType.Kill:
			additionalInformation = TM._("Eliminate other players");
			break;
		case WinningConditionType.Oculus:
			additionalInformation = TM._("Eliminate Oculi");
			break;
		case WinningConditionType.Time:
			additionalInformation = TM._("Wait for the timer to end");
			break;
		}
		winningConditionAndroid.SetAdditionalInformation(additionalInformation);
		winningConditionAndroid.transform.SetParent(offsetTransform, worldPositionStays: false);
	}

	private void OnEnable()
	{
		Clear();
		if (winningConditionMet)
		{
			GenerateBriefing();
			if (instantiatedConditions.Count != 0)
			{
				StartCoroutine(ShowBriefingCoroutine());
				winningConditionMet = false;
			}
		}
	}

	private void GenerateBriefing()
	{
		List<IWinningCondition> winnerConditions = new List<IWinningCondition>();
		MVGameControllerBase.Game.WinningConditionManager.Traverse((IWinningCondition winnerCondition) =>
		{
			if (winnerCondition.IsBriefingNode)
			{
				winnerConditions.Add(winnerCondition);
			}
			return false;
		});
		foreach (IWinningCondition item in winnerConditions)
		{
			if (item is IWinningConditionBriefing)
			{
				((IWinningConditionBriefing)item).GetBriefing(this);
				continue;
			}
			string message = $"No localized briefing for: {item.GetType()}";
			Debug.LogWarning(message);
		}
	}

	private IEnumerator ShowBriefingCoroutine()
	{
		group.alpha = 0f;
		yield return StartCoroutine(Wait(initialWaitTime));
		yield return StartCoroutine(pTween.To(fadeTime, 0f, 1f, (float t) =>
		{
			group.alpha = t;
		}));
		yield return StartCoroutine(Wait(stayTime));
		yield return StartCoroutine(pTween.To(fadeTime, 1f, 0f, (float t) =>
		{
			group.alpha = t;
			if (t == 0f)
			{
				Clear();
			}
		}));
	}

	private IEnumerator Wait(float wait)
	{
		yield return new WaitForSeconds(wait);
	}

	public void Clear()
	{
		StopAllCoroutines();
		for (int i = 0; i < instantiatedConditions.Count; i++)
		{
			UnityEngine.Object.Destroy(instantiatedConditions[i].gameObject);
		}
		instantiatedConditions.Clear();
		group.alpha = 0f;
	}
}
