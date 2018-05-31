using UnityEngine;
using UnityEngine.UI;

public class TextButton : MonoBehaviour
{
	[SerializeField]
	private Button button;

	[SerializeField]
	private Text text;

	public Button Button => button;

	public Text Text => text;

	protected void Reset()
	{
		if (button == null)
		{
			button = GetComponent<Button>();
			if (button == null)
			{
				button = GetComponentInChildren<Button>();
			}
		}
		if (text == null)
		{
			text = GetComponent<Text>();
			if (text == null)
			{
				text = GetComponentInChildren<Text>();
			}
		}
	}
}
