using System.Linq;
using UnityEngine;

[AddComponentMenu("UX/Elements/Text")]
public class UXText : MonoBehaviour
{
	public string text;

	public UXHorizontal horizontalAlign;

	public UXVertical verticalAlign;

	public bool wordWrap;

	public float wordWrapWidth = 40f;

	private TextMesh[] textMeshes;

	private TextWrapper textWrapper;

	private bool isInitialized;

	public float Alpha
	{
		get
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			return ((Component)textMeshes.First()).renderer.material.GetColor("_Color").a;
		}
		set
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			TextMesh[] array = textMeshes;
			foreach (TextMesh val in array)
			{
				Color color = ((Component)val).renderer.material.GetColor("_Color");
				color.a = value;
				((Component)val).renderer.material.SetColor("_Color", color);
			}
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
				text = textWrapper.Wrap(text, wordWrapWidth * 2f);
			}
			UpdateTextMeshes();
		}
	}

	public void Awake()
	{
		Initialize();
	}

	public void Initialize()
	{
		if (!isInitialized)
		{
			textMeshes = ((Component)this).GetComponentsInChildren<TextMesh>(true);
			TextMesh val = textMeshes.First();
			textWrapper = new TextWrapper(val.font, val.fontSize);
			isInitialized = true;
		}
	}

	public void OnEnable()
	{
		Initialize();
	}

	public void Start()
	{
		Text = text;
	}

	private void UpdateTextMeshes()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		TextMesh[] array = textMeshes;
		foreach (TextMesh val in array)
		{
			val.text = text;
			val.alignment = UXEnums.ToTextAlignment(horizontalAlign);
			val.anchor = UXEnums.ToTextAnchor(horizontalAlign, verticalAlign);
		}
	}
}
