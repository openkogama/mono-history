using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class PickController : MonoBehaviour
{
	[SerializeField]
	private Text woIDText;

	[SerializeField]
	private Text parentType;

	[SerializeField]
	private Text woType;

	[SerializeField]
	private PickHelper pickHelperPrefab;

	private UnityAction<int> pickCallback;

	private bool shouldSetText;

	private int pickedWoId;

	public void Initialize(UnityAction<int> onPickCallback, bool setText)
	{
		shouldSetText = setText;
		pickCallback = onPickCallback;
		PickHelper picker = Object.Instantiate(pickHelperPrefab);
		picker.Initialize(SelectionChanged, "Select wo.");
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(picker.gameObject, UIPushOption.HideAll, null, UIGroupFlags.InventoryUISubMenu);
		});
	}

	public void SelectionChanged(MVWorldObjectClient wo, MVWorldObjectClient parent)
	{
		pickedWoId = wo.Id;
		if (shouldSetText)
		{
			woIDText.text = pickedWoId.ToString();
			parentType.text = "WO Root Group";
			if (parent != null)
			{
				parentType.text = parent.GetType().ToString();
			}
			woType.text = wo.GetType().ToString();
			Refresh();
		}
		if (pickCallback != null)
		{
			pickCallback(pickedWoId);
		}
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.PopGroups(UIGroupFlags.InventoryUISubMenu);
		});
	}

	public void Refresh()
	{
		int result = 0;
		string empty = string.Empty;
		string text = string.Empty;
		if (int.TryParse(woIDText.text, out result))
		{
			MVWorldObjectClient worldObjectClient = MVGameControllerBase.WOCM.GetWorldObjectClient(result);
			if (worldObjectClient != null)
			{
				if (worldObjectClient.GroupId == -1)
				{
					woIDText.text = string.Empty;
					woType.text = "Can't choose WO Root Group";
					return;
				}
				empty = worldObjectClient.GetType().ToString();
				int num = FindParentID(worldObjectClient.Transform);
				if (num != -1)
				{
					MVWorldObjectClient worldObjectClient2 = MVGameControllerBase.WOCM.GetWorldObjectClient(num);
					text = ((worldObjectClient2.GroupId == -1) ? "WO Root Group" : worldObjectClient2.GetType().ToString());
				}
				else
				{
					text = "Parent is not valid wo";
				}
			}
			else
			{
				empty = "No wo with id: " + result;
			}
		}
		else
		{
			empty = "Failed to parse woid";
		}
		woType.text = empty;
		parentType.text = text;
	}

	private int FindParentID(Transform t)
	{
		if (t.parent != null)
		{
			MVWorldObjectClient worldObjectByGoId = MVGameControllerBase.WOCM.GetWorldObjectByGoId(t.parent.gameObject.GetInstanceID());
			if (worldObjectByGoId != null)
			{
				return worldObjectByGoId.Id;
			}
		}
		return -1;
	}
}
