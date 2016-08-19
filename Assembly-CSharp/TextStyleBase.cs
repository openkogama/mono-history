using UnityEngine;
using UnityEngine.UI;

public abstract class TextStyleBase : MonoBehaviour
{
	[SerializeField]
	private Text text;

	[SerializeField]
	private TextStyle textStyle;

	[SerializeField]
	private ColorStyle colorStyle = ColorStyle.White;

	private void Awake()
	{
		Styles.SetStyle(text, textStyle, colorStyle);
	}

	private void Reset()
	{
		text = GetComponent<Text>();
		Styles.SetStyle(text, textStyle, colorStyle);
	}

	private void OnValidate()
	{
		if (!Application.isPlaying && !(text == null))
		{
			Styles.SetStyle(text, textStyle, colorStyle);
		}
	}
}
