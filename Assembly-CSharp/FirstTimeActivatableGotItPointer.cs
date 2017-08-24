using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FirstTimeActivatableGotItPointer : FirstTimeActivatableButtonPointer
{
	public override void OnShow()
	{
		LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform.parent);
		ShowGotItBubble();
		button.onClick.AddListener(OnShown);
	}

	private void ShowGotItBubble()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (TextBubbleController x, BaseEventData y) =>
		{
			bubbleId = x.ShowBubble2D(pointToTransform.position, ((Vector2)pointToTransform.position + pointerBodyDirectionOffset) * 2f, bubbleLifetimeWhileShown, bubbleContent, transform);
			if (skipAllowed)
			{
				Button button = Object.Instantiate(skipElement);
				button.onClick.AddListener(OnShown);
				x.AddElement(bubbleId, (RectTransform)button.transform);
			}
		});
	}
}
