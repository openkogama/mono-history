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
			body = (MVBody)MVGameController.Instance.WOCM.GetWorldObjectClient(selectionHelperAvatarAccessory.AvatarBodyWoID);
			startTime = Time.time;
		}

		public bool Update()
		{
			if (Time.time > startTime + timeBeforeMove)
			{
				didMoveAccessory = true;
				float offset = selectionHelperAvatarAccessory.AvatarAccessory.Offset;
				offset += Input.GetAxis("Mouse Y") * 0.1f;
				selectionHelperAvatarAccessory.AvatarAccessory.Offset = Mathf.Clamp(offset, -0.8f, 0.2f);
				body.ApplyAccessoryOffset(selectionHelperAvatarAccessory.AvatarAccessory, selectionHelperAvatarAccessory.Slot);
				return true;
			}
			return false;
		}

		public void SetOffset()
		{
			MVGameController.Instance.Game.UpdateAvatarAccessoryOffset(selectionHelperAvatarAccessory.AvatarBodyWoID, selectionHelperAvatarAccessory.Slot, selectionHelperAvatarAccessory.AvatarAccessory.Offset);
		}
	}

	private AccessoryOffsetMouseWrapper accessoryOffsetMouseWrapper;

	private MVGUIAvatarAccessoryMoveIcon avatarAccessoryMoveIcon;

	public MVGUIAvatarAccessoryMoveIcon MVGUIAvatarAccessoryMoveIcon => avatarAccessoryMoveIcon;

	public AccessoryMover()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		Object val = Object.Instantiate(Resources.Load("Prefabs/GUI/AvatarAccessory/AvatarAccessoryMoveIcon"));
		GameObject val2 = (GameObject)(object)((val is GameObject) ? val : null);
		val2.transform.localPosition = Vector3.zero;
		avatarAccessoryMoveIcon = val2.GetComponent<MVGUIAvatarAccessoryMoveIcon>();
	}

	public bool MoveAccessory()
	{
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		if (MVInputWrapper.GetKeyUp((KeyCode)323))
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
		if (PickSelectionHelperAvatarAccessory(out selectionHelperAvatarAccessory, out var raycastHit) && accessoryOffsetMouseWrapper == null && (Object)(object)selectionHelperAvatarAccessory != (Object)null)
		{
			avatarAccessoryMoveIcon.SetVisible(visible: true);
			avatarAccessoryMoveIcon.WorldPosition = raycastHit.point;
		}
		else
		{
			avatarAccessoryMoveIcon.SetVisible(visible: false);
		}
		if (MVInputWrapper.GetKey((KeyCode)323) && accessoryOffsetMouseWrapper == null)
		{
			if (!((Object)(object)selectionHelperAvatarAccessory != (Object)null))
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
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		Ray val = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y));
		if (Physics.Raycast(val, ref raycastHit, float.PositiveInfinity, 1 << LayerMask.NameToLayer("Hidden")))
		{
			gameObject = ((Component)raycastHit.collider).gameObject;
			return true;
		}
		gameObject = null;
		return false;
	}
}
