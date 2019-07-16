using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class ConfirmationPopup : MonoBehaviour
{
	private UnityAction<bool, ConfirmationPopup> resultCallback;

	[SerializeField]
	private Text text;

	[SerializeField]
	private Text header;

	[SerializeField]
	private Button okButton;

	[SerializeField]
	private Button cancelButton;

	[SerializeField]
	public bool hideAll;

	public void Initialize(string text, UnityAction<bool, ConfirmationPopup> resultCallback, string header)
	{
		this.text.text = text;
		this.header.text = header;
		this.resultCallback = resultCallback;
		okButton.onClick.AddListener(Ok);
		cancelButton.onClick.AddListener(Cancel);
	}

	public void Pop()
	{
		ExecuteEvents.ExecuteHierarchy(gameObject, null, (IUIStack x, BaseEventData y) =>
		{
			x.Pop();
		});
	}

	private void Ok()
	{
		if (resultCallback != null)
		{
			resultCallback(arg0: true, this);
		}
		resultCallback = null;
	}

	private void Cancel()
	{
		if (resultCallback != null)
		{
			resultCallback(arg0: false, this);
		}
		resultCallback = null;
	}
}
