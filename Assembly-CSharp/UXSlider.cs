using System;
using UnityEngine;

[AddComponentMenu("UX/Elements/Slider")]
public class UXSlider : MonoBehaviour
{
	public delegate void OnValueChangedDelegate(UXSlider slider);

	public delegate void OnValueChangedIntermediateDelegate(UXSlider slider, float intermediateValue);

	public float value;

	public float minValue;

	public float maxValue = 1f;

	public float step;

	public UXSlideOrientation slideOrientation;

	public UXSliderBar bar;

	public GameObject slider;

	private float width;

	private float height;

	private Vector3 sliderSize;

	private float slidingLength;

	private Vector3 sliderOffset;

	private float slidingLengthWorld;

	private bool isSliding;

	private float slidingStartValue;

	private float intermediateValue;

	private Vector3 slidingStartPositionWorld;

	private UXScreen screen;

	private UXInputDispatcher inputDispatcher;

	private bool isInitialized;

	public OnValueChangedDelegate OnValueChanged;

	public OnValueChangedIntermediateDelegate OnValueChangedIntermediate;

	public float Value
	{
		get
		{
			return value;
		}
		set
		{
			ChangeValue(value);
		}
	}

	public float MinValue
	{
		get
		{
			return minValue;
		}
		set
		{
			ChangeMinValue(value);
		}
	}

	public float MaxValue
	{
		get
		{
			return maxValue;
		}
		set
		{
			ChangeMaxValue(value);
		}
	}

	public void Awake()
	{
		screen = UXUtils.FindObjectOfType<UXScreen>();
		UXScreen uXScreen = screen;
		uXScreen.OnResize = (UXScreen.OnResizeDelegate)Delegate.Combine(uXScreen.OnResize, new UXScreen.OnResizeDelegate(OnResize));
		UXMouseClickObject component = slider.GetComponent<UXMouseClickObject>();
		component.OnMouseDown = (UXMouseClickObject.OnMouseDownDelegate)Delegate.Combine(component.OnMouseDown, new UXMouseClickObject.OnMouseDownDelegate(HandleOnMouseDownIndicator));
		inputDispatcher = UXUtils.FindObjectOfType<UXInputDispatcher>();
		UXInputDispatcher uXInputDispatcher = inputDispatcher;
		uXInputDispatcher.OnMouseButton = (UXInputDispatcher.OnMouseButtonDelegate)Delegate.Combine(uXInputDispatcher.OnMouseButton, new UXInputDispatcher.OnMouseButtonDelegate(OnMouseDown));
		UXInputDispatcher uXInputDispatcher2 = inputDispatcher;
		uXInputDispatcher2.OnMouseButtonUp = (UXInputDispatcher.OnMouseButtonUpDelegate)Delegate.Combine(uXInputDispatcher2.OnMouseButtonUp, new UXInputDispatcher.OnMouseButtonUpDelegate(OnMouseUp));
	}

	public void Start()
	{
		Initialize();
	}

	public void Initialize()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		if (!isInitialized)
		{
			sliderSize = slider.transform.localScale;
			width = bar.width;
			height = bar.height;
			sliderOffset = new Vector3(sliderSize.x / 2f, (0f - height) / 2f, 0f);
			slidingLength = width - sliderOffset.x * 2f;
			ChangeValue(value);
			ChangeMinValue(minValue);
			ChangeMaxValue(maxValue);
			UpdateSlidingLengthWorld();
		}
	}

	private void OnResize()
	{
		UpdateSlidingLengthWorld();
		UpdateSliderPosition(value);
	}

	private void UpdateSlidingLengthWorld()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		slidingLengthWorld = slidingLength * screen.Scale * ((Component)this).transform.localScale.x;
	}

	private bool HandleOnMouseDownIndicator(UXMouseClickObject mouseClickObject, Vector3 mousePositionWorld)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		isSliding = true;
		slidingStartValue = value;
		intermediateValue = value;
		slidingStartPositionWorld = mousePositionWorld;
		return true;
	}

	private void OnMouseDown(Vector3 mousePositionWorld)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		if (isSliding)
		{
			float num = ((slideOrientation != UXSlideOrientation.Horizontal) ? (0f - (mousePositionWorld - slidingStartPositionWorld).y) : (mousePositionWorld - slidingStartPositionWorld).x);
			intermediateValue = FloorToStep(Mathf.Clamp(slidingStartValue + (maxValue - minValue) * num / slidingLengthWorld, minValue, maxValue));
			UpdateSliderPosition(intermediateValue);
			if (OnValueChangedIntermediate != null)
			{
				OnValueChangedIntermediate(this, intermediateValue);
			}
		}
	}

	private void OnMouseUp(Vector3 mousePositionWorld)
	{
		if (isSliding)
		{
			value = intermediateValue;
			UpdateSliderPosition(value);
			if (OnValueChanged != null)
			{
				OnValueChanged(this);
			}
			isSliding = false;
		}
	}

	private void UpdateSliderPosition(float value)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		float num = (Mathf.Clamp(value, minValue, maxValue) - minValue) / (maxValue - minValue);
		Vector3 val = ((slideOrientation != UXSlideOrientation.Horizontal) ? Vector3.down : Vector3.right);
		slider.transform.localPosition = sliderOffset + val * slidingLength * num;
	}

	private float FloorToStep(float value)
	{
		if (step == 0f)
		{
			return value;
		}
		return (float)Mathf.FloorToInt(value / step) * step;
	}

	private void ChangeValue(float value)
	{
		this.value = value;
		UpdateSliderPosition(value);
	}

	private void ChangeMinValue(float minValue)
	{
		this.minValue = minValue;
		UpdateSliderPosition(value);
	}

	private void ChangeMaxValue(float maxValue)
	{
		this.maxValue = maxValue;
		UpdateSliderPosition(value);
	}
}
