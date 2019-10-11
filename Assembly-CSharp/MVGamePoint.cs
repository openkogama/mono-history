using System;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class MVGamePoint : MVGamePointRewardLogicObject
{
	public enum GamePointClientState
	{
		Visible,
		PickedUp,
		ReShowing,
		Invisible
	}

	private GamePointClientState state;

	private bool isVisible = true;

	private bool canRespawn;

	private int respawnTime;

	private float pickUpTime;

	private const string respawnString = "respawnTime";

	private MVGamePointObject gamePointObject;

	protected override int GamePointRewardAmount => 1;

	public override bool HasInputConnector => false;

	public override bool HasOutputConnector => false;

	public override MVWorldObjectDocumentationType DocumentationType => MVWorldObjectDocumentationType.GamePoint;

	public MVGamePoint(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, PrefabPool.Instance.GamePointPrefab, worldObjects)
	{
		Create();
		interactionFlags |= InteractionFlags.CanRespawn;
		UpdateCanRespawn(Data);
	}

	private void Create()
	{
		gamePointObject = (MVGamePointObject)component;
		if (gamePointObject.TriggerBoxEvents != null)
		{
			gamePointObject.TriggerBoxEvents.TriggerEnter += Enter;
		}
		SetVisible();
		HandleStandaloneDisabling();
		UpdateCanRespawn(Data);
	}

	public override void Initialize()
	{
		base.Initialize();
		SetupCulling(gamePointObject.VisualObject);
		MVNetworkGame game = MVGameControllerBase.Game;
		game.OnWinningConditionFulfilled = (Action<IWinningCondition>)Delegate.Combine(game.OnWinningConditionFulfilled, new Action<IWinningCondition>(OnWinningConditionFulfilled));
	}

	public override void Destroy()
	{
		if (MVGameControllerBase.Game != null)
		{
			MVNetworkGame game = MVGameControllerBase.Game;
			game.OnWinningConditionFulfilled = (Action<IWinningCondition>)Delegate.Remove(game.OnWinningConditionFulfilled, new Action<IWinningCondition>(OnWinningConditionFulfilled));
		}
		base.Destroy();
	}

	private void Enter(object sender, TriggerEventArgs e)
	{
		if (state == GamePointClientState.Visible)
		{
			state = GamePointClientState.PickedUp;
			isVisible = false;
			gamePointObject.PickupItem.GreyOut();
			gamePointObject.PickupItem.gameObject.SetActive(canRespawn);
			MVGameControllerBase.OperationRequests.TriggerBoxEnter(Id, e.instigatorWOID);
			FakeGamePointGainEffectManager.FakeGainEffect(1);
			pickUpTime = Time.time;
		}
	}

	private void SetVisible()
	{
		if (!isVisible)
		{
			gamePointObject.PickupItem.GreyIn();
			gamePointObject.PickupItem.gameObject.SetActive(value: true);
			isVisible = true;
		}
		state = GamePointClientState.Visible;
	}

	public override void Reset()
	{
		SetVisible();
		HandleStandaloneDisabling();
	}

	private void OnWinningConditionFulfilled(IWinningCondition winningCondition)
	{
		Reset();
	}

	private void HandleStandaloneDisabling()
	{
		bool flag = false;
		flag = true;
		bool flag2 = MVGameControllerBase.GameSessionData.gameMode == MVGameMode.Edit;
		if (flag && !flag2)
		{
			Disable();
		}
	}

	private void Disable()
	{
		state = GamePointClientState.PickedUp;
		isVisible = false;
		gamePointObject.PickupItem.GreyOut();
		canRespawn = false;
	}

	private void UpdateCanRespawn(Dictionary<object, object> newData)
	{
		canRespawn = newData.ContainsKey("respawnTime");
		if (canRespawn)
		{
			respawnTime = (int)newData["respawnTime"];
		}
	}

	public override void OnDataUpdate()
	{
		base.OnDataUpdate();
		UpdateCanRespawn(Data);
	}

	public override void PartialUpdateWOData(Dictionary<object, object> woData)
	{
		base.PartialUpdateWOData(woData);
		UpdateCanRespawn(woData);
	}

	protected override void OnUpdate()
	{
		base.OnUpdate();
		if (canRespawn && state == GamePointClientState.PickedUp && Time.time > pickUpTime + (float)respawnTime)
		{
			Reset();
		}
	}
}
