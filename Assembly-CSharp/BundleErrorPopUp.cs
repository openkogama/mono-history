using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BundleErrorPopUp : MonoBehaviour
{
	[SerializeField]
	private Text header;

	[SerializeField]
	private Text buttonText;

	private UnityAction<bool> resultCallback;

	public void Initialize(UnityAction<bool> resultCallback, string header, string buttonText)
	{
		this.buttonText.text = buttonText;
		this.header.text = header;
		this.resultCallback = resultCallback;
	}

	public void OnButtonPressed()
	{
		resultCallback(arg0: true);
	}

	public void OnExit()
	{
		resultCallback(arg0: false);
	}
}
