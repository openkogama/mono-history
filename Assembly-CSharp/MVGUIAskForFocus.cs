using UnityEngine;

public class MVGUIAskForFocus : MonoBehaviour
{
	public UXGroup focusText;

	private void OnApplicationFocus(bool focus)
	{
		focusText.SetVisibility(!focus);
	}
}
