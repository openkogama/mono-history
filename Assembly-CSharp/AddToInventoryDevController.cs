using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class AddToInventoryDevController : MonoBehaviour
{
	[SerializeField]
	private InputField nameInputField;

	[SerializeField]
	private Dropdown categoryDropDownMenu;

	[SerializeField]
	private Toggle overwriteExisting;

	[SerializeField]
	private PickController pickController;

	[SerializeField]
	private Button next;

	[SerializeField]
	private Button prev;

	[SerializeField]
	private List<GameObject> pages;

	private int currentPage;

	private int pickedWoID;

	private readonly Dictionary<int, int> dropdownIndexToCategoryIndex = new Dictionary<int, int>
	{
		{ 0, 0 },
		{ 1, 5 },
		{ 2, 7 },
		{ 3, 8 },
		{ 4, 6 },
		{ 5, 10 }
	};

	public void OnPageTurned(int dir)
	{
		int index = Mathf.Clamp(currentPage + dir, 0, pages.Count - 1);
		pages[currentPage].SetActive(value: false);
		pages[index].SetActive(value: true);
		currentPage = index;
		prev.gameObject.SetActive(currentPage != 0);
		next.gameObject.SetActive(currentPage != pages.Count - 1);
	}

	public void OnPickInitiated()
	{
		pickController.Initialize(OnPickFinished);
	}

	private void OnPickFinished(int woid)
	{
		pickedWoID = woid;
	}

	public void OnFinished()
	{
		if (ValidateCanFinish())
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
			{
				x.Create("Are you sure you wish to add this object to inventory?", OnFinishConfirmed, string.Empty);
			});
		}
	}

	private void OnFinishConfirmed(bool confirmed, ConfirmationPopup popup)
	{
		popup.Pop();
		if (confirmed)
		{
			Action<byte[]> callback = (byte[] imageData) =>
			{
				MVNetworkGame game = MVGameControllerBase.Game;
				game.OnAddWorldObjectToInventoryCallbackDev = (UnityAction<string>)Delegate.Combine(game.OnAddWorldObjectToInventoryCallbackDev, new UnityAction<string>(OnFinishedAddingItemToDevInventory));
				MVGameControllerBase.Game.AddWorldObjectToInventorDev(pickedWoID, imageData, nameInputField.text, dropdownIndexToCategoryIndex[categoryDropDownMenu.value], overwriteExisting.isOn);
			};
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
			{
				x.Create();
			});
			StartCoroutine(ImageGenerator.CreateTextureFromData(MVGameControllerBase.WOCM.GetWorldObjectClient(pickedWoID), callback));
		}
	}

	private void OnFinishedAddingItemToDevInventory(string msg)
	{
		MVNetworkGame game = MVGameControllerBase.Game;
		game.OnAddWorldObjectToInventoryCallbackDev = (UnityAction<string>)Delegate.Remove(game.OnAddWorldObjectToInventoryCallbackDev, new UnityAction<string>(OnFinishedAddingItemToDevInventory));
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
		{
			x.Create(msg, string.Empty);
		});
	}

	private bool ValidateCanFinish()
	{
		if (string.IsNullOrEmpty(nameInputField.text))
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
			{
				x.CreateErrorNotificationPopup("Name cannot be null when adding object to inventory");
			});
			return false;
		}
		if (pickedWoID == -1)
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
			{
				x.CreateErrorNotificationPopup("pickedWoID cannot be -1, select a valid gameObject");
			});
			return false;
		}
		if (categoryDropDownMenu.value == 0)
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
			{
				x.CreateErrorNotificationPopup("Category index cannot be ''None''");
			});
			return false;
		}
		Debug.Log("Name: " + nameInputField.text + " woid: " + pickedWoID + " Category: " + MVGameControllerBase.IEditModeUI.ClientShopRepository.GetCategoryStringFromId(dropdownIndexToCategoryIndex[categoryDropDownMenu.value]));
		return true;
	}
}
