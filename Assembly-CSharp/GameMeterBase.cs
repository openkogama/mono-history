using System.Collections.Generic;
using GameMeterVisuals;
using UnityEngine;

public abstract class GameMeterBase : MonoBehaviour
{
	[SerializeField]
	protected List<GameMeterVisualEffect> gameMeterVisualEffects = new List<GameMeterVisualEffect>();

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

	public abstract void UpdateValue();

	public abstract void SetGameMeterVisibility();
}
