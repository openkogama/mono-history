using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class ScoreBoardBase : MonoBehaviour
{
	[Serializable]
	protected class ScoreData
	{
		public Text ScoreText;

		public Text NameText;

		public Text PlacementText;

		public int Id;

		public int Score;

		public Image Background;
	}

	protected GameStatCounterType statType;

	[SerializeField]
	protected List<ScoreData> scoreBoardPlayerData;

	[SerializeField]
	private VerticalLayoutGroup layoutGroup;

	protected float backgroundAlpha;

	public abstract void OnStatsChange(int id, int scoreCount);

	public virtual void Initialize(GameStatCounterType statType)
	{
		this.statType = statType;
		MVNetworkGame game = MVGameControllerBase.Game;
		game.OnWinningConditionFulfilled = (Action<IWinningCondition>)Delegate.Combine(game.OnWinningConditionFulfilled, new Action<IWinningCondition>(OnWinningConditionFulfilled));
		layoutGroup.spacing = layoutGroup.spacing / 1440f * (float)Screen.height;
		if (scoreBoardPlayerData.Count > 0)
		{
			backgroundAlpha = scoreBoardPlayerData[0].Background.color.a;
		}
	}

	public void ChangeStatType(GameStatCounterType statType)
	{
		this.statType = statType;
		ReSortScoreBoard();
	}

	public virtual void ReSortScoreBoard()
	{
		ResetScoreBoard();
	}

	protected void OnWinningConditionFulfilled(IWinningCondition winningCondition)
	{
		for (int i = 0; i < scoreBoardPlayerData.Count; i++)
		{
			scoreBoardPlayerData[i].Score = 0;
			scoreBoardPlayerData[i].ScoreText.text = ScoreIntoString(0);
		}
	}

	protected void SortNewScore(string playerName, int id, int scoreCount)
	{
		if (!HandleAlreadyOnScoreBoard(id, scoreCount))
		{
			scoreBoardPlayerData[scoreBoardPlayerData.Count - 1].Score = scoreCount;
			scoreBoardPlayerData[scoreBoardPlayerData.Count - 1].NameText.text = playerName;
			scoreBoardPlayerData[scoreBoardPlayerData.Count - 1].Background.color = GetBackgroundColor(id);
			scoreBoardPlayerData[scoreBoardPlayerData.Count - 1].Id = id;
			scoreBoardPlayerData[scoreBoardPlayerData.Count - 1].ScoreText.text = ScoreIntoString(scoreCount);
			if (id < 0)
			{
				scoreBoardPlayerData[scoreBoardPlayerData.Count - 1].ScoreText.text = string.Empty;
			}
		}
		List<ScoreData> list = new List<ScoreData>();
		for (int i = 0; i < scoreBoardPlayerData.Count; i++)
		{
			bool flag = false;
			for (int j = 0; j < list.Count; j++)
			{
				if (IsNewScoreBetter(scoreBoardPlayerData[i].Score, list[j].Score, scoreBoardPlayerData[i].Id, list[j].Id))
				{
					list.Insert(j, scoreBoardPlayerData[i]);
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				list.Add(scoreBoardPlayerData[i]);
			}
		}
		scoreBoardPlayerData = list;
		for (int k = 0; k < scoreBoardPlayerData.Count; k++)
		{
			SetPlacementTextForIndex(k);
			scoreBoardPlayerData[k].Background.transform.SetAsLastSibling();
		}
		HandleParticipantListChanged();
	}

	private bool HandleAlreadyOnScoreBoard(int id, int newScore)
	{
		for (int i = 0; i < scoreBoardPlayerData.Count; i++)
		{
			if (id != scoreBoardPlayerData[i].Id)
			{
				continue;
			}
			if (IsNewScoreBetter(newScore, scoreBoardPlayerData[i].Score))
			{
				scoreBoardPlayerData[i].Score = newScore;
				scoreBoardPlayerData[i].ScoreText.text = ScoreIntoString(newScore);
				if (id < 0)
				{
					scoreBoardPlayerData[i].ScoreText.text = string.Empty;
				}
			}
			return true;
		}
		return false;
	}

	protected bool IsNewScoreBetter(int newScore, int oldScore)
	{
		return WinningConditionControl.IsNewScoreBetter(newScore, oldScore, statType);
	}

	protected virtual bool IsNewScoreBetter(int newScore, int oldScore, int newId, int oldId)
	{
		return IsNewScoreBetter(newScore, oldScore);
	}

	protected virtual string ScoreIntoString(int score)
	{
		return WinningConditionControl.MakeIntoScoreText(score, statType);
	}

	protected void OnDestroy()
	{
		UnSubscribeToCallbacks();
	}

	protected virtual void UnSubscribeToCallbacks()
	{
		if (MVGameControllerBase.Game != null)
		{
			MVNetworkGame game = MVGameControllerBase.Game;
			game.OnWinningConditionFulfilled = (Action<IWinningCondition>)Delegate.Remove(game.OnWinningConditionFulfilled, new Action<IWinningCondition>(OnWinningConditionFulfilled));
		}
	}

	protected virtual Color GetBackgroundColor(int id)
	{
		Color color = Styles.GetColor(ColorStyle.OffWhiteTransparent);
		color.a = backgroundAlpha;
		return color;
	}

	protected abstract void HandleParticipantListChanged();

	private void SetPlacementTextForIndex(int index)
	{
		scoreBoardPlayerData[index].PlacementText.text = (index + 1).ToString();
	}

	protected void ResetScoreBoard()
	{
		for (int i = 0; i < scoreBoardPlayerData.Count; i++)
		{
			scoreBoardPlayerData[i].Score = -1;
			scoreBoardPlayerData[i].Id = -1;
		}
	}
}
