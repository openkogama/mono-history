using UnityEngine;
using UnityEngine.UI;

public class NotificationPopup : MonoBehaviour
{
	[SerializeField]
	private Text text;

	[SerializeField]
	private Text header;

	[SerializeField]
	public bool hideAll;

	public void Initialize(string text, string header)
	{
		this.text.text = text;
		this.header.text = header;
	}
}
