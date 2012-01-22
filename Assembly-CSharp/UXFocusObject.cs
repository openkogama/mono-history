using UnityEngine;

[AddComponentMenu("UX/Handlers/Focus object")]
public class UXFocusObject : MonoBehaviour
{
	public delegate void OnFocusEnterDelegate(UXFocusObject focusObject);

	public delegate void OnFocusExitDelegate(UXFocusObject focusObject);

	public delegate void OnNextFocusRequestDelegate(UXFocusObject focusObject);

	public delegate void OnPreviousFocusRequestDelegate(UXFocusObject focusObject);

	public OnFocusEnterDelegate OnFocusEnter;

	public OnFocusExitDelegate OnFocusExit;

	public OnNextFocusRequestDelegate OnNextFocusRequest;

	public OnPreviousFocusRequestDelegate OnPreviousFocusRequest;

	public void NotifyPreviousFocusRequest()
	{
		if (OnPreviousFocusRequest != null)
		{
			OnPreviousFocusRequest(this);
		}
	}

	public void NotifyNextFocusRequest()
	{
		if (OnNextFocusRequest != null)
		{
			OnNextFocusRequest(this);
		}
	}

	public void NotifyFocusExit()
	{
		if (OnFocusExit != null)
		{
			OnFocusExit(this);
		}
	}

	public void NotifyFocusEnter()
	{
		if (OnFocusEnter != null)
		{
			OnFocusEnter(this);
		}
	}
}
