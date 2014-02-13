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
		inputDispatcher = UXUtils.FindGUIObjectOfType<UXInputDispatcher>();
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
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		Bar = CreateBaseObject("SliderBar").AddComponent<UXSliderBar>();
		((Component)Bar).transform.localPosition = new Vector3(Width / 2f, Height / 2f, 0f) - Alignment;
		if ((Object)(object)_sliderBarMaterial != (Object)null)
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
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		Button = CreateBaseObject("SliderButton").AddComponent<UXSliderButton>();
		if ((Object)(object)_sliderMaterial != (Object)null)
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
		UXMouseClickObject uXMouseClickObject = ((Component)Button).gameObject.AddComponent<UXMouseClickObject>();
		uXMouseClickObject.OnMouseDown = (UXMouseClickObject.OnMouseDownDelegate)Delegate.Combine(uXMouseClickObject.OnMouseDown, (UXMouseClickObject.OnMouseDownDelegate)((UXMouseClickObject click, Vector3 pos) =>
		{
			buttonPressed = true;
			return true;
		}));
	}

	private GameObject CreateBaseObject(string name)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected Obj, but got Unknown
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = new GameObject(name);
		val.layer = LayerMask.NameToLayer("UXElement");
		val.transform.parent = ((Component)this).transform;
		val.transform.localScale = Vector3.one;
		val.AddComponent<MeshRenderer>();
		return val;
	}

	public override void SetVisible(bool visible)
	{
		base.SetVisible(visible);
		if ((Object)(object)Bar != (Object)null)
		{
			Bar.SetVisible(visible);
		}
		if ((Object)(object)Button != (Object)null)
		{
			Button.SetVisible(visible);
		}
	}

	public override void SetSize(float width, float height)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		base.SetSize(width, height);
		((Component)Bar).transform.localPosition = new Vector3(Width / 2f, Height / 2f, 0f) - Alignment;
		Bar.SetSize(Width, Height);
	}

	public override void SetAlpha(float alpha, string materialProperty)
	{
		Bar.SetAlpha(alpha, materialProperty);
		Button.SetAlpha(alpha, materialProperty);
	}

	public void SetSliderSize(float width, float height)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		sliderSize = new Vector2(width, height);
		if ((Object)(object)Button != (Object)null)
		{
			UpdateSliderSize();
		}
	}

	public void UpdateSliderSize()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		isSliding = true;
		startValue = _value - _minValue;
		startSlide = ((Component)Button).transform.localPosition + ((Component)Button).transform.InverseTransformPoint(mousePositionWorld) + Alignment;
	}

	private void OnMouseDown(Vector3 mousePositionWorld)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)this == (Object)null) && isSliding)
		{
			Vector3 val = ((Component)this).transform.InverseTransformPoint(mousePositionWorld);
			val += Alignment;
			float num = Height;
			float num2 = Width;
			float num3 = 0f;
			if (buttonPressed)
			{
				val -= startSlide;
				num -= Button.Height;
				num2 -= Button.Width;
				num3 = startValue;
			}
			else
			{
				val.y = 0f - num + val.y;
			}
			float num4 = ((!(num2 > num)) ? ((0f - val.y) / num) : (val.x / num2));
			_intermediateValue = RoundToStep(Mathf.Clamp(num3 + _minValue + (_maxValue - _minValue) * num4, _minValue, _maxValue));
			UpdateSliderPosition(_intermediateValue);
			if (!buttonPressed)
			{
				buttonPressed = true;
				startValue = _intermediateValue - _minValue;
				startSlide = ((Component)Button).transform.localPosition + ((Component)Button).transform.InverseTransformPoint(mousePositionWorld) + Alignment;
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
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)this == (Object)null))
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
			((Component)Button).transform.localPosition = new Vector3(num2 + Button.Width / 2f, num3 + Button.Height / 2f, -0.1f) - Alignment;
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
