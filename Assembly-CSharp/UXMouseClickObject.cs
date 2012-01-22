using System;
using UnityEngine;

[AddComponentMenu("UX/Handlers/Mouse click object")]
public class UXMouseClickObject : MonoBehaviour
{
	public delegate void OnClickDelegate(UXMouseClickObject clickObject, Vector3 mousePositionWorld);

	public delegate bool OnMouseDownDelegate(UXMouseClickObject clickObject, Vector3 mousePositionWorld);

	public delegate void OnMouseDownMoveDelegate(UXMouseClickObject clickObject, Vector3 mousePositionWorld);

	public delegate void OnMouseUpDelegate(UXMouseClickObject clickObject, Vector3 mousePositionWorld);

	public OnClickDelegate OnClick;

	public OnMouseDownDelegate OnMouseDown;

	public OnMouseDownMoveDelegate OnMouseDownMove;

	public OnMouseUpDelegate OnMouseUp;

	private ILogger logger;

	public void Awake()
	{
		logger = LoggerManager.Instance.GetLogger(typeof(UXMouseClickObject));
	}

	public void NotifyOnClick(Vector3 mousePositionWorld)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		logger.Log($"{((Object)((Component)this).gameObject).name} clicked");
		if (OnClick != null)
		{
			OnClick(this, mousePositionWorld);
		}
	}

	public bool NotifyMouseDown(Vector3 mousePositionWorld)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		bool result = false;
		if (OnMouseDown != null)
		{
			Delegate[] invocationList = OnMouseDown.GetInvocationList();
			for (int i = 0; i < invocationList.Length; i++)
			{
				OnMouseDownDelegate onMouseDownDelegate = (OnMouseDownDelegate)invocationList[i];
				if (onMouseDownDelegate(this, mousePositionWorld))
				{
					result = true;
				}
			}
		}
		return result;
	}

	public void NotifyMouseDownMove(Vector3 mousePositionWorld)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		if (OnMouseDownMove != null)
		{
			OnMouseDownMove(this, mousePositionWorld);
		}
	}

	public void NotifyMouseUp(Vector3 mousePositionWorld)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		if (OnMouseUp != null)
		{
			OnMouseUp(this, mousePositionWorld);
		}
	}
}
