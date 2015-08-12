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

	public bool releaseFocusOnHide;

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
			screen = UXUtils.UXScreen;
			UXScreen uXScreen = screen;
			uXScreen.OnResize = (UXScreen.OnResizeDelegate)Delegate.Combine(uXScreen.OnResize, new UXScreen.OnResizeDelegate(OnResize));
			focusManager = UXUtils.FindGUIObjectOfType<UXFocusManager>();
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
		if (releaseFocusOnHide)
		{
			ReleaseFocus();
		}
	}

	public void OnResize()
	{
		UpdatePlacement();
	}

	public void UpdatePlacement()
	{
		transform.position = screen.GetPosition(horizontalAnchor, verticalAnchor, depth);
		transform.localScale = screen.Scale * Vector3.one;
	}

	private void InitializeFocusObjects()
	{
		focusObjects = new List<UXFocusObject>(GetComponentsInChildren<UXFocusObject>(includeInactive: true));
		Vector3 primarySortAxis = Vector3.down;
		Vector3 secondarySortAxis = Vector3.right;
		focusObjects.Sort((UXFocusObject a, UXFocusObject b) =>
		{
			Vector3 position = a.transform.position;
			Vector3 position2 = b.transform.position;
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

	public void ReleaseFocus()
	{
		if (focusManager != null)
		{
			focusManager.CurrentFocus = null;
		}
	}

	public void RequestFocus(UXFocusObject focusObject)
	{
		if (focusObject == null)
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
		if (screen != null)
		{
			UXScreen uXScreen = screen;
			uXScreen.OnResize = (UXScreen.OnResizeDelegate)Delegate.Remove(uXScreen.OnResize, new UXScreen.OnResizeDelegate(OnResize));
		}
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
		Gizmos.color = Color.white;
		Matrix4x4 localToWorldMatrix = transform.localToWorldMatrix;
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
		Vector3 vector = localToWorldMatrix.MultiplyPoint(num3 * new Vector3(0f, num2 + 1f, 0f));
		Vector3 to = localToWorldMatrix.MultiplyPoint(num3 * new Vector3(0f, num2 - 1f, 0f));
		Vector3 vector2 = localToWorldMatrix.MultiplyPoint(num3 * new Vector3(num - 1f, 0f, 0f));
		Vector3 to2 = localToWorldMatrix.MultiplyPoint(num3 * new Vector3(num + 1f, 0f, 0f));
		Gizmos.DrawLine(vector, to);
		Gizmos.DrawLine(vector2, to2);
	}
}
