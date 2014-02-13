using MV.WorldObject;
using UnityEngine;

public abstract class MVGUIItemAction : MonoBehaviour
{
	public delegate void OnActionCompletedDelegate();

	public OnActionCompletedDelegate OnActionCompleted;

	protected MVItem item;

	protected bool _isInitialized;

	protected UXGroup ActionGroup => ((Component)this).GetComponent<UXGroup>();

	public virtual void UpdateItemAction(MVItem item)
	{
		this.item = item;
		if (!_isInitialized)
		{
			Initialize();
		}
	}

	protected virtual void Initialize()
	{
		_isInitialized = true;
	}

	protected void FireOnActionCompleted()
	{
		if (OnActionCompleted != null)
		{
			OnActionCompleted();
		}
	}
}
