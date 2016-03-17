using UnityEngine;
using UnityEngine.UI;

public class GameMeterAndroidKillBase : GameMeterAndroidBase
{
	[SerializeField]
	protected Image killsBar;

	[SerializeField]
	protected Text killsText;

	public override GameMeterType GameMeterType => GameMeterType.Kills;

	private void Start()
	{
		killsText.text = string.Empty;
	}

	public override void UpdateShowGameMeter()
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
		killsBar.CrossFadeAlpha(inActiveAlpha, 0.5f, ignoreTimeScale: false);
		killsText.CrossFadeAlpha(inActiveAlpha, 0.5f, ignoreTimeScale: false);
	}

	protected void Show()
	{
		gameObject.SetActive(value: true);
		killsBar.CrossFadeAlpha(1f, 0.5f, ignoreTimeScale: false);
		killsText.CrossFadeAlpha(1f, 0.5f, ignoreTimeScale: false);
	}
}
