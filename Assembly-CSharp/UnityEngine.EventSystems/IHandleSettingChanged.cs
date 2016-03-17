namespace UnityEngine.EventSystems;

public interface IHandleSettingChanged : IEventSystemHandler
{
	void OnSettingChanged(string key, object value);
}
