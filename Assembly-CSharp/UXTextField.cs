using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[AddComponentMenu("UX/Elements/Text field")]
public class UXTextField : MonoBehaviour, IInputHandler
{
	public delegate void OnValueChangedDelegate(string value);

	public delegate void OnCharacterEntryDelegate(char c);

	public delegate void OnCharacterDeleteDelegate();

	public OnValueChangedDelegate OnValueChanged;

	public OnCharacterEntryDelegate OnCharacterEntry;

	public OnCharacterDeleteDelegate OnCharacterDelete;

	public float width = 5f;

	public float height = 2f;

	public UXHorizontal horizontalAlign;

	public UXVertical verticalAlign;

	public GameObject textBoxPrefab;

	public GameObject textPrefab;

	public string text = "TextField";

	public int maxLength = int.MaxValue;

	public int visibleLength = 20;

	public float textIndent = 1f;

	public bool password;

	private bool isInitialized;

	private float TEXT_Z = -1f;

	private GameObject textObject;

	private GameObject textBoxObject;

	private UXText uiText;

	private bool hasFocus;

	private int offset;

	private int cursorIndex = -1;

	private float repeatTime;

	private UXView view;

	private float REPEAT_INTERVAL = 0.1f;

	private float REPEAT_INTERVAL_FIRST = 0.3f;

	private char BACKSPACE_CHAR = "\b"[0];

	private char NEWLINE_CHAR = "\n"[0];

	private char CARRAGE_RETURN_CHAR = "\r"[0];

	private UXFocusObject focusObject;

	private static Dictionary<UXHorizontal, float> horizontalToRatio = new Dictionary<UXHorizontal, float>
	{
		{
			UXHorizontal.Left,
			0f
		},
		{
			UXHorizontal.Center,
			0.5f
		},
		{
			UXHorizontal.Right,
			1f
		}
	};

	private static Dictionary<UXVertical, float> verticalToRatio = new Dictionary<UXVertical, float>
	{
		{
			UXVertical.Top,
			1f
		},
		{
			UXVertical.Middle,
			0.5f
		},
		{
			UXVertical.Bottom,
			0f
		}
	};

	private float boxColliderDepth = 1f;

	public int Priority => InputHandlerPriority.UI_FOCUS;

	private int CursorIndex
	{
		get
		{
			return cursorIndex;
		}
		set
		{
			cursorIndex = Mathf.Clamp(value, 0, text.Length);
			offset = cursorIndex / visibleLength * visibleLength;
			if (isInitialized)
			{
				UpdateText();
			}
		}
	}

	public string Text
	{
		get
		{
			return text;
		}
		set
		{
			text = value;
			CursorIndex = Mathf.Max(0, text.Length - 1);
			NotifyValueChanged();
		}
	}

	public void Awake()
	{
		view = UXUtils.FindComponentInParents(typeof(UXView), ((Component)this).transform.parent) as UXView;
		UXUtils.FindObjectOfType<MVInputHandlerPrioritizer>().Register(this);
		Initialize();
	}

	public void Start()
	{
		UpdateText();
	}

	public void Initialize()
	{
		if (!isInitialized)
		{
			InitializeTextBox();
			InitializeText();
			isInitialized = true;
		}
	}

	private void InitializeTextBox()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		Object val = Object.Instantiate((Object)(object)textBoxPrefab, Vector3.zero, Quaternion.identity);
		textBoxObject = (GameObject)(object)((val is GameObject) ? val : null);
		textBoxObject.transform.parent = ((Component)this).transform;
		textBoxObject.transform.localPosition = new Vector3((0f - width) * horizontalToRatio[horizontalAlign], (0f - height) * verticalToRatio[verticalAlign], 0f);
		textBoxObject.AddComponent<MeshFilter>().mesh = BuildMesh();
		BoxCollider val2 = textBoxObject.AddComponent<BoxCollider>();
		val2.size = new Vector3(width, height, boxColliderDepth);
		focusObject = textBoxObject.GetComponent<UXFocusObject>();
		UXFocusObject uXFocusObject = focusObject;
		uXFocusObject.OnFocusEnter = (UXFocusObject.OnFocusEnterDelegate)Delegate.Combine(uXFocusObject.OnFocusEnter, new UXFocusObject.OnFocusEnterDelegate(HandleOnFocusEnter));
		UXFocusObject uXFocusObject2 = focusObject;
		uXFocusObject2.OnFocusExit = (UXFocusObject.OnFocusExitDelegate)Delegate.Combine(uXFocusObject2.OnFocusExit, new UXFocusObject.OnFocusExitDelegate(HandleOnFocusExit));
		UXFocusObject uXFocusObject3 = focusObject;
		uXFocusObject3.OnNextFocusRequest = (UXFocusObject.OnNextFocusRequestDelegate)Delegate.Combine(uXFocusObject3.OnNextFocusRequest, new UXFocusObject.OnNextFocusRequestDelegate(HandleOnNextFocusRequest));
		UXFocusObject uXFocusObject4 = focusObject;
		uXFocusObject4.OnPreviousFocusRequest = (UXFocusObject.OnPreviousFocusRequestDelegate)Delegate.Combine(uXFocusObject4.OnPreviousFocusRequest, new UXFocusObject.OnPreviousFocusRequestDelegate(HandleOnPreviousFocusRequest));
	}

	private void InitializeText()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		Object val = Object.Instantiate((Object)(object)textPrefab, Vector3.zero, Quaternion.identity);
		textObject = (GameObject)(object)((val is GameObject) ? val : null);
		textObject.transform.parent = textBoxObject.transform;
		textObject.transform.localPosition = new Vector3(textIndent, height / 2f, TEXT_Z);
		uiText = textObject.GetComponent<UXText>();
		uiText.horizontalAlign = UXHorizontal.Left;
		uiText.verticalAlign = UXVertical.Middle;
	}

	public void HandleOnFocusEnter(UXFocusObject focusObject)
	{
		cursorIndex = text.Length;
		hasFocus = true;
		UpdateText();
	}

	public void HandleOnFocusExit(UXFocusObject focusObject)
	{
		hasFocus = false;
		UpdateText();
	}

	public void HandleOnNextFocusRequest(UXFocusObject focusObject)
	{
		view.RequestNextFocus();
	}

	public void HandleOnPreviousFocusRequest(UXFocusObject focusObject)
	{
		view.RequestPreviousFocus();
	}

	public void RequestFocus()
	{
		view.RequestFocus(focusObject);
	}

	public bool HandleInput()
	{
		if (hasFocus && view.isVisible)
		{
			HandleKeyInput();
		}
		return false;
	}

	private void HandleRepeatKey(KeyCode keyCode, Action handler)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (MVInputWrapper.GetKeyDown(keyCode, useKey: true))
		{
			handler();
			repeatTime = Time.realtimeSinceStartup + REPEAT_INTERVAL_FIRST;
		}
		if (MVInputWrapper.GetKey(keyCode, useKey: true) && Time.realtimeSinceStartup > repeatTime)
		{
			handler();
			repeatTime = Time.realtimeSinceStartup + REPEAT_INTERVAL;
		}
	}

	private void HandleKeyInput()
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
			if (text.Length > 0)
			{
				text = text.Remove(CursorIndex, 1);
				UpdateText();
				NotifyCharacterDelete();
			}
		});
		bool backspace = false;
		HandleRepeatKey((KeyCode)8, () =>
		{
			backspace = true;
		});
		string inputString = Input.inputString;
		foreach (char c in inputString)
		{
			if (c == BACKSPACE_CHAR)
			{
				backspace = true;
			}
			else if (c != NEWLINE_CHAR && c != CARRAGE_RETURN_CHAR)
			{
				InsertCharacter(c);
			}
			MVInputWrapper.RegisterKeyAsUsed(c);
		}
		if (backspace)
		{
			Backspace();
		}
	}

	protected virtual void InsertCharacter(char c)
	{
		if (text.Length < maxLength)
		{
			text = text.Insert(CursorIndex, c.ToString());
			MoveCursor(1);
			UpdateText();
			NotifyValueChanged();
			NotifyCharacterEntry(c);
		}
	}

	private void Backspace()
	{
		if (text.Length > 0)
		{
			text = text.Remove(Mathf.Max(0, CursorIndex - 1), 1);
			MoveCursor(-1);
			UpdateText();
			NotifyCharacterDelete();
			NotifyValueChanged();
		}
	}

	private void MoveCursor(int diff)
	{
		CursorIndex = cursorIndex + diff;
	}

	private string RepeatString(string s, int count)
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < count; i++)
		{
			stringBuilder.Append(s);
		}
		return stringBuilder.ToString();
	}

	public void UpdateText()
	{
		string text = ((!password) ? this.text : RepeatString("*", this.text.Length));
		text = text.Substring(offset, Mathf.Min(text.Length, offset + visibleLength) - offset);
		if (hasFocus)
		{
			uiText.Text = ((CursorIndex >= text.Length) ? (text + "_") : text.Insert(CursorIndex, "_"));
		}
		else
		{
			uiText.Text = text;
		}
	}

	private Mesh BuildMesh()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Expected Obj, but got Unknown
		Vector3[] array = new Vector3[4];
		int[] array2 = new int[6];
		Vector2[] array3 = new Vector2[array.Length];
		ref Vector3 reference = ref array[0];
		reference = new Vector3(0f, 0f, 0f);
		ref Vector2 reference2 = ref array3[0];
		reference2 = new Vector2(0f, 0f);
		ref Vector3 reference3 = ref array[1];
		reference3 = new Vector3(width, 0f, 0f);
		ref Vector2 reference4 = ref array3[1];
		reference4 = new Vector2(1f, 0f);
		ref Vector3 reference5 = ref array[2];
		reference5 = new Vector3(width, height, 0f);
		ref Vector2 reference6 = ref array3[2];
		reference6 = new Vector2(1f, 1f);
		ref Vector3 reference7 = ref array[3];
		reference7 = new Vector3(0f, height, 0f);
		ref Vector2 reference8 = ref array3[3];
		reference8 = new Vector2(0f, 1f);
		array2[0] = 0;
		array2[1] = 3;
		array2[2] = 2;
		array2[3] = 0;
		array2[4] = 2;
		array2[5] = 1;
		Mesh val = new Mesh();
		((Object)val).name = "UITextField Mesh";
		val.vertices = array;
		val.uv = array3;
		val.triangles = array2;
		val.RecalculateNormals();
		val.RecalculateBounds();
		return val;
	}

	private void NotifyValueChanged()
	{
		if (OnValueChanged != null)
		{
			OnValueChanged(text);
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

	public void OnDrawGizmos()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		Gizmos.color = Color.grey;
		Matrix4x4 localToWorldMatrix = ((Component)this).transform.localToWorldMatrix;
		Vector3 val = new Vector3(UXEnums.GetRatio(horizontalAlign) * width, UXEnums.GetRatio(verticalAlign) * height, 0f);
		Vector3 val2 = localToWorldMatrix.MultiplyPoint(new Vector3(0f, 0f, 0f) - val);
		Vector3 val3 = localToWorldMatrix.MultiplyPoint(new Vector3(width, 0f, 0f) - val);
		Vector3 val4 = localToWorldMatrix.MultiplyPoint(new Vector3(width, height, 0f) - val);
		Vector3 val5 = localToWorldMatrix.MultiplyPoint(new Vector3(0f, height, 0f) - val);
		Gizmos.DrawLine(val2, val3);
		Gizmos.DrawLine(val3, val4);
		Gizmos.DrawLine(val4, val5);
		Gizmos.DrawLine(val5, val2);
	}
}
