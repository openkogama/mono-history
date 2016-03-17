using UnityEngine;
using UnityEngine.EventSystems;

public class ExitCubeModeling : MonoBehaviour
{
	public void Exit()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IHandleCubeModelEdit x, BaseEventData y) =>
		{
			x.Close();
		});
	}
}
