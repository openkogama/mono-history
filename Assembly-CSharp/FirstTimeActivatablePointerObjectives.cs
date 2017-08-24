using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FirstTimeActivatablePointerObjectives : FirstTimeActivatableElementBase
{
	private const string mouseX = "Mouse X";

	private const string mouseY = "Mouse Y";

	[SerializeField]
	private List<RectTransform> winningConditionTransforms;

	[SerializeField]
	private Vector2 pointerBodyDirectionOffset;

	[SerializeField]
	private List<RectTransform> bubbleContent;

	[SerializeField]
	private float bubbleLifetimeWhenVisible = float.MaxValue;

	[SerializeField]
	private float visibleDuration;

	private bool visible;

	private float currentTime;

	private bool isUpdating;

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
		if (!visible)
		{
			return;
		}
		float axis = MVInputWrapper.GetAxis("Mouse X");
		float axis2 = MVInputWrapper.GetAxis("Mouse Y");
		if (axis > 0f || axis2 > 0f)
		{
			isUpdating = true;
		}
		if (isUpdating)
		{
			currentTime += Time.deltaTime;
			if (currentTime >= visibleDuration)
			{
				OnShown();
			}
		}
	}

	public override void OnShow()
	{
		visible = true;
		StartCoroutine(CreateBubble());
	}

	private IEnumerator CreateBubble()
	{
		yield return 0;
		RectTransform target = null;
		for (int i = 0; i < winningConditionTransforms.Count; i++)
		{
			if (winningConditionTransforms[i].gameObject.activeInHierarchy)
			{
				target = winningConditionTransforms[i];
				break;
			}
		}
		if (target == null)
		{
			Destroy();
			yield break;
		}
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (TextBubbleController x, BaseEventData y) =>
		{
			if (bubbleId != -1)
			{
				x.ClearBubblesOfTypeImmediately(bubbleId);
			}
			bubbleId = x.ShowBubble2D(target.position + new Vector3(0f, target.rect.height / 2f, 0f), ((Vector2)target.position + pointerBodyDirectionOffset) * 2f, bubbleLifetimeWhenVisible, bubbleContent, transform);
		});
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
		Destroy();
	}

	private void Destroy()
	{
		Object.Destroy(this);
	}
}
