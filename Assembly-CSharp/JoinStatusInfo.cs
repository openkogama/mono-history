using MV.Common;
using UnityEngine;

public class JoinStatusInfo : MonoBehaviour
{
	private MVEventCodes prevGameState = MVEventCodes.AddObjectLink;

	private MVConnState prevConnState;

	[SerializeField]
	private MVGUIChatWindow guiChatWindow;

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
			guiChatWindow.AddLine(TM._("Connection") + ": " + LocalizedEnums._(connState), Color.grey);
		}
		while (JoinUIUpdater.JoinEventCodes.Count > 0)
		{
			MVEventCodes mVEventCodes = JoinUIUpdater.JoinEventCodes.Dequeue();
			if (mVEventCodes != prevGameState)
			{
				StatHatWrapper.Count(mVEventCodes.ToString(), 1);
				prevGameState = mVEventCodes;
				guiChatWindow.AddLine(TM._("Game") + ": " + LocalizedEnums._(mVEventCodes), Color.grey);
				if (MVGameControllerBase.JoinState == MVJoinState.Playing)
				{
					guiChatWindow.AddLine("\n\n" + MVGameControllerBase.VersionNumber.ToString() + "\n\n", Color.grey);
					Object.Destroy(this);
				}
			}
		}
		if (MVGameControllerBase.JoinState != MVJoinState.Playing)
		{
			guiChatWindow.KeepAlive();
		}
	}
}
