using UnityEngine;

public class UXTextDialogBox : UXDialogBox
{
	private const float TEXT_X_BUFFER = 1.5f;

	private const float TEXT_Y_BUFFER = 3.25f;

	private bool _isInitialized;

	private UXTextSizeCalculator textSizeCalculator => UXUtils.FindGUIObjectOfType<UXTextSizeCalculator>();

	public override void OnShowDialog()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		base.OnShowDialog();
		if (!_isInitialized)
		{
			UXText component = UXUtils.FindChild(((Component)this).gameObject, "Text").GetComponent<UXText>();
			Vector2 val = textSizeCalculator.MeasureString(component.Text, UXTextSize.Large);
			Vector2 val2 = textSizeCalculator.MeasureString(component.Text);
			Vector2 val3 = textSizeCalculator.MeasureString(component.Text, UXTextSize.Small);
			if (val.x <= DialogWindow.Width - 3f && val.y <= DialogWindow.Height - 6.5f)
			{
				component.TextSize = UXTextSize.Large;
			}
			else if (val2.y <= DialogWindow.Width - 3f && val2.y <= DialogWindow.Height - 6.5f)
			{
				component.TextSize = UXTextSize.Medium;
			}
			else
			{
				component.TextSize = UXTextSize.Small;
			}
			if (val3.x > DialogWindow.Width - 3f)
			{
				component.wordWrapWidth = DialogWindow.Width - 3f;
				component.wrapStyle = TextWrapStyle.Mixed;
				component.WordWrap = true;
			}
			_isInitialized = true;
		}
	}
}
