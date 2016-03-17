namespace UnityEngine.EventSystems;

public interface IHandleToolTip : IEventSystemHandler
{
	void SendToolTip(Vector2 position, string text);
}
