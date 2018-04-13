using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerListToggle : MonoBehaviour
{
	private bool showPlayerWindowPressed;

	private bool showingPlayerWindow;

	[SerializeField]
	private PlayerListsHold playerListsPrefab;

	private void Start()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IShortcutKeyRegister x, BaseEventData y) =>
		{
			x.RegisterShortcutKey(KogamaControls.ShowPlayerWindow, KeyState.Pressed, Callback);
		});
	}

	private void Callback()
	{
		showPlayerWindowPressed = true;
		if (!showingPlayerWindow)
		{
			CreatePlayerList();
			showingPlayerWindow = true;
		}
	}

	public void CreatePlayerList()
	{
		PlayerListsHold newPlayerLists = Object.Instantiate(playerListsPrefab);
		newPlayerLists.Initialize(playerListsPrefab, GameStatCounterType.Kill);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.PopGroups(UIGroupFlags.InventoryUI | UIGroupFlags.InventoryUISubMenu);
		});
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(newPlayerLists.gameObject, UIPushOption.None, OnPop, UIGroupFlags.InventoryUI);
		});
	}

	private void OnPop()
	{
		showingPlayerWindow = false;
	}

	private void LateUpdate()
	{
		if (!showPlayerWindowPressed && showingPlayerWindow)
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.PopGroups(UIGroupFlags.InventoryUI | UIGroupFlags.InventoryUISubMenu | UIGroupFlags.Popup);
			});
			showingPlayerWindow = false;
		}
		showPlayerWindowPressed = false;
	}
}
