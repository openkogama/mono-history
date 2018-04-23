using UnityEngine;
using UnityEngine.UI;

public class CollectiblesWinningCondition : WinningConditionBase
{
	[SerializeField]
	private Image starImage;

	[SerializeField]
	private Text progress;

	[SerializeField]
	private ProgressBar progressBar;

	private int amountOfStars;

	protected override GameStatCounterType StatType => GameStatCounterType.Collectible;

	public override void InitializeGameUI(RectTransform lobbyState)
	{
		int num = -1;
		AllCollectiblesCollectedClient singletonWinnerConditionByType = MVGameControllerBase.Game.WinningConditionManager.GetSingletonWinnerConditionByType<AllCollectiblesCollectedClient>();
		if (singletonWinnerConditionByType != null)
		{
			num = singletonWinnerConditionByType.Limit;
		}
		else
		{
			Debug.LogError("Failed to determine collectibles limit");
		}
		amountOfStars = num;
		float num2 = 0f;
		int num3 = 0;
		num3 = ((MVGameControllerBase.Game.TeamManager.GetTeamList().Count <= 1) ? MVGameControllerBase.Game.GameStatCounterManager.GetActorCount(GameStatCounterType.Collectible, MVGameControllerBase.Game.LocalPlayer.Team, MVGameControllerBase.Game.LocalPlayer.ActorNr) : MVGameControllerBase.Game.GameStatCounterManager.GetTeamCount(GameStatCounterType.Collectible, MVGameControllerBase.Game.LocalPlayer.Team));
		num2 = (float)num3 / (float)amountOfStars;
		progress.text = num3 + "/" + amountOfStars;
		progressBar.Progress = num2;
		base.InitializeGameUI(lobbyState);
	}

	public override void UpdateValue(int newValue)
	{
		int num = -1;
		AllCollectiblesCollectedClient singletonWinnerConditionByType = MVGameControllerBase.Game.WinningConditionManager.GetSingletonWinnerConditionByType<AllCollectiblesCollectedClient>();
		if (singletonWinnerConditionByType != null)
		{
			num = singletonWinnerConditionByType.Limit;
			if (num != amountOfStars)
			{
				amountOfStars = num;
			}
		}
		else
		{
			Debug.LogError("Failed to determine collectibles limit");
		}
		progress.text = newValue + "/" + amountOfStars;
		progressBar.Progress = (float)newValue / (float)amountOfStars;
	}

	public override void RoundEndReset()
	{
		progress.text = 0 + "/" + amountOfStars;
		progressBar.Progress = 0f;
		base.RoundEndReset();
	}
}
