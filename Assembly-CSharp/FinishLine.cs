using System;
using System.Collections.Generic;
using UnityEngine;

public class FinishLine : MVLogicObject
{
	private const float captureCooldown = 5f;

	private TriggerBoxEvents triggerBoxEvents;

	private bool initializedInWorld;

	private WorldObjectEnableController worldObjectEnableController;

	private UseInteractor useInteractor;

	private float lastCaptureTime;

	private static readonly UseGUIResult purchaseOptions = UseGUIResult.CanAfford | UseGUIResult.CannotAfford;

	private FinishLineObject finishLineObject;

	public override MVWorldObjectDocumentationType DocumentationType => MVWorldObjectDocumentationType.FinishLine;

	public override Vector3 WorldPivot => transform.position;

	public FinishLine(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.FinishLinePrefab, worldObjects)
	{
		finishLineObject = (FinishLineObject)component;
		triggerBoxEvents = finishLineObject.TriggerBoxEvents;
		triggerBoxEvents.TriggerEnter += triggerBoxEvents_TriggerEnter;
		interactionFlags |= InteractionFlags.CanUseTeam;
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
		SetupUseInteractor();
		base.Initialize();
		if (MVGameControllerBase.Game.WinningConditionManager.GetSingletonWinnerConditionByType<FinishLineReachedClient>() == null)
		{
			MVGameControllerBase.Game.WinningConditionManager.CreateWinnerCondition<FinishLineReachedClient>(new object[0]);
		}
		initializedInWorld = true;
		useInteractor.UpdateData(Data);
		worldObjectEnableController = gameObject.GetComponentInChildren<WorldObjectEnableController>();
		SetupCulling(finishLineObject.VisualObject);
	}

	private void SetupUseInteractor()
	{
		useInteractor = new UseInteractor(this, finishLineObject.useInteractionRotator, reset: false, triggerBoxEvents.Collider, DoReachFinishLine);
		triggerBoxEvents.TriggerEnter += useInteractor.triggerBoxEvents_TriggerEnter;
		triggerBoxEvents.TriggerExit += useInteractor.triggerBoxEvents_TriggerExit;
		GameCoinLogic useRequirement = new GameCoinLogic(finishLineObject.useInteractionRotator, hasUseButtonWhenFree: false);
		useInteractor.AddRequirement(useRequirement);
		TeamRequirement useRequirement2 = new TeamRequirement(finishLineObject.TintObject, hasUseButtonWhenFree: false);
		useInteractor.AddRequirement(useRequirement2);
	}

	public override void OnDataUpdate()
	{
		base.OnDataUpdate();
		useInteractor.UpdateData(Data);
	}

	private void triggerBoxEvents_TriggerEnter(object sender, TriggerEventArgs e)
	{
		if (worldObjectEnableController.EnableState == EnableState.Enable && (useInteractor.EvaluateRequirementsUsability() & purchaseOptions) == 0)
		{
			DoReachFinishLine(MVGameControllerBase.WOCM.AvatarLocal.Id);
		}
	}

	private bool DoReachFinishLine(int instigator)
	{
		if (Time.time < lastCaptureTime + 5f)
		{
			return false;
		}
		lastCaptureTime = Time.time;
		MVGameControllerBase.OperationRequests.ReportReachedFinishLine();
		FlagDebriefingControl.StartFlagDebriefing();
		return true;
	}

	public override void Destroy()
	{
		triggerBoxEvents.TriggerEnter -= triggerBoxEvents_TriggerEnter;
		if (useInteractor != null)
		{
			triggerBoxEvents.TriggerEnter -= useInteractor.triggerBoxEvents_TriggerEnter;
			triggerBoxEvents.TriggerExit -= useInteractor.triggerBoxEvents_TriggerExit;
			useInteractor.OnDestroy(Data);
			useInteractor = null;
		}
		base.Destroy();
		if (initializedInWorld && MVGameControllerBase.Game.World.WorldObjectClientManager.GetWorldObjectsByType(WorldObjectType).Count == 0)
		{
			FinishLineReachedClient singletonWinnerConditionByType = MVGameControllerBase.Game.WinningConditionManager.GetSingletonWinnerConditionByType<FinishLineReachedClient>();
			if (singletonWinnerConditionByType == null)
			{
				throw new Exception("Could not find FlagReached singleton");
			}
			MVGameControllerBase.Game.WinningConditionManager.RemoveWinnerCondition(singletonWinnerConditionByType.ID);
		}
	}
}
