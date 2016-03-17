namespace UnityEngine.EventSystems;

public interface ICreateNewPrototype : IEventSystemHandler
{
	void OnAddNewPrototype(string name, float scale);
}
