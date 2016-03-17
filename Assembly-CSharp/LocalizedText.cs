using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class LocalizedText : MonoBehaviour
{
	[SerializeField]
	private Text text;

	private void Awake()
	{
		text.text = TM._(text.text);
		TM.LanguageChanged(LanguageLoadedCallback);
	}

	private void LanguageLoadedCallback()
	{
		text.text = TM._(text.text);
	}

	private void Reset()
	{
		text = GetComponent<Text>();
		text.text = "_(\"Text\")";
	}
}
