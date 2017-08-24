using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FirstTimeActivatablePointerClickToSkip : FirstTimeActivatableElementBase
{
	private int bubbleId = -1;

	[SerializeField]
	private RectTransform pointToTransform;

	[SerializeField]
	private Vector2 pointerBodyDirectionOffset;

	[SerializeField]
	private List<RectTransform> bubbleContent;

	[SerializeField]
	private float bubbleLifetimeWhileShown = float.MaxValue;

	private bool visible;

	public override bool CanShow
	{
		get
		{
			bool isBlocked = IsBlocked;
			bool activeInHierarchy = gameObject.activeInHierarchy;
			return !isBlocked && activeInHierarchy;
		}
	}

	private void Update()
	{
		if (visible && MVInputWrapper.GetBooleanControlUp(KogamaControls.PointerSelect))
		{
			OnShown();
		}
	}

	public override void OnShow()
	{
		visible = true;
		Debug.LogWarning("This class does not implement skippable functionality");
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (TextBubbleController x, BaseEventData y) =>
		{
			bubbleId = x.ShowBubble2D(pointToTransform.position, ((Vector2)pointToTransform.position + pointerBodyDirectionOffset) * 2f, bubbleLifetimeWhileShown, bubbleContent, transform);
		});
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		Clear();
	}

	private void Clear()
	{
		visible = false;
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (TextBubbleController x, BaseEventData y) =>
		{
			x.ClearBubblesWithId(bubbleId);
		});
	}

	private void OnShown()
	{
		Clear();
		FirstTimeEventManager.SetFirstTimeEvent(FirstTimeEvent);
		Object.Destroy(this);
	}
}
