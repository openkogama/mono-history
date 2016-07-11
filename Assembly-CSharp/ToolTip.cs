using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class ToolTip : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	private bool hasEntered;

	[SerializeField]
	private RectTransform rectTransform;

	[SerializeField]
	private string toolTipText = "_(\"Tooltip\")";

	private float pointerEnterTime;

	private static float timeBeforeToolTip = 0.1f;

	private Vector2 mousePosOnToolTip = default;

	private bool mousePosOnToolTipSet;

	private void Awake()
	{
		toolTipText = TM._(toolTipText);
		TM.LanguageChanged(LanguageLoadedCallback);
	}

	private void Reset()
	{
		rectTransform = GetComponent<RectTransform>();
	}

	public void SetText(string textToBeChanged)
	{
		toolTipText = TM._(textToBeChanged);
		TM.LanguageChanged(LanguageLoadedCallback);
	}

	private void LanguageLoadedCallback()
	{
		toolTipText = TM._(toolTipText);
	}

	private void Update()
	{
		if (!hasEntered)
		{
			return;
		}
		if (!RectTransformUtility.RectangleContainsScreenPoint(rectTransform, MVInputWrapper.GetPointerPosition()))
		{
			hasEntered = false;
		}
		else if (Time.time - pointerEnterTime > timeBeforeToolTip)
		{
			if (!mousePosOnToolTipSet)
			{
				mousePosOnToolTip = MVInputWrapper.GetPointerPosition();
				mousePosOnToolTipSet = true;
			}
			ExecuteEvents.ExecuteHierarchy(gameObject, null, (IHandleToolTip x, BaseEventData y) =>
			{
				x.SendToolTip(mousePosOnToolTip, toolTipText);
			});
		}
		else
		{
			mousePosOnToolTipSet = false;
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		pointerEnterTime = Time.time;
		hasEntered = true;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		pointerEnterTime = Time.time;
		hasEntered = false;
	}
}
