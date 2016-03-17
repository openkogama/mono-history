using MV.Common;
using UnityEngine;

public class JoinStatusInfo : MonoBehaviour
{
	private MVEventCodes prevGameState = MVEventCodes.AddObjectLink;

	private MVConnState prevConnState;

	private void Update()
	{
		if (MVGameControllerBase.Game == null)
		{
			return;
		}
		MVConnState connState = MVGameControllerBase.Game.ConnState;
		if (connState != prevConnState)
		{
			prevConnState = connState;
			MVGameControllerBase.PostGameMsg(MVGameMsgType.JoinFlowStatus, TM._("Connection") + ": " + LocalizedEnums._(connState));
		}
		while (JoinUIUpdater.JoinEventCodes.Count > 0)
		{
			MVEventCodes mVEventCodes = JoinUIUpdater.JoinEventCodes.Dequeue();
			if (mVEventCodes != prevGameState)
			{
				string message = TM._("Game") + ": " + LocalizedEnums._(mVEventCodes);
				StatHatWrapper.Count(mVEventCodes.ToString(), 1);
				prevGameState = mVEventCodes;
				MVGameControllerBase.PostGameMsg(MVGameMsgType.JoinFlowStatus, message);
			}
		}
		if (MVGameControllerBase.JoinState == MVJoinState.Playing)
		{
			MVGameControllerBase.PostGameMsg(MVGameMsgType.JoinFlowStatus, "\n\n" + MVGameControllerBase.VersionNumber.ToString() + "\n\n");
			if (MVGameControllerBase.IsTouristSession)
			{
				string message2 = TM._("\n\n<WASD> Move\n<Space> Jump\n<K> Respawn\n<Left Mouse> Fire Weapon\n<Q> Drop currently equipped weapon");
				MVGameControllerBase.PostGameMsg(MVGameMsgType.AdminMsg, message2);
			}
			else if (MVGameControllerBase.GameMode != MVGameMode.CharacterEditor)
			{
				string message3 = TM._("Type /h for help\nPress <Enter> or <T> to chat");
				MVGameControllerBase.PostGameMsg(MVGameMsgType.AdminMsg, message3);
			}
			Object.Destroy(this);
		}
	}
}
