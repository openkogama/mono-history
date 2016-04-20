internal class EditorStateTransitionTable3D : EditorStateTransitionTableBase
{
	public EditorStateTransitionTable3D(ContextMenuController contextMenuController, GizmoController gizmoController)
	{
		table.Add(EditorEvent.EditCubes, new ESCubeEdit());
		table.Add(EditorEvent.Rotating, new ESRotating());
		table.Add(EditorEvent.ObjectSelected, new ESSelection(contextMenuController, gizmoController));
		table.Add(EditorEvent.ESTerrainEdit, new ESTerrainEdit());
		table.Add(EditorEvent.ESWaitForSelect, new ESWaitForSelected());
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
		table.Add(EditorEvent.ESAddToMarketPlaceState, new ESAddToMarketPlaceState());
		SetStateTypes();
	}
}
