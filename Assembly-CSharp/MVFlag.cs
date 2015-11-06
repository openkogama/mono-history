using System;
using System.Collections.Generic;
using UnityEngine;

public class MVFlag : MVLogicObject
{
	private const string prefabPath = "Prefabs/FlagObject";

	private TriggerBoxEvents triggerBoxEvents;

	private bool initializedInWorld;

	private WorldObjectEnableController worldObjectEnableController;

	private GameCoinLogic gameCoinLogic;

	private Vector3 gameCoinDisplayObjectOffset = new Vector3(0f, 2.5f, 0f);

	public override Vector3 WorldPivot => transform.position;

	public MVFlag(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, "Prefabs/FlagObject", worldObjects)
	{
		triggerBoxEvents = gameObject.GetComponentInChildren<TriggerBoxEvents>();
		triggerBoxEvents.TriggerEnter += triggerBoxEvents_TriggerEnter;
		interactionFlags |= InteractionFlags.CanUseGameCoins;
		gameCoinLogic = new GameCoinLogic(gameObject, Data, gameCoinDisplayObjectOffset);
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
		worldObjectEnableController = gameObject.GetComponentInChildren<WorldObjectEnableController>();
	}

	public override void OnDataUpdate()
	{
		base.OnDataUpdate();
		gameCoinLogic.OnDataUpdate(Data);
	}

	protected override void OnUpdate()
	{
		base.OnUpdate();
		if (triggerBoxEvents.IsInTrigger && gameCoinLogic.PurchaseAmount > 0 && worldObjectEnableController.EnableState == EnableState.Enable && gameCoinLogic.ShowUseGUI())
		{
			DoCaptureFlag();
		}
	}

	private void triggerBoxEvents_TriggerEnter(object sender, TriggerEventArgs e)
	{
		if (worldObjectEnableController.EnableState == EnableState.Enable && gameCoinLogic.PurchaseAmount <= 0)
		{
			DoCaptureFlag();
		}
	}

	private void DoCaptureFlag()
	{
		MVGameControllerBase.Game.ReportCaptureFlag();
	}

	public override void Destroy()
	{
		triggerBoxEvents.TriggerEnter -= triggerBoxEvents_TriggerEnter;
		gameCoinLogic.OnDestroy(Data);
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
