using UnityEngine;

public abstract class UXLine : UXGUIElement, IUXContainer
{
	[HideInInspector]
	public bool killed;

	public virtual void DestroyLine()
	{
	}

	public virtual Vector2 GetLineSize()
	{
		return new Vector2(Width, Height);
	}
}
