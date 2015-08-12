using System;
using UnityEngine;

public class UXSlider : UXGUIElement, IUXContainer
{
	public delegate void OnValueChangedDelegate(UXSlider slider);

	public delegate void OnValueChangedIntermediateDelegate(UXSlider slider, float intermediateValue);

	public OnValueChangedDelegate OnValueChanged;

	public OnValueChangedIntermediateDelegate OnValueChangedIntermediate;

	[SerializeField]
	private float _value;

	[SerializeField]
	private float _minValue;

	[SerializeField]
	private float _maxValue = 1f;

	[SerializeField]
	private float _step;

	private float _intermediateValue;

	[SerializeField]
	private Material _sliderBarMaterial;

	[SerializeField]
	private bool _sliderBarIs9Patch;

	[SerializeField]
	private Material _sliderMaterial;

	[SerializeField]
	private bool _sliderIs9Patch;

	private Vector2 sliderSize;

	private float startValue;

	private Vector3 startSlide;

	private bool isSliding;

	private bool buttonPressed;

	private bool isInitialized;

	private UXInputDispatcher inputDispatcher;

	public float Value
	{
		get
		{
			return _value;
		}
		set
		{
			_value = value;
			UpdateSliderPosition(_value);
		}
	}

	public float MinValue
	{
		get
		{
			return _minValue;
		}
		set
		{
			_minValue = value;
			UpdateSliderPosition(_value);
		}
	}

	public float MaxValue
	{
		get
		{
			return _maxValue;
		}
		set
		{
			_maxValue = value;
			UpdateSliderPosition(_value);
		}
	}

	public UXSliderBar Bar { get; private set; }

	public UXSliderButton Button { get; private set; }

	public void Start()
	{
		inputDispatcher = UXUtils.UXInputDispatcher;
		UXInputDispatcher uXInputDispatcher = inputDispatcher;
		uXInputDispatcher.OnMouseButton = (UXInputDispatcher.OnMouseButtonDelegate)Delegate.Combine(uXInputDispatcher.OnMouseButton, new UXInputDispatcher.OnMouseButtonDelegate(OnMouseDown));
		UXInputDispatcher uXInputDispatcher2 = inputDispatcher;
		uXInputDispatcher2.OnMouseButtonUp = (UXInputDispatcher.OnMouseButtonUpDelegate)Delegate.Combine(uXInputDispatcher2.OnMouseButtonUp, new UXInputDispatcher.OnMouseButtonUpDelegate(OnMouseUp));
		Initialize();
	}

	public void Initialize()
	{
		if (!isInitialized)
		{
			isInitialized = true;
			CreateBar();
			CreateButton();
			SetVisible(Visible);
		}
	}

	private void CreateBar()
	{
		Bar = CreateBaseObject("SliderBar").AddComponent<UXSliderBar>();
		Bar.transform.localPosition = new Vector3(Width / 2f, Height / 2f, 0f) - Alignment;
		if (_sliderBarMaterial != null)
		{
			Bar.SetMaterial(_sliderBarMaterial);
			Bar.uses9PatchMaterial = _sliderBarIs9Patch;
		}
		Bar.SetSize(Width, Height);
		UXSliderBar bar = Bar;
		bar.OnSliderBarClick = (UXSliderBar.OnSliderBarClickDelegate)Delegate.Combine(bar.OnSliderBarClick, new UXSliderBar.OnSliderBarClickDelegate(MoveSlider));
	}

	private void CreateButton()
	{
		Button = CreateBaseObject("SliderButton").AddComponent<UXSliderButton>();
		if (_sliderMaterial != null)
		{
			Button.SetMaterial(_sliderMaterial);
			Button.uses9PatchMaterial = _sliderIs9Patch;
		}
		if (sliderSize == Vector2.zero)
		{
			if (Width > Height)
			{
				sliderSize = new Vector2(1.1f, Height);
			}
			else
			{
				sliderSize = new Vector2(Width, 1.1f);
			}
		}
		UpdateSliderSize();
		UXMouseClickObject uXMouseClickObject = Button.gameObject.AddComponent<UXMouseClickObject>();
		uXMouseClickObject.OnMouseDown = (UXMouseClickObject.OnMouseDownDelegate)Delegate.Combine(uXMouseClickObject.OnMouseDown, (UXMouseClickObject.OnMouseDownDelegate)((UXMouseClickObject click, Vector3 pos) =>
		{
			buttonPressed = true;
			return true;
		}));
	}

	private GameObject CreateBaseObject(string name)
	{
		GameObject gameObject = new GameObject(name);
		gameObject.layer = LayerMask.NameToLayer("UXElement");
		gameObject.transform.parent = transform;
		gameObject.transform.localScale = Vector3.one;
		gameObject.AddComponent<MeshRenderer>();
		return gameObject;
	}

	public override void SetVisible(bool visible)
	{
		base.SetVisible(visible);
		if (Bar != null)
		{
			Bar.SetVisible(visible);
		}
		if (Button != null)
		{
			Button.SetVisible(visible);
		}
	}

	public override void SetSize(float width, float height)
	{
		base.SetSize(width, height);
		Bar.transform.localPosition = new Vector3(Width / 2f, Height / 2f, 0f) - Alignment;
		Bar.SetSize(Width, Height);
	}

	public override void SetAlpha(float alpha, string materialProperty)
	{
		Bar.SetAlpha(alpha, materialProperty);
		Button.SetAlpha(alpha, materialProperty);
	}

	public void SetSliderSize(float width, float height)
	{
		sliderSize = new Vector2(width, height);
		if (Button != null)
		{
			UpdateSliderSize();
		}
	}

	public void UpdateSliderSize()
	{
		Button.SetSize(sliderSize);
		if (_value < _minValue)
		{
			_value = _minValue;
		}
		if (_value > _maxValue)
		{
			_value = _maxValue;
		}
		Value = _value;
		MinValue = _minValue;
		MaxValue = _maxValue;
	}

	private void MoveSlider(Vector3 mousePositionWorld)
	{
		isSliding = true;
		startValue = _value - _minValue;
		startSlide = Button.transform.localPosition + Button.transform.InverseTransformPoint(mousePositionWorld) + Alignment;
	}

	private void OnMouseDown(Vector3 mousePositionWorld)
	{
		if (!(this == null) && isSliding)
		{
			Vector3 vector = transform.InverseTransformPoint(mousePositionWorld);
			vector += Alignment;
			float num = Height;
			float num2 = Width;
			float num3 = 0f;
			if (buttonPressed)
			{
				vector -= startSlide;
				num -= Button.Height;
				num2 -= Button.Width;
				num3 = startValue;
			}
			else
			{
				vector.y = 0f - num + vector.y;
			}
			float num4 = ((!(num2 > num)) ? ((0f - vector.y) / num) : (vector.x / num2));
			_intermediateValue = RoundToStep(Mathf.Clamp(num3 + _minValue + (_maxValue - _minValue) * num4, _minValue, _maxValue));
			UpdateSliderPosition(_intermediateValue);
			if (!buttonPressed)
			{
				buttonPressed = true;
				startValue = _intermediateValue - _minValue;
				startSlide = Button.transform.localPosition + Button.transform.InverseTransformPoint(mousePositionWorld) + Alignment;
			}
			if (OnValueChangedIntermediate != null)
			{
				OnValueChangedIntermediate(this, _intermediateValue);
			}
		}
	}

	private void OnMouseUp(Vector3 mousePositionWorld)
	{
		if (isSliding)
		{
			_value = _intermediateValue;
			UpdateSliderPosition(_value);
			if (OnValueChanged != null)
			{
				OnValueChanged(this);
			}
			isSliding = false;
			buttonPressed = false;
		}
	}

	private void UpdateSliderPosition(float value)
	{
		if (!(this == null))
		{
			if (!isInitialized)
			{
				Initialize();
			}
			float num = (Mathf.Clamp(value, _minValue, _maxValue) - _minValue) / (_maxValue - _minValue);
			float num2 = Width - Button.Width;
			float num3 = Height - Button.Height;
			if (Width > Height)
			{
				num2 *= num;
			}
			else
			{
				num3 *= 1f - num;
			}
			Button.transform.localPosition = new Vector3(num2 + Button.Width / 2f, num3 + Button.Height / 2f, -0.1f) - Alignment;
		}
	}

	private float RoundToStep(float value)
	{
		if (_step == 0f)
		{
			return value;
		}
		return (float)Mathf.RoundToInt(value / _step) * _step;
	}
}
