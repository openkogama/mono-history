using System;
using UnityEngine;

public abstract class UXTextInputElement : UXGUIElement, IInputHandler, IUXContainer
{
	public delegate void OnValueChangedDelegate(string value);

	public delegate void OnCharacterEntryDelegate(char c);

	public delegate void OnCharacterDeleteDelegate();

	public delegate void OnFocusChangeDelegate(bool hasFocus);

	public OnValueChangedDelegate OnValueChanged;

	public OnCharacterEntryDelegate OnCharacterEntry;

	public OnCharacterDeleteDelegate OnCharacterDelete;

	public OnFocusChangeDelegate OnFocusChange;

	protected float REPEAT_INTERVAL = 0.05f;

	protected float REPEAT_INTERVAL_FIRST = 0.3f;

	protected float TEXT_INDENT = 0.5f;

	protected char ESCAPE_CHAR = '\u001b';

	protected char BACKSPACE_CHAR = "\b"[0];

	protected char NEWLINE_CHAR = "\n"[0];

	protected char CARRAGE_RETURN_CHAR = "\r"[0];

	protected char TAB_CHAR = "\t"[0];

	private bool _isInitialized;

	private float _repeatTime;

	protected float textOffset;

	protected bool multiLine;

	private UXView _view;

	[SerializeField]
	private string _text = string.Empty;

	private bool _hasFocus;

	protected UXText _uiText;

	protected UXPlane _uiCursor;

	protected UXFocusObject _focusObject;

	protected UXTextSizeCalculator _textCalculator;

	[SerializeField]
	private Material _cursorMaterial;

	[SerializeField]
	protected float _textScale = 1f;

	[SerializeField]
	private int _maxCharLength = int.MaxValue;

	public TextInputType allowedInput;

	private int _cursorIndex;

	private UXView View
	{
		get
		{
			if ((Object)(object)_view == (Object)null)
			{
				_view = UXUtils.FindComponentInParents(typeof(UXView), ((Component)this).transform.parent) as UXView;
			}
			return _view;
		}
	}

	public string Text
	{
		get
		{
			return _text;
		}
		set
		{
			_text = value;
			if (_isInitialized)
			{
				CursorIndex = Mathf.Max(0, _text.Length - 1);
				NotifyValueChanged();
			}
		}
	}

	public virtual bool HasFocus
	{
		get
		{
			return _hasFocus;
		}
		protected set
		{
			_hasFocus = value;
			if ((Object)(object)_uiCursor != (Object)null)
			{
				_uiCursor.SetVisible(_hasFocus);
			}
			NotifyFocusChange(_hasFocus);
			if (MVGameController.Instance.IngameController != null)
			{
				MVGameController.Instance.IngameController.SetIgnoreKeyInput(_hasFocus);
			}
			CursorIndex = _cursorIndex;
		}
	}

	protected int CursorIndex
	{
		get
		{
			return _cursorIndex;
		}
		set
		{
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			_cursorIndex = Mathf.Clamp(value, 0, _text.Length);
			if (GetActualText(_text).Length == 0)
			{
				_cursorIndex = 0;
			}
			if (_isInitialized)
			{
				UpdateText();
				if ((Object)(object)_uiCursor != (Object)null)
				{
					((Component)_uiCursor).transform.localPosition = GetCursorPosition(GetActualText(_text));
				}
			}
		}
	}

	public int Priority => InputHandlerPriority.UI_FOCUS;

	public override void Awake()
	{
		base.Awake();
		_textCalculator = UXUtils.FindGUIObjectOfType<UXTextSizeCalculator>();
		UXUtils.FindObjectOfType<MVInputHandlerPrioritizer>().Register(this);
		InitializeFocusObject();
	}

	public virtual void Start()
	{
		CreateTextBoxBG();
		CreateText();
		CreateCursor();
		UpdateText();
		_isInitialized = true;
	}

	private void InitializeFocusObject()
	{
		_focusObject = ((Component)this).gameObject.AddComponent<UXFocusObject>();
		UXFocusObject focusObject = _focusObject;
		focusObject.OnFocusEnter = (UXFocusObject.OnFocusEnterDelegate)Delegate.Combine(focusObject.OnFocusEnter, new UXFocusObject.OnFocusEnterDelegate(HandleOnFocusEnter));
		UXFocusObject focusObject2 = _focusObject;
		focusObject2.OnFocusExit = (UXFocusObject.OnFocusExitDelegate)Delegate.Combine(focusObject2.OnFocusExit, new UXFocusObject.OnFocusExitDelegate(HandleOnFocusExit));
		UXFocusObject focusObject3 = _focusObject;
		focusObject3.OnNextFocusRequest = (UXFocusObject.OnNextFocusRequestDelegate)Delegate.Combine(focusObject3.OnNextFocusRequest, new UXFocusObject.OnNextFocusRequestDelegate(HandleOnNextFocusRequest));
		UXFocusObject focusObject4 = _focusObject;
		focusObject4.OnPreviousFocusRequest = (UXFocusObject.OnPreviousFocusRequestDelegate)Delegate.Combine(focusObject4.OnPreviousFocusRequest, new UXFocusObject.OnPreviousFocusRequestDelegate(HandleOnPreviousFocusRequest));
	}

	private void CreateTextBoxBG()
	{
		((Component)this).gameObject.AddComponent<MeshFilter>().mesh = BuildMesh();
		((Component)this).gameObject.AddComponent<BoxCollider>();
		((Component)this).renderer.enabled = Visible;
		((Component)this).collider.enabled = Visible;
	}

	protected virtual void CreateText()
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		Object val = Object.Instantiate(Resources.Load("Prefabs/UX/Text"));
		_uiText = ((GameObject)((val is GameObject) ? val : null)).GetComponent<UXText>();
		((Component)_uiText).transform.parent = ((Component)this).transform;
		((Component)_uiText).transform.localScale = Vector3.one * _textScale;
		((Component)_uiText).transform.localPosition = GetTextBasePosition();
		_uiText.horizontalAlign = UXHorizontal.Left;
		_uiText.verticalAlign = UXVertical.Middle;
		_uiText.SetVisible(Visible);
	}

	protected abstract Vector3 GetTextBasePosition();

	private void CreateCursor()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected Obj, but got Unknown
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = new GameObject("Cursor");
		val.layer = LayerMask.NameToLayer("UXElement");
		val.transform.parent = ((Component)_uiText).transform;
		val.transform.localScale = Vector3.one;
		_uiCursor = val.AddComponent<UXPlane>();
		_uiCursor.SetSize(0.1f, GetCursorHeight());
		_uiCursor.SetMaterial(_cursorMaterial);
		_uiCursor.SetVisible(Visible && HasFocus);
		((Component)_uiCursor).transform.localPosition = GetCursorPosition(GetActualText(_text));
	}

	protected abstract float GetCursorHeight();

	protected abstract Vector3 GetCursorPosition(string text);

	private void MoveCursor(int diff)
	{
		CursorIndex = _cursorIndex + diff;
	}

	protected virtual string GetActualText(string text)
	{
		return text;
	}

	protected abstract Vector3 GetTextOffset(string text);

	protected void UpdateText()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)_uiText != (Object)null)
		{
			_uiText.Text = GetActualText(_text);
			((Component)_uiText).transform.localPosition = GetTextOffset(_uiText.Text);
		}
	}

	protected Vector2 MeasureTextLength(string text)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)_textCalculator == (Object)null)
		{
			_textCalculator = UXUtils.FindGUIObjectOfType<UXTextSizeCalculator>();
		}
		Vector2 val = new Vector2(((Component)this).transform.localScale.x, ((Component)this).transform.localScale.y);
		return _textCalculator.MeasureString(GetActualText(text), val * _textScale, _uiText.TextSize);
	}

	public bool HandleInput()
	{
		if (_hasFocus && View.isVisible)
		{
			HandleKeyInput();
		}
		return false;
	}

	private void HandleRepeatKey(KeyCode keyCode, Action handler)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		if (MVInputWrapper.GetKeyDown(keyCode, useKey: false, forceKeyUse: true))
		{
			handler();
			_repeatTime = Time.realtimeSinceStartup + REPEAT_INTERVAL_FIRST;
		}
		if (MVInputWrapper.GetKey(keyCode, useKey: false, forceKeyUse: true) && Time.realtimeSinceStartup > _repeatTime)
		{
			handler();
			_repeatTime = Time.realtimeSinceStartup + REPEAT_INTERVAL;
		}
	}

	protected virtual void HandleKeyInput()
	{
		HandleRepeatKey((KeyCode)276, () =>
		{
			MoveCursor(-1);
		});
		HandleRepeatKey((KeyCode)275, () =>
		{
			MoveCursor(1);
		});
		HandleRepeatKey((KeyCode)127, () =>
		{
			if (_text.Length > 0 && CursorIndex < _text.Length)
			{
				_text = _text.Remove(CursorIndex, 1);
				UpdateText();
				NotifyCharacterDelete();
			}
		});
		bool flag = false;
		string inputString = Input.inputString;
		foreach (char c in inputString)
		{
			if (c == BACKSPACE_CHAR)
			{
				flag = true;
			}
			else if (c == NEWLINE_CHAR || c == CARRAGE_RETURN_CHAR)
			{
				if (multiLine)
				{
					InsertCharacter(c);
				}
			}
			else if (c != TAB_CHAR)
			{
				if (c == ESCAPE_CHAR)
				{
					ReleaseFocus();
				}
				else
				{
					InsertCharacter(c);
				}
			}
			MVInputWrapper.RegisterKeyAsUsed(c);
		}
		if (flag)
		{
			Backspace();
		}
	}

	protected virtual void InsertCharacter(char c)
	{
		if ((allowedInput != TextInputType.Text || !char.IsDigit(c)) && (allowedInput != TextInputType.Chars || char.IsLetter(c)) && (allowedInput != TextInputType.Numerical || char.IsDigit(c)) && (allowedInput != TextInputType.NumericalWithDecimals || char.IsDigit(c) || (c == '.' && !_text.Contains("."))) && _text.Length < _maxCharLength)
		{
			_text = _text.Insert(CursorIndex, c.ToString());
			MoveCursor(1);
			NotifyValueChanged();
			NotifyCharacterEntry(c);
		}
	}

	private void Backspace()
	{
		if (_text.Length > 0 && CursorIndex != 0)
		{
			_text = _text.Remove(Mathf.Max(0, CursorIndex - 1), 1);
			MoveCursor(-1);
			NotifyCharacterDelete();
			NotifyValueChanged();
		}
	}

	public override void SetAlpha(float alpha, string materialProperty = "_MainColor")
	{
		base.SetAlpha(alpha, materialProperty);
		_uiText.SetAlpha(alpha);
		_uiCursor.SetAlpha(alpha, materialProperty);
	}

	public override void SetVisible(bool visible)
	{
		Visible = visible;
		((Component)this).renderer.enabled = visible;
		if ((Object)(object)((Component)this).collider != (Object)null)
		{
			((Component)this).collider.enabled = visible;
		}
		if ((Object)(object)_uiText != (Object)null)
		{
			_uiText.SetVisible(visible);
		}
		if ((Object)(object)_uiCursor != (Object)null)
		{
			_uiCursor.SetVisible(visible && HasFocus);
		}
	}

	public void OnDestroy()
	{
		ReleaseFocus();
	}

	public void HandleOnFocusEnter(UXFocusObject focusObject)
	{
		_cursorIndex = _text.Length;
		HasFocus = true;
	}

	public void HandleOnFocusExit(UXFocusObject focusObject)
	{
		HasFocus = false;
	}

	public void HandleOnNextFocusRequest(UXFocusObject focusObject)
	{
		View.RequestNextFocus();
	}

	public void HandleOnPreviousFocusRequest(UXFocusObject focusObject)
	{
		View.RequestPreviousFocus();
	}

	public void RequestFocus()
	{
		View.RequestFocus(_focusObject);
	}

	public void ReleaseFocus()
	{
		if ((Object)(object)View != (Object)null)
		{
			View.ReleaseFocus();
		}
	}

	private void NotifyValueChanged()
	{
		if (OnValueChanged != null)
		{
			OnValueChanged(_text);
		}
	}

	private void NotifyCharacterEntry(char c)
	{
		if (OnCharacterEntry != null)
		{
			OnCharacterEntry(c);
		}
	}

	private void NotifyCharacterDelete()
	{
		if (OnCharacterDelete != null)
		{
			OnCharacterDelete();
		}
	}

	private void NotifyFocusChange(bool b)
	{
		if (OnFocusChange != null)
		{
			OnFocusChange(_hasFocus);
		}
	}
}
