using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AvatarSelectionSlot : MonoBehaviour
{
	[SerializeField]
	private RawImage image;

	[SerializeField]
	private RectTransform selectionOutline;

	public int BodyIndex { get; private set; }

	private void Awake()
	{
		selectionOutline.gameObject.SetActive(value: false);
	}

	public void BuildAvatarSelectionSlot(int index, Texture2D texture)
	{
		BodyIndex = index;
		image.texture = texture;
	}

	public void SlotClicked()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IAvatarSlotClicked x, BaseEventData y) =>
		{
			x.AvatarSlotClicked(BodyIndex);
		});
	}

	public void ToggleActive(bool active)
	{
		selectionOutline.gameObject.SetActive(active);
	}
}
