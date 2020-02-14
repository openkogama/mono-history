using UnityEngine;

public class ToggleButtonAnimation : MonoBehaviour
{
	[SerializeField]
	private RectTransform toggleOffMaskTransform;

	[SerializeField]
	private RectTransform toggleOffContentTransform;

	[SerializeField]
	private RectTransform toggleButtonTransform;

	[SerializeField]
	private bool isToggleOn;

	[SerializeField]
	private float toggleInterpolationDuration = 0.1f;

	[SerializeField]
	private float toggleButtonMoveAmount = 131f;

	private float toggleOffOriginalPositionX;

	private float toggleButtonOriginalPositionX;

	private float interpolateToggleMaskStartPositionX;

	private float interpolateToggleMaskNewPositionX;

	private float interpolateToggleContentStartPositionX;

	private float interpolateToggleContentNewPositionX;

	private float interpolateToggleButtonStartPositionX;

	private float interpolateToggleButtonNewPositionX;

	private float interpolationStartTime;

	private bool isInitialized;

	public bool IsToggleOn => isToggleOn;

	private void Start()
	{
		Initialize();
	}

	public void Initialize()
	{
		if (!isInitialized)
		{
			isInitialized = true;
			toggleOffOriginalPositionX = toggleOffMaskTransform.localPosition.x;
			toggleButtonOriginalPositionX = toggleButtonTransform.localPosition.x;
			if (IsToggleOn)
			{
				SetToggleOnWithoutInterpolation();
			}
			interpolateToggleMaskNewPositionX = toggleOffMaskTransform.localPosition.x;
			interpolateToggleContentNewPositionX = toggleOffContentTransform.localPosition.x;
			interpolateToggleButtonNewPositionX = toggleButtonTransform.localPosition.x;
		}
	}

	public void ToggleOn()
	{
		if (!isToggleOn)
		{
			Toggle();
		}
	}

	public void ToggleOff()
	{
		if (isToggleOn)
		{
			Toggle();
		}
	}

	public void Toggle()
	{
		isToggleOn = !isToggleOn;
		HandleToggle();
	}

	public void SetToggleOnWithoutInterpolation()
	{
		isToggleOn = true;
		Vector3 localPosition = toggleOffMaskTransform.localPosition;
		localPosition.x = toggleOffOriginalPositionX + toggleOffMaskTransform.rect.width;
		toggleOffMaskTransform.localPosition = localPosition;
		localPosition = toggleOffContentTransform.localPosition;
		localPosition.x = toggleOffOriginalPositionX - toggleOffContentTransform.rect.width;
		toggleOffContentTransform.localPosition = localPosition;
		localPosition = toggleButtonTransform.localPosition;
		localPosition.x = toggleButtonOriginalPositionX + toggleButtonMoveAmount;
		toggleButtonTransform.localPosition = localPosition;
		interpolateToggleMaskNewPositionX = toggleOffMaskTransform.localPosition.x;
		interpolateToggleContentNewPositionX = toggleOffContentTransform.localPosition.x;
		interpolateToggleButtonNewPositionX = toggleButtonTransform.localPosition.x;
	}

	public void SetToggleOffWithoutInterpolation()
	{
		isToggleOn = false;
		Vector3 localPosition = toggleOffMaskTransform.localPosition;
		localPosition.x = toggleOffOriginalPositionX;
		toggleOffMaskTransform.localPosition = localPosition;
		localPosition = toggleOffContentTransform.localPosition;
		localPosition.x = toggleOffOriginalPositionX;
		toggleOffContentTransform.localPosition = localPosition;
		localPosition = toggleButtonTransform.localPosition;
		localPosition.x = toggleButtonOriginalPositionX;
		toggleButtonTransform.localPosition = localPosition;
		interpolateToggleMaskNewPositionX = toggleOffMaskTransform.localPosition.x;
		interpolateToggleContentNewPositionX = toggleOffContentTransform.localPosition.x;
		interpolateToggleButtonNewPositionX = toggleButtonTransform.localPosition.x;
	}

	private void HandleToggle()
	{
		interpolationStartTime = Time.time;
		interpolateToggleMaskNewPositionX = toggleOffOriginalPositionX;
		interpolateToggleMaskStartPositionX = toggleOffOriginalPositionX;
		if (isToggleOn)
		{
			interpolateToggleMaskNewPositionX += toggleOffMaskTransform.rect.width;
		}
		else
		{
			interpolateToggleMaskStartPositionX += toggleOffMaskTransform.rect.width;
		}
		interpolateToggleContentNewPositionX = toggleOffOriginalPositionX;
		interpolateToggleContentStartPositionX = toggleOffOriginalPositionX;
		if (isToggleOn)
		{
			interpolateToggleContentNewPositionX -= toggleOffMaskTransform.rect.width;
		}
		else
		{
			interpolateToggleContentStartPositionX -= toggleOffMaskTransform.rect.width;
		}
		interpolateToggleButtonNewPositionX = toggleButtonOriginalPositionX;
		interpolateToggleButtonStartPositionX = toggleButtonOriginalPositionX;
		if (isToggleOn)
		{
			interpolateToggleButtonNewPositionX += toggleButtonMoveAmount;
		}
		else
		{
			interpolateToggleButtonStartPositionX += toggleButtonMoveAmount;
		}
	}

	private void Update()
	{
		float t = (Time.time - interpolationStartTime) / (toggleInterpolationDuration + Time.deltaTime);
		Vector3 localPosition = toggleOffMaskTransform.localPosition;
		localPosition.x = Mathf.Lerp(interpolateToggleMaskStartPositionX, interpolateToggleMaskNewPositionX, t);
		toggleOffMaskTransform.localPosition = localPosition;
		localPosition = toggleOffMaskTransform.localPosition;
		localPosition.x = Mathf.Lerp(interpolateToggleContentStartPositionX, interpolateToggleContentNewPositionX, t);
		toggleOffContentTransform.localPosition = localPosition;
		localPosition = toggleButtonTransform.localPosition;
		localPosition.x = Mathf.Lerp(interpolateToggleButtonStartPositionX, interpolateToggleButtonNewPositionX, t);
		toggleButtonTransform.localPosition = localPosition;
	}
}
