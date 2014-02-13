using System.Collections.Generic;

internal class EditorStateTransitionTable : StateTransitionTable
{
	public EditorStateTransitionTable()
	{
		table.Add(EditorEvent.EditCubes, new ESCubeEdit());
		table.Add(EditorEvent.Rotating, new ESRotating());
		table.Add(EditorEvent.ObjectSelected, new ESSelection());
		table.Add(EditorEvent.ESTerrainEdit, new ESTerrainEdit());
		table.Add(EditorEvent.ESWaitForSelect, new ESWaitForSelected());
		table.Add(EditorEvent.ESSettingsMenu, new ESSettingsDialog());
		table.Add(EditorEvent.ESWaitForGroup, new ESWaitForGroup());
		table.Add(EditorEvent.ESWaitForUngroup, new ESWaitForUngroup());
		table.Add(EditorEvent.ESTranslate, new ESTranslate());
		table.Add(EditorEvent.ESWaitForClone, new ESWaitForClone());
		table.Add(EditorEvent.ESAddLink, new ESAddLink());
		table.Add(EditorEvent.ESPlaceEndpoint, new ESPlaceEndpoint());
		table.Add(EditorEvent.ESWalkMode, new ESWalkMode());
		table.Add(EditorEvent.ESInsert, new ESInsert());
		table.Add(EditorEvent.ESAddObjectLink, new ESAddObjectLink());
		table.Add(EditorEvent.ESBlueprintCreator, new ESBlueprintCreator());
		table.Add(EditorEvent.CERoam, new CERoam());
		table.Add(EditorEvent.CEEditBody, new CEEditBody());
		table.Add(EditorEvent.ESBodyCreator, new ESBodyCreator());
		table.Add(EditorEvent.ESAddToMarketPlaceState, new ESAddToMarketPlaceState());
		table.Add(EditorEvent.CEAvatarAccessory, new CEAvatarAccessory());
		foreach (KeyValuePair<object, IState> item in table)
		{
			ESStateBase eSStateBase = (ESStateBase)item.Value;
			eSStateBase.SetStateType((EditorEvent)(int)item.Key);
		}
	}
}
