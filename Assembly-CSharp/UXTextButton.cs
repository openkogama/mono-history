using Localize;
using UnityEngine;

[AddComponentMenu("UX/Elements/Button")]
public class UXTextButton : UXBaseButton
{
	public TextSlotIndex index = TextSlotIndex.Empty;

	[SerializeField]
	private string _text;

	public bool FitButtonSizeToText;

	private UXText uiText;

	public string Text
	{
		get
		{
			return _text;
		}
		set
		{
			_text = value;
			if ((Object)(object)uiText != (Object)null)
			{
				uiText.Text = _text;
				if (FitButtonSizeToText)
				{
					FitToText();
				}
				else
				{
					FitTextToButton();
				}
			}
		}
	}

	public void Start()
	{
		if (index != TextSlotIndex.Empty)
		{
			Text = Localization.Instance.GetText(index);
		}
	}

	protected override void Initialize()
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		base.Initialize();
		Object val = Object.Instantiate(Resources.Load("Prefabs/UX/Text"));
		uiText = ((GameObject)((val is GameObject) ? val : null)).GetComponent<UXText>();
		((Component)uiText).transform.parent = ((Component)this).transform;
		((Component)uiText).transform.localPosition = new Vector3(Width / 2f, Height / 2f, -0.01f) - Alignment;
		((Component)uiText).transform.localScale = Vector3.one;
		uiText.Text = _text;
		if (FitButtonSizeToText)
		{
			FitToText();
		}
		else
		{
			FitTextToButton();
		}
		SetVisible(Visible);
	}

	public override void SetAlpha(float alpha, string materialProperty = "_MainColor")
	{
		base.SetAlpha(alpha, materialProperty);
		uiText.SetAlpha(alpha);
	}

	public void FitTextToButton()
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		if (Width > 0f)
		{
			while (uiText.Size.x > Width)
			{
				Transform transform = ((Component)uiText).transform;
				transform.localScale *= 0.9f;
			}
		}
	}

	public void FitToText(float minButtonWidth = 0f, float minButtonHeight = 0f)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = UXUtils.FindGUIObjectOfType<UXTextSizeCalculator>().MeasureString(_text);
		Width = Mathf.Max(minButtonWidth, val.x);
		Height = Mathf.Max(minButtonHeight, val.y);
		Width++;
		SetSize(Width, Height);
		MeshFilter component = ((Component)this).gameObject.GetComponent<MeshFilter>();
		if ((Object)(object)component != (Object)null)
		{
			component.mesh = BuildMesh();
		}
		((Component)uiText).transform.localPosition = new Vector3(Width / 2f, Height / 2f, -0.01f) - Alignment;
	}
}
