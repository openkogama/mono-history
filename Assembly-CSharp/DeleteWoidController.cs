using UnityEngine;
using UnityEngine.EventSystems;

public class DeleteWoidController : MonoBehaviour
{
	[SerializeField]
	private PickHelper pickHelperPrefab;

	private int woid;

	public void Initialize()
	{
		PickHelper pickHelper = Object.Instantiate(pickHelperPrefab);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(pickHelper.gameObject, UIPushOption.HideAll, null, UIGroupFlags.InventoryUISubMenu);
		});
		pickHelper.Initialize(OnPick, "Select object to delete");
	}

	private void OnPick(MVWorldObjectClient wo, MVWorldObjectClient woParent)
	{
		woid = wo.Id;
		if (woid > 0)
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
			{
				x.Create("Are you sure you wish to delete worldobject " + woid + "?", DeleteWorldObject, string.Empty);
			});
		}
		else
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
			{
				x.Create("woid not valid: " + woid, string.Empty);
			});
		}
	}

	private void DeleteWorldObject(bool success, ConfirmationPopup popup)
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.PopGroups(UIGroupFlags.InventoryUI | UIGroupFlags.InventoryUISubMenu);
		});
		if (success)
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IEditModeController x, BaseEventData y) =>
			{
				x.DeleteWoid(woid);
			});
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IModalPopupCreator x, BaseEventData y) =>
			{
				x.Create("Worldobject deleted", string.Empty);
			});
		}
		else
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.PopGroups(UIGroupFlags.Popup);
			});
		}
	}
}
