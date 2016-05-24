using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AvatarSelectionController : MonoBehaviour, IEventSystemHandler, IAvatarSlotClicked
{
	private static int currSelectedSlot = -1;

	[SerializeField]
	private AvatarSelectionSlot avatarSelectionSlotPrefab;

	[SerializeField]
	private RectTransform avatarSelectionContentRoot;

	[SerializeField]
	private SellAvatarController sellAvatarPrefab;

	private AvatarEditModeBodyController avatarBodyController;

	private static readonly Dictionary<int, AvatarSelectionSlot> avatarSlots = new Dictionary<int, AvatarSelectionSlot>();

	public static int CurrentlySelectedSlotIndex
	{
		get
		{
			return currSelectedSlot;
		}
		set
		{
			if (currSelectedSlot != -1)
			{
				avatarSlots[currSelectedSlot].ToggleActive(active: false);
				avatarSlots[value].ToggleActive(active: true);
			}
			currSelectedSlot = value;
		}
	}

	public void Initialize(AvatarEditModeBodyController bodyController)
	{
		avatarBodyController = bodyController;
		avatarBodyController.CaptureScreenshotsForAllAvatars(OnPictureTakenCallback);
	}

	public void ResetCurrentAvatar()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
		{
			x.Create("Do you really wish to reset all changes made to the avatar this session?", ResetCallback, "Reset avatar");
		});
	}

	private void ResetCallback(bool confirmed, ConfirmationPopup popup)
	{
		if (confirmed)
		{
			avatarBodyController.ResetCurrentBody();
		}
		else
		{
			popup.Pop();
		}
	}

	public void AvatarSlotClicked(int slotIndex)
	{
		if (slotIndex != currSelectedSlot)
		{
			int num = currSelectedSlot;
			CurrentlySelectedSlotIndex = slotIndex;
			avatarBodyController.SetCurrentBody(slotIndex);
			MVGameControllerBase.OperationRequests.SetActiveAvatar(avatarBodyController.CurrentBody.Id);
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IAvatarSetBodyGroup x, BaseEventData y) =>
			{
				x.SetBodyGroup(avatarBodyController.CurrentBody);
			});
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (ISetEditState x, BaseEventData y) =>
			{
				x.SetState(EditorEvent.CERoamUUI);
			});
			Object.Destroy(avatarSlots[num].gameObject);
			avatarSlots[num] = null;
			avatarBodyController.CaptureScreenshotForBody(num, OnPicUpdateForPrevAvatar);
		}
	}

	private void OnPicUpdateForPrevAvatar(int index, Texture2D image)
	{
		AvatarSelectionSlot avatarSelectionSlot = Object.Instantiate(avatarSelectionSlotPrefab);
		avatarSelectionSlot.BuildAvatarSelectionSlot(index, image);
		avatarSelectionSlot.transform.SetParent(avatarSelectionContentRoot, worldPositionStays: false);
		avatarSelectionSlot.transform.SetSiblingIndex(index);
		avatarSlots[index] = avatarSelectionSlot;
	}

	public void SellCurrentAvatar()
	{
		SellAvatarController sellAvatar = Object.Instantiate(sellAvatarPrefab);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(sellAvatar.gameObject, UIPushOption.Blocking | UIPushOption.HideAll, null, UIGroupFlags.Popup);
		});
		avatarBodyController.SellCurrentAvatar(sellAvatar);
	}

	public void TakeScreenshotForProfile()
	{
		avatarBodyController.TakeScreenshotForProfile();
	}

	public void SetToNextAnimation()
	{
		avatarBodyController.SetToNextAnimation();
	}

	public void OpenAvatarShop()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IActivateUIElement x, BaseEventData y) =>
		{
			x.Activate(ActivateUIElement.AvatarShop);
		});
	}

	private void OnPictureTakenCallback(int index, Texture2D image)
	{
		AvatarSelectionSlot avatarSelectionSlot = Object.Instantiate(avatarSelectionSlotPrefab);
		avatarSelectionSlot.BuildAvatarSelectionSlot(index, image);
		avatarSelectionSlot.transform.SetParent(avatarSelectionContentRoot, worldPositionStays: false);
		avatarSelectionSlot.transform.SetSiblingIndex(index);
		avatarSlots[index] = avatarSelectionSlot;
		if (index == CurrentlySelectedSlotIndex)
		{
			avatarSelectionSlot.ToggleActive(active: true);
		}
	}
}
