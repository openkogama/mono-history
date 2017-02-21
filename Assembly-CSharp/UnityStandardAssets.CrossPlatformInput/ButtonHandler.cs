using UnityEngine;

namespace UnityStandardAssets.CrossPlatformInput;

public class ButtonHandler : MonoBehaviour
{
	public string Name;

	private void OnEnable()
	{
	}

	private void OnDisable()
	{
		if (CrossPlatformInputManager.GetButton(Name))
		{
			SetUpState();
		}
	}

	public void SetDownState()
	{
		CrossPlatformInputManager.SetButtonDown(Name);
	}

	public void SetUpState()
	{
		CrossPlatformInputManager.SetButtonUp(Name);
	}

	public void SetAxisPositiveState()
	{
		CrossPlatformInputManager.SetAxisPositive(Name);
	}

	public void SetAxisNeutralState()
	{
		CrossPlatformInputManager.SetAxisZero(Name);
	}

	public void SetAxisNegativeState()
	{
		CrossPlatformInputManager.SetAxisNegative(Name);
	}
}
