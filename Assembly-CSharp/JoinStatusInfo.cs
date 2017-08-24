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
			MVGameControllerBase.PostGameMsg(MVGameMsgType.JoinFlowStatus, "\n\n" + MVGameControllerBase.KoGaMaSettings.VersionString + "\n\n");
			if (MVGameControllerBase.IsTouristSession)
			{
				MVGameControllerBase.PostGameMsg(MVGameMsgType.AdminMsg, SendMessageControl.CreateHelpTxt());
			}
			else if (MVGameControllerBase.GameMode != MVGameMode.CharacterEditor)
			{
				string message2 = TM._("Type /h for help\nPress <Enter> or <T> to chat");
				MVGameControllerBase.PostGameMsg(MVGameMsgType.AdminMsg, message2);
			}
			if (HackingToolDetector.InstallTracesDetected)
			{
				MVGameControllerBase.PostGameMsg(MVGameMsgType.Warning, HackingToolDetector.CheatWarning);
			}
			Object.Destroy(this);
		}
	}
}
