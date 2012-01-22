using UnityEngine;

public class UXVisibility : MonoBehaviour
{
	public delegate void VisibilityChangeDelegate(float visibility);

	public VisibilityChangeDelegate OnVisibilityChange;

	private float visibility;

	public float Visibility
	{
		get
		{
			return visibility;
		}
		set
		{
			visibility = Mathf.Clamp01(value);
			NotifyVisibilityChange();
		}
	}

	private void NotifyVisibilityChange()
	{
		if (OnVisibilityChange != null)
		{
			OnVisibilityChange(visibility);
		}
	}
}
