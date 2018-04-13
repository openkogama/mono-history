using UnityEngine;
using UnityEngine.UI;

public class OculusKillLimitWinningCondition : WinningConditionBase
{
	[SerializeField]
	private Image killImage;

	[SerializeField]
	private Text progress;

	[SerializeField]
	private ProgressBar progressBar;

	private int oculusKillLimit;

	protected override GameStatCounterType StatType => GameStatCounterType.OculusKill;

	public override void InitializeGameUI(RectTransform lobbyState)
	{
		int num = -1;
		OculusKillLimitClient singletonWinnerConditionByType = MVGameControllerBase.Game.WinningConditionManager.GetSingletonWinnerConditionByType<OculusKillLimitClient>();
		if (singletonWinnerConditionByType != null)
		{
			num = singletonWinnerConditionByType.Limit;
		}
		else
		{
			Debug.LogError("Failed to determine collectibles limit");
		}
		oculusKillLimit = num;
		float num2 = 0f;
		int num3 = 0;
		num3 = ((MVGameControllerBase.Game.TeamManager.GetTeamList().Count <= 1) ? MVGameControllerBase.Game.GameStatCounterManager.GetActorCount(GameStatCounterType.OculusKill, MVGameControllerBase.Game.LocalPlayer.Team, MVGameControllerBase.Game.LocalPlayer.ActorNr) : MVGameControllerBase.Game.GameStatCounterManager.GetTeamCount(GameStatCounterType.OculusKill, MVGameControllerBase.Game.LocalPlayer.Team));
		num2 = (float)num3 / (float)oculusKillLimit;
		progress.text = num3 + "/" + oculusKillLimit;
		progressBar.Progress = num2;
		base.InitializeGameUI(lobbyState);
	}

	public override void UpdateValue(int newValue)
	{
		int num = -1;
		OculusKillLimitClient singletonWinnerConditionByType = MVGameControllerBase.Game.WinningConditionManager.GetSingletonWinnerConditionByType<OculusKillLimitClient>();
		if (singletonWinnerConditionByType != null)
		{
			num = singletonWinnerConditionByType.Limit;
			if (num != oculusKillLimit)
			{
				oculusKillLimit = num;
			}
		}
		else
		{
			Debug.LogError("Failed to determine collectibles limit");
		}
		progress.text = newValue + "/" + oculusKillLimit;
		progressBar.Progress = (float)newValue / (float)oculusKillLimit;
	}

	public override void RoundEndReset()
	{
		progress.text = 0 + "/" + oculusKillLimit;
		progressBar.Progress = 0f / (float)oculusKillLimit;
		base.RoundEndReset();
	}
}
