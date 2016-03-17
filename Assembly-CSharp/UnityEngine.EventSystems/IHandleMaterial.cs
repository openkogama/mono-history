namespace UnityEngine.EventSystems;

public interface IHandleMaterial : IEventSystemHandler
{
	void OnMaterialChanged(byte id);

	void ShowMaterialInventory();
}
