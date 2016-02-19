using MV.Common;
using UnityEngine;

public class JoinStatusInfo : MonoBehaviour
{
	private MVOperationCodes prevGameState = MVOperationCodes.Join;

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
		while (JoinUIUpdater.joinOperations.Count > 0)
		{
			MVOperationCodes mVOperationCodes = JoinUIUpdater.joinOperations.Dequeue();
			if (mVOperationCodes != prevGameState)
			{
				StatHatWrapper.Count(mVOperationCodes.ToString(), 1);
				prevGameState = mVOperationCodes;
				guiChatWindow.AddLine(TM._("Game") + ": " + LocalizedEnums._(mVOperationCodes), Color.grey);
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
