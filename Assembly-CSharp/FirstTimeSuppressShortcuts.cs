using UnityEngine;

public class FirstTimeSuppressShortcuts : MonoBehaviour
{
	private void Update()
	{
		MVInputWrapper.SuppressShortcutKeys();
	}
}
