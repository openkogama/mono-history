using System.Collections.Generic;
using MV.Common;

public class MVGameModeChangeNotifier : IUpdatecontrollerSubscriber
{
	private UpdateCondition _currentState = UpdateCondition.ALLWAYS;

	private List<IGameStateControllerSubscriber> UpdateList = new List<IGameStateControllerSubscriber>();

	public MVGameModeChangeNotifier()
	{
		_currentState = GetPresentState();
		UpdateController.AddUpdateObject(this, UpdatePriority.UPDATEBUCKET_STANDARD);
	}

	public void AddUpdateObject(IGameStateControllerSubscriber obj)
	{
		UpdateList.Add(obj);
		obj.GameStateChanged(GetPresentState());
	}

	public void RemoveObject(IGameStateControllerSubscriber obj)
	{
		UpdateList.RemoveAll((IGameStateControllerSubscriber x) => x == obj);
	}

	public void UpdateControllerFixedUpdate()
	{
	}

	public void UpdateControllerUpdate()
	{
		UpdateCondition presentState = GetPresentState();
		UpdateList.RemoveAll((IGameStateControllerSubscriber pd) => pd == null);
		if (presentState == _currentState)
		{
			return;
		}
		foreach (IGameStateControllerSubscriber update in UpdateList)
		{
			update?.GameStateChanged(presentState);
		}
		_currentState = presentState;
	}

	private UpdateCondition GetPresentState()
	{
		if (MVGameController.GameMode == MVGameMode.Edit && MVGameController.Game.IsPlaying)
		{
			return UpdateCondition.EDITOR_PLAYMODE;
		}
		if (MVGameController.GameMode == MVGameMode.Edit)
		{
			return UpdateCondition.EDITOR;
		}
		if (MVGameController.GameMode == MVGameMode.Play)
		{
			return UpdateCondition.INGAME;
		}
		return UpdateCondition.ALLWAYS;
	}
}
