using UnityEngine.Events;

namespace UnityEngine.EventSystems;

public interface IShortcutKeyRegister : IEventSystemHandler
{
	void RegisterShortcutKey(KogamaControls kogamaControl, KeyState keyState, UnityAction callback);
}
