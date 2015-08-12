using System;
using System.Collections.Generic;
using MV.Common;
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

	private HighScores report;

	private GameStatCounterType gameStatCounterType;

	private MVNetworkGameStateListener GameStateListener => MVGameController.Game.NetworkGameStateListener;

	private MVWorldObjectClientManager WOCM => MVGameController.WOCM;

	public void BuildWinnerDialog(HighScores report)
	{
		this.report = report;
	}

	public override void OnShowDialog()
	{
		base.OnShowDialog();
		TimeOutTexts.SetActive(value: false);
		AdventureRoot.SetActive(value: false);
		PodiumBG.gameObject.SetActive(value: false);
		SinglePodiumBG.gameObject.SetActive(value: false);
		players.gameObject.SetActive(value: false);
		Debug.Log(report);
		if (report.highScores.Count == 0)
		{
			string reasonText = TM._("Time's up");
			Debug.LogWarning("Out commented so report winning state  NoWinner is always assumed to be time out");
			BuildTimeOutDialog(reasonText);
		}
		else if (report.winningConditionPresentStyle == WinningConditionPresentStyle.OneWinner)
		{
			BuildSingleWinner();
			PlayFX();
		}
		else if (report.winningConditionPresentStyle == WinningConditionPresentStyle.MultipleWinners)
		{
			BuildSeveralWinners();
			PlayFX();
		}
	}

	private void PlayFX()
	{
		particleFXInstance = UnityEngine.Object.Instantiate(particleFXPrefab);
		particleFXInstance.transform.position = Camera.main.transform.position + new Vector3(0f, 0f, -5f);
		particleFXInstance.Emit(1);
		particleFXInstance.Play();
	}

	private void BuildTimeOutDialog(string reasonText)
	{
		TimeOutTexts.SetActive(value: true);
		noWinnerReasonText.Text = reasonText;
	}

	private void BuildSingleWinner()
	{
		SinglePodiumBG.gameObject.SetActive(value: true);
		List<KeyValuePair<string, int>> list = GenerateWinnerList();
		AddPodiumSeat(list[0].Key, list[0].Value, new Vector3(0f, -1f, 0f));
	}

	private List<KeyValuePair<string, int>> GenerateWinnerList()
	{
		gameStatCounterType = report.gameStatCounterType;
		List<KeyValuePair<string, int>> list = new List<KeyValuePair<string, int>>();
		if (report.presentAsTeamScore)
		{
			List<ScoreTeamEntry> list2 = report.GenerateTeamScores();
			foreach (ScoreTeamEntry item in list2)
			{
				list.Add(new KeyValuePair<string, int>(GetPodiumName(item), item.counter));
			}
		}
		else
		{
			Debug.Log(report);
			List<ScoreActorEntry> list3 = report.GenerateActorScores();
			foreach (ScoreActorEntry item2 in list3)
			{
				list.Add(new KeyValuePair<string, int>(GetPodiumName(item2), item2.counter));
			}
		}
		return list;
	}

	private void BuildSeveralWinners()
	{
		PodiumBG.gameObject.SetActive(value: true);
		List<KeyValuePair<string, int>> list = GenerateWinnerList();
		AddPodiumSeat(list[0].Key, list[0].Value, new Vector3(0f, 0f, 0f));
		if (list.Count >= 2)
		{
			AddPodiumSeat(list[1].Key, list[1].Value, new Vector3(-13.25f, 0.15f, 0f));
		}
		else
		{
			PodiumMedals[1].SetActive(value: false);
		}
		if (list.Count >= 3)
		{
			AddPodiumSeat(list[1].Key, list[1].Value, new Vector3(13.25f, 0.15f, 0f));
		}
		else
		{
			PodiumMedals[2].SetActive(value: false);
		}
		if (list.Count > 3)
		{
			BuildWinnerList(list.GetRange(3, list.Count - 3));
		}
	}

	private string GetPodiumName(IScoreEntry scoreEntry)
	{
		if (scoreEntry is ScoreTeamEntry)
		{
			return GetPodiumNameFromScoreTeamEntry((ScoreTeamEntry)scoreEntry);
		}
		if (scoreEntry is ScoreActorEntry)
		{
			return GetPodiumNameFromScoreActorEntry((ScoreActorEntry)scoreEntry);
		}
		throw new Exception("Unknown scoreEntry type");
	}

	private string GetPodiumNameFromScoreTeamEntry(ScoreTeamEntry scoreTeamEntry)
	{
		return string.Concat(scoreTeamEntry.team, " team");
	}

	public string GetPodiumNameFromScoreActorEntry(ScoreActorEntry scoreActorEntry)
	{
		MVPlayer mVPlayer = MVGameController.Game.Players[scoreActorEntry.actorNumber];
		return mVPlayer.Username;
	}

	private void AddPodiumSeat(string name, int winnerData, Vector3 position)
	{
		MVGUIPodiumSeat mVGUIPodiumSeat = UnityEngine.Object.Instantiate(PodiumSeatPrefab);
		mVGUIPodiumSeat.transform.parent = PodiumRoot;
		mVGUIPodiumSeat.transform.localScale = Vector3.one;
		mVGUIPodiumSeat.transform.localPosition = position;
		mVGUIPodiumSeat.BuildPodiumSeat(name, winnerData, gameStatCounterType);
	}

	private void BuildWinnerList(List<KeyValuePair<string, int>> winnerListNodes)
	{
		players.gameObject.SetActive(value: true);
		foreach (KeyValuePair<string, int> winnerListNode in winnerListNodes)
		{
			MVGUIWinnerLine mVGUIWinnerLine = UnityEngine.Object.Instantiate(WinnerLinePrefab);
			mVGUIWinnerLine.BuildLine(winnerListNodes.IndexOf(winnerListNode) + 1, winnerListNode.Value, winnerListNode.Key, report.gameStatCounterType);
			players.AddLine(mVGUIWinnerLine);
		}
	}

	public void Update()
	{
		if (GameStateListener.CurrentGameState == MVGameStateType.PrepareRound)
		{
			nextRoundTime.Text = "New round starting in " + (Mathf.Ceil(GameStateListener.TimeLeftMS / 1000) + 1f);
		}
		else
		{
			nextRoundTime.Text = string.Empty;
		}
		if (particleFXInstance != null && particleFXInstance.isPlaying && Camera.main != null)
		{
			particleFXInstance.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 5f;
		}
	}
}
