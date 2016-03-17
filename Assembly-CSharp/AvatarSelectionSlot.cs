using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AvatarSelectionSlot : MonoBehaviour
{
	[SerializeField]
	private RawImage image;

	[SerializeField]
	private RectTransform selectionOutline;

	private int bodyIndex = -1;

	private void Awake()
	{
		selectionOutline.gameObject.SetActive(value: false);
	}

	public void BuildAvatarSelectionSlot(int index, Texture2D texture)
	{
		bodyIndex = index;
		image.texture = texture;
	}

	public void SlotClicked()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IAvatarSlotClicked x, BaseEventData y) =>
		{
			x.AvatarSlotClicked(bodyIndex);
		});
	}

	public void ToggleActive()
	{
		selectionOutline.gameObject.SetActive(value: true);
	}
}
