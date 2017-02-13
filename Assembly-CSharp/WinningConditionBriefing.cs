using System;
using System.Collections;
using System.Collections.Generic;
using MV.Common;
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
	private CanvasGroup winningConditionGroup;

	private bool avatarRespawned = true;

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
		winningConditionGroup.alpha = 0f;
		MVNetworkGame game = MVGameControllerBase.Game;
		game.OnWinningCondition = (Action<IWinningCondition>)Delegate.Combine(game.OnWinningCondition, new Action<IWinningCondition>(OnWinningConditionReceived));
		MVRuntimeDataVariable avatarModeTypeFlags = MVGameControllerBase.WOCM.AvatarLocal.avatarModeTypeFlags;
		avatarModeTypeFlags.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(avatarModeTypeFlags.OnChange, new MVRuntimeDataVariable.OnChangeDelegate(AvatarStateChanged));
	}

	private void AvatarStateChanged(object state)
	{
		int num = (int)state;
		if ((num & 4) > 0)
		{
			avatarRespawned = true;
		}
	}

	private void OnWinningConditionReceived(IWinningCondition winningCondition)
	{
		winningConditionMet = true;
	}

	public void AddBriefing(WinningConditionType winType)
	{
		SetupCondition(winType);
	}

	public void AddBriefing(WinningConditionType winType, int limit)
	{
		WinningConditionAndroid winningConditionAndroid = SetupCondition(winType);
		winningConditionAndroid.SetLimit(limit);
	}

	private WinningConditionAndroid CreateWinningCondition(WinningConditionType winType)
	{
		WinningConditionAndroid winningConditionAndroid = UnityEngine.Object.Instantiate(winningConditionPrefab);
		instantiatedConditions.Add(winningConditionAndroid);
		winningConditionAndroid.SetSprite(currentWinningConditions[winType]);
		return winningConditionAndroid;
	}

	private WinningConditionAndroid SetupCondition(WinningConditionType winType)
	{
		WinningConditionAndroid winningConditionAndroid = CreateWinningCondition(winType);
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
		return winningConditionAndroid;
	}

	private void OnEnable()
	{
		Clear();
		StartCoroutine(PlaySequentialInformation());
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

	private IEnumerator PlaySequentialInformation()
	{
		if (winningConditionMet)
		{
			GenerateBriefing();
			if (instantiatedConditions.Count != 0)
			{
				yield return StartCoroutine(ShowBriefingCoroutine(winningConditionGroup));
				winningConditionMet = false;
			}
		}
		if (avatarRespawned)
		{
			NotificationController.PushNotification(NotificationType.TeamNotification, NotificationsManager.eNotificationPanel.secondary);
			avatarRespawned = false;
		}
		Clear();
		yield return 0;
	}

	private IEnumerator ShowBriefingCoroutine(CanvasGroup group)
	{
		group.alpha = 0f;
		yield return StartCoroutine(Wait(initialWaitTime));
		CanvasGroup group2 = default;
		yield return StartCoroutine(pTween.To(fadeTime, 0f, 1f, (float t) =>
		{
			group2.alpha = t;
		}));
		yield return StartCoroutine(Wait(stayTime));
		yield return StartCoroutine(pTween.To(fadeTime, 1f, 0f, (float t) =>
		{
			group2.alpha = t;
		}));
		yield return 0;
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
		winningConditionGroup.alpha = 0f;
	}
}
