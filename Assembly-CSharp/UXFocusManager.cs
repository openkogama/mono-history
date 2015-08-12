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
			if (value != currentFocus)
			{
				if (currentFocus != null)
				{
					currentFocus.NotifyFocusExit();
				}
				currentFocus = value;
				if (currentFocus != null)
				{
					currentFocus.NotifyFocusEnter();
				}
			}
		}
	}

	public void Update()
	{
		if (currentFocus != null && MVInputWrapper.GetBooleanControlUp(KogamaControls.ChangeFocus))
		{
			if (MVInputWrapper.GetBooleanControl(KogamaControls.ChangeChangeFocusDirection))
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
