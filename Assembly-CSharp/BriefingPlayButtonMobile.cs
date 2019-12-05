using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;

public class BriefingPlayButtonMobile : PlayButtonMobile
{
	protected override void OnCountdownEnd()
	{
		base.OnConfirmPlay();
	}

	public override void OnConfirmPlay()
	{
		bool flag = MVGameControllerBase.Game.NetworkGameStateListener.CurrentGameState == MVGameStateType.RoundEnded;
		bool flag2 = Time.time < MVGameControllerBase.LocalPlayer.RespawnTime;
		if (!flag && !flag2)
		{
			base.OnConfirmPlay();
		}
		else
		{
			button.interactable = false;
		}
	}

	protected override void StartPlaying()
	{
		MVGameControllerBase.PlayModeUI.InLobbyState = false;
		if (MVGameControllerBase.LocalPlayer.SpawnRoleDataMediator.WoId != MVGameControllerBase.LocalPlayer.DefaultSpawnRoleId)
		{
			MVGameControllerBase.LocalPlayer.SetActiveSpawnRole(MVGameControllerBase.LocalPlayer.DefaultSpawnRoleId);
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
