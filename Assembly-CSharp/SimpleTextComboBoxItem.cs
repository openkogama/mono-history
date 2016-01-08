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
		nameText.SetAlpha(alpha, string.Empty);
	}

	protected override void UpdateMouseOver(bool mouseOver)
	{
		base.UpdateMouseOver(mouseOver);
		nameText.Color = ((!mouseOver) ? Color.black : Color.white);
	}

	public override GameObject CreateClone()
	{
		SimpleTextComboBoxItem component = Object.Instantiate(PrefabPool.Instance.SimpleTextComboBoxItemObject).GetComponent<SimpleTextComboBoxItem>();
		component.SetSize(new Vector2(Width - 2f, 2f));
		component.CreateItem(text);
		return component.gameObject;
	}

	public override object GetValue()
	{
		return text;
	}
}
