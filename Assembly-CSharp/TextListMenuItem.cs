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
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize();
		Object val = Object.Instantiate(Resources.Load("Prefabs/UX/Text"));
		textObject = ((GameObject)((val is GameObject) ? val : null)).GetComponent<UXText>();
		((Component)textObject).transform.parent = ((Component)this).transform;
		((Component)textObject).transform.localPosition = Vector3.zero;
		((Component)textObject).transform.localScale = new Vector3(textScale, textScale, textScale);
		((Component)textObject).transform.localRotation = Quaternion.identity;
		textObject.Text = text;
	}

	public override float GetHeight()
	{
		return 1f;
	}

	protected override void UpdateMouseOver()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		base.UpdateMouseOver();
		textObject.Color = ((!mouseOver) ? Color.black : Color.white);
	}
}
