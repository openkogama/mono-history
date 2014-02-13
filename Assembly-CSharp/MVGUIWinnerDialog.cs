using System;
using System.Collections.Generic;
using System.Linq;
using Localize;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class MVGUIWinnerDialog : UXCustomDialogBox
{
	public UXPlane PodiumBG;

	public UXPlane SinglePodiumBG;

	public MVGUIPodiumSeat PodiumSeatPrefab;

	public Transform PodiumRoot;

	public GameObject[] PodiumMedals;

	public UXScrollableBox players;

	public MVGUIWinnerLine WinnerLinePrefab;

	public UXText nextRoundTime;

	public GameObject TimeOutTexts;

	public UXText noWinnerReasonText;

	public GameObject AdventureRoot;

	public UXTextButton PlayAgainButton;

	public ParticleSystem particleFXPrefab;

	private ParticleSystem particleFXInstance;

	private WinnerReportBase report;

	private MVNetworkGameStateListener GameStateListener => MVGameController.Instance.Game.NetworkGameStateListener;

	private MVWorldObjectClientManager WOCM => MVGameController.Instance.WOCM;

	public void BuildWinnerDialog(WinnerReportBase report)
	{
		this.report = report;
	}

	public override void OnShowDialog()
	{
		base.OnShowDialog();
		TimeOutTexts.SetActiveRecursively(false);
		AdventureRoot.SetActiveRecursively(false);
		((Component)PodiumBG).gameObject.SetActiveRecursively(false);
		((Component)SinglePodiumBG).gameObject.SetActiveRecursively(false);
		((Component)players).gameObject.SetActiveRecursively(false);
		if (report.winningState == MVWinningState.NoWinner)
		{
			string reasonText = Localization.Instance.GetText(TextSlotIndex.TimesUp);
			MVGameStateReason lastReason = MVGameController.Instance.Game.NetworkGameStateListener.LastReason;
			int lastInstigatorActorNr = MVGameController.Instance.Game.NetworkGameStateListener.LastInstigatorActorNr;
			if (lastReason == MVGameStateReason.FlagReached)
			{
				reasonText = Localization.Instance.GetText(TextSlotIndex.NoWinnerFlag) + MVGameController.Instance.Game.Players[lastInstigatorActorNr].Username;
			}
			if (lastReason == MVGameStateReason.AllCollectiblesFound)
			{
				reasonText = Localization.Instance.GetText(TextSlotIndex.NoWinnerCollectibles) + MVGameController.Instance.Game.Players[lastInstigatorActorNr].Username;
			}
			BuildTimeOutDialog(reasonText);
		}
		else if (report.winningState == MVWinningState.OneWinner)
		{
			BuildSingleWinner();
			PlayFX();
		}
		else if (report.winningState == MVWinningState.OrderedListOfWinners || report.winningState == MVWinningState.SeveralWinners)
		{
			BuildSeveralWinners();
			PlayFX();
		}
	}

	private void PlayFX()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected Obj, but got Unknown
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		particleFXInstance = (ParticleSystem)Object.Instantiate((Object)(object)particleFXPrefab);
		((Component)particleFXInstance).transform.position = ((Component)Camera.main).transform.position + new Vector3(0f, 0f, -5f);
		particleFXInstance.Emit(1);
		particleFXInstance.Play();
	}

	private void FakeWinningReport()
	{
		if (report.winnerList.Count != 0)
		{
			WinnerListNode winnerListNode = report.winnerList[0];
			List<WinnerListNode> list = new List<WinnerListNode>();
			for (int i = 0; i < 20; i++)
			{
				list.Add(new WinnerListNode(winnerListNode.actorNr, Random.Range(1, 30)));
			}
			report.winnerList = list;
			report.winningType = MVWinningCondition.ReachTheFlagFirst;
			report.winningState = MVWinningState.OneWinner;
		}
	}

	private void BuildAdventureDialog()
	{
		AdventureRoot.SetActiveRecursively(true);
		UXTextButton playAgainButton = PlayAgainButton;
		playAgainButton.OnClick = (UXBaseButton.OnClickDelegate)Delegate.Combine(playAgainButton.OnClick, new UXBaseButton.OnClickDelegate(PlayAgainAdventure));
	}

	private void PlayAgainAdventure()
	{
		WOCM.GetWorldObjectsByType(WorldObjectType.CollectibleItem).ForEach((MVWorldObjectClient wo) =>
		{
			wo.Initialize();
		});
		MVGameController.Instance.WOCM.AvatarLocal.Respawn();
		UXUtils.FindGUIObjectOfType<UXDialogFactory>().CloseDialog();
	}

	private void BuildDeathMatchDialog()
	{
		Dictionary<int, int> dictionary = new Dictionary<int, int>();
		if (report.isTeamGame)
		{
			foreach (MVTeam team in MVGameController.Instance.Game.TeamManager.GetTeamList())
			{
				dictionary.Add((int)team, MVGameController.Instance.Game.TeamManager.GetScore(team));
			}
		}
		else
		{
			foreach (KeyValuePair<int, MVPlayer> player in MVGameController.Instance.Game.Players)
			{
				dictionary.Add(player.Key, player.Value.Score);
			}
		}
		List<KeyValuePair<int, int>> list = dictionary.ToList();
		list.Sort((KeyValuePair<int, int> firstPair, KeyValuePair<int, int> nextPair) => nextPair.Value.CompareTo(firstPair.Value));
		List<WinnerListNode> list2 = new List<WinnerListNode>();
		foreach (KeyValuePair<int, int> item in list)
		{
			list2.Add(new WinnerListNode(item.Key, item.Value));
		}
		report.winnerList = list2;
		BuildSeveralWinners();
	}

	private void BuildTimeOutDialog(string reasonText)
	{
		TimeOutTexts.SetActiveRecursively(true);
		noWinnerReasonText.Text = reasonText;
	}

	private void BuildSingleWinner()
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		((Component)SinglePodiumBG).gameObject.SetActiveRecursively(true);
		AddPodiumSeat(report.winnerList[0], report.winningType, new Vector3(0f, -1f, 0f));
	}

	private void BuildSeveralWinners()
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		((Component)PodiumBG).gameObject.SetActiveRecursively(true);
		AddPodiumSeat(report.winnerList[0], report.winningType, new Vector3(0f, 0f, 0f));
		if (report.winnerList.Count >= 2)
		{
			AddPodiumSeat(report.winnerList[1], report.winningType, new Vector3(-13.25f, 0.15f, 0f));
		}
		else
		{
			PodiumMedals[1].SetActiveRecursively(false);
		}
		if (report.winnerList.Count >= 3)
		{
			AddPodiumSeat(report.winnerList[2], report.winningType, new Vector3(13.25f, 0.15f, 0f));
		}
		else
		{
			PodiumMedals[2].SetActiveRecursively(false);
		}
		if (report.winnerList.Count > 3)
		{
			BuildWinnerList(report.winnerList.GetRange(3, report.winnerList.Count - 3));
		}
	}

	private void AddPodiumSeat(WinnerListNode node, MVWinningCondition winningType, Vector3 position)
	{
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		string empty = string.Empty;
		int num = 0;
		if (report.isTeamGame)
		{
			MVTeam actorNr = (MVTeam)node.actorNr;
			empty = string.Concat(actorNr, " team");
		}
		else
		{
			MVPlayer mVPlayer = MVGameController.Instance.Game.Players[node.actorNr];
			empty = mVPlayer.Username;
		}
		num = node.data;
		AddPodiumSeat(empty, num, winningType, position);
	}

	private void AddPodiumSeat(string name, int winnerData, MVWinningCondition winningType, Vector3 position)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		MVGUIPodiumSeat mVGUIPodiumSeat = Object.Instantiate((Object)(object)PodiumSeatPrefab) as MVGUIPodiumSeat;
		((Component)mVGUIPodiumSeat).transform.parent = PodiumRoot;
		((Component)mVGUIPodiumSeat).transform.localScale = Vector3.one;
		((Component)mVGUIPodiumSeat).transform.localPosition = position;
		mVGUIPodiumSeat.BuildPodiumSeat(name, winnerData, winningType);
	}

	private void BuildWinnerList(List<WinnerListNode> winnerNodes)
	{
		((Component)players).gameObject.SetActiveRecursively(true);
		foreach (WinnerListNode winnerNode in winnerNodes)
		{
			MVGUIWinnerLine mVGUIWinnerLine = Object.Instantiate((Object)(object)WinnerLinePrefab) as MVGUIWinnerLine;
			mVGUIWinnerLine.BuildLine(report.winnerList.IndexOf(winnerNode) + 1, winnerNode, report.winningType, report.isTeamGame);
			players.AddLine(mVGUIWinnerLine);
		}
	}

	public void Update()
	{
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		if (GameStateListener.CurrentGameState == MVGameStateType.PrepareRound)
		{
			nextRoundTime.Text = "New round starting in " + (Mathf.Ceil((float)(GameStateListener.TimeLeftMS / 1000)) + 1f);
		}
		else
		{
			nextRoundTime.Text = string.Empty;
		}
		if ((Object)(object)particleFXInstance != (Object)null && particleFXInstance.isPlaying && (Object)(object)Camera.main != (Object)null)
		{
			((Component)particleFXInstance).transform.position = ((Component)Camera.main).transform.position + ((Component)Camera.main).transform.forward * 5f;
		}
	}
}
