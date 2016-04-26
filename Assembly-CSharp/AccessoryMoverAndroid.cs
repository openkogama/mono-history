using UnityEngine;
using UnityEngine.EventSystems;

public class AccessoryMoverAndroid : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IEventSystemHandler
{
	private class AccessoryOffsetMouseWrapper
	{
		private SelectionHelperAvatarAccessory selectionHelperAvatarAccessory;

		private bool didMoveAccessory;

		private MVBody body;

		public bool DidMoveAccessory => didMoveAccessory;

		public AccessoryOffsetMouseWrapper(SelectionHelperAvatarAccessory selectionHelperAvatarAccessory)
		{
			this.selectionHelperAvatarAccessory = selectionHelperAvatarAccessory;
			body = (MVBody)MVGameControllerBase.WOCM.GetWorldObjectClient(selectionHelperAvatarAccessory.AvatarBodyWoID);
		}

		public void Update(float moveOffset)
		{
			didMoveAccessory = true;
			float offset = selectionHelperAvatarAccessory.AvatarAccessory.Offset;
			offset += moveOffset * 0.01f;
			selectionHelperAvatarAccessory.AvatarAccessory.Offset = Mathf.Clamp(offset, -0.8f, 0.2f);
			body.ApplyAccessoryOffset(selectionHelperAvatarAccessory.AvatarAccessory, selectionHelperAvatarAccessory.Slot);
		}

		public void SetOffset()
		{
			MVGameControllerBase.Game.UpdateAvatarAccessoryOffset(selectionHelperAvatarAccessory.AvatarBodyWoID, selectionHelperAvatarAccessory.Slot, selectionHelperAvatarAccessory.AvatarAccessory.Offset);
		}
	}

	private AccessoryOffsetMouseWrapper accessoryOffsetMouseWrapper;

	private bool dragging;

	private float moveOffset;

	private float prevMouseY;

	private void Update()
	{
		if (dragging)
		{
			moveOffset = Input.mousePosition.y - prevMouseY;
			prevMouseY = Input.mousePosition.y;
		}
		if (accessoryOffsetMouseWrapper != null)
		{
			accessoryOffsetMouseWrapper.Update(moveOffset);
		}
	}

	public void SetActive(bool isActive)
	{
		enabled = isActive;
		MVGameControllerBase.WOCM.AvatarLocal.Body.AccessoryMoveOverride = isActive;
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		dragging = false;
		if (accessoryOffsetMouseWrapper != null)
		{
			if (accessoryOffsetMouseWrapper.DidMoveAccessory)
			{
				accessoryOffsetMouseWrapper.SetOffset();
			}
			accessoryOffsetMouseWrapper = null;
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		SelectionHelperAvatarAccessory selectionHelperAvatarAccessory = null;
		PickSelectionHelperAvatarAccessory(out selectionHelperAvatarAccessory, out var _);
		if (accessoryOffsetMouseWrapper == null && selectionHelperAvatarAccessory != null)
		{
			accessoryOffsetMouseWrapper = new AccessoryOffsetMouseWrapper(selectionHelperAvatarAccessory);
		}
		dragging = true;
	}

	private void PickSelectionHelperAvatarAccessory(out SelectionHelperAvatarAccessory selectionHelperAvatarAccessory, out RaycastHit raycastHit)
	{
		selectionHelperAvatarAccessory = null;
		if (PickAccessory(out var gameObject, out raycastHit))
		{
			selectionHelperAvatarAccessory = gameObject.GetComponent<SelectionHelperAvatarAccessory>();
		}
	}

	private bool PickAccessory(out GameObject gameObject, out RaycastHit raycastHit)
	{
		Ray ray = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y));
		if (Physics.Raycast(ray, out raycastHit, float.PositiveInfinity, 1 << LayerMask.NameToLayer("Hidden")))
		{
			gameObject = raycastHit.collider.gameObject;
			return true;
		}
		gameObject = null;
		return false;
	}
}
