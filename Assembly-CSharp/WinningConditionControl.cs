using System;
using MV.Common;
using UnityEngine;

public class WinningConditionControl : MonoBehaviour
{
	private bool displayTeam;

	[SerializeField]
	private FlagWinningCondition flagWinConditionUI;

	[SerializeField]
	private CollectiblesWinningCondition collectiblesWinningCondition;

	[SerializeField]
	private KillLimitWinningCondition killLimitWinningCondition;

	[SerializeField]
	private OculusKillLimitWinningCondition oculusKillLimitWinningCondition;

	[SerializeField]
	private NoWinningCondition noWinningConditionPrefab;

	private Transform ingameUIController;

	private RectTransform lobbyStateUI;

	private static WinningConditionBase currentWinningCondition;

	private NoWinningCondition noWinningCondition;

	public static WinningConditionBase CurrentWinningCondition => currentWinningCondition;

	public static bool TryGetPrioritizedWinCondition(out WinningConditionType condition)
	{
		condition = WinningConditionType.None;
		if (MVGameControllerBase.Game.WinningConditionManager.GetSingletonWinnerConditionByType<FlagReachedClient>() != null)
		{
			condition = WinningConditionType.Flag;
			return true;
		}
		if (MVGameControllerBase.Game.WinningConditionManager.GetSingletonWinnerConditionByType<AllCollectiblesCollectedClient>() != null)
		{
			condition = WinningConditionType.Collectible;
			return true;
		}
		if (MVGameControllerBase.Game.WinningConditionManager.GetSingletonWinnerConditionByType<KillLimitClient>() != null)
		{
			condition = WinningConditionType.Kill;
			return true;
		}
		if (MVGameControllerBase.Game.WinningConditionManager.GetSingletonWinnerConditionByType<OculusKillLimitClient>() != null)
		{
			condition = WinningConditionType.Oculus;
			return true;
		}
		return false;
	}

	public static bool TryGetPrioritizedStat(out GameStatCounterType statType)
	{
		statType = GameStatCounterType.None;
		if (MVGameControllerBase.Game.WinningConditionManager.GetSingletonWinnerConditionByType<FlagReachedClient>() != null)
		{
			statType = GameStatCounterType.Flag;
			return true;
		}
		if (MVGameControllerBase.Game.WinningConditionManager.GetSingletonWinnerConditionByType<AllCollectiblesCollectedClient>() != null)
		{
			statType = GameStatCounterType.Collectible;
			return true;
		}
		if (MVGameControllerBase.Game.WinningConditionManager.GetSingletonWinnerConditionByType<KillLimitClient>() != null)
		{
			statType = GameStatCounterType.Kill;
			return true;
		}
		if (MVGameControllerBase.Game.WinningConditionManager.GetSingletonWinnerConditionByType<OculusKillLimitClient>() != null)
		{
			statType = GameStatCounterType.OculusKill;
			return true;
		}
		return false;
	}

	public static int GetPrioritizedStatLimit(GameStatCounterType gameStatType)
	{
		return gameStatType switch
		{
			GameStatCounterType.Flag => 0, 
			GameStatCounterType.Collectible => MVGameControllerBase.Game.WinningConditionManager.GetSingletonWinnerConditionByType<AllCollectiblesCollectedClient>().Limit, 
			GameStatCounterType.Kill => MVGameControllerBase.Game.WinningConditionManager.GetSingletonWinnerConditionByType<KillLimitClient>().Limit, 
			GameStatCounterType.OculusKill => MVGameControllerBase.Game.WinningConditionManager.GetSingletonWinnerConditionByType<OculusKillLimitClient>().Limit, 
			_ => 0, 
		};
	}

	public static bool IsNewScoreBetter(int newScore, int oldScore, GameStatCounterType statType)
	{
		switch (statType)
		{
		case GameStatCounterType.Kill:
		case GameStatCounterType.Collectible:
		case GameStatCounterType.OculusKill:
			if (newScore > oldScore)
			{
				return true;
			}
			break;
		case GameStatCounterType.Flag:
			if (oldScore < 0)
			{
				return true;
			}
			if (newScore <= 0)
			{
				return false;
			}
			if (newScore < oldScore || oldScore == 0)
			{
				return true;
			}
			break;
		}
		return false;
	}

	public static string MakeIntoScoreText(int score, GameStatCounterType statType)
	{
		switch (statType)
		{
		case GameStatCounterType.Kill:
		case GameStatCounterType.Collectible:
		case GameStatCounterType.OculusKill:
			return score.ToString();
		case GameStatCounterType.Flag:
		{
			string text = string.Empty;
			if (score == 0)
			{
				return "--:--";
			}
			score = (int)((float)score / 1000f);
			int num = score % 60;
			int num2 = Mathf.FloorToInt((float)score / 60f);
			if (num2 >= 60)
			{
				int num3 = Mathf.FloorToInt((float)num2 / 60f);
				num2 %= 60;
				text = text + num3 + ":";
			}
			string text2 = string.Empty;
			if (num < 10)
			{
				text2 += "0";
			}
			text2 += num;
			string text3 = string.Empty;
			if (num2 < 10)
			{
				text3 += "0";
			}
			text3 += num2;
			return text + text3 + ":" + text2;
		}
		default:
			return score.ToString();
		}
	}

	public void Initialize(Transform ingameUI, RectTransform lobbyStateUI)
	{
		ingameUIController = ingameUI;
		this.lobbyStateUI = lobbyStateUI;
		if (MVGameControllerBase.Game.LocalPlayer.IsReady)
		{
			InitializeOnPlayerReady();
			return;
		}
		MVPlayerContainer mVPlayerContainer = MVGameControllerBase.Game.MVPlayerContainer;
		mVPlayerContainer.OnLocalPlayerReady = (Action)Delegate.Combine(mVPlayerContainer.OnLocalPlayerReady, new Action(InitializeOnPlayerReady));
	}

	private void InitializeOnPlayerReady()
	{
		Debug.Log("Subscribing to winningcondition callbacks.");
		displayTeam = MVGameControllerBase.Game.TeamManager.TeamCount() > 1;
		MVPlayerContainer mVPlayerContainer = MVGameControllerBase.Game.MVPlayerContainer;
		mVPlayerContainer.OnLocalPlayerReady = (Action)Delegate.Remove(mVPlayerContainer.OnLocalPlayerReady, new Action(InitializeOnPlayerReady));
		MVRuntimeDataVariable avatarModeTypeFlags = MVGameControllerBase.WOCM.AvatarLocal.avatarModeTypeFlags;
		avatarModeTypeFlags.OnChange = (MVRuntimeDataVariable.OnChangeDelegate)Delegate.Combine(avatarModeTypeFlags.OnChange, new MVRuntimeDataVariable.OnChangeDelegate(AvatarStateChanged));
		MVNetworkGame game = MVGameControllerBase.Game;
		game.OnWinningConditionFulfilled = (Action<IWinningCondition>)Delegate.Combine(game.OnWinningConditionFulfilled, new Action<IWinningCondition>(OnWinningConditionFulfilled));
		MVGameControllerBase.Game.GameStatCounterManager.OnCounterTypeChanged += WinningConditionProgressUpdated;
		MVGameControllerBase.Game.WinningConditionManager.OnWinningConditionAddedOrRemoved += CurrentWinConditionChanged;
		CreateWinningCondition();
	}

	private void WinningConditionProgressUpdated(object sender, OnCounterTypeChangedArgs args)
	{
		if (!(currentWinningCondition == null))
		{
			currentWinningCondition.UpdateStats(args.actorNumber, args.counterType, args.count);
			if (args.actorNumber == MVGameControllerBase.WOCM.AvatarLocal.OwnerActorNr)
			{
				currentWinningCondition.UpdateValue(args.count);
			}
		}
	}

	private void CreateWinningCondition()
	{
		bool flag = true;
		if (TryGetPrioritizedWinCondition(out var condition))
		{
			flag = false;
			Debug.Log("Found winning condition: " + condition);
			switch (condition)
			{
			case WinningConditionType.Flag:
			{
				FlagWinningCondition flagWinningCondition = (FlagWinningCondition)(currentWinningCondition = UnityEngine.Object.Instantiate(flagWinConditionUI));
				flagWinningCondition.transform.SetParent(ingameUIController, worldPositionStays: false);
				flagWinningCondition.transform.SetAsFirstSibling();
				flagWinningCondition.InitializeGameUI(lobbyStateUI);
				break;
			}
			case WinningConditionType.Collectible:
			{
				CollectiblesWinningCondition collectiblesWinningCondition = (CollectiblesWinningCondition)(currentWinningCondition = UnityEngine.Object.Instantiate(this.collectiblesWinningCondition));
				collectiblesWinningCondition.transform.SetParent(ingameUIController, worldPositionStays: false);
				collectiblesWinningCondition.transform.SetAsFirstSibling();
				collectiblesWinningCondition.InitializeGameUI(lobbyStateUI);
				break;
			}
			case WinningConditionType.Kill:
			{
				KillLimitWinningCondition killLimitWinningCondition = (KillLimitWinningCondition)(currentWinningCondition = UnityEngine.Object.Instantiate(this.killLimitWinningCondition));
				killLimitWinningCondition.transform.SetParent(ingameUIController, worldPositionStays: false);
				killLimitWinningCondition.transform.SetAsFirstSibling();
				killLimitWinningCondition.InitializeGameUI(lobbyStateUI);
				break;
			}
			case WinningConditionType.Oculus:
			{
				OculusKillLimitWinningCondition oculusKillLimitWinningCondition = (OculusKillLimitWinningCondition)(currentWinningCondition = UnityEngine.Object.Instantiate(this.oculusKillLimitWinningCondition));
				oculusKillLimitWinningCondition.transform.SetParent(ingameUIController, worldPositionStays: false);
				oculusKillLimitWinningCondition.transform.SetAsFirstSibling();
				oculusKillLimitWinningCondition.InitializeGameUI(lobbyStateUI);
				break;
			}
			default:
				flag = true;
				break;
			}
		}
		if (currentWinningCondition != null && !currentWinningCondition.WinningConditionAbleToBeFulfilled)
		{
			DestroyCurrentWinningCondition();
			flag = true;
		}
		if (flag && noWinningCondition == null)
		{
			Debug.Log("NoWinningCondition is active.");
			noWinningCondition = UnityEngine.Object.Instantiate(noWinningConditionPrefab);
			noWinningCondition.transform.SetParent(ingameUIController, worldPositionStays: false);
			noWinningCondition.Initialize(lobbyStateUI);
		}
		else if (flag && noWinningCondition != null)
		{
			noWinningCondition.TryInitializeRoundCube();
		}
	}

	private void CurrentWinConditionChanged(object sender, EventArgs args)
	{
		DestroyCurrentWinningCondition();
		CreateWinningCondition();
	}

	private void AvatarStateChanged(object state)
	{
		int num = (int)state;
		if ((num & 4) > 0)
		{
			displayTeam = MVGameControllerBase.Game.TeamManager.TeamCount() > 1;
			CurrentWinConditionChanged(this, null);
		}
		if ((num & 1) > 0 && displayTeam)
		{
			NotificationController.PushNotification(NotificationType.TeamNotification);
			displayTeam = false;
		}
	}

	private void OnWinningConditionFulfilled(IWinningCondition winningCondition)
	{
		if (currentWinningCondition != null)
		{
			currentWinningCondition.RoundEndReset();
		}
	}

	private void DestroyCurrentWinningCondition()
	{
		Debug.Log("Destroying current win condition logic.");
		if (currentWinningCondition != null)
		{
			currentWinningCondition.Clear();
			UnityEngine.Object.Destroy(currentWinningCondition.gameObject);
			currentWinningCondition = null;
		}
		if (noWinningCondition != null)
		{
			noWinningCondition.Clear();
			UnityEngine.Object.Destroy(noWinningCondition.gameObject);
			noWinningCondition = null;
		}
	}
}
