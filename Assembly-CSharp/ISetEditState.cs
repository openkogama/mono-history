using UnityEngine.EventSystems;

public interface ISetEditState : IEventSystemHandler
{
	void SetState(EditorEvent state);
}
