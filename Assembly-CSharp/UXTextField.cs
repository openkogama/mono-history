using UnityEngine;

public class UXTextField : UXTextInputElement
{
	public string hint = string.Empty;

	public bool isPassword;

	private float VisibleWidth => Width - TEXT_INDENT * 2f;

	protected override Vector3 GetTextBasePosition()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		return new Vector3(0f - Alignment.x + TEXT_INDENT, Height / 2f - Alignment.y, -0.01f);
	}

	protected override float GetCursorHeight()
	{
		return Height * 0.6f;
	}

	protected override Vector3 GetCursorPosition(string text)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		float x = MeasureTextLength(text.Substring(0, CursorIndex)).x;
		return new Vector3(x, 0f, -0.02f);
	}

	protected override Vector3 GetTextOffset(string text)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		if (text.Length == 0)
		{
			return GetTextBasePosition();
		}
		float x = MeasureTextLength(text.Substring(0, CursorIndex)).x;
		if (CursorIndex <= 1 || MeasureTextLength(text).x < VisibleWidth)
		{
			textOffset = 0f;
		}
		else
		{
			if (x + textOffset < TEXT_INDENT)
			{
				textOffset += MeasureTextLength(text.Substring(CursorIndex - 1, 1)).x;
			}
			while (x + textOffset > VisibleWidth)
			{
				textOffset -= MeasureTextLength(text.Substring(CursorIndex - 1, 1)).x;
			}
		}
		return GetTextBasePosition() + new Vector3(textOffset, 0f, 0f);
	}

	protected override string GetActualText(string text)
	{
		string actualText = base.GetActualText(text);
		string text2 = ((!isPassword) ? actualText : UXUtils.RepeatString("*", actualText.Length));
		if (text2.Equals(string.Empty) && !HasFocus)
		{
			text2 += hint;
		}
		return text2;
	}

	public override Rect GetBoundingBox()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		Rect boundingBox = base.GetBoundingBox();
		boundingBox.center += new Vector2(TEXT_INDENT * screen.Scale, 0f);
		boundingBox.width -= TEXT_INDENT * 2f * screen.Scale;
		return boundingBox;
	}
}
