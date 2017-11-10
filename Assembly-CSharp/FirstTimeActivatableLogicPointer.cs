using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FirstTimeActivatableLogicPointer : FirstTimeActivatableElementBase
{
	private bool hasPlacedObject;

	private bool showing;

	private WorldObjectClientRef placedWo;

	private int bubbleId = -1;

	private EditorStateMachine editorStateMachine;

	private bool hasButtonBeenAdded;

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

	public override bool CanShow => !IsBlocked && gameObject.activeInHierarchy && hasPlacedObject;

	protected override void Start()
	{
		base.Start();
		DesktopEditModeController componentInParent = GetComponentInParent<DesktopEditModeController>();
		editorStateMachine = componentInParent.EditModeStateMachine;
	}

	public override void OnShow()
	{
		base.OnShow();
		showing = placedWo.WorldObjectClient != null;
		if (!showing)
		{
			return;
		}
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (TextBubbleController x, BaseEventData y) =>
		{
			bubbleId = x.ShowBubble3D(placedWo.WorldObjectClient.WorldPosition + bubbleWorldSpaceOffset, bubbleLifetimeWhileShown, bubbleContent, transform, offset);
			if (skipAllowed && !hasButtonBeenAdded)
			{
				hasButtonBeenAdded = true;
				Button button = Object.Instantiate(skipElement);
				button.onClick.AddListener(SkipEvent);
				x.AddElement(bubbleId, (RectTransform)button.transform);
			}
		});
	}

	private void Update()
	{
		if (showing)
		{
			if (placedWo.WorldObjectClient == null)
			{
				OnShown();
				return;
			}
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (TextBubbleController x, BaseEventData y) =>
			{
				x.UpdatePosition3D(bubbleId, placedWo.WorldObjectClient.WorldPosition + bubbleWorldSpaceOffset, offset);
			});
			if (hasPlacedObject && editorStateMachine.CurEvent == EditorEvent.ObjectSelected && MVInputWrapper.GetBooleanControlUp(KogamaControls.PointerSelectAlt))
			{
				OnShown();
			}
		}
		else if (editorStateMachine.CurEvent == EditorEvent.ESInsert)
		{
			placedWo = MVGameControllerBase.WOCM.GetWorldObjectClientRef(editorStateMachine.SingleSelectedWO.Id);
			hasPlacedObject = true;
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IFirstTimeElementActivator x, BaseEventData y) =>
			{
				x.RequestEvaluateActivatableElements();
			});
		}
	}

	protected override void OnDestroy()
	{
		if (bubbleId != -1)
		{
			Clear();
		}
		editorStateMachine = null;
		base.OnDestroy();
	}

	private void Clear()
	{
		Debug.Log("Clear");
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
