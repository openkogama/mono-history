using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerListHoldHotkey : MonoBehaviour
{
	[SerializeField]
	private PlayerListsLayout playerListsPrefab;

	private bool isActive;

	private bool registeredHotkeys;

	private void Start()
	{
		registeredHotkeys = true;
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IShortcutKeyRegister x, BaseEventData y) =>
		{
			x.RegisterShortcutKey(KogamaControls.ShowPlayerWindow, KeyState.Down, CreatePlayerList);
		});
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IShortcutKeyRegister x, BaseEventData y) =>
		{
			x.RegisterShortcutKey(KogamaControls.ShowPlayerWindow, KeyState.Up, DestroyPlayerList);
		});
	}

	public void CreatePlayerList()
	{
		isActive = true;
		PlayerListsLayout newPlayerLists = Object.Instantiate(playerListsPrefab);
		WinningConditionControl.TryGetPrioritizedStat(out var statType);
		newPlayerLists.Initialize(playerListsPrefab, statType, UIPushOption.None);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.PopGroups(UIGroupFlags.InventoryUI | UIGroupFlags.InventoryUISubMenu);
		});
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(newPlayerLists.gameObject, UIPushOption.None, null, UIGroupFlags.InventoryUI);
		});
	}

	public void DestroyPlayerList()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.PopGroups(UIGroupFlags.InventoryUI | UIGroupFlags.InventoryUISubMenu);
		});
		isActive = false;
	}

	private void OnDestroy()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.PopGroups(UIGroupFlags.InventoryUI | UIGroupFlags.InventoryUISubMenu);
		});
		if (registeredHotkeys)
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IShortcutKeyUnRegister x, BaseEventData y) =>
			{
				x.UnRegisterShortcutKey(KogamaControls.ShowPlayerWindow, KeyState.Pressed);
			});
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IShortcutKeyUnRegister x, BaseEventData y) =>
			{
				x.UnRegisterShortcutKey(KogamaControls.ShowPlayerWindow, KeyState.Up);
			});
			isActive = false;
			registeredHotkeys = false;
		}
	}

	private void OnDisable()
	{
		if (isActive)
		{
			DestroyPlayerList();
		}
	}
}
