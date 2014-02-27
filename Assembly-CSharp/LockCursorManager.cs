using System;
using UnityEngine;

public class LockCursorManager : MonoBehaviour
{
	private static bool hasFocus = true;

	private static bool suppressScreenLock;

	[SerializeField]
	private GameObject clickWall;

	private BoxCollider _clickWallCollider;

	private UXScreen _uxScreen;

	private bool regainFocus;

	public bool showInDevelopment;

	private BoxCollider ClickWallCollider
	{
		get
		{
			if ((Object)(object)_clickWallCollider == (Object)null)
			{
				BuildClickWall();
			}
			return _clickWallCollider;
		}
	}

	public static bool HasFocus => hasFocus;

	public static bool LockCursor => Screen.lockCursor;

	public static bool HasFocusAndLockCursor => HasFocus && LockCursor;

	private void LoseFocus()
	{
		hasFocus = false;
		((Collider)ClickWallCollider).enabled = true;
	}

	public static void SuppressScreenLock()
	{
		suppressScreenLock = true;
	}

	public void ForceLoseFocus()
	{
		HandleApplicationFocusChange(focus: false);
	}

	private void Awake()
	{
		_uxScreen = UXUtils.FindGUIObjectOfType<UXScreen>();
		UXScreen uxScreen = _uxScreen;
		uxScreen.OnResize = (UXScreen.OnResizeDelegate)Delegate.Combine(uxScreen.OnResize, new UXScreen.OnResizeDelegate(OnResize));
		if ((Object)(object)_clickWallCollider == (Object)null)
		{
			BuildClickWall();
		}
		OnResize();
	}

	private void LateUpdate()
	{
		HandleCursorLock();
		suppressScreenLock = false;
		if (regainFocus)
		{
			MVInputWrapper.ignoreAllKeys = false;
			((Collider)ClickWallCollider).enabled = false;
			regainFocus = false;
			hasFocus = true;
		}
	}

	private void OnApplicationFocus(bool focus)
	{
		Debug.Log((object)("OnApplicationFocus: " + focus));
		HandleApplicationFocusChange(focus);
	}

	private void HandleApplicationFocusChange(bool focus)
	{
		if (!focus)
		{
			MVInputWrapper.hasLostFocus = true;
		}
		if (UXUtils.FindGUIObjectOfType<UXScreen>().Fullscreen)
		{
			RegainFocus();
		}
		if (!focus)
		{
			LoseFocus();
			MVInputWrapper.ignoreAllKeys = true;
		}
	}

	private void HandleCursorLock()
	{
		if (MVGameController.Instance.Game.IsPlaying)
		{
			HandlePlayingCursorLock();
		}
		else
		{
			Screen.lockCursor = false;
		}
	}

	private void HandlePlayingCursorLock()
	{
		MVGUIMenu mVGUIMenu = UXUtils.FindGUIObjectOfType<MVGUIMenu>();
		bool flag = (Object)(object)mVGUIMenu != (Object)null && mVGUIMenu.View.isVisible;
		UXDialogFactory uXDialogFactory = UXUtils.FindGUIObjectOfType<UXDialogFactory>();
		bool flag2 = (Screen.lockCursor = !flag && !uXDialogFactory.DialogOpen && !suppressScreenLock && hasFocus);
		if (flag2 && !Screen.lockCursor)
		{
			HandleApplicationFocusChange(focus: false);
		}
	}

	private void RegainFocus()
	{
		regainFocus = true;
	}

	private void BuildClickWall()
	{
		_clickWallCollider = clickWall.AddComponent<BoxCollider>();
		UXMouseClickObject uXMouseClickObject = clickWall.AddComponent<UXMouseClickObject>();
		uXMouseClickObject.OnMouseDown = (UXMouseClickObject clickObject, Vector3 mousePositionWorld) => true;
		uXMouseClickObject.OnMouseUp = (UXMouseClickObject clickObject, Vector3 mousePositionWorld) =>
		{
			RegainFocus();
			((Collider)_clickWallCollider).enabled = false;
		};
		((Collider)_clickWallCollider).enabled = false;
	}

	private void OnResize()
	{
		UpdatePlacement();
	}

	private void UpdatePlacement()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = _uxScreen.GetPosition(UXHorizontal.Left, UXVertical.Top, 0f) - _uxScreen.GetPosition(UXHorizontal.Right, UXVertical.Bottom, 0f);
		ClickWallCollider.size = new Vector3(Mathf.Abs(val.x), Mathf.Abs(val.y), 0f);
	}
}
