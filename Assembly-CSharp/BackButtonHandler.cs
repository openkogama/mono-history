using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class BackButtonHandler : MonoBehaviour
{
	[SerializeField]
	private Button invokeButton;

	[SerializeField]
	private KogamaControls kogamaControl = KogamaControls.Escape;

	[SerializeField]
	private KeyState onKeyState = KeyState.Down;

	private void OnEnable()
	{
		BackButtonManager.Subscribe(this, kogamaControl, onKeyState, InvokeButton);
	}

	private void InvokeButton()
	{
		if (invokeButton.interactable)
		{
			invokeButton.onClick.Invoke();
		}
	}

	private void OnDisable()
	{
		BackButtonManager.Unsubscribe(this);
	}
}
