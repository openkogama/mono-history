using System;
using System.Collections;
using System.Collections.Generic;
using MV.Common;
using MV.WorldObject;
using UnityEngine;

public class MVGUIWinningConditionDebriefingView : UXViewScript
{
	private const string _debriefingTeamPrefab = "Prefabs/GUI/WinnerScreen/WinningConditionBriefings/DebriefingTeam";

	private const string _debriefingPlayerPrefab = "Prefabs/GUI/WinnerScreen/WinningConditionBriefings/DebriefingPlayer";

	private const string _debriefingNoWinnerPrefab = "Prefabs/GUI/WinnerScreen/WinningConditionBriefings/DebriefingNoWinner";

	private const string _winningConditionMaterialsFolder = "Materials/WinningConditionMaterials/";

	private MVGUIDebriefing debriefing;

	[SerializeField]
	public UXGroup group;

	[SerializeField]
	private Transform offset;

	private float fadeTime = 0.3f;

	public override void OnInitialize()
	{
		base.OnInitialize();
		UXGroup uXGroup = group;
		uXGroup.OnShowGroup = (UXGroup.OnGroupEventDelegate)Delegate.Combine(uXGroup.OnShowGroup, new UXGroup.OnGroupEventDelegate(OnShowGroup));
	}

	public void SetupDebriefing(string materialName, HighScores highScores, bool teamMode)
	{
		if (teamMode)
		{
			debriefing = SetupDebriefingTeam(materialName, highScores.GenerateTeamScores(), highScores.gameStatCounterType);
		}
		else
		{
			debriefing = SetupDebriefingPlayer(materialName, highScores.GenerateActorScores(), highScores.gameStatCounterType);
		}
		SetToParent(debriefing);
	}

	private void OnShowGroup()
	{
		offset.gameObject.SetActive(value: true);
		if (!(debriefing == null))
		{
			StartCoroutine(ShowDebriefingCoroutine());
		}
	}

	public void Clear()
	{
		offset.gameObject.SetActive(value: false);
		StopAllCoroutines();
		if (debriefing != null)
		{
			UnityEngine.Object.Destroy(debriefing.transform.gameObject);
			debriefing = null;
		}
		group.SetVisible(visible: false);
	}

	private MVGUIDebriefing SetupDebriefingPlayer(string materialName, List<ScoreActorEntry> scoreActorEntries, GameStatCounterType counterType)
	{
		if (scoreActorEntries.Count == 0)
		{
			Debug.Log("No winner");
			return SetupDebriefingNoWinner();
		}
		MVGUIDebriefingPlayer component = (UnityEngine.Object.Instantiate(Resources.Load("Prefabs/GUI/WinnerScreen/WinningConditionBriefings/DebriefingPlayer")) as GameObject).GetComponent<MVGUIDebriefingPlayer>();
		component.Init(scoreActorEntries[0].actorNumber);
		string winValue = FormatCount(counterType, scoreActorEntries[0].counter);
		component.SetWinValue(winValue);
		SetWinnerConditionMaterial(materialName, component);
		return component;
	}

	private MVGUIDebriefing SetupDebriefingTeam(string materialName, List<ScoreTeamEntry> scoreTeamEntries, GameStatCounterType counterType)
	{
		if (scoreTeamEntries.Count == 0)
		{
			Debug.Log("No winner");
			return SetupDebriefingNoWinner();
		}
		MVGUIDebriefingTeam component = (UnityEngine.Object.Instantiate(Resources.Load("Prefabs/GUI/WinnerScreen/WinningConditionBriefings/DebriefingTeam")) as GameObject).GetComponent<MVGUIDebriefingTeam>();
		component.SetTeam(scoreTeamEntries[0].team);
		string winValue = FormatCount(counterType, scoreTeamEntries[0].counter);
		component.SetWinValue(winValue);
		SetWinnerConditionMaterial(materialName, component);
		return component;
	}

	private MVGUIDebriefing SetupDebriefingNoWinner()
	{
		return (UnityEngine.Object.Instantiate(Resources.Load("Prefabs/GUI/WinnerScreen/WinningConditionBriefings/DebriefingNoWinner")) as GameObject).GetComponent<MVGUIDebriefingNoWinner>();
	}

	private IEnumerator ShowDebriefingCoroutine()
	{
		yield return StartCoroutine(pTween.To(fadeTime, 0f, 1f, (float t) =>
		{
			group.SetAlpha(t, string.Empty);
			debriefing.SetFadeInFadeOut(t);
		}));
		yield return StartCoroutine(WaitForFadeOut());
		yield return StartCoroutine(pTween.To(fadeTime, 1f, 0f, (float t) =>
		{
			group.SetAlpha(t, string.Empty);
			debriefing.SetFadeInFadeOut(t);
			if (t == 0f)
			{
				group.SetVisible(visible: false);
				Clear();
			}
		}));
	}

	private IEnumerator Wait(float wait)
	{
		yield return new WaitForSeconds(wait);
	}

	private IEnumerator WaitForFadeOut()
	{
		while (MVGameController.Game.NetworkGameStateListener.CurrentGameState != MVGameStateType.PrepareRound)
		{
			yield return 0;
		}
		while ((float)MVGameController.Game.NetworkGameStateListener.TimeLeftMS / 1000f - fadeTime > 0f)
		{
			yield return 0;
		}
	}

	private void SetToParent(MVGUIDebriefing mvguiDebriefingBase)
	{
		mvguiDebriefingBase.transform.parent = offset;
		mvguiDebriefingBase.transform.localPosition = new Vector3(0f, 0f, 0f);
		mvguiDebriefingBase.transform.localScale = Vector3.one;
	}

	private void SetWinnerConditionMaterial(string materialName, MVGUIDebriefing mvguiDebriefingBase)
	{
		Material winnerConditionIconMaterial = (Material)UnityEngine.Object.Instantiate(Resources.Load("Materials/WinningConditionMaterials/" + materialName));
		mvguiDebriefingBase.SetWinnerConditionIconMaterial(winnerConditionIconMaterial);
	}

	private static string FormatCount(GameStatCounterType statType, int count)
	{
		switch (statType)
		{
		case GameStatCounterType.Kill:
		case GameStatCounterType.OculusKill:
			return count.ToString();
		case GameStatCounterType.Collectible:
			return count.ToString();
		case GameStatCounterType.YUp:
		case GameStatCounterType.YDown:
			return string.Empty;
		default:
		{
			TimeSpan timeSpan = new TimeSpan(0, 0, 0, 0, count);
			return $"{timeSpan.Minutes:00}:{timeSpan.Seconds:00}";
		}
		}
	}

	private void Update()
	{
		Tests();
	}

	private void Tests()
	{
		if (MVInputWrapper.DebugGetKeyUp(KeyCode.B) && Application.isEditor)
		{
			debriefing = TestGetDebriefingPlayer();
			SetToParent(debriefing);
			View.Show();
		}
	}

	private MVGUIDebriefing TestGetDebriefingTeam()
	{
		List<ScoreTeamEntry> list = new List<ScoreTeamEntry>();
		list.Add(new ScoreTeamEntry(MVTeam.Yellow, 3));
		return SetupDebriefingTeam("Kill", list, GameStatCounterType.Kill);
	}

	private MVGUIDebriefing TestGetDebriefingPlayer()
	{
		List<ScoreActorEntry> list = new List<ScoreActorEntry>();
		list.Add(new ScoreActorEntry(1, 20));
		return SetupDebriefingPlayer("Kill", list, GameStatCounterType.Kill);
	}

	private MVGUIDebriefing TestGetDebriefingNoWinner()
	{
		return SetupDebriefingNoWinner();
	}
}
