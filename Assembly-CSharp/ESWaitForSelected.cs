using UnityEngine;

internal class ESWaitForSelected : ESStateBase
{
	private bool useESInsert = true;

	private bool isNewPrototype;

	public override void Enter(EditorStateMachine e)
	{
		Debug.Log("wait for selected");
		e.DeSelectAll();
		isNewPrototype = e.Data.ContainsKey("IsNewPrototype");
	}

	public override void Execute(EditorStateMachine e)
	{
		base.Execute(e);
		if (e.SingleSelectedWO != null)
		{
			if (useESInsert)
			{
				e.PushState(EditorEvent.ESInsert, EditorEvent.ObjectSelected);
			}
			else
			{
				e.PushState(EditorEvent.ESTranslate, EditorEvent.ObjectSelected);
			}
		}
	}

	public override void Exit(EditorStateMachine e)
	{
		if (isNewPrototype)
		{
			e.Data["IsNewPrototype"] = true;
		}
	}
}
