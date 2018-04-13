using UnityEngine;

public class NoWinningCondition : MonoBehaviour
{
	[SerializeField]
	private RoundTimer roundTimerPrefab;

	private RoundTimer roundTimer;

	public void Initialize(RectTransform lobbyStateUI)
	{
		TryInitializeRoundCube();
	}

	public void TryInitializeRoundCube()
	{
		WorldObjectClientRef<MVRoundCube> singletonWorldObjectRef = MVGameControllerBase.WOCM.GetSingletonWorldObjectRef<MVRoundCube>();
		if (singletonWorldObjectRef != null)
		{
			CreateRoundTimer(singletonWorldObjectRef);
		}
	}

	public virtual void RoundEndReset()
	{
		if (roundTimer != null)
		{
			roundTimer.ResetOnRoundEnd();
		}
	}

	public void Clear()
	{
		if (roundTimer != null)
		{
			Object.Destroy(roundTimer.gameObject);
			roundTimer = null;
		}
	}

	private void CreateRoundTimer(WorldObjectClientRef<MVRoundCube> roundCube)
	{
		roundTimer = Object.Instantiate(roundTimerPrefab);
		roundTimer.transform.SetParent(transform.parent, worldPositionStays: false);
		roundTimer.transform.SetAsFirstSibling();
		roundTimer.Initialize(roundCube);
	}
}
