using UnityEngine;

public class UXGroup : MonoBehaviour
{
	public bool Visible;

	public void Start()
	{
		SetVisibility(Visible);
	}

	public void SetVisibility(bool visible)
	{
		Renderer[] componentsInChildren = ((Component)this).GetComponentsInChildren<Renderer>();
		foreach (Renderer val in componentsInChildren)
		{
			val.enabled = visible;
		}
	}

	public void Hide()
	{
		SetVisibility(visible: false);
	}

	public void Show()
	{
		SetVisibility(visible: true);
	}
}
