using System;
using System.Collections.Generic;
using UnityEngine;

public class TimeAttackFlag : MVGamePointRewardLogicObject
{
	private TriggerBoxEvents triggerBoxEvents;

	private bool initializedInWorld;

	private WorldObjectEnableController worldObjectEnableController;

	private UseInteractor useInteractor;

	private float lastCaptureTime;

	private const float captureCooldown = 5f;

	private bool isTimeAttackDebriefingOn;

	private const UseGUIResult purchaseOptions = UseGUIResult.CanAfford | UseGUIResult.CannotAfford;

	private TimeAttackFlagObject timeAttackFlagObject;

	public override MVWorldObjectDocumentationType DocumentationType => MVWorldObjectDocumentationType.TimeAttackFlag;

	public override Vector3 WorldPivot => transform.position;

	public TimeAttackFlag(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.TimeAttackFlagPrefab, worldObjects)
	{
		timeAttackFlagObject = (TimeAttackFlagObject)component;
		triggerBoxEvents = timeAttackFlagObject.TriggerBoxEvents;
		triggerBoxEvents.TriggerEnter += triggerBoxEvents_TriggerEnter;
		interactionFlags |= InteractionFlags.CanUseTeam;
		interactionFlags |= InteractionFlags.CanEarnGamePoints;
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
		if (MVGameControllerBase.Game.WinningConditionManager.GetSingletonWinnerConditionByType<TimeAttackFlagReachedClient>() == null)
		{
			MVGameControllerBase.Game.WinningConditionManager.CreateWinnerCondition<TimeAttackFlagReachedClient>(new object[0]);
		}
		initializedInWorld = true;
		useInteractor.UpdateData(Data);
		worldObjectEnableController = gameObject.GetComponentInChildren<WorldObjectEnableController>();
		SetupCulling(timeAttackFlagObject.VisualObject);
		if (MVGameControllerBase.WOCM.AvatarLocal == null)
		{
			MVPlayerContainer mVPlayerContainer = MVGameControllerBase.Game.MVPlayerContainer;
			mVPlayerContainer.OnLocalPlayerReady = (Action)Delegate.Combine(mVPlayerContainer.OnLocalPlayerReady, new Action(LateInitialize));
		}
		else
		{
			InitializeCallbacks();
		}
	}

	private void LateInitialize()
	{
		InitializeCallbacks();
		MVPlayerContainer mVPlayerContainer = MVGameControllerBase.Game.MVPlayerContainer;
		mVPlayerContainer.OnLocalPlayerReady = (Action)Delegate.Remove(mVPlayerContainer.OnLocalPlayerReady, new Action(Initialize));
	}

	private void InitializeCallbacks()
	{
		FlagDebriefingControl.OnFlagDebriefing = (Action<int>)Delegate.Combine(FlagDebriefingControl.OnFlagDebriefing, new Action<int>(OnStartFlagDebriefing));
		FlagDebriefingControl.OnFlagDebriefingEnd = (Action)Delegate.Combine(FlagDebriefingControl.OnFlagDebriefingEnd, new Action(OnEndFlagDebriefing));
	}

	private void SetupUseInteractor()
	{
		useInteractor = new UseInteractor(this, timeAttackFlagObject.useInteractionRotator, reset: false, triggerBoxEvents.Collider, DoReachTimeAttackFlag);
		triggerBoxEvents.TriggerEnter += useInteractor.triggerBoxEvents_TriggerEnter;
		triggerBoxEvents.TriggerExit += useInteractor.triggerBoxEvents_TriggerExit;
		GameCoinLogic useRequirement = new GameCoinLogic(timeAttackFlagObject.useInteractionRotator, hasUseButtonWhenFree: false);
		useInteractor.AddRequirement(useRequirement);
		TeamRequirement useRequirement2 = new TeamRequirement(timeAttackFlagObject.TintObject, hasUseButtonWhenFree: false);
		useInteractor.AddRequirement(useRequirement2);
	}

	public override void OnDataUpdate()
	{
		base.OnDataUpdate();
		useInteractor.UpdateData(Data);
	}

	private void triggerBoxEvents_TriggerEnter(object sender, TriggerEventArgs e)
	{
		if (worldObjectEnableController.EnableState == EnableState.Enable && (useInteractor.EvaluateRequirementsUsability() & (UseGUIResult.CanAfford | UseGUIResult.CannotAfford)) == 0)
		{
			DoReachTimeAttackFlag(MVGameControllerBase.WOCM.AvatarLocal.Id);
		}
	}

	private bool DoReachTimeAttackFlag(int instigator)
	{
		if (isTimeAttackDebriefingOn || Time.time < lastCaptureTime + 5f)
		{
			return false;
		}
		lastCaptureTime = Time.time;
		int captureTime = Mathf.FloorToInt((Time.time - FlagDebriefingControl.RunStartTime) * 1000f);
		MVGameControllerBase.OperationRequests.ReportReachedTimeAttackFlag(captureTime, Id);
		FlagDebriefingControl.StartFlagDebriefing(captureTime);
		return true;
	}

	public void OnStartFlagDebriefing(int captureTime)
	{
		isTimeAttackDebriefingOn = true;
	}

	public void OnEndFlagDebriefing()
	{
		isTimeAttackDebriefingOn = false;
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
			TimeAttackFlagReachedClient singletonWinnerConditionByType = MVGameControllerBase.Game.WinningConditionManager.GetSingletonWinnerConditionByType<TimeAttackFlagReachedClient>();
			if (singletonWinnerConditionByType == null)
			{
				throw new Exception("Could not find FlagReached singleton");
			}
			MVGameControllerBase.Game.WinningConditionManager.RemoveWinnerCondition(singletonWinnerConditionByType.ID);
		}
	}
}
