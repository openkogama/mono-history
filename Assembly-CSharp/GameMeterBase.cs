using UnityEngine;

public abstract class GameMeterBase : MonoBehaviour
{
	[SerializeField]
	public UXGroup uxGroup;

	protected float inActiveAlpha = 0.3f;

	protected bool meterActive = true;

	public abstract GameMeterType GameMeterType { get; }

	public virtual bool MeterActive
	{
		get
		{
			return meterActive;
		}
		set
		{
			if (meterActive != value)
			{
				meterActive = value;
				if (meterActive)
				{
					uxGroup.SetAlpha(1f, string.Empty);
				}
				else
				{
					uxGroup.SetAlpha(inActiveAlpha, string.Empty);
				}
			}
		}
	}
}
