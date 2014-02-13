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
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return new Vector2(Width, Height);
	}
}
