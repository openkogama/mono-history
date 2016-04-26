using UnityEngine;
using UnityEngine.EventSystems;

public class AvatarSelectionController : MonoBehaviour, IEventSystemHandler, IAvatarSlotClicked
{
	[SerializeField]
	private AvatarSelectionSlot avatarSelectionSlotPrefab;

	[SerializeField]
	private RectTransform avatarSelectionContentRoot;

	[SerializeField]
	private SellAvatarController sellAvatarPrefab;

	private AvatarEditModeBodyController avatarBodyController;

	public static int CurrentlySelectedSlotIndex { get; set; }

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
		CurrentlySelectedSlotIndex = slotIndex;
		avatarBodyController.SetCurrentBody(slotIndex);
		MVGameControllerBase.Game.SetActiveAvatar(avatarBodyController.CurrentBody.Id);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IAvatarSetBodyGroup x, BaseEventData y) =>
		{
			x.SetBodyGroup(avatarBodyController.CurrentBody);
		});
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (ISetEditState x, BaseEventData y) =>
		{
			x.SetState(EditorEvent.CERoamUUI);
		});
		for (int num = 0; num < avatarSelectionContentRoot.childCount; num++)
		{
			Object.Destroy(avatarSelectionContentRoot.GetChild(num).gameObject);
		}
		avatarBodyController.CaptureScreenshotsForAllAvatars(OnPictureTakenCallback);
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
		if (index == CurrentlySelectedSlotIndex)
		{
			avatarSelectionSlot.ToggleActive();
		}
	}
}
