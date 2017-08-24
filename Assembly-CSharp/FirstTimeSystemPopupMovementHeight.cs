using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FirstTimeSystemPopupMovementHeight : MonoBehaviour
{
	[Serializable]
	private struct ControlImage
	{
		public KogamaControls key;

		public Image control;

		public GameObject checkMark;
	}

	[SerializeField]
	private List<ControlImage> controlImages;

	[SerializeField]
	private float fadeDuration = 0.4f;

	[SerializeField]
	private CanvasGroup group;

	[SerializeField]
	private Color deactivated = new Color(0.75f, 0.75f, 0.75f, 0.75f);

	private float currentFade;

	private bool IsFinished
	{
		get
		{
			return controlImages.Count == 0;
		}
		set
		{
		}
	}

	private void Update()
	{
		if (MVInputWrapper.GetBooleanControlUp(KogamaControls.EditMoveUp))
		{
			SetControl(KogamaControls.EditMoveUp);
		}
		if (MVInputWrapper.GetBooleanControlUp(KogamaControls.EditMoveDown))
		{
			SetControl(KogamaControls.EditMoveDown);
		}
		if (!IsFinished)
		{
			return;
		}
		currentFade += Time.deltaTime;
		group.alpha = 1f - currentFade / fadeDuration;
		if (currentFade >= fadeDuration)
		{
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
			{
				x.PopGroups(UIGroupFlags.Popup);
			});
		}
	}

	private void SetControl(KogamaControls control)
	{
		for (int num = controlImages.Count - 1; num >= 0; num--)
		{
			if (control == controlImages[num].key)
			{
				controlImages[num].control.color = deactivated;
				controlImages[num].checkMark.SetActive(value: true);
				controlImages.RemoveAt(num);
				break;
			}
		}
	}
}
