using UnityEngine;

public class TextListMenuItem : ListMenuItem
{
	private string text;

	private UXText textObject;

	private float textScale = 0.75f;

	public void SetText(string text)
	{
		this.text = text;
	}

	protected override void Initialize()
	{
		base.Initialize();
		textObject = (Object.Instantiate(Resources.Load("Prefabs/UX/Text")) as GameObject).GetComponent<UXText>();
		textObject.transform.parent = transform;
		textObject.transform.localPosition = Vector3.zero;
		textObject.transform.localScale = new Vector3(textScale, textScale, textScale);
		textObject.transform.localRotation = Quaternion.identity;
		textObject.Text = text;
	}

	public override float GetHeight()
	{
		return 1f;
	}

	protected override void UpdateMouseOver()
	{
		base.UpdateMouseOver();
		textObject.Color = ((!mouseOver) ? Color.black : Color.white);
	}
}
