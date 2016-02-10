using System.Collections.Generic;
using UnityEngine;

public class MVCheckpoint : MVLogicObject
{
	private TriggerBoxEvents triggerBoxEvents;

	private Animation animation;

	private UseInteractor useInteractor;

	private static readonly UseGUIResult purchaseOptions = UseGUIResult.CanAfford | UseGUIResult.CannotAfford;

	public override Vector3 WorldPivot => transform.position;

	public MVCheckpoint(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.MVCheckpointPrefab, worldObjects)
	{
		triggerBoxEvents = gameObject.GetComponentInChildren<TriggerBoxEvents>();
		triggerBoxEvents.TriggerEnter += triggerBoxEvents_TriggerEnter;
		animation = gameObject.GetComponentInChildren<Animation>();
		interactionFlags |= InteractionFlags.CanUseGameCoins;
		interactionFlags |= InteractionFlags.CanUseLevel;
		useInteractor = new UseInteractor(Id, gameObject, reset: false, triggerBoxEvents.Collider, DoReachCheckpoint);
		triggerBoxEvents.TriggerEnter += useInteractor.triggerBoxEvents_TriggerEnter;
		triggerBoxEvents.TriggerExit += useInteractor.triggerBoxEvents_TriggerExit;
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
		triggerBoxEvents.TriggerEnter -= triggerBoxEvents_TriggerEnter;
		triggerBoxEvents.TriggerEnter -= useInteractor.triggerBoxEvents_TriggerEnter;
		triggerBoxEvents.TriggerExit -= useInteractor.triggerBoxEvents_TriggerExit;
		useInteractor.OnDestroy(Data);
		base.Destroy();
	}

	private bool DoReachCheckpoint(int instigatorId)
	{
		if (MVGameControllerBase.Game.LocalPlayer.GetCheckpoint() == null || MVGameControllerBase.Game.LocalPlayer.GetCheckpoint().Id != Id)
		{
			MVGameControllerBase.Game.LocalPlayer.SetCheckpoint(id);
			if (animation != null)
			{
				animation.Play("CheckpointReach");
			}
			return true;
		}
		return false;
	}
}
