using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

public abstract class MVWorldObjectSpawner(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects) : MVBlueprintBase(data, worldObjects)
{
	private UseInteractor useInteractor;

	protected SpawnStateWrapper spawnStateWrapper;

	protected int spawnWorldObjectID = -1;

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
		TriggerBoxEvents componentInChildren = gameObject.GetComponentInChildren<TriggerBoxEvents>();
		if (componentInChildren == null)
		{
			Debug.LogWarning("Did not find triggerBoxEvents");
			return;
		}
		useInteractor = new UseInteractor(Id, reset: true, componentInChildren.GetComponent<Collider>(), Use);
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

	protected abstract bool Use(int userWoID);
}
