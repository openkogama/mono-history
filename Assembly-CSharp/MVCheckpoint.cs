using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class MVCheckpoint : MVLogicObject
{
	private MVCheckpointObject checkpointObject;

	private UseInteractor useInteractor;

	private const UseGUIResult purchaseOptions = UseGUIResult.CanAfford | UseGUIResult.CannotAfford;

	private bool playingAnimation;

	protected override bool HasVisualsInPlaymode => true;

	public override MVWorldObjectDocumentationType DocumentationType => MVWorldObjectDocumentationType.Checkpoint;

	public override Vector3 WorldPivot => transform.position;

	public MVCheckpoint(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.MVCheckpointPrefab, worldObjects)
	{
		checkpointObject = (MVCheckpointObject)component;
		checkpointObject.TriggerBoxEvents.TriggerEnter += triggerBoxEvents_TriggerEnter;
		interactionFlags |= InteractionFlags.CanUseGameCoins;
		interactionFlags |= InteractionFlags.CanUseLevel;
	}

	public override Vector3 GetClosestGridPoint(float gridSize, Vector3 position)
	{
		Vector3 vector = Vector3.one * 2f;
		vector.z = 1f;
		vector.x = 1f;
		return SharedCubeFunctions.GetClosestGridPoint(position, gameObject.transform.rotation, gridSize, vector);
	}

	protected override void OnUpdate()
	{
		base.OnUpdate();
		if (playingAnimation && !checkpointObject.VisualObject.activeInHierarchy)
		{
			checkpointObject.Animation.Rewind();
			checkpointObject.Animation.Play();
			checkpointObject.Animation.Sample();
			checkpointObject.Animation.Stop();
			playingAnimation = false;
		}
	}

	public override void Initialize()
	{
		SetupUseInteractor();
		base.Initialize();
		useInteractor.UpdateData(Data);
		SetupCulling(checkpointObject.VisualObject);
	}

	private void SetupUseInteractor()
	{
		useInteractor = new UseInteractor(this, checkpointObject.useInteractionRotator, reset: false, checkpointObject.TriggerBoxEvents.Collider, DoReachCheckpoint);
		checkpointObject.TriggerBoxEvents.TriggerEnter += useInteractor.triggerBoxEvents_TriggerEnter;
		checkpointObject.TriggerBoxEvents.TriggerExit += useInteractor.triggerBoxEvents_TriggerExit;
		GameCoinLogic useRequirement = new GameCoinLogic(checkpointObject.useInteractionRotator, hasUseButtonWhenFree: false);
		useInteractor.AddRequirement(useRequirement);
		LevelBasedUseRequirement useRequirement2 = new LevelBasedUseRequirement(checkpointObject.useInteractionRotator, hasUseButtonWhenFree: false);
		useInteractor.AddRequirement(useRequirement2);
	}

	public override void OnDataUpdate()
	{
		useInteractor.UpdateData(Data);
		base.OnDataUpdate();
	}

	private void triggerBoxEvents_TriggerEnter(object sender, TriggerEventArgs e)
	{
		if ((useInteractor.EvaluateRequirementsUsability() & (UseGUIResult.CanAfford | UseGUIResult.CannotAfford)) == 0)
		{
			DoReachCheckpoint(e.instigatorWOID);
		}
	}

	public override void Destroy()
	{
		checkpointObject.TriggerBoxEvents.TriggerEnter -= triggerBoxEvents_TriggerEnter;
		if (useInteractor != null)
		{
			checkpointObject.TriggerBoxEvents.TriggerEnter -= useInteractor.triggerBoxEvents_TriggerEnter;
			checkpointObject.TriggerBoxEvents.TriggerExit -= useInteractor.triggerBoxEvents_TriggerExit;
			useInteractor.OnDestroy(Data);
			useInteractor = null;
		}
		base.Destroy();
	}

	private bool DoReachCheckpoint(int instigatorId)
	{
		if (MVGameControllerBase.Game.LocalPlayer.GetCheckpoint() == null || MVGameControllerBase.Game.LocalPlayer.GetCheckpoint().Id != Id)
		{
			MVGameControllerBase.Game.LocalPlayer.SetCheckpoint(id);
			DoHeal(instigatorId);
			if (checkpointObject.Animation != null)
			{
				checkpointObject.Animation.Play("CheckpointReach");
				playingAnimation = true;
			}
			return true;
		}
		return false;
	}

	private void DoHeal(int instigator)
	{
		MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(instigator);
		if (worldObjectClient != null)
		{
			MVEquipable mVEquipable = worldObjectClient.GameObject.GetComponent<MVEquipable>();
			if (!(mVEquipable == null))
			{
				mVEquipable.Equip(AvatarItemType.Health, AvatarEquipableType.Modifier, null);
			}
		}
	}
}
