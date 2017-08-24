using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FirstTimeSystemPopupMovement : MonoBehaviour
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
		if (MVInputWrapper.GetBooleanControlDown(KogamaControls.EditMoveForward))
		{
			SetControl(KogamaControls.EditMoveForward);
		}
		if (MVInputWrapper.GetBooleanControlDown(KogamaControls.EditMoveLeft))
		{
			SetControl(KogamaControls.EditMoveLeft);
		}
		if (MVInputWrapper.GetBooleanControlDown(KogamaControls.EditMoveRight))
		{
			SetControl(KogamaControls.EditMoveRight);
		}
		if (MVInputWrapper.GetBooleanControlDown(KogamaControls.EditMoveBackwards))
		{
			SetControl(KogamaControls.EditMoveBackwards);
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
