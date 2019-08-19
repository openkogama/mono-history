using MV.Common;
using UnityEngine.EventSystems;

public class BriefingPlayButtonMobile : PlayButtonMobile
{
	protected override void StartPlaying()
	{
		MVGameControllerBase.PlayModeUI.InLobbyState = false;
		if (MVGameControllerBase.LocalPlayer.SpawnRoleDataMediator.WoId != MVGameControllerBase.LocalPlayer.DefaultSpawnRoleId)
		{
			MVGameControllerBase.OperationRequests.SetActiveSpawnRole(MVGameControllerBase.LocalPlayer.DefaultSpawnRoleId);
		}
		else if (MVGameControllerBase.SpawnRoleDataMediatorLocal.SpawnRoleModeTypeWrapper.IsInMode(SpawnRoleModeType.Hidden))
		{
			MVGameControllerBase.GameEventManager.AvatarCommandsPlayMode.Spawn();
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
