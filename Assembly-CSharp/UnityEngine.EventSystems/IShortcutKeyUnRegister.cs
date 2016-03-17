namespace UnityEngine.EventSystems;

public interface IShortcutKeyUnRegister : IEventSystemHandler
{
	void UnRegisterShortcutKey(KogamaControls kogamaControl, KeyState keyState);
}
