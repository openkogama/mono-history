using UnityEngine;
using UnityEngine.EventSystems;

public class FirstTimeSystemPopupCamera : MonoBehaviour
{
	[SerializeField]
	private float mouseMoveDistance = 30f;

	[SerializeField]
	private float fadeDuration = 0.4f;

	[SerializeField]
	private CanvasGroup group;

	[SerializeField]
	private GameObject activeImage;

	[SerializeField]
	private GameObject inActiveImage;

	private float mouseMoved;

	private Vector3 mousePos = new Vector3(0f, 0f, 0f);

	private float currentFade;

	private void Update()
	{
		if (MVInputWrapper.GetBooleanControlDown(KogamaControls.PointerSelectAlt))
		{
			mousePos = MVInputWrapper.GetPointerPosition();
			inActiveImage.SetActive(value: false);
			activeImage.SetActive(value: true);
		}
		else if (MVInputWrapper.GetBooleanControl(KogamaControls.PointerSelectAlt))
		{
			mouseMoved += (MVInputWrapper.GetPointerPosition() - mousePos).magnitude;
			mousePos = MVInputWrapper.GetPointerPosition();
		}
		if (MVInputWrapper.GetBooleanControlUp(KogamaControls.PointerSelectAlt))
		{
			inActiveImage.SetActive(value: true);
			activeImage.SetActive(value: false);
		}
		if (!(mouseMoved >= mouseMoveDistance))
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
}
