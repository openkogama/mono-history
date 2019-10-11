using System.Collections.Generic;
using MV.Common;
using MV.WorldObject;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpawnRoleMenuButton : MonoBehaviour
{
	[SerializeField]
	private SpawnRoleMenu spawnRoleMenuPrefab;

	[SerializeField]
	private GameObject buttonObject;

	private void Start()
	{
		SetButtonIsActive();
		MVGameControllerBase.SpawnRoleDataMediatorLocal.SpawnRoleModeTypeWrapper.OnChange += AvatarStateChanged;
	}

	private void OnDestroy()
	{
		if (MVGameControllerBase.IsAlive)
		{
			MVGameControllerBase.SpawnRoleDataMediatorLocal.SpawnRoleModeTypeWrapper.OnChange -= AvatarStateChanged;
		}
	}

	private void AvatarStateChanged(SpawnRoleModeType state)
	{
		if ((state & SpawnRoleModeType.Hidden) > SpawnRoleModeType.None)
		{
			SetButtonIsActive();
		}
	}

	private void SetButtonIsActive()
	{
		List<MVWorldObjectClient> worldObjectsByType = MVGameControllerBase.Game.WorldObjectClientManager.GetWorldObjectsByType(WorldObjectType.AvatarSpawnRoleCreator);
		bool flag = worldObjectsByType.Count > 0;
		bool active = MVGameControllerBase.Game.TeamManager.TeamCount() <= 1 && flag;
		buttonObject.SetActive(active);
	}

	public void ShowSpawnRoleMenu()
	{
		SpawnRoleMenu newSpawnRoleMenu = Object.Instantiate(spawnRoleMenuPrefab);
		newSpawnRoleMenu.Initialize(MVGameControllerBase.LocalPlayer.Team);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack handler, BaseEventData data) =>
		{
			handler.PopGroups(UIGroupFlags.InventoryUI | UIGroupFlags.InventoryUISubMenu);
		});
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(newSpawnRoleMenu.gameObject, UIPushOption.HideAll | UIPushOption.InvisibleBlocker, null, UIGroupFlags.InventoryUI);
		});
		MVGameControllerBase.GameEventManager.AvatarCommandsPlayMode.RemoveFromGame();
		Debug.Log("ShowSpawnRoleMenu");
	}
}
