namespace UnityEngine.EventSystems;

public interface IMaterialClicked : IEventSystemHandler
{
	void OnMaterialClicked(byte materialID);
}
