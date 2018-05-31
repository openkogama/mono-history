using UnityEngine;

public abstract class ThemeComponent : MonoBehaviour
{
	public abstract void Activate();

	public abstract void Deactivate();

	public void Initialize(Theme theme)
	{
		theme.Add(this);
	}
}
