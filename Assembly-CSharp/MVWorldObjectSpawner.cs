using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

public abstract class MVWorldObjectSpawner : MVBlueprintBase
{
	protected UseInteractor useInteractor;

	protected SpawnStateWrapper spawnStateWrapper;

	protected int spawnWorldObjectID = -1;

	protected TriggerBoxEvents triggerBoxEvents;

	protected MVWorldObjectSpawner(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, worldObjects)
	{
	}

	protected MVWorldObjectSpawner(Dictionary<object, object> data, ObjectPrefab prefabObject, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, prefabObject, worldObjects)
	{
	}

	public override void Initialize()
	{
		base.Initialize();
		int respawnInterval = (int)Data["RespawnInterval"];
		int takenTime = (ObscuredInt)RunTimeData.GetObscuredType("UseTime");
		spawnStateWrapper = new SpawnStateWrapper(respawnInterval, takenTime, OnSpawnStateChange);
		if (!childIdMap.ContainsKey("spawnWorldObjectID"))
		{
			Debug.LogError("No spawnWorldObject");
			return;
		}
		spawnWorldObjectID = (int)childIdMap["spawnWorldObjectID"];
		triggerBoxEvents = gameObject.GetComponentInChildren<TriggerBoxEvents>();
		if (triggerBoxEvents == null)
		{
			Debug.LogWarning("Did not find triggerBoxEvents");
		}
	}

	public override void Destroy()
	{
		base.Destroy();
		if (spawnStateWrapper != null)
		{
			spawnStateWrapper.Destroy();
		}
	}

	public virtual void Take(int takeTime)
	{
		RunTimeData.SetObscuredType("UseTime", (ObscuredInt)takeTime);
		spawnStateWrapper.TakenTime = takeTime;
	}

	protected virtual void OnSpawnStateChange(SpawnState spawnState)
	{
		if (spawnState == SpawnState.None)
		{
			Debug.LogError("SpawnState is none");
		}
		switch (spawnState)
		{
		case SpawnState.Listening:
			Debug.Log("Switch to listening");
			break;
		case SpawnState.Taken:
			Debug.Log("Switch to taken");
			break;
		}
	}

	protected abstract bool CheckCanUse(MVInteractableBase userWoID);

	protected abstract bool Use(int userWoID);
}
