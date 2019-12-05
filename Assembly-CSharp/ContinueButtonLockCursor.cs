using System;
using UnityEngine;

public class ContinueButtonLockCursor : MonoBehaviour
{
	[SerializeField]
	private ContinueButtonHandler button;

	public void Initialize(Action cursorLockCallback)
	{
		button.OnClick = cursorLockCallback;
	}
}
