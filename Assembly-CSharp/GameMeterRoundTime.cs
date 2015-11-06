using UnityEngine;

public class GameMeterRoundTime : GameMeterBase
{
	[SerializeField]
	private UXText score;

	public override GameMeterType GameMeterType => GameMeterType.Time;

	private void Update()
	{
		MVRoundCube singletonWorldObject = MVGameControllerBase.WOCM.GetSingletonWorldObject<MVRoundCube>();
		if (singletonWorldObject != null)
		{
			MeterActive = true;
			int timeLeft = GetTimeLeft(singletonWorldObject);
			if (timeLeft > 0)
			{
				int num = (int)((float)timeLeft / 1000f) + 1;
				score.Text = $"{num / 60:00}:{num % 60:00}";
			}
		}
		else
		{
			MeterActive = false;
			score.text = string.Empty;
		}
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
