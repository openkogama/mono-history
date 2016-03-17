using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public abstract class ToggleStatHandlerBase : MonoBehaviour
{
	private bool waitingForToggleCallback;

	[SerializeField]
	protected Button button;

	[SerializeField]
	protected bool toggleState;

	[SerializeField]
	protected UnityAction<bool> toggleCallback;

	[SerializeField]
	protected ToggleHandler toggleHandler;

	public bool ToggleState
	{
		get
		{
			return toggleState;
		}
		set
		{
			if (waitingForToggleCallback)
			{
				Debug.LogError("Overriding toggle state while waiting for callback. This is undefined behaviour");
			}
			toggleState = value;
			UpdateToggleState();
		}
	}

	protected abstract void UpdateToggleState();

	private void Start()
	{
		UpdateToggleState();
		button.onClick.AddListener(Toggle);
	}

	private void Reset()
	{
		button = GetComponent<Button>();
	}

	private void OnValidate()
	{
		UpdateToggleState();
	}

	public void Toggle()
	{
		if (!waitingForToggleCallback)
		{
			bool flag = !toggleState;
			if (toggleHandler != null)
			{
				waitingForToggleCallback = true;
				toggleHandler.ExecuteToggleState(flag, ToggleCallback);
			}
		}
	}

	private void ToggleCallback(bool toggleState)
	{
		this.toggleState = toggleState;
		UpdateToggleState();
		waitingForToggleCallback = false;
	}
}
