using System.Collections;
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
		if (!gameObject.activeInHierarchy)
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (ILeaveEditPlayModeHandler x, BaseEventData y) =>
			{
				x.LeaveEditPlayMode();
			});
		}
		else
		{
			StopAllCoroutines();
			StartCoroutine(ExecuteLeaveEditDelayed());
		}
	}

	private IEnumerator ExecuteLeaveEditDelayed()
	{
		yield return 0;
		yield return 0;
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (ILeaveEditPlayModeHandler x, BaseEventData y) =>
		{
			x.LeaveEditPlayMode();
		});
	}
}
