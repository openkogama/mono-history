using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AvatarSelectionController : MonoBehaviour, IAvatarSlotClicked, IEventSystemHandler
{
	[SerializeField]
	private AvatarSelectionSlot avatarSelectionSlotPrefab;

	[SerializeField]
	private RectTransform avatarSelectionContentRoot;

	[SerializeField]
	private SellAvatarController sellAvatarPrefab;

	[SerializeField]
	private GameObject publishAvatarGO;

	private int currSelectedSlot = -1;

	private readonly Dictionary<int, AvatarSelectionSlot> avatarSlots = new Dictionary<int, AvatarSelectionSlot>();

	private static AvatarSelectionController instance;

	private AvatarEditModeBodyController avatarBodyController;

	public static int CurrentlySelectedSlotIndex
	{
		get
		{
			return instance.currSelectedSlot;
		}
		set
		{
			if (instance.currSelectedSlot != -1)
			{
				instance.avatarSlots[instance.currSelectedSlot].ToggleActive(active: false);
				instance.avatarSlots[value].ToggleActive(active: true);
			}
			instance.currSelectedSlot = value;
		}
	}

	public void Initialize(AvatarEditModeBodyController bodyController, EditorStateMachine esm)
	{
		instance = this;
		avatarBodyController = bodyController;
		avatarBodyController.SetPublishAvatarGO(publishAvatarGO);
		avatarBodyController.CaptureScreenshotsForAllAvatars(OnPictureTakenCallback);
	}

	public void Destroy()
	{
		instance = null;
	}

	public void ResetCurrentAvatar()
	{
		avatarBodyController.ResetCurrentBody();
	}

	public void AvatarSlotClicked(int slotIndex)
	{
		if (slotIndex != currSelectedSlot)
		{
			int num = currSelectedSlot;
			CurrentlySelectedSlotIndex = slotIndex;
			avatarBodyController.SetCurrentBody(slotIndex);
			MVGameControllerBase.OperationRequests.SetActiveAvatar(avatarBodyController.CurrentBody.Id);
			SetStateToRoam();
			Object.Destroy(avatarSlots[num].gameObject);
			avatarSlots[num] = null;
			avatarBodyController.CaptureScreenshotForBody(num, OnPicUpdateForPrevAvatar);
		}
	}

	public void SetStateToRoam()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IAvatarSetBodyGroup x, BaseEventData y) =>
		{
			x.SetBodyGroup(avatarBodyController.CurrentBody);
		});
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (ISetEditState x, BaseEventData y) =>
		{
			x.SetState(EditorEvent.CERoamUUI);
		});
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
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
		{
			x.CreateErrorNotificationPopup(TM._("You cannot sell your avatar through the standalone client. Play in browser to place in shop.\n"));
		});
	}

	public void TakeScreenshotForProfile()
	{
		avatarBodyController.TakeScreenshot();
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
