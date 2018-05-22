using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinningConditionBriefing : MonoBehaviour
{
	[Serializable]
	private class WinningConditionBriefingDef
	{
		public WinningConditionType type = WinningConditionType.None;

		public Image scoreIconPrefab;

		public Image winConImagePrefab;
	}

	[SerializeField]
	private GameObject winningConditionImageBackground;

	[SerializeField]
	private ProgressBar scoreProgressBar;

	[SerializeField]
	private Text scoreText;

	[SerializeField]
	private Text scoreSlash;

	[SerializeField]
	private Text scoreLimit;

	[SerializeField]
	private GameObject scoreGameObject;

	[SerializeField]
	private GameObject reachTheFlagPresent;

	[SerializeField]
	private Text flagScoreText;

	[SerializeField]
	private GameObject progressableWinningConditionPresent;

	[SerializeField]
	private ProgressBar roundTimeProgressBar;

	[SerializeField]
	private Text roundTimeText;

	[SerializeField]
	private Image roundTimeIconPrefab;

	[SerializeField]
	private Text winningConditionHeader;

	[SerializeField]
	private WinningConditionBriefingLayoutFitter briefingLayoutFitter;

	[SerializeField]
	private ScoreBoardController scoreBoardController;

	[SerializeField]
	private List<WinningConditionBriefingDef> winningConditionMapping;

	private WinningConditionType winConType;

	private bool isInitialized;

	private Action initializeCallback;

	private Image winConImage;

	private readonly Dictionary<WinningConditionType, string> headerMap = new Dictionary<WinningConditionType, string>
	{
		{
			WinningConditionType.Flag,
			TM._("REACH THE FLAG!")
		},
		{
			WinningConditionType.Collectible,
			TM._("FIND ALL THE STARS!")
		},
		{
			WinningConditionType.Kill,
			TM._("ELIMINATE YOUR ENEMIES!")
		},
		{
			WinningConditionType.Oculus,
			TM._("ELIMINATE THE OCULI!")
		},
		{
			WinningConditionType.FinishLine,
			TM._("REACH THE FINISH LINE!")
		}
	};

	private Vector2 screensize;

	public void Initialize(WinningConditionType winConType)
	{
		this.winConType = winConType;
		isInitialized = true;
		if (initializeCallback != null)
		{
			initializeCallback();
		}
	}

	private void Start()
	{
		screensize = new Vector2(Screen.width, Screen.height);
		if (isInitialized)
		{
			SetupBriefing();
		}
		else
		{
			initializeCallback = (Action)Delegate.Combine(initializeCallback, new Action(SetupBriefing));
		}
	}

	private void SetupBriefing()
	{
		initializeCallback = (Action)Delegate.Remove(initializeCallback, new Action(SetupBriefing));
		WinningConditionBriefingDef winningConditionBriefingDef = null;
		for (int i = 0; i < winningConditionMapping.Count; i++)
		{
			if (winConType == winningConditionMapping[i].type)
			{
				winningConditionBriefingDef = winningConditionMapping[i];
				break;
			}
		}
		if (winningConditionBriefingDef == null)
		{
			Debug.Log("winConType: " + winConType);
			Debug.LogError("WinningConditionData is null, but a winning condition was expected");
		}
		if (winConType != WinningConditionType.None)
		{
			winningConditionHeader.text = headerMap[winConType];
			if (WinningConditionControl.TryGetPrioritizedStat(out var statType))
			{
				int num = 0;
				num = ((MVGameControllerBase.Game.TeamManager.TeamCount() <= 1) ? MVGameControllerBase.Game.GameStatCounterManager.GetActorCount(statType, MVGameControllerBase.Game.LocalPlayer.Team, MVGameControllerBase.Game.LocalPlayer.ActorNr) : MVGameControllerBase.Game.GameStatCounterManager.GetTeamCount(statType, MVGameControllerBase.Game.LocalPlayer.Team));
				int prioritizedStatLimit = WinningConditionControl.GetPrioritizedStatLimit(statType);
				if (prioritizedStatLimit == 0 && winConType == WinningConditionType.Flag)
				{
					progressableWinningConditionPresent.SetActive(value: false);
					reachTheFlagPresent.SetActive(value: true);
					flagScoreText.text = WinningConditionControl.MakeIntoScoreText(GetFlagHighScore(), GameStatCounterType.Flag);
				}
				else
				{
					reachTheFlagPresent.SetActive(value: false);
					progressableWinningConditionPresent.SetActive(value: true);
				}
				scoreLimit.text = prioritizedStatLimit.ToString();
				scoreProgressBar.Progress = (float)num / (float)prioritizedStatLimit;
				scoreText.text = WinningConditionControl.MakeIntoScoreText(num, statType);
			}
			ScoreBoardBase instantiatedScoreboard = scoreBoardController.GetInstantiatedScoreboard(winConType);
			instantiatedScoreboard.transform.SetParent(scoreBoardController.transform, worldPositionStays: false);
			instantiatedScoreboard.transform.SetAsFirstSibling();
			instantiatedScoreboard.Initialize(statType);
			winConImage = UnityEngine.Object.Instantiate(winningConditionBriefingDef.winConImagePrefab);
			winConImage.transform.SetParent(winningConditionImageBackground.transform, worldPositionStays: false);
			UnityEngine.Object.Instantiate(winningConditionBriefingDef.scoreIconPrefab).transform.SetParent(scoreGameObject.transform, worldPositionStays: false);
		}
		WorldObjectClientRef<MVRoundCube> singletonWorldObjectRef = MVGameControllerBase.WOCM.GetSingletonWorldObjectRef<MVRoundCube>();
		if (singletonWorldObjectRef != null)
		{
			roundTimeProgressBar.gameObject.SetActive(value: true);
			roundTimeProgressBar.Progress = (float)singletonWorldObjectRef.WorldObjectClient.GetTimeLeft() / (float)singletonWorldObjectRef.WorldObjectClient.DurationInMilliseconds;
			roundTimeText.text = singletonWorldObjectRef.WorldObjectClient.MakeTimeIntoText(singletonWorldObjectRef.WorldObjectClient.GetTimeLeft());
			UnityEngine.Object.Instantiate(roundTimeIconPrefab).transform.SetParent(roundTimeProgressBar.transform, worldPositionStays: false);
		}
		StopAllCoroutines();
		StartCoroutine(FixAspectRatioDelay());
	}

	private void Update()
	{
		MVGameControllerBase.WOCM.AvatarLocal.Visible = false;
		if (screensize.x != (float)Screen.width || screensize.y != (float)Screen.height)
		{
			screensize = new Vector2(Screen.width, Screen.height);
			StopAllCoroutines();
			StartCoroutine(FixAspectRatioDelay());
		}
		WorldObjectClientRef<MVRoundCube> singletonWorldObjectRef = MVGameControllerBase.WOCM.GetSingletonWorldObjectRef<MVRoundCube>();
		if (singletonWorldObjectRef != null)
		{
			roundTimeProgressBar.Progress = (float)singletonWorldObjectRef.WorldObjectClient.GetTimeLeft() / (float)singletonWorldObjectRef.WorldObjectClient.DurationInMilliseconds;
			roundTimeText.text = singletonWorldObjectRef.WorldObjectClient.MakeTimeIntoText(singletonWorldObjectRef.WorldObjectClient.GetTimeLeft());
		}
	}

	private void OnDestroy()
	{
		MVGameControllerBase.WOCM.AvatarLocal.Visible = true;
	}

	private IEnumerator FixAspectRatioDelay()
	{
		yield return new WaitForEndOfFrame();
		yield return null;
		briefingLayoutFitter.FixAspectRatio();
		yield return null;
		winConImage.transform.SetParent(transform, worldPositionStays: true);
		winConImage.transform.SetAsLastSibling();
	}

	private int GetFlagHighScore()
	{
		int num = 0;
		foreach (KeyValuePair<int, MVPlayer> item in MVGameControllerBase.Game.MVPlayerContainer)
		{
			if (item.Value != null)
			{
				int actorNr = item.Value.ActorNr;
				int actorCount = MVGameControllerBase.Game.GameStatCounterManager.GetActorCount(GameStatCounterType.Flag, item.Value.Team, actorNr);
				if (WinningConditionControl.IsNewScoreBetter(actorCount, num, GameStatCounterType.Flag))
				{
					num = actorCount;
				}
			}
		}
		return num;
	}
}
