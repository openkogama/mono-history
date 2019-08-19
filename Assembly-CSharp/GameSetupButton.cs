using UnityEngine;
using UnityEngine.EventSystems;

public class GameSetupButton : MonoBehaviour
{
	[SerializeField]
	private GameSetupMenu gameSetupMenuPrefab;

	public void ShowGameSetupMenu()
	{
		GameSetupMenu gameSetupMenu = Object.Instantiate(gameSetupMenuPrefab);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(gameSetupMenu.gameObject, UIPushOption.Blocking, null, UIGroupFlags.Popup);
		});
	}
}
