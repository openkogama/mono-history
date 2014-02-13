using UnityEngine;

public class SimpleTextComboBoxItem : UXComboBoxItem
{
	private string text;

	public UXText nameText;

	public void CreateItem(string text)
	{
		this.text = text;
		BuildItem();
		nameText.text = text;
	}

	public override void SetAlpha(float alpha, string materialProperty = "_MainColor")
	{
		base.SetAlpha(alpha, materialProperty);
		nameText.SetAlpha(alpha);
	}

	protected override void UpdateMouseOver(bool mouseOver)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		base.UpdateMouseOver(mouseOver);
		nameText.Color = ((!mouseOver) ? Color.black : Color.white);
	}

	public override GameObject CreateClone()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		Object val = Object.Instantiate(Resources.Load("Prefabs/GUI/ComboBox/SimpleTextComboBoxItem"));
		SimpleTextComboBoxItem component = ((GameObject)((val is GameObject) ? val : null)).GetComponent<SimpleTextComboBoxItem>();
		component.SetSize(new Vector2(Width - 2f, 2f));
		component.CreateItem(text);
		return ((Component)component).gameObject;
	}

	public override object GetValue()
	{
		return text;
	}
}
