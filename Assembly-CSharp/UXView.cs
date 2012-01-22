using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("UX/View")]
public class UXView : MonoBehaviour
{
	public delegate void OnShowDelegate();

	public delegate void OnHideDelegate();

	public OnShowDelegate OnShow;

	public OnHideDelegate OnHide;

	public UXHorizontal horizontalAnchor = UXHorizontal.Center;

	public UXVertical verticalAnchor = UXVertical.Middle;

	public float depth;

	public bool isVisible;

	private UXScreen screen;

	private List<UXFocusObject> focusObjects;

	private UXFocusManager focusManager;

	private bool isInitialized;

	public void Start()
	{
		Initialize();
	}

	public void Initialize()
	{
		if (!isInitialized)
		{
			screen = UXUtils.FindObjectOfType<UXScreen>();
			UXScreen uXScreen = screen;
			uXScreen.OnResize = (UXScreen.OnResizeDelegate)Delegate.Combine(uXScreen.OnResize, new UXScreen.OnResizeDelegate(OnResize));
			focusManager = UXUtils.FindObjectOfType<UXFocusManager>();
			if (isVisible)
			{
				Show();
			}
			else
			{
				Hide();
			}
			InitializeFocusObjects();
			isInitialized = true;
		}
	}

	public UXFocusObject NextFocusObject(UXFocusObject currentFocusObject)
	{
		return GetFocusObject(currentFocusObject, 1);
	}

	public UXFocusObject PreviousFocusObject(UXFocusObject currentFocusObject)
	{
		return GetFocusObject(currentFocusObject, -1);
	}

	private UXFocusObject GetFocusObject(UXFocusObject currentFocusObject, int diff)
	{
		int num = focusObjects.IndexOf(currentFocusObject);
		if (num == -1)
		{
			return null;
		}
		int index = UXUtils.WrapIndex(num + diff, focusObjects.Count);
		return focusObjects[index];
	}

	public void Show()
	{
		UpdatePlacement();
		isVisible = true;
		if (OnShow != null)
		{
			OnShow();
		}
	}

	public void Hide()
	{
		if (OnHide != null)
		{
			OnHide();
		}
		isVisible = false;
	}

	public void OnResize()
	{
		UpdatePlacement();
	}

	public void UpdatePlacement()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).transform.position = screen.GetPosition(horizontalAnchor, verticalAnchor, depth);
		((Component)this).transform.localScale = screen.Scale * Vector3.one;
	}

	private void InitializeFocusObjects()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		focusObjects = new List<UXFocusObject>(((Component)this).GetComponentsInChildren<UXFocusObject>(true));
		Vector3 primarySortAxis = Vector3.down;
		Vector3 secondarySortAxis = Vector3.right;
		focusObjects.Sort((UXFocusObject a, UXFocusObject b) =>
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			Vector3 position = ((Component)a).transform.position;
			Vector3 position2 = ((Component)b).transform.position;
			float num = Vector3.Dot(position, primarySortAxis);
			float num2 = Vector3.Dot(position2, primarySortAxis);
			if (num > num2)
			{
				return 1;
			}
			if (num < num2)
			{
				return -1;
			}
			float num3 = Vector3.Dot(position, secondarySortAxis);
			float value = Vector3.Dot(position2, secondarySortAxis);
			return num3.CompareTo(value);
		});
	}

	public void RequestNextFocus()
	{
		focusManager.CurrentFocus = NextFocusObject(focusManager.CurrentFocus);
	}

	public void RequestPreviousFocus()
	{
		focusManager.CurrentFocus = PreviousFocusObject(focusManager.CurrentFocus);
	}

	public void RequestFocus(UXFocusObject focusObject)
	{
		if ((Object)(object)focusObject == (Object)null)
		{
			throw new ArgumentNullException();
		}
		if (focusObjects.Contains(focusObject))
		{
			focusManager.CurrentFocus = focusObject;
		}
	}

	public void OnDestroy()
	{
		UXScreen uXScreen = screen;
		uXScreen.OnResize = (UXScreen.OnResizeDelegate)Delegate.Remove(uXScreen.OnResize, new UXScreen.OnResizeDelegate(OnResize));
	}

	public void SetVisible(bool isVisible)
	{
		if (isVisible != this.isVisible)
		{
			if (isVisible)
			{
				Show();
			}
			else
			{
				Hide();
			}
		}
	}

	public void ToggleVisibility()
	{
		SetVisible(!isVisible);
	}

	public void OnDrawGizmosSelected()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		Gizmos.color = Color.white;
		Matrix4x4 localToWorldMatrix = ((Component)this).transform.localToWorldMatrix;
		float num;
		if (horizontalAnchor != UXHorizontal.Left)
		{
			num = ((horizontalAnchor != UXHorizontal.Center) ? (-1f) : 0f);
		}
		else
		{
			num = 1f;
		}
		float num2;
		if (verticalAnchor != UXVertical.Top)
		{
			num2 = ((verticalAnchor != UXVertical.Middle) ? 1f : 0f);
		}
		else
		{
			num2 = -1f;
		}
		float num3 = 1f;
		Vector3 val = localToWorldMatrix.MultiplyPoint(num3 * new Vector3(0f, num2 + 1f, 0f));
		Vector3 val2 = localToWorldMatrix.MultiplyPoint(num3 * new Vector3(0f, num2 - 1f, 0f));
		Vector3 val3 = localToWorldMatrix.MultiplyPoint(num3 * new Vector3(num - 1f, 0f, 0f));
		Vector3 val4 = localToWorldMatrix.MultiplyPoint(num3 * new Vector3(num + 1f, 0f, 0f));
		Gizmos.DrawLine(val, val2);
		Gizmos.DrawLine(val3, val4);
	}
}
