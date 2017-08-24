using UnityEngine;

internal class CEEditorStateTransitionTableUUI : EditorStateTransitionTableBase
{
	public CEEditorStateTransitionTableUUI(Vector3 centerPos)
	{
		table.Add(EditorEvent.CERoamUUI, new CERoamUUI(centerPos));
		table.Add(EditorEvent.CEEditBodyUUI, new CEEditBodyUUI());
		table.Add(EditorEvent.CEAvatarAccessoryUUI, new CEAvatarAccessoryUUI());
		table.Add(EditorEvent.ESEnterCubeTutorial, new CEEnterCubeTutorial());
		table.Add(EditorEvent.ESEditCubeTutorial, new CEEditCubeTutorial());
		table.Add(EditorEvent.ESLeaveCubeTutorial, new CELeaveCubeTutorial());
		table.Add(EditorEvent.EditCubes, new ESCubeEdit());
		SetStateTypes();
	}
}
