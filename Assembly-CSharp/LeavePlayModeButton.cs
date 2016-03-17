using MV.Common;
using UnityEngine;
using UnityEngine.EventSystems;

public class LeavePlayModeButton : MonoBehaviour
{
	private void Awake()
	{
		if (MVGameControllerBase.GameMode != MVGameMode.Edit)
		{
			gameObject.SetActive(value: false);
		}
	}

	public void Execute()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (ILeaveEditPlayModeHandler x, BaseEventData y) =>
		{
			x.LeaveEditPlayMode();
		});
	}
}
