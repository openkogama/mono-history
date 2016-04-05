using System.Collections.Generic;
using UnityEngine;

public class MVCheckpoint : MVLogicObject
{
	private MVCheckpointObject checkpointObject;

	private UseInteractor useInteractor;

	private static readonly UseGUIResult purchaseOptions = UseGUIResult.CanAfford | UseGUIResult.CannotAfford;

	public override Vector3 WorldPivot => transform.position;

	public MVCheckpoint(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.MVCheckpointPrefab, worldObjects)
	{
		checkpointObject = (MVCheckpointObject)component;
		checkpointObject.TriggerBoxEvents.TriggerEnter += triggerBoxEvents_TriggerEnter;
		interactionFlags |= InteractionFlags.CanUseGameCoins;
		interactionFlags |= InteractionFlags.CanUseLevel;
		useInteractor = new UseInteractor(Id, gameObject, reset: false, checkpointObject.TriggerBoxEvents.Collider, DoReachCheckpoint);
		checkpointObject.TriggerBoxEvents.TriggerEnter += useInteractor.triggerBoxEvents_TriggerEnter;
		checkpointObject.TriggerBoxEvents.TriggerExit += useInteractor.triggerBoxEvents_TriggerExit;
		GameCoinLogic useRequirement = new GameCoinLogic(gameObject, hasUseButtonWhenFree: false);
		useInteractor.AddRequirement(useRequirement);
		LevelBasedUseRequirement useRequirement2 = new LevelBasedUseRequirement(gameObject, hasUseButtonWhenFree: false);
		useInteractor.AddRequirement(useRequirement2);
	}

	public override Vector3 GetClosestGridPoint(float gridSize, Vector3 position)
	{
		Vector3 vector = Vector3.one * 2f;
		vector.z = 1f;
		vector.x = 1f;
		return SharedCubeFunctions.GetClosestGridPoint(position, gameObject.transform.rotation, gridSize, vector);
	}

	public override void Initialize()
	{
		base.Initialize();
		useInteractor.UpdateData(Data);
	}

	public override void OnDataUpdate()
	{
		useInteractor.UpdateData(Data);
		base.OnDataUpdate();
	}

	private void triggerBoxEvents_TriggerEnter(object sender, TriggerEventArgs e)
	{
		if ((useInteractor.EvaluateRequirementsUsability() & purchaseOptions) == 0)
		{
			DoReachCheckpoint(MVGameControllerBase.WOCM.AvatarLocal.Id);
		}
	}

	public override void Destroy()
	{
		checkpointObject.TriggerBoxEvents.TriggerEnter -= triggerBoxEvents_TriggerEnter;
		checkpointObject.TriggerBoxEvents.TriggerEnter -= useInteractor.triggerBoxEvents_TriggerEnter;
		checkpointObject.TriggerBoxEvents.TriggerExit -= useInteractor.triggerBoxEvents_TriggerExit;
		useInteractor.OnDestroy(Data);
		base.Destroy();
	}

	private bool DoReachCheckpoint(int instigatorId)
	{
		if (MVGameControllerBase.Game.LocalPlayer.GetCheckpoint() == null || MVGameControllerBase.Game.LocalPlayer.GetCheckpoint().Id != Id)
		{
			MVGameControllerBase.Game.LocalPlayer.SetCheckpoint(id);
			if (checkpointObject.Animation != null)
			{
				checkpointObject.Animation.Play("CheckpointReach");
			}
			return true;
		}
		return false;
	}
}
