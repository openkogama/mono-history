using System;
using UnityEngine;

public class MVGUIClickWall : MonoBehaviour
{
	private UXScreen _uxScreen;

	private BoxCollider _clickWallCollider;

	private BoxCollider ClickWallCollider
	{
		get
		{
			if (_clickWallCollider == null)
			{
				BuildClickWall();
			}
			return _clickWallCollider;
		}
	}

	public bool EnableClickWall
	{
		get
		{
			return ClickWallCollider.enabled;
		}
		set
		{
			ClickWallCollider.enabled = value;
		}
	}

	public void Init()
	{
		_uxScreen = UXUtils.UXScreen;
		UXScreen uxScreen = _uxScreen;
		uxScreen.OnResize = (UXScreen.OnResizeDelegate)Delegate.Combine(uxScreen.OnResize, new UXScreen.OnResizeDelegate(OnResize));
		BuildClickWall();
		OnResize();
	}

	private void BuildClickWall()
	{
		_clickWallCollider = gameObject.AddComponent<BoxCollider>();
	}

	private void OnResize()
	{
		Vector3 vector = _uxScreen.GetPosition(UXHorizontal.Left, UXVertical.Top, 0f) - _uxScreen.GetPosition(UXHorizontal.Right, UXVertical.Bottom, 0f);
		ClickWallCollider.size = new Vector3(Mathf.Abs(vector.x), Mathf.Abs(vector.y), 0f);
	}
}
