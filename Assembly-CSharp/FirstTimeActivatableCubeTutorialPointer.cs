using MV.WorldObject.MetaData;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FirstTimeActivatableCubeTutorialPointer : FirstTimeActivatableButtonPointer
{
	private bool skipRequested;

	public override bool CanShow
	{
		get
		{
			bool isBlocked = IsBlocked;
			bool activeInHierarchy = gameObject.activeInHierarchy;
			bool flag = !FirstTimeEventManager.HasFirstTimeEventOccured(FirstTimeEvent.BM_CubeTutorialDone);
			return !isBlocked && activeInHierarchy && flag;
		}
	}

	protected override void Start()
	{
		if (!FirstTimeEventManager.HasFirstTimeEventOccured(FirstTimeEvent.BM_CubeTutorialDone))
		{
			button.gameObject.SetActive(value: true);
		}
		base.Start();
	}

	public override void OnShow()
	{
		skipRequested = false;
		button.onClick.AddListener(OnShown);
		CreateBubble();
	}

	private void CreateBubble()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (TextBubbleController x, BaseEventData y) =>
		{
			bubbleId = x.ShowBubble2D(pointToTransform.position, ((Vector2)pointToTransform.position + pointerBodyDirectionOffset) * 2f, bubbleLifetimeWhileShown, bubbleContent, transform);
			if (skipAllowed)
			{
				Button button = Object.Instantiate(skipElement);
				button.onClick.AddListener(OnSkipPressed);
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
		button.onClick.RemoveListener(OnShown);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (TextBubbleController x, BaseEventData y) =>
		{
			x.ClearBubblesWithId(bubbleId);
		});
		if (skipRequested)
		{
			button.gameObject.SetActive(value: false);
		}
	}

	private void OnSkipPressed()
	{
		skipRequested = true;
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IFirstTimeElementActivator x, BaseEventData y) =>
		{
			x.SkipFirstTimeEvent(FirstTimeEvent.BM_CubeTutorialDone, this);
		});
	}
}
