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
			if (_view == null)
			{
				_view = UXUtils.FindComponentInParents(typeof(UXView), transform.parent) as UXView;
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
			if (_uiCursor != null)
			{
				_uiCursor.SetVisible(_hasFocus);
			}
			NotifyFocusChange(_hasFocus);
			MVInputWrapper.ignoreAllKeys = _hasFocus;
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
			_cursorIndex = Mathf.Clamp(value, 0, _text.Length);
			if (GetActualText(_text).Length == 0)
			{
				_cursorIndex = 0;
			}
			if (_isInitialized)
			{
				UpdateText();
				if (_uiCursor != null)
				{
					_uiCursor.transform.localPosition = GetCursorPosition(GetActualText(_text));
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
		_focusObject = gameObject.AddComponent<UXFocusObject>();
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
		BuildMesh(gameObject.AddComponent<MeshFilter>().mesh);
		gameObject.AddComponent<BoxCollider>();
		GetComponent<Renderer>().enabled = Visible;
		GetComponent<Collider>().enabled = Visible;
	}

	protected virtual void CreateText()
	{
		_uiText = UnityEngine.Object.Instantiate(PrefabPool.Instance.UXTextObject).GetComponent<UXText>();
		_uiText.transform.parent = transform;
		_uiText.transform.localScale = Vector3.one * _textScale;
		_uiText.transform.localPosition = GetTextBasePosition();
		_uiText.horizontalAlign = UXHorizontal.Left;
		_uiText.verticalAlign = UXVertical.Middle;
		_uiText.SetVisible(Visible);
	}

	protected abstract Vector3 GetTextBasePosition();

	private void CreateCursor()
	{
		GameObject gameObject = new GameObject("Cursor");
		gameObject.layer = LayerMask.NameToLayer("UXElement");
		gameObject.transform.parent = _uiText.transform;
		gameObject.transform.localScale = Vector3.one;
		_uiCursor = gameObject.AddComponent<UXPlane>();
		_uiCursor.SetSize(0.1f, GetCursorHeight());
		_uiCursor.SetMaterial(_cursorMaterial);
		_uiCursor.SetVisible(Visible && HasFocus);
		_uiCursor.transform.localPosition = GetCursorPosition(GetActualText(_text));
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
		if (_uiText != null)
		{
			_uiText.Text = GetActualText(_text);
			_uiText.transform.localPosition = GetTextOffset(_uiText.Text);
		}
	}

	protected Vector2 MeasureTextLength(string text)
	{
		if (_textCalculator == null)
		{
			_textCalculator = UXUtils.FindGUIObjectOfType<UXTextSizeCalculator>();
		}
		Vector2 vector = new Vector2(transform.localScale.x, transform.localScale.y);
		return _textCalculator.MeasureString(GetActualText(text), vector * _textScale, _uiText.TextSize);
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
		if (MVInputWrapper.InputCharActiveDown(keyCode))
		{
			handler();
			_repeatTime = Time.realtimeSinceStartup + REPEAT_INTERVAL_FIRST;
		}
		if (MVInputWrapper.InputCharActive(keyCode) && Time.realtimeSinceStartup > _repeatTime)
		{
			handler();
			_repeatTime = Time.realtimeSinceStartup + REPEAT_INTERVAL;
		}
	}

	protected virtual void HandleKeyInput()
	{
		HandleRepeatKey(KeyCode.LeftArrow, () =>
		{
			MoveCursor(-1);
		});
		HandleRepeatKey(KeyCode.RightArrow, () =>
		{
			MoveCursor(1);
		});
		HandleRepeatKey(KeyCode.Delete, () =>
		{
			if (_text.Length > 0 && CursorIndex < _text.Length)
			{
				_text = _text.Remove(CursorIndex, 1);
				UpdateText();
				NotifyCharacterDelete();
			}
		});
		bool flag = false;
		string stringInput = MVInputWrapper.GetStringInput();
		foreach (char c in stringInput)
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
		_uiText.SetAlpha(alpha, string.Empty);
		_uiCursor.SetAlpha(alpha, materialProperty);
	}

	public override void SetVisible(bool visible)
	{
		Visible = visible;
		GetComponent<Renderer>().enabled = visible;
		if (GetComponent<Collider>() != null)
		{
			GetComponent<Collider>().enabled = visible;
		}
		if (_uiText != null)
		{
			_uiText.SetVisible(visible);
		}
		if (_uiCursor != null)
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
		Debug.Log("On FOcus Enter");
		HasFocus = true;
	}

	public void HandleOnFocusExit(UXFocusObject focusObject)
	{
		Debug.Log("On FOcus Exit");
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
		if (View != null)
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
