using System;
using UnityEngine;

public class MVGUIAskForFocus : MonoBehaviour
{
	public GameObject clickWall;

	private BoxCollider _clickWallCollider;

	public UXGroup clickControlGroup;

	public UXGroup registerGroup;

	public UXTextButton buttonContinue;

	public UXTextButton buttonRegister;

	public bool showInDevelopment;

	private bool _focusFix;

	private UXScreen _uxScreen;

	private bool _isInitialized;

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

	private bool AllowLoseFocus => (Application.isEditor && showInDevelopment) || (Debug.isDebugBuild && showInDevelopment) || (!Debug.isDebugBuild && Application.isPlaying && !Application.isEditor);

	public void LoseFocus()
	{
		if (AllowLoseFocus)
		{
			clickControlGroup.SetVisible(visible: true);
			if (MVGameController.Instance.WOCM != null && MVGameController.Instance.Game.LocalPlayer.IsAnonymous)
			{
				registerGroup.SetVisible(visible: true);
			}
			((Collider)ClickWallCollider).enabled = true;
		}
	}

	public void RegainFocus()
	{
		clickControlGroup.SetVisible(visible: false);
		registerGroup.SetVisible(visible: false);
		((Collider)ClickWallCollider).enabled = false;
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

	private void Start()
	{
		if (!_isInitialized)
		{
			UXMouseClickObject component = ((Component)buttonRegister).GetComponent<UXMouseClickObject>();
			component.OnMouseDown = (UXMouseClickObject.OnMouseDownDelegate)Delegate.Combine(component.OnMouseDown, (UXMouseClickObject.OnMouseDownDelegate)((UXMouseClickObject mouseClickObject, Vector3 mouseClickPos) =>
			{
				OnRegisterClick();
				return false;
			}));
			_isInitialized = true;
		}
	}

	private void OnResize()
	{
		UpdatePlacement();
	}

	private void OnApplicationFocus(bool focus)
	{
		if (!focus)
		{
			MVInputWrapper.hasLostFocus = true;
		}
		if (UXUtils.FindGUIObjectOfType<UXScreen>().Fullscreen)
		{
			focus = true;
		}
		if (focus && _focusFix)
		{
			_focusFix = false;
			RegainFocus();
		}
		if (!focus)
		{
			LoseFocus();
		}
	}

	public void DoFocusFix()
	{
		_focusFix = true;
	}

	private void UpdatePlacement()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = _uxScreen.GetPosition(UXHorizontal.Left, UXVertical.Top, 0f) - _uxScreen.GetPosition(UXHorizontal.Right, UXVertical.Bottom, 0f);
		ClickWallCollider.size = new Vector3(Mathf.Abs(val.x), Mathf.Abs(val.y), 10f);
	}

	private void OnRegisterClick()
	{
		Debug.Log((object)"Send register call");
		Application.ExternalCall("gotoRegisterForm", new object[1] { "play" });
	}
}
