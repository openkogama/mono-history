using UnityEngine;
using UnityEngine.EventSystems;

public class CreateNewCubeModel : MonoBehaviour
{
	[SerializeField]
	private CubeModelPopup cubeModelPopupPrefab;

	private byte currentByteMaterial;

	public void UpdateButtonTextures(byte materialId)
	{
		currentByteMaterial = materialId;
	}

	public void OnAddCubeModelPressed()
	{
		CubeModelPopup popup = Object.Instantiate(cubeModelPopupPrefab);
		popup.Initialize(currentByteMaterial);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack handler, BaseEventData data) =>
		{
			handler.PopGroups(UIGroupFlags.GameObjectUI | UIGroupFlags.InventoryUI | UIGroupFlags.InventoryUISubMenu | UIGroupFlags.GameObjectUISubMenu);
		});
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(popup.gameObject, UIPushOption.Blocking, null, UIGroupFlags.InventoryUI);
		});
	}
}
