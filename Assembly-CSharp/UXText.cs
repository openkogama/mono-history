using System.Collections.Generic;
using System.Linq;
using Localize;
using UnityEngine;

public class UXText : UXGUIElement
{
	public UXTextSize textSize = UXTextSize.Medium;

	public TextSlotIndex index = TextSlotIndex.Empty;

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
				Object.Destroy((Object)(object)((Component)textMesh).gameObject);
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
				TextMesh val = textMeshes.First();
				textWrapper = new TextWrapper(val.font, ((Component)val).renderer.material, val.fontSize);
			}
		}
	}

	public Color Color
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return color;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			color = value;
			TextMesh val = textMeshes.First((TextMesh mesh) => ((Object)mesh).name.Equals("Front"));
			if ((Object)(object)val != (Object)null)
			{
				((Component)val).renderer.material.SetColor("_Color", value);
			}
		}
	}

	public Color ShadowColor
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return shadowColor;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			shadowColor = value;
			if (hasShadow)
			{
				TextMesh val = textMeshes.First((TextMesh mesh) => ((Object)mesh).name.Equals("Shadow"));
				if ((Object)(object)val != (Object)null)
				{
					((Component)val).renderer.material.SetColor("_Color", value);
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
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			hasShadow = value;
			TextMesh val = textMeshes.Find((TextMesh mesh) => ((Object)mesh).name.Equals("Shadow"));
			if (hasShadow && (Object)(object)val == (Object)null)
			{
				AddTextMesh("Shadow", ShadowColor, GetShadowPos());
				ShadowColor = shadowColor;
			}
			else if (!hasShadow && (Object)(object)val != (Object)null)
			{
				Object.Destroy((Object)(object)((Component)val).gameObject);
			}
		}
	}

	public new Vector3 Size
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return Vector2.op_Implicit(GetTextBounds());
		}
	}

	public float TextWidth
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return Size.x;
		}
	}

	public float TextHeight
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return Size.y;
		}
	}

	public int LineCount => text.Split(new char[1] { '\n' }).Length;

	public UXText()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
	}

	public override void Awake()
	{
		base.Awake();
		Initialize();
	}

	public void Start()
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		if (index == TextSlotIndex.Empty)
		{
			Text = text;
		}
		else
		{
			Text = Localization.Instance.GetText(index);
		}
		Color = color;
		ShadowColor = shadowColor;
	}

	public void Initialize()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		Color val = Color;
		Color val2 = ShadowColor;
		val.a = alpha;
		val2.a = alpha;
		Color = val;
		ShadowColor = val2;
	}

	private void AddTextMesh(string name, Color color, Vector3 offset)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected Obj, but got Unknown
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Expected Obj, but got Unknown
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = new GameObject(name);
		val.layer = LayerMask.NameToLayer("UXElement");
		val.transform.parent = ((Component)this).transform;
		val.transform.localScale = Vector3.one;
		val.transform.localPosition = offset;
		TextMesh val2 = val.AddComponent<TextMesh>();
		val2.font = fontManager.GetFont(textSize);
		val2.lineSpacing = 0.8f;
		MeshRenderer val3 = val.AddComponent<MeshRenderer>();
		((Renderer)val3).material = new Material(fontManager.GetFontMaterial(textSize));
		((Renderer)val3).material.SetColor("_Color", color);
		textMeshes.Add(val2);
	}

	public override void Update()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		Rect clippedBounds = GetClippedBounds();
		Renderer[] componentsInChildren = ((Component)this).GetComponentsInChildren<Renderer>(true);
		foreach (Renderer val in componentsInChildren)
		{
			val.material.SetVector("_ClipRect", new Vector4(clippedBounds.xMin, clippedBounds.yMin, clippedBounds.xMax, clippedBounds.yMax));
		}
	}

	public int GetNumberOfLines()
	{
		return text.Split(new char[1] { '\n' }).Length;
	}

	public Vector2 RenderAndGetTextBounds()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		Bounds bounds = ((Component)textMeshes.First()).renderer.bounds;
		return bounds.size.xy() / screen.Scale;
	}

	private Vector2 GetTextBounds()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		return textSizeCalculator.MeasureString(Text, ((Component)this).transform.localScale.xy(), TextSize);
	}

	private void UpdateTextMeshes()
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		foreach (TextMesh textMesh in textMeshes)
		{
			if (!((Object)(object)textMesh == (Object)null))
			{
				textMesh.text = text;
				textMesh.alignment = UXEnums.ToTextAlignment(horizontalAlign);
				textMesh.anchor = UXEnums.ToTextAnchor(horizontalAlign, verticalAlign);
			}
		}
	}

	private Vector3 GetShadowPos()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		return new Vector3(0.1f, 0.2f, 0.1f);
	}
}
