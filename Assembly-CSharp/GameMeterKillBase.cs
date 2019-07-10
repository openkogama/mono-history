using UnityEngine;
using UnityEngine.UI;

public class GameMeterKillBase : GameMeterBase
{
	[SerializeField]
	protected Image killsBar;

	[SerializeField]
	protected Text killsText;

	public override GameMeterType GameMeterType => GameMeterType.Kills;

	public override void Initialize()
	{
		killsText.text = string.Empty;
	}

	public override void SetGameMeterVisibility()
	{
	}

	public override void UpdateValue()
	{
	}

	protected void SetCount(GameStatCounterType gameStatCounterType, int limit)
	{
		int count = GetCount(gameStatCounterType);
		string text = count + "/" + limit;
		killsText.text = text;
	}

	private static int GetCount(GameStatCounterType gameStatCounterType)
	{
		if (MVGameControllerBase.Game.TeamManager.TeamCount() > 1)
		{
			return MVGameControllerBase.Game.GameStatCounterManager.GetTeamCount(gameStatCounterType, MVGameControllerBase.Game.LocalPlayer.Team);
		}
		return MVGameControllerBase.Game.LocalPlayer.GetGameStat(gameStatCounterType);
	}

	public override void SetShowGameMeter(bool show)
	{
		killsBar.enabled = show;
		killsText.enabled = show;
	}

	protected void Hide()
	{
		gameObject.SetActive(value: false);
	}

	protected void Show()
	{
		gameObject.SetActive(value: true);
	}
}
