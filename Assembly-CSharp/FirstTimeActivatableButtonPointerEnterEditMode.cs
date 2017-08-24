using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FirstTimeActivatableButtonPointerEnterEditMode : FirstTimeActivatableElementBase
{
	private int bubbleId = -1;

	[SerializeField]
	private EnterPlayModeButton button;

	[SerializeField]
	private RectTransform pointToTransform;

	[SerializeField]
	private Vector2 pointerBodyDirectionOffset;

	[SerializeField]
	private List<RectTransform> bubbleContent;

	[SerializeField]
	private float bubbleLifetimeWhileShown = float.MaxValue;

	[SerializeField]
	private Button skipElement;

	[SerializeField]
	protected bool skipAllowed = true;

	private bool shouldBeDelayedDestroyed;

	public override bool CanShow
	{
		get
		{
			bool isBlocked = IsBlocked;
			bool activeInHierarchy = gameObject.activeInHierarchy;
			return !isBlocked && activeInHierarchy;
		}
	}

	public override void OnActivate()
	{
		EnterPlayModeButton enterPlayModeButton = button;
		enterPlayModeButton.enteringPlayMode = (Action)Delegate.Combine(enterPlayModeButton.enteringPlayMode, new Action(OnShown));
	}

	public override void OnShow()
	{
		CreateBubble();
	}

	private void CreateBubble()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (TextBubbleController x, BaseEventData y) =>
		{
			bubbleId = x.ShowBubble2D(pointToTransform.position, ((Vector2)pointToTransform.position + pointerBodyDirectionOffset) * 2f, bubbleLifetimeWhileShown, bubbleContent, transform);
			if (skipAllowed)
			{
				Button button = UnityEngine.Object.Instantiate(skipElement);
				button.onClick.AddListener(SkipEvent);
				x.AddFirstElement(bubbleId, (RectTransform)button.transform);
			}
		});
	}

	protected override void OnDestroy()
	{
		Clear();
		base.OnDestroy();
	}

	private void Clear()
	{
		Debug.Log("Clear");
		EnterPlayModeButton enterPlayModeButton = button;
		enterPlayModeButton.enteringPlayMode = (Action)Delegate.Remove(enterPlayModeButton.enteringPlayMode, new Action(OnShown));
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (TextBubbleController x, BaseEventData y) =>
		{
			if (bubbleId != -1)
			{
				x.ClearBubblesOfTypeImmediately(bubbleId);
			}
		});
	}

	protected void OnShown()
	{
		EnterPlayModeButton enterPlayModeButton = button;
		enterPlayModeButton.enteringPlayMode = (Action)Delegate.Remove(enterPlayModeButton.enteringPlayMode, new Action(OnShown));
		shouldBeDelayedDestroyed = true;
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		if (shouldBeDelayedDestroyed)
		{
			FirstTimeEventManager.SetFirstTimeEvent(FirstTimeEvent);
		}
	}
}
