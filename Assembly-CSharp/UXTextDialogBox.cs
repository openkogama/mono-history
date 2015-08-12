using UnityEngine;

public class UXTextDialogBox : UXDialogBox
{
	private const float TEXT_X_BUFFER = 1.5f;

	private const float TEXT_Y_BUFFER = 3.25f;

	private bool _isInitialized;

	private UXTextSizeCalculator textSizeCalculator => UXUtils.FindGUIObjectOfType<UXTextSizeCalculator>();

	public override void OnShowDialog()
	{
		base.OnShowDialog();
		if (!_isInitialized)
		{
			UXText component = UXUtils.FindChild(gameObject, "Text").GetComponent<UXText>();
			Vector2 vector = textSizeCalculator.MeasureString(component.Text, UXTextSize.Large);
			Vector2 vector2 = textSizeCalculator.MeasureString(component.Text);
			Vector2 vector3 = textSizeCalculator.MeasureString(component.Text, UXTextSize.Small);
			if (vector.x <= DialogWindow.Width - 3f && vector.y <= DialogWindow.Height - 6.5f)
			{
				component.TextSize = UXTextSize.Large;
			}
			else if (vector2.y <= DialogWindow.Width - 3f && vector2.y <= DialogWindow.Height - 6.5f)
			{
				component.TextSize = UXTextSize.Medium;
			}
			else
			{
				component.TextSize = UXTextSize.Small;
			}
			if (vector3.x > DialogWindow.Width - 3f)
			{
				component.wordWrapWidth = DialogWindow.Width - 3f;
				component.wrapStyle = TextWrapStyle.Mixed;
				component.WordWrap = true;
			}
			_isInitialized = true;
		}
	}
}
