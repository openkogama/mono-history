using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonStyleObject : MonoBehaviour
{
	[SerializeField]
	private Button button;

	[SerializeField]
	private ButtonStyle buttonStyle = ButtonStyle.RegularButton;

	[SerializeField]
	private ColorStyle colorStyle = ColorStyle.LightBlue;

	[SerializeField]
	private SoundStyle soundStyle;

	private void Awake()
	{
		Styles.SetStyle(button, buttonStyle, colorStyle, soundStyle);
	}

	private void Reset()
	{
		button = GetComponent<Button>();
		Styles.SetStyle(button, buttonStyle, colorStyle, SoundStyle.NoSound);
	}

	private void OnValidate()
	{
		if (!Application.isPlaying)
		{
			Styles.SetStyle(button, buttonStyle, colorStyle, SoundStyle.NoSound);
		}
	}
}
