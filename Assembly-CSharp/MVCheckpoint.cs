using System.Collections.Generic;
using UnityEngine;

public class MVCheckpoint : MVLogicObject
{
	private const string prefabPath = "Prefabs/CheckpointObject";

	private TriggerBoxEvents triggerBoxEvents;

	private Animation animation;

	private GameCoinLogic gameCoinLogic;

	public MVCheckpoint(Dictionary<object, object> data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, "Prefabs/CheckpointObject", worldObjects)
	{
		triggerBoxEvents = gameObject.GetComponentInChildren<TriggerBoxEvents>();
		triggerBoxEvents.TriggerEnter += triggerBoxEvents_TriggerEnter;
		animation = gameObject.GetComponentInChildren<Animation>();
		interactionFlags |= InteractionFlags.CanUseGameCoins;
		gameCoinLogic = new GameCoinLogic(gameObject, Data);
	}

	public override Vector3 GetClosestGridPoint(float gridSize, Vector3 position)
	{
		return SharedCubeFunctions.GetClosestGridPoint(position, gameObject.transform.rotation, gridSize, Vector3.one * 2f);
	}

	protected override void OnUpdate()
	{
		base.OnUpdate();
		if (triggerBoxEvents.IsInTrigger && CanReachCheckpoint() && gameCoinLogic.PurchaseAmount > 0 && gameCoinLogic.ShowUseGUI())
		{
			DoReachCheckpoint();
		}
	}

	public override void OnDataUpdate()
	{
		gameCoinLogic.OnDataUpdate(Data);
		base.OnDataUpdate();
	}

	private void triggerBoxEvents_TriggerEnter(object sender, TriggerEventArgs e)
	{
		if (CanReachCheckpoint() && gameCoinLogic.PurchaseAmount <= 0)
		{
			DoReachCheckpoint();
		}
	}

	public override void Destroy()
	{
		triggerBoxEvents.TriggerEnter -= triggerBoxEvents_TriggerEnter;
		gameCoinLogic.OnDestroy(Data);
		base.Destroy();
	}

	private bool CanReachCheckpoint()
	{
		return MVGameController.Game.LocalPlayer.GetCheckpoint() == null || MVGameController.Game.LocalPlayer.GetCheckpoint().Id != Id;
	}

	private void DoReachCheckpoint()
	{
		MVGameController.Game.LocalPlayer.SetCheckpoint(id);
		if (animation != null)
		{
			animation.Play("CheckpointReach");
		}
	}
}
