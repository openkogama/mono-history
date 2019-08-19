using UnityEngine.EventSystems;

public class BriefingPlayButton : PlayButton
{
	protected override void StartPlaying()
	{
		MVGameControllerDesktop.LockCursorManager.CursorLock = true;
		if (MVGameControllerBase.LocalPlayer.SpawnRoleDataMediator.WoId != MVGameControllerBase.LocalPlayer.DefaultSpawnRoleId)
		{
			MVGameControllerBase.OperationRequests.SetActiveSpawnRole(MVGameControllerBase.LocalPlayer.DefaultSpawnRoleId);
		}
		if (shouldPop)
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack handler, BaseEventData data) =>
			{
				handler.Pop();
			});
		}
	}
}
