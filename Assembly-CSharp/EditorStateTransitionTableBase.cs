using System.Collections.Generic;

internal abstract class EditorStateTransitionTableBase : StateTransitionTable
{
	protected void SetStateTypes()
	{
		foreach (KeyValuePair<object, IState> item in table)
		{
			ESStateBase eSStateBase = (ESStateBase)item.Value;
			eSStateBase.SetStateType((EditorEvent)(int)item.Key);
		}
	}
}
