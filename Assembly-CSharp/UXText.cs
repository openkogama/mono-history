using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UXText : UXGUIElement
{
	public UXTextSize textSize = UXTextSize.Medium;

	public string text;

	[SerializeField]
	private bool wordWrap;

	public Color color = Color.white;

	public Color shadowColor = Color.black;

	public bool hasShadow = true;

	public TextWrapStyle wrapStyle = TextWrapStyle.Mixed;

	public float wordWrapWidth = 40f;

	private List<TextMesh> textMeshes = new List<TextMesh>();

	private TextWrapper textWrapper;

	private MeshRenderer meshRenderer;

	private UXTextSizeCalculator textSizeCalculator;

	private UXFontManager fontManager;

	public UXTextSize TextSize
	{
		get
		{
			return textSize;
		}
		set
		{
			textSize = value;
			if (textMeshes.Count <= 0)
			{
				return;
			}
			foreach (TextMesh textMesh in textMeshes)
			{
				Object.Destroy(textMesh.gameObject);
			}
			Initialize();
			UpdateTextMeshes();
		}
	}

	public string Text
	{
		get
		{
			return text;
		}
		set
		{
			text = value;
			if (wordWrap)
			{
				text = textWrapper.Wrap(text, wordWrapWidth, wrapStyle);
			}
			UpdateTextMeshes();
		}
	}

	public bool WordWrap
	{
		get
		{
			return wordWrap;
		}
		set
		{
			wordWrap = value;
			if (wordWrap && textWrapper == null && textMeshes != null)
			{
				TextMesh textMesh = textMeshes.First();
				textWrapper = new TextWrapper(textMesh.font, textMesh.GetComponent<Renderer>().material, textMesh.fontSize);
			}
		}
	}

	public Color Color
	{
		get
		{
			return color;
		}
		set
		{
			color = value;
			TextMesh textMesh = textMeshes.First((TextMesh mesh) => mesh.name.Equals("Front"));
			if (textMesh != null)
			{
				textMesh.GetComponent<Renderer>().material.SetColor("_Color", value);
			}
		}
	}

	public Color ShadowColor
	{
		get
		{
			return shadowColor;
		}
		set
		{
			shadowColor = value;
			if (hasShadow)
			{
				TextMesh textMesh = textMeshes.First((TextMesh mesh) => mesh.name.Equals("Shadow"));
				if (textMesh != null)
				{
					textMesh.GetComponent<Renderer>().material.SetColor("_Color", value);
				}
			}
		}
	}

	public bool HasShadow
	{
		get
		{
			return hasShadow;
		}
		set
		{
			hasShadow = value;
			TextMesh textMesh = textMeshes.Find((TextMesh mesh) => mesh.name.Equals("Shadow"));
			if (hasShadow && textMesh == null)
			{
				AddTextMesh("Shadow", ShadowColor, GetShadowPos());
				ShadowColor = shadowColor;
			}
			else if (!hasShadow && textMesh != null)
			{
				Object.Destroy(textMesh.gameObject);
			}
		}
	}

	public new Vector3 Size => GetTextBounds();

	public float TextWidth => Size.x;

	public float TextHeight => Size.y;

	public int LineCount => text.Split('\n').Length;

	public override void Awake()
	{
		base.Awake();
		Initialize();
	}

	public void Start()
	{
		SetText();
		TM.LanguageChanged(SetText);
		Color = color;
		ShadowColor = shadowColor;
	}

	private void SetText()
	{
		Text = TM._(text);
	}

	public void Initialize()
	{
		fontManager = UXUtils.FindGUIObjectOfType<UXFontManager>();
		textMeshes = new List<TextMesh>();
		AddTextMesh("Front", Color, new Vector3(0f, 0.3f, 0f));
		if (hasShadow)
		{
			AddTextMesh("Shadow", ShadowColor, GetShadowPos());
		}
		if (wordWrap)
		{
			textWrapper = new TextWrapper(fontManager.GetFont(textSize), fontManager.GetFontMaterial(textSize), 0);
		}
		textSizeCalculator = UXUtils.FindGUIObjectOfType<UXTextSizeCalculator>();
	}

	public override void SetAlpha(float alpha, string materialProperty = "_MainColor")
	{
		Color color = Color;
		Color color2 = ShadowColor;
		color.a = alpha;
		color2.a = alpha;
		Color = color;
		ShadowColor = color2;
	}

	private void AddTextMesh(string name, Color color, Vector3 offset)
	{
		GameObject gameObject = new GameObject(name);
		gameObject.layer = LayerMask.NameToLayer("UXElement");
		gameObject.transform.parent = transform;
		gameObject.transform.localScale = Vector3.one;
		gameObject.transform.localPosition = offset;
		TextMesh textMesh = gameObject.AddComponent<TextMesh>();
		textMesh.font = fontManager.GetFont(textSize);
		textMesh.lineSpacing = 0.8f;
		meshRenderer = gameObject.GetComponent<MeshRenderer>();
		meshRenderer.material = new Material(fontManager.GetFontMaterial(textSize));
		meshRenderer.material.SetColor("_Color", color);
		textMeshes.Add(textMesh);
	}

	public override void Update()
	{
		Rect clippedBounds = GetClippedBounds();
		Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>(includeInactive: true);
		foreach (Renderer renderer in componentsInChildren)
		{
			renderer.material.SetVector("_ClipRect", new Vector4(clippedBounds.xMin, clippedBounds.yMin, clippedBounds.xMax, clippedBounds.yMax));
		}
	}

	public int GetNumberOfLines()
	{
		return text.Split('\n').Length;
	}

	public Vector2 RenderAndGetTextBounds()
	{
		return textMeshes.First().GetComponent<Renderer>().bounds.size.xy() / screen.Scale;
	}

	private Vector2 GetTextBounds()
	{
		return textSizeCalculator.MeasureString(Text, transform.localScale.xy(), TextSize);
	}

	private void UpdateTextMeshes()
	{
		foreach (TextMesh textMesh in textMeshes)
		{
			if (!(textMesh == null))
			{
				textMesh.text = text;
				textMesh.alignment = UXEnums.ToTextAlignment(horizontalAlign);
				textMesh.anchor = UXEnums.ToTextAnchor(horizontalAlign, verticalAlign);
			}
		}
	}

	private Vector3 GetShadowPos()
	{
		return new Vector3(0.1f, 0.2f, 0.1f);
	}
}
