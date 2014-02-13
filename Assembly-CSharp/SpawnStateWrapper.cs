using System;

public class SpawnStateWrapper : IUpdatecontrollerSubscriber
{
	private SpawnState prevSpawnState;

	private int takenTime;

	private int respawnInterval;

	private SpawnState spawnState;

	private Action<SpawnState> stateChangeCallback;

	public SpawnState SpawnState => spawnState;

	public int TakenTime
	{
		set
		{
			takenTime = value;
		}
	}

	public SpawnStateWrapper(int respawnInterval, int takenTime, Action<SpawnState> stateChangeCallback)
	{
		this.respawnInterval = respawnInterval;
		this.stateChangeCallback = stateChangeCallback;
		this.takenTime = takenTime;
		MVGameController.Instance.UpdateController.AddUpdateObject(this, UpdatePriority.UPDATEBUCKET_STANDARD);
	}

	public void UpdateControllerUpdate()
	{
		if (WaitForTicks.Diff(takenTime) > respawnInterval)
		{
			spawnState = SpawnState.Listening;
		}
		else
		{
			spawnState = SpawnState.Taken;
		}
		if (prevSpawnState != spawnState)
		{
			stateChangeCallback(spawnState);
		}
		prevSpawnState = spawnState;
	}

	public void UpdateControllerFixedUpdate()
	{
		throw new NotImplementedException();
	}

	public void Destroy()
	{
		MVGameController.Instance.UpdateController.RemoveObject(this);
	}
}
