using UnityEngine;
using UnityEngine.EventSystems;

public class GizmoController : MonoBehaviour, IGizmoHandler, IEventSystemHandler
{
	private int woID;

	private EditorStateMachine editorStateMachine;

	[SerializeField]
	private GizmoMenu gizmoMenuPrefab;

	public void Initialize(EditorStateMachine editorStateMachine)
	{
		this.editorStateMachine = editorStateMachine;
	}

	public void Show(int woID, Vector3 worldPosition, EditorStateMachine e)
	{
		GizmoMenu gizmoMenu = Object.Instantiate(gizmoMenuPrefab);
		gizmoMenu.Initialize(woID, worldPosition, e);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack handler, BaseEventData data) =>
		{
			handler.PopGroups(UIGroupFlags.GameObjectUI);
		});
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(gizmoMenu.gameObject, UIPushOption.None, OnGizmoMenuPop, UIGroupFlags.GameObjectUI);
		});
	}

	private void OnGizmoMenuPop()
	{
	}

	public void Handle(GizmoAction action)
	{
		switch (action)
		{
		case GizmoAction.Rotate:
			editorStateMachine.Data.Add("rotationDegreesStep", 15f);
			editorStateMachine.PushState(EditorEvent.Rotating);
			break;
		case GizmoAction.TranslateY:
			editorStateMachine.Data.Add("moveWithAvatar", true);
			editorStateMachine.Data.Add("translateMode", TranslateMode.Y);
			editorStateMachine.PushState(EditorEvent.ESTranslate);
			break;
		case GizmoAction.TranslateXZ:
			editorStateMachine.Data.Add("translateMode", TranslateMode.XZ);
			editorStateMachine.Data.Add("moveWithAvatar", true);
			editorStateMachine.PushState(EditorEvent.ESTranslate);
			break;
		}
	}
}
