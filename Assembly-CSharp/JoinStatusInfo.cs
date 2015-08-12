using UnityEngine;

public class JoinStatusInfo : MonoBehaviour
{
	private MVJoinState prevGameState;

	private MVConnState prevConnState;

	[SerializeField]
	private MVGUIChatWindow guiChatWindow;

	private void Update()
	{
		if (MVGameController.Game == null)
		{
			return;
		}
		MVConnState connState = MVGameController.Game.ConnState;
		MVJoinState joinState = MVGameController.Game.JoinState;
		if (connState != prevConnState)
		{
			prevConnState = connState;
			guiChatWindow.AddLine(TM._("Connection") + ": " + LocalizedEnums._(connState), Color.grey);
		}
		if (joinState != prevGameState)
		{
			StatHatWrapper.Count(joinState.ToString(), 1);
			prevGameState = joinState;
			guiChatWindow.AddLine(TM._("Game") + ": " + LocalizedEnums._(joinState), Color.grey);
			if (joinState == MVJoinState.Playing)
			{
				guiChatWindow.AddLine("\n\n" + MVGameController.VersionNumber.ToString() + "\n\n", Color.grey);
			}
		}
		if (joinState != MVJoinState.Playing)
		{
			guiChatWindow.KeepAlive();
		}
	}
}
