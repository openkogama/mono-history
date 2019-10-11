using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameSetupButton : MonoBehaviour
{
	[SerializeField]
	private GamePassesTextBubble OnActivatedToolTip;

	[SerializeField]
	private GameSetupMenu gameSetupMenuPrefab;

	private bool isTiersActivated;

	public void ShowGameSetupMenu()
	{
		GameSetupMenu gameSetupMenu = UnityEngine.Object.Instantiate(gameSetupMenuPrefab);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(gameSetupMenu.gameObject, UIPushOption.Blocking, null, UIGroupFlags.Popup);
		});
	}

	private void Start()
	{
		GamePassProgressionController.OnGamePassesProgressionUpdate = (Action)Delegate.Combine(GamePassProgressionController.OnGamePassesProgressionUpdate, new Action(OnProgressionUpdate));
		isTiersActivated = GamePassProgressionController.IsProgressionEnabled;
	}

	private void OnDestroy()
	{
		GamePassProgressionController.OnGamePassesProgressionUpdate = (Action)Delegate.Remove(GamePassProgressionController.OnGamePassesProgressionUpdate, new Action(OnProgressionUpdate));
	}

	private void OnProgressionUpdate()
	{
		bool isProgressionEnabled = GamePassProgressionController.IsProgressionEnabled;
		if (isProgressionEnabled && !isTiersActivated)
		{
			OnActivatedToolTip.Activate("Game Tiers Activated");
		}
		isTiersActivated = isProgressionEnabled;
	}
}
