using UnityEngine;
using UnityEngine.UI;

public class GameMeterAndroidRoundTime : GameMeterAndroidBase
{
	[SerializeField]
	private Image roundTimeBar;

	[SerializeField]
	private Text roundTime;

	private MVRoundCube roundCube;

	public override GameMeterType GameMeterType => GameMeterType.Time;

	private void Start()
	{
		SetGameMeterVisibility();
	}

	public override void SetGameMeterVisibility()
	{
		roundCube = MVGameControllerBase.WOCM.GetSingletonWorldObject<MVRoundCube>();
		if (roundCube != null)
		{
			if (!gameObject.activeSelf)
			{
				Show();
			}
			int timeLeft = GetTimeLeft(roundCube);
			if (timeLeft > 0)
			{
				int num = (int)((float)timeLeft / 1000f) + 1;
				roundTime.text = $"{num / 60:00}:{num % 60:00}";
			}
		}
		else
		{
			Hide();
			roundTime.text = string.Empty;
		}
	}

	public override void UpdateValue()
	{
	}

	private void Update()
	{
		if (roundCube == null)
		{
			Hide();
			return;
		}
		int timeLeft = GetTimeLeft(roundCube);
		if (timeLeft > 0)
		{
			int num = (int)((float)timeLeft / 1000f) + 1;
			roundTime.text = $"{num / 60:00}:{num % 60:00}";
		}
	}

	public override void SetShowGameMeter(bool show)
	{
		roundTimeBar.enabled = show;
		roundTime.enabled = show;
	}

	private void Hide()
	{
		gameObject.SetActive(value: false);
	}

	private void Show()
	{
		gameObject.SetActive(value: true);
	}

	private int GetTimeLeft(MVRoundCube roundCube)
	{
		int num = roundCube.DurationInMilliseconds - (MVGameControllerBase.Game.ServerTimeInMilliSeconds - MVGameControllerBase.Game.NetworkGameStateListener.StartTime);
		if (num < 0)
		{
			num = 0;
		}
		return num;
	}
}
