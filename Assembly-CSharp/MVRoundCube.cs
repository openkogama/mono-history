using System.Collections;
using System.Collections.Generic;
using MV.Common;
using MV.WorldObject;

public class MVRoundCube : MVLogicObject
{
	private const string prefabPath = "Prefabs/RoundCubeObject";

	public override bool HasInputConnector => false;

	public override bool HasOutputConnector => true;

	public MVRoundCube(Hashtable data, Dictionary<int, MVWorldObjectClient> worldObjects)
		: base(data, "Prefabs/RoundCubeObject", worldObjects)
	{
		interactionFlags |= InteractionFlags.HasSettings;
		MVGameController.Instance.Game.NetworkGameStateListener.OnGameStateChanged += NetworkGameStateListener_OnGameStateChanged;
	}

	public override bool IsSingletonObject()
	{
		return true;
	}

	public override void Destroy()
	{
		MVGameController.Instance.Game.NetworkGameStateListener.OnGameStateChanged -= NetworkGameStateListener_OnGameStateChanged;
		base.Destroy();
	}

	private void NetworkGameStateListener_OnGameStateChanged(object sender, GameStateChangeEventArgs e)
	{
		MVNetworkGameStateListener networkGameStateListener = MVGameController.Instance.Game.NetworkGameStateListener;
		if (networkGameStateListener.CurrentGameState == MVGameStateType.RoundEnded)
		{
			foreach (Link outputLinkRef in OutputLinkRefs)
			{
				outputLinkRef.isSet = true;
			}
			return;
		}
		foreach (Link outputLinkRef2 in OutputLinkRefs)
		{
			outputLinkRef2.isSet = false;
		}
	}
}
