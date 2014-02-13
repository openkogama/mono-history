using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MVWorldObjectSpawner(Hashtable data, Dictionary<int, MVWorldObjectClient> worldObjects) : MVBlueprintBase(data, worldObjects)
{
	private UseInteractor useInteractor;

	protected SpawnStateWrapper spawnStateWrapper;

	protected int spawnWorldObjectID = -1;

	public override void Initialize()
	{
		base.Initialize();
		int respawnInterval = (int)Data["RespawnInterval"];
		int takenTime = (int)RunTimeData["UseTime"];
		spawnStateWrapper = new SpawnStateWrapper(respawnInterval, takenTime, OnSpawnStateChange);
		if (!childIdMap.Contains("spawnWorldObjectID"))
		{
			Debug.LogError((object)"No spawnWorldObject");
			return;
		}
		spawnWorldObjectID = (int)childIdMap["spawnWorldObjectID"];
		TriggerBoxEvents componentInChildren = gameObject.GetComponentInChildren<TriggerBoxEvents>();
		if ((Object)(object)componentInChildren == (Object)null)
		{
			Debug.LogWarning((object)"Did not find triggerBoxEvents");
			return;
		}
		useInteractor = new UseInteractor(Id, reset: true, ((Component)componentInChildren).collider, Use);
		componentInChildren.TriggerEnterOverride += useInteractor.triggerBoxEvents_TriggerEnter;
		componentInChildren.TriggerExitOverride += useInteractor.triggerBoxEvents_TriggerExit;
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
		RunTimeData["UseTime"] = takeTime;
		spawnStateWrapper.TakenTime = takeTime;
	}

	protected virtual void OnSpawnStateChange(SpawnState spawnState)
	{
		if (spawnState == SpawnState.None)
		{
			Debug.LogError((object)"SpawnState is none");
		}
		switch (spawnState)
		{
		case SpawnState.Listening:
			Debug.Log((object)"Switch to listening");
			break;
		case SpawnState.Taken:
			Debug.Log((object)"Switch to taken");
			break;
		}
	}

	protected abstract bool Use(int userWoID);
}
