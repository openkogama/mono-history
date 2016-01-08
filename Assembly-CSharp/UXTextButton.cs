using UnityEngine;

[AddComponentMenu("UX/Elements/Button")]
public class UXTextButton : UXBaseButton
{
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
			if (uiText != null)
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
		if (Text != TM._(Text))
		{
			Text = TM._(Text);
		}
	}

	protected override void Initialize()
	{
		base.Initialize();
		uiText = Object.Instantiate(Resources.Load("Prefabs/UX/Text") as GameObject).GetComponent<UXText>();
		uiText.transform.parent = transform;
		uiText.transform.localPosition = new Vector3(Width / 2f, Height / 2f, -0.01f) - Alignment;
		uiText.transform.localScale = Vector3.one;
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

	public override void SetVisible(bool visible)
	{
		base.SetVisible(visible);
		if (uiText != null)
		{
			uiText.SetVisible(visible);
		}
	}

	public override void SetAlpha(float alpha, string materialProperty = "_MainColor")
	{
		base.SetAlpha(alpha, materialProperty);
		uiText.SetAlpha(alpha, string.Empty);
	}

	public void FitTextToButton()
	{
		if (Width > 0f)
		{
			while (uiText.Size.x > Width)
			{
				uiText.transform.localScale *= 0.9f;
			}
		}
	}

	public void FitToText(float minButtonWidth = 0f, float minButtonHeight = 0f)
	{
		Vector2 vector = UXUtils.FindGUIObjectOfType<UXTextSizeCalculator>().MeasureString(_text);
		Width = Mathf.Max(minButtonWidth, vector.x);
		Height = Mathf.Max(minButtonHeight, vector.y);
		Width++;
		SetSize(Width, Height);
		MeshFilter component = gameObject.GetComponent<MeshFilter>();
		if (component != null)
		{
			BuildMesh(component.mesh);
		}
		uiText.transform.localPosition = new Vector3(Width / 2f, Height / 2f, -0.01f) - Alignment;
	}
}
