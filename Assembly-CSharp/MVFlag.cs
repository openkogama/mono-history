using System;
using System.Collections.Generic;
using UnityEngine;

public class MVFlag : MVLogicObject
{
	private TriggerBoxEvents triggerBoxEvents;

	private bool initializedInWorld;

	private WorldObjectEnableController worldObjectEnableController;

	private UseInteractor useInteractor;

	private static readonly UseGUIResult purchaseOptions = UseGUIResult.CanAfford | UseGUIResult.CannotAfford;

	public override Vector3 WorldPivot => transform.position;

	public MVFlag(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.MVFlagPrefab, worldObjects)
	{
		triggerBoxEvents = gameObject.GetComponentInChildren<TriggerBoxEvents>();
		triggerBoxEvents.TriggerEnter += triggerBoxEvents_TriggerEnter;
		interactionFlags |= InteractionFlags.CanUseGameCoins;
		useInteractor = new UseInteractor(Id, gameObject, reset: false, triggerBoxEvents.Collider, DoCaptureFlag, null, 3f);
		triggerBoxEvents.TriggerEnter += useInteractor.triggerBoxEvents_TriggerEnter;
		triggerBoxEvents.TriggerExit += useInteractor.triggerBoxEvents_TriggerExit;
		GameCoinLogic useRequirement = new GameCoinLogic(gameObject, hasUseButtonWhenFree: false);
		useInteractor.AddRequirement(useRequirement);
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
		if (MVGameControllerBase.Game.WinningConditionManager.GetSingletonWinnerConditionByType<FlagReachedClient>() == null)
		{
			MVGameControllerBase.Game.WinningConditionManager.CreateWinnerCondition<FlagReachedClient>(new object[0]);
		}
		initializedInWorld = true;
		useInteractor.UpdateData(Data);
		worldObjectEnableController = gameObject.GetComponentInChildren<WorldObjectEnableController>();
	}

	public override void OnDataUpdate()
	{
		base.OnDataUpdate();
		useInteractor.UpdateData(Data);
	}

	protected override void OnUpdate()
	{
		base.OnUpdate();
	}

	private void triggerBoxEvents_TriggerEnter(object sender, TriggerEventArgs e)
	{
		if (worldObjectEnableController.EnableState == EnableState.Enable && (useInteractor.EvaluateRequirementsUsability() & purchaseOptions) == 0)
		{
			DoCaptureFlag(MVGameControllerBase.WOCM.AvatarLocal.Id);
		}
	}

	private bool DoCaptureFlag(int instigator)
	{
		MVGameControllerBase.Game.ReportCaptureFlag();
		return true;
	}

	public override void Destroy()
	{
		triggerBoxEvents.TriggerEnter -= triggerBoxEvents_TriggerEnter;
		triggerBoxEvents.TriggerEnter -= useInteractor.triggerBoxEvents_TriggerEnter;
		triggerBoxEvents.TriggerExit -= useInteractor.triggerBoxEvents_TriggerExit;
		useInteractor.OnDestroy(Data);
		base.Destroy();
		if (initializedInWorld && MVGameControllerBase.Game.World.WorldObjectClientManager.GetWorldObjectsByType(WorldObjectType).Count == 0)
		{
			FlagReachedClient singletonWinnerConditionByType = MVGameControllerBase.Game.WinningConditionManager.GetSingletonWinnerConditionByType<FlagReachedClient>();
			if (singletonWinnerConditionByType == null)
			{
				throw new Exception("Could not find FlagReached singleton");
			}
			MVGameControllerBase.Game.WinningConditionManager.RemoveWinnerCondition(singletonWinnerConditionByType.ID);
		}
	}
}
