using UnityEngine;

public class DisableInput : MonoBehaviour
{
	private void Update()
	{
		MVInputWrapper.IsShortcutKeysSuppressed = true;
		MVInputWrapper.IsInGameInputSuppressed = true;
		MVInputWrapper.IsInputSuppressed = true;
	}
}
