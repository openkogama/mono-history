using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FirstTimeActivatablePopupTimed : FirstTimeActivatableElementBase
{
	[SerializeField]
	private CanvasGroup popupPrefab;

	[SerializeField]
	private float visibleDuration = 2f;

	[SerializeField]
	private float fadeTime = 0.3f;

	[SerializeField]
	private List<UIPushOption> pushOptions;

	[SerializeField]
	private bool inputRequired = true;

	protected bool isShown;

	private float currentTime;

	private CanvasGroup createdPopup;

	private const string mouseX = "Mouse X";

	private const string mouseY = "Mouse Y";

	private bool isFading;

	private bool isUpdating;

	private bool destroyCreatedPopup;

	public override bool CanShow => !IsBlocked && gameObject.activeInHierarchy;

	private void Update()
	{
		if (!isShown)
		{
			return;
		}
		float axis = MVInputWrapper.GetAxis("Mouse X");
		float axis2 = MVInputWrapper.GetAxis("Mouse Y");
		if (axis > 0f || axis2 > 0f || isFading || !inputRequired)
		{
			isUpdating = true;
		}
		if (!isUpdating)
		{
			return;
		}
		currentTime += Time.deltaTime;
		if (currentTime >= visibleDuration)
		{
			if (destroyCreatedPopup)
			{
				Object.Destroy(this);
				return;
			}
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.PopGroups(UIGroupFlags.Popup);
			});
		}
		else if (currentTime >= visibleDuration - fadeTime)
		{
			createdPopup.alpha = 1f - (currentTime - (visibleDuration - fadeTime)) / fadeTime;
			isFading = true;
		}
	}

	public override void OnShow()
	{
		if (!isShown)
		{
			isShown = true;
			CreatePopup();
			PushToStack();
		}
	}

	protected void ParentPopupToGameObject()
	{
		createdPopup.transform.SetParent(transform, worldPositionStays: false);
		destroyCreatedPopup = true;
	}

	protected override void OnDestroy()
	{
		if (destroyCreatedPopup)
		{
			Object.Destroy(createdPopup.gameObject);
		}
		FirstTimeEventManager.SetFirstTimeEvent(FirstTimeEvent);
		base.OnDestroy();
	}

	protected void CreatePopup()
	{
		Debug.LogWarning("This class does not implement skippable functionality");
		createdPopup = Object.Instantiate(popupPrefab);
	}

	private void PushToStack()
	{
		UIPushOption options = UIPushOption.None;
		if (pushOptions.Count > 0)
		{
			options = pushOptions[0];
			for (int i = 1; i < pushOptions.Count; i++)
			{
				options |= pushOptions[i];
			}
		}
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(createdPopup.gameObject, options, OnPop, UIGroupFlags.Popup);
		});
	}

	private void OnPop()
	{
		Object.Destroy(this);
	}
}
