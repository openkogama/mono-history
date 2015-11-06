internal class CEEditorStateTransitionTable : EditorStateTransitionTableBase
{
	public CEEditorStateTransitionTable()
	{
		table.Add(EditorEvent.CERoam, new CERoam());
		table.Add(EditorEvent.CEEditBody, new CEEditBody());
		table.Add(EditorEvent.CEAvatarAccessory, new CEAvatarAccessory());
		SetStateTypes();
	}
}
