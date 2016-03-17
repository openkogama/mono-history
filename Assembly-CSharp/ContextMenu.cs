using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class ContextMenu : MonoBehaviour
{
	private bool linkMenu;

	private Vector3 worldPosition;

	private int woID;

	[SerializeField]
	private ContextMenuButton contextMenuButtonPrefab;

	[SerializeField]
	private RectTransform rectTransform;

	public void Initialize(int woID, Vector3 worldPosition)
	{
		this.woID = woID;
		MVGameControllerBase.WOCM.SubscribeWODestroyedEvent(woID, PopWoDestroyed);
		this.worldPosition = worldPosition;
		SetToScreenPoint();
	}

	public void InitializeLink(int linkID, Vector3 worldPosition)
	{
		linkMenu = true;
		this.worldPosition = worldPosition;
		SetToScreenPoint();
	}

	private void PopWoDestroyed(object obj, WorldObjectDestroyedEventArgs args)
	{
		Pop();
	}

	private void Pop()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack handler, BaseEventData data) =>
		{
			handler.PopGroups(UIGroupFlags.GameObjectUI);
		});
	}

	public void AddButton(string buttonText, UnityAction onClickCallback)
	{
		ContextMenuButton contextMenuButton = Object.Instantiate(contextMenuButtonPrefab);
		contextMenuButton.Initialize(buttonText, onClickCallback);
		contextMenuButton.transform.SetParent(transform, worldPositionStays: false);
	}

	private void Update()
	{
		SetToScreenPoint();
	}

	private void SetToScreenPoint()
	{
		Vector3 position = MVGameControllerBase.CameraController.MainCamera.WorldToScreenPoint(worldPosition);
		rectTransform.transform.position = position;
	}

	private void LateUpdate()
	{
		if (MVInputWrapper.GetBooleanControlUp(KogamaControls.PointerSelect) && !EventSystem.current.IsPointerOverGameObject())
		{
			Pop();
		}
	}

	private void OnDestroy()
	{
		if (!linkMenu)
		{
			MVGameControllerBase.WOCM.UnsubscribeWODestroyedEvent(woID, PopWoDestroyed);
		}
	}
}
