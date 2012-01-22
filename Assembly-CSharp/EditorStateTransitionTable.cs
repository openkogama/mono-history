using System.Collections.Generic;

internal class EditorStateTransitionTable : StateTransitionTable
{
	public EditorStateTransitionTable()
	{
		table.Add(EditorEvent.EditCubes, new ESOldSchoolModelEdit());
		table.Add(EditorEvent.ESSingleModelEdit, new ESSingleModelEdit());
		table.Add(EditorEvent.Rotating, new ESRotating());
		table.Add(EditorEvent.ObjectSelected, new ESSelectionMode());
		table.Add(EditorEvent.ESWaitForSelect, new ESWaitForSelected());
		table.Add(EditorEvent.ESContextMenu, new ESContextMenu());
		table.Add(EditorEvent.ESWaitForGroup, new ESWaitForGroup());
		table.Add(EditorEvent.ESWaitForUngroup, new ESWaitForUngroup());
		table.Add(EditorEvent.ESTranslate, new ESTranslate());
		table.Add(EditorEvent.ESWaitForClone, new ESWaitForClone());
		table.Add(EditorEvent.ESAddLink, new ESAddLink());
		table.Add(EditorEvent.ESPlaceEndpoint, new ESPlaceEndpoint());
		table.Add(EditorEvent.ESWalkMode, new ESWalkMode());
		foreach (KeyValuePair<object, IState> item in table)
		{
			ESStateBase eSStateBase = (ESStateBase)item.Value;
			eSStateBase.SetStateType((EditorEvent)(int)item.Key);
		}
	}
}
