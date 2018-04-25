using MV.WorldObject;
using UnityEngine;

public abstract class WinningConditionBase : MonoBehaviour
{
	[SerializeField]
	private RoundTimer roundTimerPrefab;

	private RoundTimer roundTimer;

	protected abstract GameStatCounterType StatType { get; }

	public virtual bool WinningConditionAbleToBeFulfilled => true;

	public abstract void UpdateValue(int newValue);

	public void UpdateStats(int actorNumber, GameStatCounterType counterType, int scoreCount)
	{
		if (StatType == counterType)
		{
			UpdateProgressNotification(actorNumber, counterType, scoreCount);
		}
	}

	protected void UpdateProgressNotification(int actorNumber, GameStatCounterType counterType, int scoreCount)
	{
	}

	public virtual bool CanWinningConditionBeFullfilledForTeam(MVTeam team)
	{
		return true;
	}

	public virtual void Clear()
	{
		if (roundTimer != null)
		{
			Object.Destroy(roundTimer.gameObject);
			roundTimer = null;
		}
	}

	public virtual void RoundEndReset()
	{
		if (roundTimer != null)
		{
			roundTimer.ResetOnRoundEnd();
		}
	}

	public virtual void InitializeGameUI(RectTransform lobbyState)
	{
		WorldObjectClientRef<MVRoundCube> singletonWorldObjectRef = MVGameControllerBase.WOCM.GetSingletonWorldObjectRef<MVRoundCube>();
		if (singletonWorldObjectRef != null)
		{
			CreateRoundTimer(singletonWorldObjectRef);
		}
	}

	private void CreateRoundTimer(WorldObjectClientRef<MVRoundCube> roundCube)
	{
		roundTimer = Object.Instantiate(roundTimerPrefab);
		roundTimer.transform.SetParent(transform.parent, worldPositionStays: false);
		roundTimer.transform.SetAsFirstSibling();
		roundTimer.Initialize(roundCube);
		transform.SetAsFirstSibling();
	}
}
