using System;
using UnityEngine;

public class UXTextBox : UXTextInputElement
{
	public string hint = string.Empty;

	[SerializeField]
	private UXSlider uiSlider;

	private float textLineHeight;

	private float slideTextOffset;

	private bool waitWithFocus;

	private float WorkingHeight => Height - TEXT_INDENT * 2f;

	private float TotalLinesHeight => (float)_uiText.LineCount * textLineHeight;

	private bool TextLargerThanBox => TotalLinesHeight > Height;

	public override bool HasFocus
	{
		get
		{
			return base.HasFocus;
		}
		protected set
		{
			base.HasFocus = value;
			UpdateSliderVisibility();
		}
	}

	public override void Start()
	{
		base.Start();
		multiLine = true;
		InitializeSlider();
		UXMouseClickObject uXMouseClickObject = ((Component)this).gameObject.AddComponent<UXMouseClickObject>();
		uXMouseClickObject.OnClick = (UXMouseClickObject.OnClickDelegate)Delegate.Combine(uXMouseClickObject.OnClick, new UXMouseClickObject.OnClickDelegate(OnClickOnLine));
	}

	private void InitializeSlider()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		((Component)uiSlider).transform.localPosition = new Vector3(Width, Height / 2f, -0.5f) - Alignment;
		uiSlider.SetSize(uiSlider.Width, Height);
		uiSlider.Initialize();
		uiSlider.OnValueChangedIntermediate = Slide;
		uiSlider.OnValueChanged = SlideEnd;
		UpdateSliderSize();
		UpdateSliderVisibility();
	}

	protected override void CreateText()
	{
		base.CreateText();
		_uiText.horizontalAlign = UXHorizontal.Left;
		_uiText.verticalAlign = UXVertical.Top;
		_uiText.wordWrapWidth = Width - TEXT_INDENT * 2f;
		_uiText.WordWrap = true;
	}

	protected override Vector3 GetTextBasePosition()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		return new Vector3(0f - Alignment.x + TEXT_INDENT, Height - Alignment.y - TEXT_INDENT / 2f, -0.01f);
	}

	public override void SetVisible(bool visible)
	{
		base.SetVisible(visible);
		UpdateSliderVisibility();
	}

	protected override void HandleKeyInput()
	{
		base.HandleKeyInput();
		if (Input.GetKeyUp((KeyCode)273))
		{
			ChangeLine(-1);
		}
		if (Input.GetKeyUp((KeyCode)274))
		{
			ChangeLine(1);
		}
		UpdateSliderSize();
		UpdateSliderVisibility();
	}

	private void OnClickOnLine(UXMouseClickObject clickObject, Vector3 mousePositionWorld)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		waitWithFocus = true;
		Vector3 val = ((Component)_uiText).transform.InverseTransformPoint(mousePositionWorld);
		int num = (int)(Mathf.Abs(val.y) / textLineHeight);
		string[] array = _uiText.Text.Split(new char[1] { '\n' });
		if (num >= array.Length)
		{
			num = array.Length - 1;
		}
		int num2 = 0;
		for (int i = 0; i < num; i++)
		{
			num2 += array[i].Length;
		}
		CursorIndex = num2;
		int lineIndexInWrappedText;
		for (lineIndexInWrappedText = GetLineIndexInWrappedText(Text); lineIndexInWrappedText < num; lineIndexInWrappedText = GetLineIndexInWrappedText(Text))
		{
			CursorIndex++;
		}
		float x = MeasureTextLength(array[lineIndexInWrappedText]).x;
		int num3 = 0;
		if (x > 0f)
		{
			float x2 = val.x;
			float num4 = x2 / x;
			if (num4 > 1f)
			{
				num4 = 1f;
			}
			float num5 = (float)array[lineIndexInWrappedText].Length * num4;
			num3 = (int)num5;
		}
		waitWithFocus = false;
		CursorIndex += num3;
	}

	private void ChangeLine(int direction)
	{
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		int lineIndexInWrappedText = GetLineIndexInWrappedText(GetActualText(Text), out var cursorIndex);
		string[] array = _uiText.Text.Split(new char[1] { '\n' });
		if (lineIndexInWrappedText + direction < 0 || lineIndexInWrappedText + direction >= array.Length)
		{
			return;
		}
		float num = 0f;
		if (cursorIndex > 0)
		{
			num = MeasureTextLength(array[lineIndexInWrappedText].Substring(0, cursorIndex)).x;
		}
		float x = MeasureTextLength(array[lineIndexInWrappedText + direction]).x;
		int num2 = 0;
		if (x > 0f)
		{
			float num3 = num / x;
			if (num3 > 1f)
			{
				num3 = 1f;
			}
			float num4 = (float)array[lineIndexInWrappedText + direction].Length * num3;
			num2 = Mathf.RoundToInt(num4);
		}
		lineIndexInWrappedText += direction;
		int num5 = 0;
		num5 = ((direction >= 0) ? (array[lineIndexInWrappedText - 1].Length - cursorIndex + num2) : (cursorIndex + (array[lineIndexInWrappedText].Length - num2)));
		CursorIndex += direction * (num5 + 1);
	}

	private void FocusOnLine(int lineNumber)
	{
		if (!waitWithFocus)
		{
			float num = (float)lineNumber * textLineHeight + textLineHeight / 2f;
			if (num < slideTextOffset)
			{
				slideTextOffset = (float)lineNumber * textLineHeight;
			}
			else if (num > slideTextOffset + WorkingHeight)
			{
				slideTextOffset = (float)(lineNumber + 1) * textLineHeight - WorkingHeight;
			}
			uiSlider.Value = slideTextOffset / (TotalLinesHeight - WorkingHeight) * uiSlider.MaxValue;
			Slide(uiSlider, uiSlider.Value);
		}
	}

	private void UpdateSliderVisibility()
	{
		if ((Object)(object)uiSlider != (Object)null)
		{
			uiSlider.SetVisible(Visible && HasFocus && TextLargerThanBox);
		}
	}

	private void Slide(UXSlider slider, float sliderValue)
	{
		slideTextOffset = (TotalLinesHeight - WorkingHeight) * (sliderValue / slider.MaxValue);
		if (TotalLinesHeight < WorkingHeight)
		{
			slideTextOffset = 0f;
		}
		UpdateText();
	}

	private void SlideEnd(UXSlider slider)
	{
		Slide(slider, slider.Value);
	}

	private void UpdateSliderSize()
	{
		float num = WorkingHeight / TotalLinesHeight;
		float height = Mathf.Clamp((!(TotalLinesHeight < WorkingHeight)) ? (num * WorkingHeight) : WorkingHeight, 2f, WorkingHeight);
		uiSlider.SetSliderSize(1f, height);
		if (!TextLargerThanBox && slideTextOffset > 0f)
		{
			slideTextOffset = 0f;
			UpdateText();
		}
	}

	protected override float GetCursorHeight()
	{
		if (textLineHeight == 0f)
		{
			_uiText.Text = "T";
			textLineHeight = _uiText.TextHeight;
			_uiText.Text = Text;
		}
		return textLineHeight * 0.6f;
	}

	protected override Vector3 GetCursorPosition(string text)
	{
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		int lineIndexInWrappedText = GetLineIndexInWrappedText(text, out var cursorIndex);
		string text2 = _uiText.Text.Split(new char[1] { '\n' })[lineIndexInWrappedText];
		float num = 0f;
		if (text2.Length > 0 && cursorIndex > 0)
		{
			_uiText.Text = text2.Substring(0, cursorIndex);
			num = _uiText.TextWidth;
		}
		_uiText.Text = text;
		FocusOnLine(lineIndexInWrappedText);
		return new Vector3(num, 0f - (float)(lineIndexInWrappedText + 1) * textLineHeight, -0.02f) + Vector3.up * textLineHeight / 2f;
	}

	private int GetLineIndexInWrappedText(string text)
	{
		int cursorIndex = 0;
		return GetLineIndexInWrappedText(text, out cursorIndex);
	}

	private int GetLineIndexInWrappedText(string text, out int cursorIndex)
	{
		string[] array = text.Split(new char[1] { '\n' });
		cursorIndex = CursorIndex;
		int num = 0;
		bool flag = false;
		for (int i = 0; i < array.Length; i++)
		{
			_uiText.Text = array[i];
			if (_uiText.LineCount > 1)
			{
				string[] array2 = _uiText.Text.Split(new char[1] { '\n' });
				for (int j = 0; j < array2.Length; j++)
				{
					if (cursorIndex - array2[j].Length > 0)
					{
						cursorIndex -= array2[j].Length;
						num++;
						continue;
					}
					flag = true;
					break;
				}
			}
			else if (cursorIndex - array[i].Length > 0)
			{
				cursorIndex -= array[i].Length;
				num++;
			}
			else
			{
				flag = true;
			}
			if (flag)
			{
				break;
			}
			if (i < array.Length - 1)
			{
				cursorIndex--;
			}
		}
		_uiText.Text = text;
		return num;
	}

	protected override Vector3 GetTextOffset(string text)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		return GetTextBasePosition() + Vector3.up * slideTextOffset;
	}

	protected override string GetActualText(string text)
	{
		if (text.Equals(string.Empty) && !HasFocus)
		{
			return hint;
		}
		return base.GetActualText(text);
	}

	public override Rect GetBoundingBox()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		Rect boundingBox = base.GetBoundingBox();
		boundingBox.center += new Vector2(TEXT_INDENT * screen.Scale, TEXT_INDENT * screen.Scale);
		boundingBox.width -= TEXT_INDENT * 2f * screen.Scale;
		boundingBox.height -= TEXT_INDENT * 2f * screen.Scale;
		return boundingBox;
	}
}
