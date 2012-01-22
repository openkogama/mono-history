using UnityEngine;

[AddComponentMenu("UX/Handlers/Mouse over object")]
public class UXMouseOverObject : MonoBehaviour
{
	public delegate void OnMouseOverEnterDelegate(UXMouseOverObject mouseOverObject);

	public delegate void OnMouseOverExitDelegate(UXMouseOverObject mouseOverObject);

	public OnMouseOverEnterDelegate OnMouseOverEnter;

	public OnMouseOverExitDelegate OnMouseOverExit;

	public void NotifyOnMouseOverEnter()
	{
		if (OnMouseOverEnter != null)
		{
			OnMouseOverEnter(this);
		}
	}

	public void NotifyOnMouseOverExit()
	{
		if (OnMouseOverExit != null)
		{
			OnMouseOverExit(this);
		}
	}
}
