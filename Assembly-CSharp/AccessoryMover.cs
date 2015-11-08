using UnityEngine;

public class AccessoryMover
{
	private class AccessoryOffsetMouseWrapper
	{
		private SelectionHelperAvatarAccessory selectionHelperAvatarAccessory;

		private float startTime;

		private float timeBeforeMove = 0.2f;

		private bool didMoveAccessory;

		private MVBody body;

		public bool DidMoveAccessory => didMoveAccessory;

		public AccessoryOffsetMouseWrapper(SelectionHelperAvatarAccessory selectionHelperAvatarAccessory)
		{
			this.selectionHelperAvatarAccessory = selectionHelperAvatarAccessory;
			body = (MVBody)MVGameControllerBase.WOCM.GetWorldObjectClient(selectionHelperAvatarAccessory.AvatarBodyWoID);
			startTime = Time.time;
		}

		public bool Update()
		{
			if (Time.time > startTime + timeBeforeMove)
			{
				didMoveAccessory = true;
				float offset = selectionHelperAvatarAccessory.AvatarAccessory.Offset;
				offset += MVInputWrapper.GetAxis("Mouse Y") * 0.01f;
				selectionHelperAvatarAccessory.AvatarAccessory.Offset = Mathf.Clamp(offset, -0.8f, 0.2f);
				body.ApplyAccessoryOffset(selectionHelperAvatarAccessory.AvatarAccessory, selectionHelperAvatarAccessory.Slot);
				return true;
			}
			return false;
		}

		public void SetOffset()
		{
			MVGameControllerBase.Game.UpdateAvatarAccessoryOffset(selectionHelperAvatarAccessory.AvatarBodyWoID, selectionHelperAvatarAccessory.Slot, selectionHelperAvatarAccessory.AvatarAccessory.Offset);
		}
	}

	private AccessoryOffsetMouseWrapper accessoryOffsetMouseWrapper;

	private MVGUIAvatarAccessoryMoveIcon avatarAccessoryMoveIcon;

	public MVGUIAvatarAccessoryMoveIcon MVGUIAvatarAccessoryMoveIcon => avatarAccessoryMoveIcon;

	public AccessoryMover()
	{
		GameObject gameObject = Object.Instantiate(Resources.Load("Prefabs/GUI/AvatarAccessory/AvatarAccessoryMoveIcon")) as GameObject;
		gameObject.transform.localPosition = Vector3.zero;
		avatarAccessoryMoveIcon = gameObject.GetComponent<MVGUIAvatarAccessoryMoveIcon>();
		avatarAccessoryMoveIcon.SetVisible(visible: false);
	}

	public bool MoveAccessory()
	{
		if (MVInputWrapper.GetBooleanControlUp(KogamaControls.PointerSelect))
		{
			if (accessoryOffsetMouseWrapper == null)
			{
				return false;
			}
			bool result = false;
			if (accessoryOffsetMouseWrapper.DidMoveAccessory)
			{
				accessoryOffsetMouseWrapper.SetOffset();
				result = true;
			}
			accessoryOffsetMouseWrapper = null;
			return result;
		}
		SelectionHelperAvatarAccessory selectionHelperAvatarAccessory = null;
		if (PickSelectionHelperAvatarAccessory(out selectionHelperAvatarAccessory, out var raycastHit) && accessoryOffsetMouseWrapper == null && selectionHelperAvatarAccessory != null)
		{
			avatarAccessoryMoveIcon.SetVisible(visible: true);
			avatarAccessoryMoveIcon.WorldPosition = raycastHit.point;
		}
		else
		{
			avatarAccessoryMoveIcon.SetVisible(visible: false);
		}
		if (MVInputWrapper.GetBooleanControl(KogamaControls.PointerSelect) && accessoryOffsetMouseWrapper == null)
		{
			if (!(selectionHelperAvatarAccessory != null))
			{
				return false;
			}
			accessoryOffsetMouseWrapper = new AccessoryOffsetMouseWrapper(selectionHelperAvatarAccessory);
		}
		if (accessoryOffsetMouseWrapper != null && accessoryOffsetMouseWrapper.Update())
		{
			return true;
		}
		return false;
	}

	private bool PickSelectionHelperAvatarAccessory(out SelectionHelperAvatarAccessory selectionHelperAvatarAccessory, out RaycastHit raycastHit)
	{
		if (PickAccessory(out var gameObject, out raycastHit))
		{
			selectionHelperAvatarAccessory = gameObject.GetComponent<SelectionHelperAvatarAccessory>();
			return true;
		}
		selectionHelperAvatarAccessory = null;
		return false;
	}

	private bool PickAccessory(out GameObject gameObject, out RaycastHit raycastHit)
	{
		Ray ray = Camera.main.ScreenPointToRay(new Vector3(MVInputWrapper.GetPointerPosition().x, MVInputWrapper.GetPointerPosition().y));
		if (Physics.Raycast(ray, out raycastHit, float.PositiveInfinity, 1 << LayerMask.NameToLayer("Hidden")))
		{
			gameObject = raycastHit.collider.gameObject;
			return true;
		}
		gameObject = null;
		return false;
	}
}
