using System.Collections;
using System.Collections.Generic;
using MV.Common;
using UnityEngine;

public class MVCheckpoint : MVLogicObject
{
	private const string prefabPath = "Prefabs/CheckpointObject";

	private TriggerBoxEvents triggerBoxEvents;

	public MVCheckpoint(Hashtable data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, "Prefabs/CheckpointObject", worldObjects)
	{
		triggerBoxEvents = gameObject.GetComponentInChildren<TriggerBoxEvents>();
		triggerBoxEvents.TriggerEnter += triggerBoxEvents_TriggerEnter;
	}

	public override Vector3 GetClosestGridPoint(float gridSize, Vector3 position)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		return SharedCubeFunctions.GetClosestGridPoint(position, gameObject.transform.rotation, gridSize, Vector3.one * 2f);
	}

	private void triggerBoxEvents_TriggerEnter(object sender, TriggerEventArgs e)
	{
		if (MVGameController.Instance.Game.LocalPlayer.GetCheckpoint() == null || MVGameController.Instance.Game.LocalPlayer.GetCheckpoint().Id != Id)
		{
			Debug.Log((object)"CHECKPOINT REACHED!");
			MVGameController.Instance.Game.LocalPlayer.SetCheckpoint(id);
			Hashtable hashtable = new Hashtable();
			hashtable.Add((byte)0, MVGameController.Instance.Game.LocalPlayer.ActorNr);
			MVGameController.Instance.Game.PostGameMsg(MVGameMsgType.CheckpointReached, hashtable);
		}
	}

	public override void Destroy()
	{
		triggerBoxEvents.TriggerEnter -= triggerBoxEvents_TriggerEnter;
		base.Destroy();
	}
}
