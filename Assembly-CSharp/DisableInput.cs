using UnityEngine;

public class DisableInput : MonoBehaviour
{
	private void Update()
	{
		MVInputWrapper.SuppressShortcutKeys();
		MVInputWrapper.SuppressInGameInput();
		MVInputWrapper.SuppressAllInput();
	}
}
