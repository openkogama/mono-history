using UnityEngine;

public abstract class GameMeterAndroidBase : MonoBehaviour
{
	protected float inActiveAlpha;

	protected bool meterActive = true;

	public abstract GameMeterType GameMeterType { get; }

	public bool MeterActive
	{
		get
		{
			return meterActive;
		}
		set
		{
			meterActive = value;
			SetShowGameMeter(value);
		}
	}

	public abstract void SetShowGameMeter(bool show);

	public abstract void UpdateShowGameMeter();
}
