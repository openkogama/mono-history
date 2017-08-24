using UnityEngine.EventSystems;

public class CELeaveCubeTutorial : ESStateBase
{
	public override void Enter(EditorStateMachine e)
	{
		base.Enter(e);
		ExecuteEvents.ExecuteHierarchy(e.GameObject, null, (IAvatarSetBodyGroup x, BaseEventData y) =>
		{
			x.SelectEditorStateMachineToBodyGroup();
		});
	}
}
