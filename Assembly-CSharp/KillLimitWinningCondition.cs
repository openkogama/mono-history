using UnityEngine;
using UnityEngine.UI;

public class KillLimitWinningCondition : WinningConditionBase
{
	[SerializeField]
	private Image killImage;

	[SerializeField]
	private Text progress;

	[SerializeField]
	private ProgressBar progressBar;

	private int killLimit;

	protected override GameStatCounterType StatType => GameStatCounterType.Kill;

	public override void InitializeGameUI(RectTransform lobbyState)
	{
		int num = -1;
		KillLimitClient singletonWinnerConditionByType = MVGameControllerBase.Game.WinningConditionManager.GetSingletonWinnerConditionByType<KillLimitClient>();
		if (singletonWinnerConditionByType != null)
		{
			num = singletonWinnerConditionByType.Limit;
		}
		else
		{
			Debug.LogError("Failed to determine collectibles limit");
		}
		killLimit = num;
		float num2 = 0f;
		int num3 = 0;
		num3 = ((MVGameControllerBase.Game.TeamManager.GetTeamList().Count <= 1) ? MVGameControllerBase.Game.GameStatCounterManager.GetActorCount(GameStatCounterType.Kill, MVGameControllerBase.Game.LocalPlayer.Team, MVGameControllerBase.Game.LocalPlayer.ActorNr) : MVGameControllerBase.Game.GameStatCounterManager.GetTeamCount(GameStatCounterType.Kill, MVGameControllerBase.Game.LocalPlayer.Team));
		num2 = (float)num3 / (float)killLimit;
		progress.text = num3 + "/" + killLimit;
		progressBar.Progress = num2;
		base.InitializeGameUI(lobbyState);
	}

	public override void UpdateValue(int newValue)
	{
		int num = -1;
		KillLimitClient singletonWinnerConditionByType = MVGameControllerBase.Game.WinningConditionManager.GetSingletonWinnerConditionByType<KillLimitClient>();
		if (singletonWinnerConditionByType != null)
		{
			num = singletonWinnerConditionByType.Limit;
			if (num != killLimit)
			{
				killLimit = num;
			}
		}
		else
		{
			Debug.LogError("Failed to determine collectibles limit");
		}
		progress.text = newValue + "/" + killLimit;
		progressBar.Progress = (float)newValue / (float)killLimit;
	}

	public override void RoundEndReset()
	{
		progress.text = 0 + "/" + killLimit;
		progressBar.Progress = 0f / (float)killLimit;
		base.RoundEndReset();
	}
}
