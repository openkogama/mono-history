using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FirstTimeActivatablePopupPressKeyToSkip : FirstTimeActivatableElementBase
{
	[SerializeField]
	private GameObject popup;

	[SerializeField]
	private float minLifeTime = 1f;

	[SerializeField]
	private List<KogamaControls> keysToDetect;

	private float currentlifeTime;

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
		if (!visible)
		{
			return;
		}
		currentlifeTime += Time.deltaTime;
		if (!(currentlifeTime >= minLifeTime))
		{
			return;
		}
		for (int i = 0; i < keysToDetect.Count; i++)
		{
			if (MVInputWrapper.GetBooleanControlDown(keysToDetect[i]))
			{
				OnShown();
				break;
			}
		}
	}

	public override void OnShow()
	{
		visible = true;
		Debug.LogWarning("This class does not implement skippable functionality");
		GameObject instantiatedPopup = Object.Instantiate(popup);
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Push(instantiatedPopup, UIPushOption.None, null, UIGroupFlags.Popup);
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
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.PopGroups(UIGroupFlags.Popup);
		});
	}

	private void OnShown()
	{
		Clear();
		FirstTimeEventManager.SetFirstTimeEvent(FirstTimeEvent);
		Object.Destroy(this);
	}
}
