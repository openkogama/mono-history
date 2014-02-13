using UnityEngine;

[AddComponentMenu("UX/Handlers/Mouse over object")]
public class UXMouseOverObject : MonoBehaviour
{
	public delegate void OnMouseOverDelegate(UXMouseOverObject mouseOverObject);

	public OnMouseOverDelegate OnMouseOverEnter;

	public OnMouseOverDelegate OnMouseOver;

	public OnMouseOverDelegate OnMouseOverExit;

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

	public void NotifyOnMouseOver()
	{
		if (OnMouseOver != null)
		{
			OnMouseOver(this);
		}
	}
}
