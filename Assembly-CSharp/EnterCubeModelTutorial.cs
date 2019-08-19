using UnityEngine;
using UnityEngine.EventSystems;

public class EnterCubeModelTutorial : MonoBehaviour
{
	public void OnClick()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (ISetEditState x, BaseEventData y) =>
		{
			x.SetState(EditorEvent.ESEnterCubeTutorial);
		});
	}
}
