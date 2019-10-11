using UnityEngine.EventSystems;

public interface IEditStateCommands : IEventSystemHandler
{
	void SetState(EditorEvent state);

	void ClearStateStack();
}
