using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FirstTimeActivatablePointerAvatarBody : FirstTimeActivatableElementBase
{
	private bool showing;

	private bool hasButtonBeenAdded;

	private AvatarEditModeBodyController bodyController;

	[SerializeField]
	private List<RectTransform> bubbleContent;

	[SerializeField]
	private float bubbleLifetimeWhileShown = float.MaxValue;

	[SerializeField]
	private Vector3 bubbleWorldSpaceOffset = new Vector3(0f, 1.3f, 0f);

	[SerializeField]
	private Button skipElement;

	[SerializeField]
	protected bool skipAllowed = true;

	[SerializeField]
	private Vector2 offset;

	private int bubbleId = -1;

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
		if (showing && !(bodyController.CurrentBody.GameObject == null))
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (TextBubbleController x, BaseEventData y) =>
			{
				x.UpdatePosition3D(bubbleId, bodyController.CurrentBody.WorldPosition + bubbleWorldSpaceOffset, offset);
			});
			if (bodyController.CurrentBody.Animation.IsPlaying("TPose"))
			{
				Coroutines.Start(WaitForFrames.Frames(1, OnShown));
			}
		}
	}

	public override void OnShow()
	{
		showing = true;
		bodyController = GetComponentInParent<AvatarEditModeBodyController>();
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (TextBubbleController x, BaseEventData y) =>
		{
			bubbleId = x.ShowBubble3D(bodyController.CurrentBody.WorldPosition + bubbleWorldSpaceOffset, bubbleLifetimeWhileShown, bubbleContent, transform, offset);
			if (skipAllowed && !hasButtonBeenAdded)
			{
				hasButtonBeenAdded = true;
				Button button = Object.Instantiate(skipElement);
				button.onClick.AddListener(SkipEvent);
			}
		});
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		showing = false;
	}

	protected override void OnDestroy()
	{
		if (bubbleId != -1)
		{
			Clear();
		}
		bodyController = null;
		base.OnDestroy();
	}

	private void Clear()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (TextBubbleController x, BaseEventData y) =>
		{
			x.ClearBubblesWithId(bubbleId);
		});
	}

	private void OnShown()
	{
		Debug.Log("OnShown done");
		showing = false;
		Clear();
		Object.Destroy(this);
		FirstTimeEventManager.SetFirstTimeEvent(FirstTimeEvent);
	}
}
