using UnityEngine;

[AddComponentMenu("UX/Management/Focus manager")]
public class UXFocusManager : MonoBehaviour
{
	private UXFocusObject currentFocus;

	public UXFocusObject CurrentFocus
	{
		get
		{
			return currentFocus;
		}
		set
		{
			if ((Object)(object)value != (Object)(object)currentFocus)
			{
				if ((Object)(object)currentFocus != (Object)null)
				{
					currentFocus.NotifyFocusExit();
				}
				currentFocus = value;
				if ((Object)(object)currentFocus != (Object)null)
				{
					currentFocus.NotifyFocusEnter();
				}
			}
		}
	}

	public void Update()
	{
		if ((Object)(object)currentFocus != (Object)null && Input.GetKeyDown((KeyCode)9))
		{
			if (Input.GetKey((KeyCode)304) || Input.GetKey((KeyCode)303))
			{
				currentFocus.NotifyPreviousFocusRequest();
			}
			else
			{
				currentFocus.NotifyNextFocusRequest();
			}
		}
	}
}
