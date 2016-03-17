using System;
using System.Collections.Generic;
using System.Globalization;
using Gamestrap;
using MV.WorldObject;
using UnityEngine;
using UnityEngine.UI;

public class Styles : MonoBehaviour
{
	[Serializable]
	private class EffectStyleDef
	{
		[SerializeField]
		public EffectStyle effectStyle;

		[SerializeField]
		private bool shadow;

		[SerializeField]
		private Color shadowEffectColor = Color.black;

		[SerializeField]
		private Vector2 shadowEffectDistance = new Vector2(1f, -1f);

		[SerializeField]
		private bool shadowUseGraphicAlpha = true;

		[SerializeField]
		private bool outline;

		[SerializeField]
		private Color outlineEffectColor = Color.black;

		[SerializeField]
		private Vector2 outlineEffectDistance = new Vector2(1f, -1f);

		[SerializeField]
		private bool outlineUseGraphicAlpha = true;

		[SerializeField]
		private bool gradient;

		[SerializeField]
		private Color gradientTop = Color.white;

		[SerializeField]
		private Color gradientBottom = Color.gray;

		public void Set(EffectStyleObject effectStyleObject)
		{
			SetShadow(effectStyleObject.shadow);
			SetOutline(effectStyleObject.outline);
			SetGradient(effectStyleObject.gradient);
		}

		private void SetShadow(Shadow shadow)
		{
			shadow.enabled = this.shadow;
			shadow.effectColor = shadowEffectColor;
			shadow.effectDistance = shadowEffectDistance;
			shadow.useGraphicAlpha = shadowUseGraphicAlpha;
		}

		private void SetOutline(Outline outline)
		{
			outline.enabled = this.outline;
			outline.effectColor = outlineEffectColor;
			outline.effectDistance = outlineEffectDistance;
			outline.useGraphicAlpha = outlineUseGraphicAlpha;
		}

		private void SetGradient(GradientEffect gradient)
		{
			gradient.enabled = this.gradient;
			gradient.top = gradientTop;
			gradient.bottom = gradientBottom;
		}
	}

	[Serializable]
	private class ButtonStyleDef
	{
		[SerializeField]
		private ColorBlock colorBlock;

		[SerializeField]
		public ButtonStyle buttonStyle;

		public void Set(Button button)
		{
			button.colors = colorBlock;
		}
	}

	[Serializable]
	private class TextStyleDef
	{
		[SerializeField]
		public TextStyle textStyle;

		[SerializeField]
		private Font font;

		[SerializeField]
		private int fontSize;

		[SerializeField]
		private FontStyle fontStyle;

		[SerializeField]
		private float lineSpacing;

		public void Set(Text text)
		{
			text.font = font;
			text.fontSize = fontSize;
			text.fontStyle = fontStyle;
			text.lineSpacing = lineSpacing;
		}
	}

	[Serializable]
	private class SoundStyleDef
	{
		[SerializeField]
		public SoundStyle soundStyle;

		[SerializeField]
		public AudioSource audioSource;
	}

	[Serializable]
	private class ColorStyleDef
	{
		[SerializeField]
		public ColorStyle colorStyle;

		[SerializeField]
		public Color color;

		public void Set(Graphic graphic)
		{
			graphic.color = color;
		}
	}

	[Serializable]
	private class TeamIconStyleDef
	{
		[SerializeField]
		public MVTeam team;

		[SerializeField]
		public Sprite sprite;
	}

	private static Dictionary<MVTeam, ColorStyle> teamToColorStyle = new Dictionary<MVTeam, ColorStyle>
	{
		{
			MVTeam.None,
			ColorStyle.TeamNone
		},
		{
			MVTeam.Blue,
			ColorStyle.TeamBlue
		},
		{
			MVTeam.Red,
			ColorStyle.TeamRed
		},
		{
			MVTeam.Green,
			ColorStyle.TeamGreen
		},
		{
			MVTeam.Yellow,
			ColorStyle.TeamYellow
		}
	};

	private static Dictionary<ButtonStyle, ButtonStyleDef> buttonStylesDictionary = new Dictionary<ButtonStyle, ButtonStyleDef>();

	private static Dictionary<TextStyle, TextStyleDef> textStylesDictionary = new Dictionary<TextStyle, TextStyleDef>();

	private static Dictionary<ColorStyle, ColorStyleDef> colorStylesDictionary = new Dictionary<ColorStyle, ColorStyleDef>();

	private static Dictionary<MVTeam, TeamIconStyleDef> teamIconStylesDictionary = new Dictionary<MVTeam, TeamIconStyleDef>();

	private static Dictionary<EffectStyle, EffectStyleDef> effectStylesDictionary = new Dictionary<EffectStyle, EffectStyleDef>();

	private static Dictionary<SoundStyle, AudioSource> soundStylesDictionary = new Dictionary<SoundStyle, AudioSource>();

	[SerializeField]
	private List<ButtonStyleDef> buttonStyles = new List<ButtonStyleDef>();

	[SerializeField]
	private List<TextStyleDef> textStyles = new List<TextStyleDef>();

	[SerializeField]
	private List<ColorStyleDef> colorStyles = new List<ColorStyleDef>();

	[SerializeField]
	private List<TeamIconStyleDef> teamIconStyles = new List<TeamIconStyleDef>();

	[SerializeField]
	private List<EffectStyleDef> effectStyles = new List<EffectStyleDef>();

	[SerializeField]
	private List<SoundStyleDef> soundStyles = new List<SoundStyleDef>();

	private static bool isInitialized;

	private void Awake()
	{
		Initialize();
	}

	public void Initialize()
	{
		if (isInitialized)
		{
			return;
		}
		isInitialized = true;
		foreach (ButtonStyleDef buttonStyle in buttonStyles)
		{
			buttonStylesDictionary.Add(buttonStyle.buttonStyle, buttonStyle);
		}
		foreach (TextStyleDef textStyle in textStyles)
		{
			textStylesDictionary.Add(textStyle.textStyle, textStyle);
		}
		foreach (ColorStyleDef colorStyle in colorStyles)
		{
			colorStylesDictionary.Add(colorStyle.colorStyle, colorStyle);
		}
		foreach (TeamIconStyleDef teamIconStyle in teamIconStyles)
		{
			teamIconStylesDictionary.Add(teamIconStyle.team, teamIconStyle);
		}
		foreach (EffectStyleDef effectStyle in effectStyles)
		{
			effectStylesDictionary.Add(effectStyle.effectStyle, effectStyle);
		}
		foreach (SoundStyleDef soundStyle in soundStyles)
		{
			soundStylesDictionary.Add(soundStyle.soundStyle, soundStyle.audioSource);
		}
	}

	private static bool HandleUnInitalized()
	{
		if (isInitialized)
		{
			return true;
		}
		Styles styles = (Styles)UnityEngine.Object.FindObjectOfType(typeof(Styles));
		if (styles == null && !Application.isPlaying)
		{
			return false;
		}
		styles.Initialize();
		return isInitialized;
	}

	public static void SetStyle(Button button, ButtonStyle buttonStyle, ColorStyle colorStyle, SoundStyle soundStyle)
	{
		if (!HandleUnInitalized())
		{
			return;
		}
		buttonStylesDictionary[buttonStyle].Set(button);
		if (button.image != null)
		{
			SetStyle(button.image, colorStyle);
		}
		if (soundStyle == SoundStyle.NoSound)
		{
			return;
		}
		AudioSource audioSource = soundStylesDictionary[soundStyle];
		button.onClick.AddListener(() =>
		{
			if (audioSource == null)
			{
				Debug.LogWarning("Audio source was null");
			}
			else
			{
				audioSource.Play();
			}
		});
	}

	public static void SetStyle(Text text, TextStyle textStyle, ColorStyle colorStyle)
	{
		if (HandleUnInitalized())
		{
			textStylesDictionary[textStyle].Set(text);
			SetStyle(text, colorStyle);
		}
	}

	public static void SetStyle(Graphic graphic, ColorStyle colorStyle)
	{
		if (HandleUnInitalized())
		{
			colorStylesDictionary[colorStyle].Set(graphic);
		}
	}

	public static void SetStyle(Graphic image, MVTeam team)
	{
		if (HandleUnInitalized())
		{
			SetStyle(image, teamToColorStyle[team]);
		}
	}

	public static void SetStyle(Button button, ButtonStyle buttonStyle, MVTeam team, SoundStyle soundStyle = SoundStyle.NoSound)
	{
		if (HandleUnInitalized())
		{
			SetStyle(button, buttonStyle, teamToColorStyle[team], soundStyle);
		}
	}

	public static void SetStyle(EffectStyleObject effectStyleObject, EffectStyle effectStyle)
	{
		if (HandleUnInitalized())
		{
			effectStylesDictionary[effectStyle].Set(effectStyleObject);
		}
	}

	public static Color GetTeamColor(MVTeam team)
	{
		if (!HandleUnInitalized())
		{
			return Color.magenta;
		}
		return colorStylesDictionary[teamToColorStyle[team]].color;
	}

	public static string ColorToHex(Color32 color)
	{
		return color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
	}

	public static Color HexToColor(string hex)
	{
		byte r = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
		byte g = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
		byte b = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
		return new Color32(r, g, b, byte.MaxValue);
	}

	public static void TeamToSprite(Image image, MVTeam team)
	{
		image.sprite = teamIconStylesDictionary[team].sprite;
	}
}
